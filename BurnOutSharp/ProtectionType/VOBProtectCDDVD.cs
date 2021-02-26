using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace BurnOutSharp.ProtectionType
{
    public class VOBProtectCDDVD : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "VOB ProtectCD"
            byte[] check = new byte[] { 0x56, 0x4F, 0x42, 0x20, 0x50, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x43, 0x44 };
            if (fileContent.Contains(check, out int position))
                return $"VOB ProtectCD/DVD {GetOldVersion(fileContent, --position)}" + (includePosition ? $" (Index {position})" : string.Empty); // TODO: Verify this subtract

            // "DCP-BOV" + (char)0x00 + (char)0x00
            check = new byte[] { 0x44, 0x43, 0x50, 0x2D, 0x42, 0x4F, 0x56, 0x00, 0x00 };
            if (fileContent.Contains(check, out position))
            {
                string version = GetVersion(fileContent, --position); // TODO: Verify this subtract
                if (version.Length > 0)
                    return $"VOB ProtectCD/DVD {version}" + (includePosition ? $" (Index {position})" : string.Empty);

                version = SearchProtectDiscVersion(file, fileContent);
                if (version.Length > 0)
                {
                    if (version.StartsWith("2"))
                        version = $"6{version.Substring(1)}";

                    return $"VOB ProtectCD/DVD {version}" + (includePosition ? $" (Index {position})" : string.Empty);
                }

                return $"VOB ProtectCD/DVD 5.9-6.0 {GetBuild(fileContent, position)}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            // ".vob.pcd"
            check = new byte[] { 0x2E, 0x76, 0x6F, 0x62, 0x2E, 0x70, 0x63, 0x64 };
            if (fileContent.Contains(check, out position))
                return "VOB ProtectCD" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        /// <inheritdoc/>
        public string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("VOB-PCD.KEY", StringComparison.OrdinalIgnoreCase)))
                    return "VOB ProtectCD/DVD";
            }
            else
            {
                if (Path.GetFileName(path).Equals("VOB-PCD.KEY", StringComparison.OrdinalIgnoreCase))
                    return "VOB ProtectCD/DVD";
            }

            return null;
        }

        private static string GetBuild(byte[] fileContent, int position)
        {
            if (!char.IsNumber((char)fileContent[position - 13]))
                return ""; //Build info removed

            int build = BitConverter.ToInt16(fileContent, position - 4); // Check if this is supposed to be a 4-byte read
            return $" (Build {build})";
        }

        private static string GetOldVersion(byte[] fileContent, int position)
        {
            char[] version = new ArraySegment<byte>(fileContent, position + 16, 4).Select(b => (char)b).ToArray(); // Begin reading after "VOB ProtectCD"
            if (char.IsNumber(version[0]) && char.IsNumber(version[2]) && char.IsNumber(version[3]))
                return $"{version[0]}.{version[2]}{version[3]}";

            return "old";
        }

        private static string GetVersion(byte[] fileContent, int position)
        {
            if (fileContent[position - 2] == 5)
            {
                int index = position - 4;
                byte subsubVersion = (byte)((fileContent[index] & 0xF0) >> 4);
                index++;
                byte subVersion = (byte)((fileContent[index] & 0xF0) >> 4);
                return $"5.{subVersion}.{subsubVersion}";
            }

            return "";
        }
    
        // TODO: Analyze this method and figure out if this can be done without attempting execution
        private static string SearchProtectDiscVersion(string file, byte[] fileContent)
        {
            string version = "";
            DateTime timestart;
            if (!EVORE.IsEXE(fileContent))
                return "";

            string tempexe = EVORE.MakeTempFile(fileContent);
            string[] DependentDlls = EVORE.CopyDependentDlls(file, fileContent);
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

            Process exe = EVORE.StartSafe(tempexe);
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
            } while (processes.Length > 0 && DateTime.Now.Subtract(timestart).TotalSeconds < 20);

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
    }
}
