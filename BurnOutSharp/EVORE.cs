//this file is part of BurnOut
//Copyright (C)2005-2010 Gernot Knippen
//Ported code with augments Copyright (C)2018 Matt Nadareski
//
//This program is free software; you can redistribute it and/or
//modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation; either
//version 2 of the License, or (at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU General Public License for more details.
//
//You can get a copy of the GNU General Public License
//by writing to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

using System;
using System.Diagnostics;
using System.IO;

namespace BurnOutSharp
{
    internal static class EVORE
    {
        // TODO: Replace this with BurnOutSharp.ExecutableType.Microsoft.IMAGE_SECTION_HEADER
        internal struct Section
        {
            public uint iVirtualSize;
            public uint iVirtualOffset;
            public uint iRawOffset;
        }

        internal const int WaitSeconds = 20;

        internal static Process StartSafe(string file)
        {
            if (file == null || !File.Exists(file))
                return null;

            Process startingprocess = new Process();
            startingprocess.StartInfo.FileName = file;
            startingprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startingprocess.StartInfo.CreateNoWindow = true;
            startingprocess.StartInfo.ErrorDialog = false;
            try
            {
                startingprocess.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return startingprocess;
        }

        internal static string MakeTempFile(byte[] fileContent, string sExtension = ".exe")
        {
            string filei = Guid.NewGuid().ToString();
            string tempPath = Path.Combine(Path.GetTempPath(), "tmp", $"{filei}{sExtension}");
            try
            {
                File.Delete(tempPath);
            }
            catch { }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(tempPath)))
                {
                    bw.Write(fileContent);
                }

                return Path.GetFullPath(tempPath);
            }
            catch { }

            return null;
        }

        internal static bool IsEXE(byte[] fileContent)
        {
            int PEHeaderOffset = BitConverter.ToInt32(fileContent, 60);
            short Characteristics = BitConverter.ToInt16(fileContent, PEHeaderOffset + 22);

            // Check if file is dll
            if ((Characteristics & 0x2000) == 0x2000)
                return false;
            else
                return true;
        }

        internal static string[] CopyDependentDlls(string file, byte[] fileContent)
        {
            Section[] sections = ReadSections(fileContent);

            long lastPosition;
            string[] saDependentDLLs = null;
            int index = 60;
            int PEHeaderOffset = BitConverter.ToInt32(fileContent, index);
            index = PEHeaderOffset + 120 + 8; //120 Bytes till IMAGE_DATA_DIRECTORY array,8 Bytes=size of IMAGE_DATA_DIRECTORY
            uint ImportTableRVA = BitConverter.ToUInt32(fileContent, index);
            index += 4;
            uint ImportTableSize = BitConverter.ToUInt32(fileContent, index);
            index = (int)RVA2Offset(ImportTableRVA, sections);
            index += 12;
            uint DllNameRVA = BitConverter.ToUInt32(fileContent, index);
            index += 4;
            while (DllNameRVA != 0)
            {
                string sDllName = "";
                byte bChar;
                lastPosition = index;
                uint DLLNameOffset = RVA2Offset(DllNameRVA, sections);
                if (DLLNameOffset > 0)
                {
                    index = (int)DLLNameOffset;
                    if ((char)fileContent[index] > -1)
                    {
                        do
                        {
                            bChar = fileContent[index];
                            index++;
                            sDllName += (char)bChar;
                        } while (bChar != 0 && (char)fileContent[index] > -1);

                        sDllName = sDllName.Remove(sDllName.Length - 1, 1);
                        if (File.Exists(Path.Combine(Path.GetDirectoryName(file), sDllName)))
                        {
                            if (saDependentDLLs == null)
                                saDependentDLLs = new string[0];
                            else
                                saDependentDLLs = new string[saDependentDLLs.Length];

                            FileInfo fiDLL = new FileInfo(Path.Combine(Path.GetDirectoryName(file), sDllName));
                            saDependentDLLs[saDependentDLLs.Length - 1] = fiDLL.CopyTo(Path.GetTempPath() + sDllName, true).FullName;
                        }
                    }

                    index = (int)lastPosition;
                }

                index += 4 + 12;
                DllNameRVA = BitConverter.ToUInt32(fileContent, index);
                index += 4;
            }

            return saDependentDLLs;
        }

        internal static Section[] ReadSections(byte[] fileContent)
        {
            if (fileContent == null)
                return null;

            uint PEHeaderOffset = BitConverter.ToUInt32(fileContent, 60);
            ushort NumberOfSections = BitConverter.ToUInt16(fileContent, (int)PEHeaderOffset + 6);
            Section[] sections = new Section[NumberOfSections];
            int index = (int)PEHeaderOffset + 120 + 16 * 8;            
            for (int i = 0; i < NumberOfSections; i++)
            {
                index += 8;
                uint ivs = BitConverter.ToUInt32(fileContent, index);
                index += 4;
                uint ivo = BitConverter.ToUInt32(fileContent, index);
                index += 4;
                index += 4;
                uint iro = BitConverter.ToUInt32(fileContent, index);
                index += 4;
                index += 16;

                sections[i] = new Section()
                {
                    iVirtualSize = ivs,
                    iVirtualOffset = ivo,
                    iRawOffset = iro,
                };
            }

            return sections;
        }

        internal static uint RVA2Offset(uint RVA, Section[] sections)
        {
            int i = 0;
            while (i != sections.Length)
            {
                if (sections[i].iVirtualOffset <= RVA && sections[i].iVirtualOffset + sections[i].iVirtualSize > RVA)
                    return RVA - sections[i].iVirtualOffset + sections[i].iRawOffset;
                i++;
            }
            return 0;
        }
    }
}
