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
using System.Text;
using System.Threading;

namespace BurnOutSharp
{
    internal static class EVORE
    {
        private struct Section
        {
            public uint iVirtualSize;
            public uint iVirtualOffset;
            public uint iRawOffset;
        }

        private const int WaitSeconds = 20;

        private static Process StartSafe(string file)
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

        private static string MakeTempFile(byte[] fileContent, string sExtension = ".exe")
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

        private static bool IsEXE(byte[] fileContent)
        {
            int PEHeaderOffset = BitConverter.ToInt32(fileContent, 60);
            short Characteristics = BitConverter.ToInt16(fileContent, PEHeaderOffset + 22);

            // Check if file is dll
            if ((Characteristics & 0x2000) == 0x2000)
                return false;
            else
                return true;
        }

        private static string[] CopyDependentDlls(string file, byte[] fileContent)
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

        private static Section[] ReadSections(byte[] fileContent)
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

        private static uint RVA2Offset(uint RVA, Section[] sections)
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

        #region "EVORE version-search-functions"

        public static string SearchProtectDiscVersion(string file, byte[] fileContent)
        {
            string version = "";
            DateTime timestart;
            if (!IsEXE(fileContent))
                return "";

            string tempexe = MakeTempFile(fileContent);
            string[] DependentDlls = CopyDependentDlls(file, fileContent);
            try
            {
                File.Delete(Path.Combine(Path.GetTempPath(), "a*.tmp"));
            }
            catch { }
            try
            {
                File.Delete(Path.Combine(Path.GetTempPath(), "PCD*.sys"));
            }
            catch { }
            if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtectDisc")))
            {
                try
                {
                    File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtectDisc", "p*.dll"));
                }
                catch { }
            }

            Process exe = StartSafe(tempexe);
            if (exe == null)
                return "";

            Process[] processes = new Process[0];
            timestart = DateTime.Now;
            do
            {
                exe.Refresh();
                string[] files = null;

                //check for ProtectDisc 8.2-x
                if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtectDisc")))
                {
                    files = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtectDisc"), "p*.dll");
                }

                if (files != null)
                {
                    if (files.Length > 0)
                    {
                        FileVersionInfo fvinfo = FileVersionInfo.GetVersionInfo(files[0]);
                        if (fvinfo.FileVersion != "")
                        {
                            version = fvinfo.FileVersion.Replace(" ", "").Replace(",", ".");
                            //ProtectDisc 9 uses a ProtectDisc-Core dll version 8.0.x
                            if (version.StartsWith("8.0"))
                                version = "";
                            fvinfo = null;
                            break;
                        }
                    }
                }

                //check for ProtectDisc 7.1-8.1
                files = Directory.GetFiles(Path.GetTempPath(), "a*.tmp");
                if (files.Length > 0)
                {
                    FileVersionInfo fvinfo = FileVersionInfo.GetVersionInfo(files[0]);
                    if (fvinfo.FileVersion != "")
                    {
                        version = fvinfo.FileVersion.Replace(" ", "").Replace(",", ".");
                        fvinfo = null;
                        break;
                    }
                }

                if (exe.HasExited)
                    break;

                processes = Process.GetProcessesByName(exe.ProcessName);
                if (processes.Length == 2)
                {
                    processes[0].Refresh();
                    processes[1].Refresh();
                    if (processes[1].WorkingSet64 > exe.WorkingSet64)
                        exe = processes[1];
                    else if (processes[0].WorkingSet64 > exe.WorkingSet64) //else if (processes[0].Modules.Count > exe.Modules.Count)
                        exe = processes[0];
                }
            } while (processes.Length > 0 && DateTime.Now.Subtract(timestart).TotalSeconds < WaitSeconds);

            Thread.Sleep(500);
            if (!exe.HasExited)
            {
                processes = Process.GetProcessesByName(exe.ProcessName);
                if (processes.Length == 2)
                {
                    try
                    {
                        processes[0].Kill();
                    }
                    catch { }
                    processes[0].Close();
                    try
                    {
                        processes[1].Kill();
                    }
                    catch { }
                }
                else
                {
                    exe.Refresh();
                    try
                    {
                        exe.Kill();
                    }
                    catch { }
                }
            }

            exe.Close();
            Thread.Sleep(500);
            if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtectDisc")))
            {
                try
                {
                    File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtectDisc", "p*.dll"));
                }
                catch { }
            }

            try
            {
                File.Delete(Path.Combine(Path.GetTempPath(), "a*.tmp"));
            }
            catch { }

            try
            {
                File.Delete(Path.Combine(Path.GetTempPath(), "PCD*.sys"));
            }
            catch { }
            File.Delete(tempexe);
            if (DependentDlls != null)
            {
                for (int i = 0; i < DependentDlls.Length; i++)
                {
                    try
                    {
                        File.Delete(DependentDlls[i]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!error while deleting file " + DependentDlls[i] + "; " + ex.Message);
                    }
                }
            }

            return version;
        }

        public static string SearchSafeDiscVersion(string file, byte[] fileContent)
        {
            Process exe = new Process();
            string version = "";
            DateTime timestart;
            if (!IsEXE(fileContent))
                return "";

            string tempexe = MakeTempFile(fileContent);
            string[] DependentDlls = CopyDependentDlls(file, fileContent);
            try
            {
                Directory.Delete(Path.Combine(Path.GetTempPath(), "~e*"), true);
            }
            catch { }
            try
            {
                File.Delete(Path.Combine(Path.GetTempPath(), "~e*"));
            }
            catch { }

            exe = StartSafe(tempexe);
            if (exe == null)
                return "";

            timestart = DateTime.Now;
            do
            {
                if (Directory.GetDirectories(Path.GetTempPath(), "~e*").Length > 0)
                {
                    string[] files = Directory.GetFiles(Directory.GetDirectories(Path.GetTempPath(), "~e*")[0], "~de*.tmp");
                    if (files.Length > 0)
                    {
                        StreamReader sr;
                        try
                        {
                            sr = new StreamReader(files[0], Encoding.Default);
                            string FileContent = sr.ReadToEnd();
                            sr.Close();
                            int position = FileContent.IndexOf("%ld.%ld.%ld, %ld, %s,") - 1;
                            if (position > -1)
                                version = FileContent.Substring(position + 28, 12);
                            break;
                        }
                        catch { }
                    }
                }
            } while (!exe.HasExited && DateTime.Now.Subtract(timestart).TotalSeconds < WaitSeconds);

            if (!exe.HasExited)
                exe.Kill();
            exe.Close();

            try
            {
                Directory.Delete(Path.Combine(Path.GetTempPath(), "~e*"), true);
            }
            catch { }
            try
            {
                File.Delete(Path.Combine(Path.GetTempPath(), "~e*"));
                File.Delete(tempexe);
            }
            catch { }

            if (DependentDlls != null)
            {
                for (int i = 0; i < DependentDlls.Length; i--)
                {
                    try
                    {
                        File.Delete(DependentDlls[i]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("!error while deleting file " + DependentDlls[i] + "; " + ex.Message);
                    }
                }
            }

            return version;
        }

        #endregion
    }
}
