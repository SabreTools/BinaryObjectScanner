using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class VOBProtectCDDVD : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // VOB ProtectCD
                new ContentMatchSet(new byte?[]
                {
                    0x56, 0x4F, 0x42, 0x20, 0x50, 0x72, 0x6F, 0x74,
                    0x65, 0x63, 0x74, 0x43, 0x44
                }, GetOldVersion, "VOB ProtectCD/DVD"),

                // DCP-BOV + (char)0x00 + (char)0x00
                new ContentMatchSet(new byte?[] { 0x44, 0x43, 0x50, 0x2D, 0x42, 0x4F, 0x56, 0x00, 0x00 }, GetVersion, "VOB ProtectCD/DVD"),

                // .vob.pcd
                new ContentMatchSet(new byte?[] { 0x2E, 0x76, 0x6F, 0x62, 0x2E, 0x70, 0x63, 0x64 }, "VOB ProtectCD"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("VOB-PCD.KEY", useEndsWith: true), "VOB ProtectCD/DVD"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("VOB-PCD.KEY", useEndsWith: true), "VOB ProtectCD/DVD"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string GetOldVersion(string file, byte[] fileContent, List<int> positions)
        {
            int position = positions[0]--; // TODO: Verify this subtract
            char[] version = new ArraySegment<byte>(fileContent, position + 16, 4).Select(b => (char)b).ToArray(); // Begin reading after "VOB ProtectCD"
            if (char.IsNumber(version[0]) && char.IsNumber(version[2]) && char.IsNumber(version[3]))
                return $"{version[0]}.{version[2]}{version[3]}";

            return "old";
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            string version = GetVersion(fileContent, --positions[0]); // TODO: Verify this subtract
            if (version.Length > 0)
                return version;

            version = SearchProtectDiscVersion(file, fileContent);
            if (version.Length > 0)
            {
                if (version.StartsWith("2"))
                    version = $"6{version.Substring(1)}";

                return version;
            }

            return $"5.9-6.0 {GetBuild(fileContent, positions[0])}";
        }

        private static string GetBuild(byte[] fileContent, int position)
        {
            if (!char.IsNumber((char)fileContent[position - 13]))
                return string.Empty; //Build info removed

            int build = BitConverter.ToInt16(fileContent, position - 4); // Check if this is supposed to be a 4-byte read
            return $" (Build {build})";
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

            return string.Empty;
        }
    
        // TODO: Analyze this method and figure out if this can be done without attempting execution
        private static string SearchProtectDiscVersion(string file, byte[] fileContent)
        {
            // If the file isn't executable, don't even bother
            if (!EVORE.IsEXE(fileContent))
                return string.Empty;

            // Get some of the required paths
            string tempexe = EVORE.MakeTempFile(fileContent);
            string[] dependentDlls = EVORE.CopyDependentDlls(file, fileContent);
            string pdPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtectDisc");

            // Clean up any temp files before attempting to run
            Utilities.SafeTempDelete("a*.tmp");
            Utilities.SafeTempDelete("PCD*.sys");
            if (Directory.Exists(pdPath))
                Utilities.SafeDelete(Path.Combine(pdPath, "p*.dll"));

            // Try to safely start the temp executable
            Process exe = EVORE.StartSafe(tempexe);
            if (exe == null)
                return string.Empty;

            string version = "";
            Process[] processes = new Process[0];
            DateTime timestart = DateTime.Now;
            do
            {
                exe.Refresh();
                string[] files = null;

                // Check for ProtectDisc 8.2-x
                if (Directory.Exists(pdPath))
                    files = Directory.GetFiles(pdPath, "p*.dll");

                if (files.Any())
                {
                    string fileVersion = Utilities.GetFileVersion(files[0]);
                    if (!string.IsNullOrWhiteSpace(fileVersion))
                    {
                        version = fileVersion;

                        // ProtectDisc 9 uses a ProtectDisc-Core dll version 8.0.x
                        if (version.StartsWith("8.0"))
                            version = string.Empty;

                        break;
                    }
                }

                //check for ProtectDisc 7.1-8.1
                files = Directory.GetFiles(Path.GetTempPath(), "a*.tmp");
                if (files.Any())
                {
                    string fileVersion = Utilities.GetFileVersion(files[0]);
                    if (!string.IsNullOrWhiteSpace(fileVersion))
                    {
                        version = fileVersion;
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

            // Clean up any temp files after running
            Utilities.SafeDelete(tempexe);
            Utilities.SafeTempDelete("a*.tmp");
            Utilities.SafeTempDelete("PCD*.sys");
            if (Directory.Exists(pdPath))
                Utilities.SafeDelete(Path.Combine(pdPath, "p*.dll"));

            if (dependentDlls != null)
            {
                foreach (string dll in dependentDlls)
                {
                    Utilities.SafeDelete(dll);
                }
            }

            return version;
        }
    }
}
