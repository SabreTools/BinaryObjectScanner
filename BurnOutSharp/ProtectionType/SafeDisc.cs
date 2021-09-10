using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

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
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .text section, if it exists
            var textSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".text"));
            if (textSection != null)
            {
                // This subtract is needed because BoG_ starts before the .text section
                int sectionAddr = (int)textSection.PointerToRawData - 64;
                int sectionEnd = sectionAddr + (int)textSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // BoG_ *90.0&!!  Yy>
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                            0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                            0x79, 0x3E
                        }, start: sectionAddr, end: sectionEnd),
                    GetVersion, "SafeDisc"),

                    // (char)0x00 + (char)0x00 + BoG_
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x00, 0x00, 0x42, 0x6F, 0x47, 0x5F }, start: sectionAddr, end: sectionEnd),
                    Get320to4xVersion, "SafeDisc"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .txt2 section, if it exists
            var txt2Section = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".txt2"));
            if (txt2Section != null)
            {
                // This subtract is needed because BoG_ starts before the .txt2 section
                int sectionAddr = (int)txt2Section.PointerToRawData - 64;
                int sectionEnd = sectionAddr + (int)txt2Section.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // BoG_ *90.0&!!  Yy>
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                            0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                            0x79, 0x3E
                        }, start: sectionAddr, end: sectionEnd),
                    GetVersion, "SafeDisc"),

                    // (char)0x00 + (char)0x00 + BoG_
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x00, 0x00, 0x42, 0x6F, 0x47, 0x5F }, start: sectionAddr, end: sectionEnd),
                    Get320to4xVersion, "SafeDisc"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the .data section, if it exists
            var dataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".data"));
            if (dataSection != null)
            {
                int sectionAddr = (int)dataSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)dataSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // BoG_ *90.0&!!  Yy>
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x42, 0x6F, 0x47, 0x5F, 0x20, 0x2A, 0x39, 0x30,
                            0x2E, 0x30, 0x26, 0x21, 0x21, 0x20, 0x20, 0x59,
                            0x79, 0x3E
                        }, start: sectionAddr, end: sectionEnd),
                    GetVersion, "SafeDisc"),

                    // (char)0x00 + (char)0x00 + BoG_
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x00, 0x00, 0x42, 0x6F, 0x47, 0x5F }, start: sectionAddr, end: sectionEnd),
                    Get320to4xVersion, "SafeDisc"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the stxt371 and stxt774 sections, if they exist -- TODO: Confirm if both are needed or either/or is fine
            var stxt371Section = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith("stxt371"));
            var stxt774Section = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith("stxt774"));
            if (stxt371Section != null || stxt774Section != null)
                return $"SafeDisc {Get320to4xVersion(file, fileContent, null)}";

            return null;
        }

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

        // TODO: Try to find a file that this actually triggers for
        public static string Get320to4xVersion(string file, byte[] fileContent, List<int> positions) => "3.20-4.xx (version removed)";

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

        // TODO: Continue collecting SHA-1 hashes instead of sizes
        public static string GetDPlayerXVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (firstMatchedString == null || !File.Exists(firstMatchedString))
                return string.Empty;

            FileInfo fi = new FileInfo(firstMatchedString);
            if (fi.Length == 81408)
                return "1.0x";
            else if (fi.Length == 155648)
                return "1.1x";

            // a8ed1613d47d1b5064300ff070484528ebb20a3b - Bundled with 1.11.000
            else if (fi.Length == 156160)
                return "1.1x-1.2x";

            // ed680e9a13f593e7a80a69ee1035d956ab62212b
            // 66d8589343e00fa3e11bbf462e38c6f502515bea - Bundled with 1.30.010
            else if (fi.Length == 163328)
                return "1.3x";
            else if (fi.Length == 165888)
                return "1.35";

            // 5751ae2ee805d31227cfe7680f3c8be4ab8945a3
            else if (fi.Length == 172544)
                return "1.40";
            else if (fi.Length == 173568)
                return "1.4x";
            else if (fi.Length == 136704)
                return "1.4x";
            else if (fi.Length == 138752)
                return "1.5x";

            // f7a57f83bdc29040e20fd37cd0c6d7e6b2984180 - 78848 - Bundled with 1.00.030
            else
                return "1";
        }

        // TODO: Continue collecting SHA-1 hashes instead of sizes
        public static string GetDrvmgtVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (firstMatchedString == null || !File.Exists(firstMatchedString))
                return string.Empty;

            FileInfo fi = new FileInfo(firstMatchedString);
            if (fi.Length == 34816)
                return "1.0x";

            // d31725ff99be44bc1bfff171f4c4705f786b8e91
            // 87c0da1b52681fa8052a915e85699738993bea72 - Bundled with 1.11.000
            else if (fi.Length == 32256)
                return "1.1x-1.3x";

            // 8e41db1c60bbac631b06ad4f94adb4214a0e65dc
            else if (fi.Length == 31744)
                return "1.4x";

            // 04ed7ac39fe7a6fab497a498cbcff7da19bf0556
            // 5198da51184ca9f3a8096c6136f645b454a85f6c - Bundled with 2.30.030
            // 1437c8c149917c76f741c6dbee6b6b0cc0664f13 - Bundled with 2.40.010, 4.60.000
            else if (fi.Length == 34304)
                return "1.5x-2.40";

            // 27d5e7f7eee1f22ebdaa903a9e58a7fdb50ef82c
            else if (fi.Length == 35840)
                return "2.51-2.60";

            // 88c7aa6e91c9ba5f2023318048e3c3571088776f
            else if (fi.Length == 40960)
                return "2.70";

            // ea6e24b1f306391cd78a1e7c2f2c0c31828ef004
            else if (fi.Length == 23552)
                return "2.80";

            // e21ff43c2e663264d6cb11fbbc31eb1dcee42b1a
            // b824ed257946eee93f438b25c855e9dde7a3671a
            // 7c5ab9bdf965b70e60b99086519327168f43f362 - Bundled with 4.00.002
            else if (fi.Length == 41472)
                return "2.90-3.10";

            // ecb341ab36c5b3b912f568d347368a6a2def8d5f
            else if (fi.Length == 24064)
                return "3.15-3.20";

            // a5247ec0ec50b8f470c93bf23e3f2514c402d5ad - 46592 - Bundled with 4.60.000 (2x)
            else
                return "1-4";
        }

        // TODO: Continue collecting SHA-1 hashes instead of sizes
        public static string GetSecdrvVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (firstMatchedString == null || !File.Exists(firstMatchedString))
                return string.Empty;

            FileInfo fi = new FileInfo(firstMatchedString);
            if (fi.Length == 20128)
                return "2.10";

            // f68a1370660f8b94f896bbba8dc6e47644d19092
            else if (fi.Length == 27440)
                return "2.30";

            // 60bc8c3222081bf76466c521474d63714afd43cd
            else if (fi.Length == 28624)
                return "2.40";

            // 08ceca66432278d8c4e0f448436b77583c3c61c8
            else if (fi.Length == 18768)
                return "2.50";

            // 10080eb46bf76ac9cf9ea74372cfa4313727f0ca
            else if (fi.Length == 28400)
                return "2.51";
            else if (fi.Length == 29392)
                return "2.60";

            // 832d359a6de191c788b0e61e33f3d01f8d793d3c
            else if (fi.Length == 11376)
                return "2.70";

            // afcfaac945a5b47712719a5e6a7eb69e36a5a6e0
            // cb24fbe8aa23a49e95f3c83fb15123ffb01f43f4
            else if (fi.Length == 12464)
                return "2.80";

            // 0383b69f98d0a9c0383c8130d52d6b431c79ac48
            else if (fi.Length == 12400)
                return "2.90";
            else if (fi.Length == 12528)
                return "3.10";
            else if (fi.Length == 12528)
                return "3.15";

            // d7c9213cc78ff57f2f655b050c4d5ac065661aa9
            else if (fi.Length == 11973)
                return "3.20";

            // b64ad3ec82f2eb9fb854512cb59c25a771322181 -  14304 - Bundled wtih 1.11.000
            // ebf69b0a96adfc903b7e486708474dc864cc0c7c -  10848 - Bundled with 1.40.004
            // 2d9f54f35f5bacb8959ef3affdc3e4209a4629cb -  14368 - UNKNOWN
            // fc6fedacc21a7244975b8f410ff8673285374cc2 - 163644 - Bundled with 4.00.002, 4.60.000
            else
                return "1-4";
        }
    }
}
