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
using System.Runtime.InteropServices;
using BurnOutSharp.ExecutableType.Microsoft;

namespace BurnOutSharp
{
    internal static class EVORE
    {
        /// <summary>
        /// Checks if the file contents that represent a PE is a DLL or an EXE
        /// </summary>
        /// <param name="fileContent">File contents to check</param>
        /// <returns>True if the file is an EXE, false if it's a DLL</returns>
        internal static bool IsEXE(byte[] fileContent)
        {
            int PEHeaderOffset = BitConverter.ToInt32(fileContent, 60);
            short Characteristics = BitConverter.ToInt16(fileContent, PEHeaderOffset + 22);

            // Check if file is dll
            return (Characteristics & 0x2000) != 0x2000;
        }

        /// <summary>
        /// Writes the file contents to a temporary file, if possible
        /// </summary>
        /// <param name="fileContent">File contents to write</param>
        /// <param name="sExtension">Optional extension for the temproary file, defaults to ".exe"</param>
        /// <returns>Name of the new temporary file, null on error</returns>
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

        /// <summary>
        /// Copies all required DLLs for a given executable
        /// </summary>
        /// <param name="file">Temporary file path</param>
        /// <param name="fileContent">File contents to read</param>
        /// <returns>Paths for all of the copied DLLs, null on error</returns>
        internal static string[] CopyDependentDlls(string file, byte[] fileContent)
        {
            var sections = ReadSections(fileContent);

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

        /// <summary>
        /// Attempt to run an executable
        /// </summary>
        /// <param name="file">Executable to attempt to run</param>
        /// <returns>Process representing the running executable, null on error</returns>
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
            catch
            {
                return null;
            }

            return startingprocess;
        }

        private static IMAGE_SECTION_HEADER?[] ReadSections(byte[] fileContent)
        {
            if (fileContent == null)
                return null;

            uint PEHeaderOffset = BitConverter.ToUInt32(fileContent, 60);
            ushort NumberOfSections = BitConverter.ToUInt16(fileContent, (int)PEHeaderOffset + 6);
            var sections = new IMAGE_SECTION_HEADER?[NumberOfSections];
            int index = (int)PEHeaderOffset + 120 + 16 * 8;            
            for (int i = 0; i < NumberOfSections; i++)
            {
                sections[i] = ReadSection(fileContent, index);
            }

            return sections;
        }

        private static IMAGE_SECTION_HEADER? ReadSection(byte[] fileContent, int ptr)
        {
            // Get the size of a section header for later
            int sectionSize = Marshal.SizeOf<IMAGE_SECTION_HEADER>();

            // If the contents are null or the wrong size, we can't read a section
            if (fileContent == null || fileContent.Length < sectionSize)
                return null;

            // Create a new section and try our best to read one
            IMAGE_SECTION_HEADER? section = null;
            IntPtr tempPtr = IntPtr.Zero;
            try
            {
                // Get the pointer to where the section will go
                tempPtr = Marshal.AllocHGlobal(sectionSize);
                
                // If we couldn't get the space, just return null
                if (tempPtr == IntPtr.Zero)
                    return null;

                // Copy from the array to the new space
                Marshal.Copy(fileContent, ptr, tempPtr, sectionSize);

                // Get the new section and return
                section = Marshal.PtrToStructure<IMAGE_SECTION_HEADER>(tempPtr);
            }
            catch
            {
                // We don't care what the error was
                return null;
            }
            finally
            {
                if (tempPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(tempPtr);
            }

            return section;
        }

        private static uint RVA2Offset(uint RVA, IMAGE_SECTION_HEADER?[] sections)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                if (!sections[i].HasValue)
                    continue;

                var section = sections[i].Value;
                if (section.VirtualAddress <= RVA && section.VirtualAddress + section.PhysicalAddressOrVirtualSize > RVA)
                    return RVA - section.VirtualAddress + section.PointerToRawData;
            }

            return 0;
        }
    }
}
