using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class SafeDisc : IContentCheck, IPathCheck
    {
        /// <summary>
        /// Set of all PathMatchSets for this protection
        /// </summary>
        private static readonly List<PathMatchSet> pathMatchers = new List<PathMatchSet>
        {
            new PathMatchSet(new List<PathMatch>
            {
                new PathMatch("CLCD16.DLL", useEndsWith: true),
                new PathMatch("CLCD32.DLL", useEndsWith: true),
                new PathMatch("CLOKSPL.EXE", useEndsWith: true),
                new PathMatch(".icd", useEndsWith: true),
            }, "SafeDisc 1"),

            new PathMatchSet(new List<PathMatch>
            {
                new PathMatch("00000001.TMP", useEndsWith: true),
                //new PathMatch(".016", useEndsWith: true), // Potentially over-matching
                //new PathMatch(".256", useEndsWith: true), // Potentially over-matching
            }, "SafeDisc 1-3"),

            new PathMatchSet(new PathMatch("00000002.TMP", useEndsWith: true), "SafeDisc 2"),

            new PathMatchSet(new PathMatch("DPLAYERX.DLL", useEndsWith: true), GetDPlayerXVersion, "SafeDisc (dplayerx.dll)"),
            new PathMatchSet(new PathMatch("drvmgt.dll", useEndsWith: true), GetDrvmgtVersion, "SafeDisc (drvmgt.dll)"),
            new PathMatchSet(new PathMatch("secdrv.sys", useEndsWith: true), GetSecdrvVersion, "SafeDisc (secdrv.sys)"),
            new PathMatchSet(".SafeDiscDVD.bundle", "SafeDisc for Macintosh"),
        };

        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                new ContentMatchSet(new List<byte?[]>
                {
                    // BoG_ *90.0&!!  Yy>
                    new byte?[]
                    {
                        0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                        0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                        0x79, 0x3E
                    },

                    // product activation library
                    new byte?[]
                    {
                        0x70, 0x72, 0x6F, 0x64, 0x75, 0x63, 0x74, 0x20,
                        0x61, 0x63, 0x74, 0x69, 0x76, 0x61, 0x74, 0x69,
                        0x6F, 0x6E, 0x20, 0x6C, 0x69, 0x62, 0x72, 0x61,
                        0x72, 0x79
                    },
                }, GetVersion, "SafeCast"),

                // BoG_ *90.0&!!  Yy>
                new ContentMatchSet(new byte?[]
                {
                    0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                    0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                    0x79, 0x3E
                }, GetVersion, "SafeDisc"),

                // (char)0x00 + (char)0x00 + BoG_
                new ContentMatchSet(new byte?[] { 0x00, 0x00, 0x42, 0x6F, 0x47, 0x5F }, Get320to4xVersion, "SafeDisc"),

                // TODO: These two following are section headers. They should be converted to section header checks instead

                // stxt774
                new ContentMatchSet(new byte?[] { 0x73, 0x74, 0x78, 0x74, 0x37, 0x37, 0x34 }, Get320to4xVersion, "SafeDisc"),

                // stxt371
                new ContentMatchSet(new byte?[] { 0x73, 0x74, 0x78, 0x74, 0x33, 0x37, 0x31 }, Get320to4xVersion, "SafeDisc"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false) => null;

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            return MatchUtil.GetAllMatches(files, pathMatchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            return MatchUtil.GetFirstMatch(path, pathMatchers, any: true);
        }

        public static string Get320to4xVersion(string file, byte[] fileContent, List<int> positions)
        {
            string version = SearchSafeDiscVersion(file, fileContent);
            if (version.Length > 0)
                return version;

            return "3.20-4.xx (version removed)";
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            int index = positions[0] + 20; // Begin reading after "BoG_ *90.0&!!  Yy>" for old SafeDisc
            int version = BitConverter.ToInt32(fileContent, index);
            index += 4;
            int subVersion = BitConverter.ToInt32(fileContent, index);
            index += 4;
            int subsubVersion = BitConverter.ToInt32(fileContent, index);

            if (version != 0)
                return $"{version}.{subVersion:00}.{subsubVersion:000}";

            index = positions[0] + 18 + 14; // Begin reading after "BoG_ *90.0&!!  Yy>" for newer SafeDisc
            version = BitConverter.ToInt32(fileContent, index);
            index += 4;
            subVersion = BitConverter.ToInt32(fileContent, index);
            index += 4;
            subsubVersion = BitConverter.ToInt32(fileContent, index);

            if (version == 0)
                return string.Empty;

            return $"{version}.{subVersion:00}.{subsubVersion:000}";
        }

        public static string GetDPlayerXVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (firstMatchedString == null || !File.Exists(firstMatchedString))
                return string.Empty;

            FileInfo fi = new FileInfo(firstMatchedString);
            if (fi.Length == 81408)
                return "1.0x";
            else if (fi.Length == 155648)
                return "1.1x";
            else if (fi.Length == 156160)
                return "1.1x-1.2x";
            else if (fi.Length == 163328)
                return "1.3x";
            else if (fi.Length == 165888)
                return "1.35";
            else if (fi.Length == 172544)
                return "1.40";
            else if (fi.Length == 173568)
                return "1.4x";
            else if (fi.Length == 136704)
                return "1.4x";
            else if (fi.Length == 138752)
                return "1.5x";
            else
                return "1";
        }

        public static string GetDrvmgtVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (firstMatchedString == null || !File.Exists(firstMatchedString))
                return string.Empty;

            FileInfo fi = new FileInfo(firstMatchedString);
            if (fi.Length == 34816)
                return "1.0x";
            else if (fi.Length == 32256)
                return "1.1x-1.3x";
            else if (fi.Length == 31744)
                return "1.4x";
            else if (fi.Length == 34304)
                return "1.5x-2.40";
            else if (fi.Length == 35840)
                return "2.51-2.60";
            else if (fi.Length == 40960)
                return "2.70";
            else if (fi.Length == 23552)
                return "2.80";
            else if (fi.Length == 41472)
                return "2.90-3.10";
            else if (fi.Length == 24064)
                return "3.15-3.20";
            else
                return "1-3";
        }

        public static string GetSecdrvVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (firstMatchedString == null || !File.Exists(firstMatchedString))
                return string.Empty;

            FileInfo fi = new FileInfo(firstMatchedString);
            if (fi.Length == 20128)
                return "2.10";
            else if (fi.Length == 27440)
                return "2.30";
            else if (fi.Length == 28624)
                return "2.40";
            else if (fi.Length == 18768)
                return "2.50";
            else if (fi.Length == 28400)
                return "2.51";
            else if (fi.Length == 29392)
                return "2.60";
            else if (fi.Length == 11376)
                return "2.70";
            else if (fi.Length == 12464)
                return "2.80";
            else if (fi.Length == 12400)
                return "2.90";
            else if (fi.Length == 12528)
                return "3.10";
            else if (fi.Length == 12528)
                return "3.15";
            else if (fi.Length == 11973)
                return "3.20";
            else
                return "1-3";
        }

        // TODO: Analyze this method and figure out if this can be done without attempting execution
        private static string SearchSafeDiscVersion(string file, byte[] fileContent)
        {
            // If the file isn't executable, don't even bother
            if (!EVORE.IsPEExecutable(fileContent))
                return string.Empty;

            // Get some of the required paths
            string tempexe = EVORE.MakeTempFile(fileContent);
            string[] dependentDlls = EVORE.CopyDependentDlls(file, fileContent);

            // Clean up any temp files before attempting to run
            Utilities.SafeTempDelete("~e*", isDirectory: true);
            Utilities.SafeTempDelete("~e*", isDirectory: false);

            // Try to safely start the temp executable
            Process exe = EVORE.StartSafe(tempexe);
            if (exe == null)
                return string.Empty;

            string version = "";
            DateTime timestart = DateTime.Now;
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
                            string localFileContent = sr.ReadToEnd();
                            sr.Close();
                            int position = localFileContent.IndexOf("%ld.%ld.%ld, %ld, %s,") - 1;
                            if (position > -1)
                                version = localFileContent.Substring(position + 28, 12);
                            break;
                        }
                        catch { }
                    }
                }
            } while (!exe.HasExited && DateTime.Now.Subtract(timestart).TotalSeconds < 20);

            if (!exe.HasExited)
                exe.Kill();

            exe.Close();

            // Clean up any temp files after running
            Utilities.SafeDelete(tempexe);
            Utilities.SafeTempDelete("~e*", isDirectory: true);
            Utilities.SafeTempDelete("~e*", isDirectory: false);

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
