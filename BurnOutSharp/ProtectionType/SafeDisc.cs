using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how to properly distinguish SafeDisc and SafeCast since both use
    // the same generic BoG_ string. The current combination check doesn't seem consistent
    public class SafeDisc : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("SafeCast2", StringComparison.OrdinalIgnoreCase))
                return $"SafeCast";

            // Get the .text section, if it exists
            string match = CheckSectionForProtection(file, includeDebug, pex, ".text");
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the .txt2 section, if it exists
            match = CheckSectionForProtection(file, includeDebug, pex, ".txt2");
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the CODE section, if it exists
            match = CheckSectionForProtection(file, includeDebug, pex, "CODE");
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the .data section, if it exists
            match = CheckSectionForProtection(file, includeDebug, pex, ".data");
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            // Get the stxt371 and stxt774 sections, if they exist -- TODO: Confirm if both are needed or either/or is fine
            bool stxt371Section = pex.ContainsSection("stxt371", exact: true);
            bool stxt774Section = pex.ContainsSection("stxt774", exact: true);
            if (stxt371Section || stxt774Section)
                return $"SafeDisc {Get320to4xVersion(null, null, null)}";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("CLCD16.DLL", useEndsWith: true),
                    new PathMatch("CLCD32.DLL", useEndsWith: true),
                    new PathMatch("CLOKSPL.EXE", useEndsWith: true),
                    new PathMatch(".icd", useEndsWith: true),
                }, "SafeDisc 1/Lite"),

                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("00000001.TMP", useEndsWith: true),
                    new PathMatch(".016", useEndsWith: true),
                    new PathMatch(".256", useEndsWith: true),
                }, "SafeDisc 1-3"),

                new PathMatchSet(new PathMatch("00000002.TMP", useEndsWith: true), "SafeDisc 2"),

                new PathMatchSet(new PathMatch("DPLAYERX.DLL", useEndsWith: true), GetDPlayerXVersion, "SafeDisc (dplayerx.dll)"),
                new PathMatchSet(new PathMatch("drvmgt.dll", useEndsWith: true), GetDrvmgtVersion, "SafeDisc (drvmgt.dll)"),
                new PathMatchSet(new PathMatch("secdrv.sys", useEndsWith: true), GetSecdrvVersion, "SafeDisc (secdrv.sys)"),

                new PathMatchSet(new PathMatch("00000001.LT1", useEndsWith: true), "SafeDisc Lite"),

                new PathMatchSet(".SafeDiscDVD.bundle", "SafeDisc for Macintosh"),

                new PathMatchSet(new PathMatch("cdac11ba.exe", useEndsWith: true), "SafeCast"),
                new PathMatchSet(new PathMatch("cdac14ba.dll", useEndsWith: true), "SafeCast"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("CLCD16.DLL", useEndsWith: true),
                    new PathMatch("CLCD32.DLL", useEndsWith: true),
                    new PathMatch("CLOKSPL.EXE", useEndsWith: true),
                }, "SafeDisc 1/Lite"),

                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("00000001.TMP", useEndsWith: true),
                }, "SafeDisc 1-3"),

                new PathMatchSet(new PathMatch("00000002.TMP", useEndsWith: true), "SafeDisc 2"),

                new PathMatchSet(new PathMatch("DPLAYERX.DLL", useEndsWith: true), GetDPlayerXVersion, "SafeDisc (dplayerx.dll)"),
                new PathMatchSet(new PathMatch("drvmgt.dll", useEndsWith: true), GetDrvmgtVersion, "SafeDisc (drvmgt.dll)"),
                new PathMatchSet(new PathMatch("secdrv.sys", useEndsWith: true), GetSecdrvVersion, "SafeDisc (secdrv.sys)"),

                new PathMatchSet(new PathMatch("00000001.LT1", useEndsWith: true), "SafeDisc Lite"),

                new PathMatchSet(".SafeDiscDVD.bundle", "SafeDisc for Macintosh"),

                new PathMatchSet(new PathMatch("cdac11ba.exe", useEndsWith: true), "SafeCast"),
                new PathMatchSet(new PathMatch("cdac14ba.dll", useEndsWith: true), "SafeCast"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string Get320to4xVersion(string file, byte[] fileContent, List<int> positions) => "3.20-4.xx (version removed)";

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            int index = positions[0] + 20; // Begin reading after "BoG_ *90.0&!!  Yy>" for old SafeDisc
            int version = fileContent.ReadInt32(ref index);
            int subVersion = fileContent.ReadInt32(ref index);
            int subsubVersion = fileContent.ReadInt32(ref index);

            if (version != 0)
                return $"{version}.{subVersion:00}.{subsubVersion:000}";

            index = positions[0] + 18 + 14; // Begin reading after "BoG_ *90.0&!!  Yy>" for newer SafeDisc
            version = fileContent.ReadInt32(ref index);
            subVersion = fileContent.ReadInt32(ref index);
            subsubVersion = fileContent.ReadInt32(ref index);

            if (version == 0)
                return string.Empty;

            return $"{version}.{subVersion:00}.{subsubVersion:000}";
        }

        public static string GetDPlayerXVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (firstMatchedString == null || !File.Exists(firstMatchedString))
                return string.Empty;

            FileInfo fi = new FileInfo(firstMatchedString);
            switch (fi.Length)
            {
                case  81_408:
                    return "1.0x";
                case 155_648:
                    return "1.1x";
                case 156_160:
                    return "1.1x-1.2x";
                case 163_328:
                    return "1.3x";
                case 165_888:
                    return "1.35";
                case 172_544:
                    return "1.40";
                case 173_568:
                    return "1.4x";
                case 136_704:
                    return "1.4x";
                case 138_752:
                    return "1.5x";
                default:
                    return "1";
            }
        }

        public static string GetDrvmgtVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (firstMatchedString == null || !File.Exists(firstMatchedString))
                return string.Empty;

            FileInfo fi = new FileInfo(firstMatchedString);
            switch (fi.Length)
            {
                case 34_816:
                    return "1.0x";
                case 32_256:
                    return "1.1x-1.3x";
                case 31_744:
                    return "1.4x";
                case 34_304:
                    return "1.5x-2.40";
                case 35_840:
                    return "2.51-2.60";
                case 40_960:
                    return "2.70";
                case 23_552:
                    return "2.80";
                case 41_472:
                    return "2.90-3.10";
                case 24_064:
                    return "3.15-3.20";
                default:
                    return "1-4";
            }
        }

        public static string GetSecdrvVersion(string firstMatchedString, IEnumerable<string> files)
        {
            if (firstMatchedString == null || !File.Exists(firstMatchedString))
                return string.Empty;

            FileInfo fi = new FileInfo(firstMatchedString);
            switch (fi.Length)
            {
                case 20_128:
                    return "2.10";
                case 27_440:
                    return "2.30";
                case 28_624:
                    return "2.40";
                case 18_768:
                    return "2.50";
                case 28_400:
                    return "2.51";
                case 29_392:
                    return "2.60";
                case 11_376:
                    return "2.70";
                case 12_464:
                    return "2.80";
                case 12_400:
                    return "2.90";
                case 12_528:
                    return "3.10-3.15";
                case 11_973:
                    return "3.20";

                //  14_304 - Bundled wtih 1.11.000
                //  10_848 - Bundled with 1.40.004
                //  143_68 - UNKNOWN
                // 163_644 - Bundled with 4.00.002, 4.60.000
                default:
                    return "1-4";
            }
        }

        // TODO: Continue collecting SHA-1 hashes instead of sizes
        private string GetVersionFromSHA1Hash(string sha1Hash)
        {
            switch (sha1Hash.ToLowerInvariant())
            {
                // dplayerx.dll
                case "f7a57f83bdc29040e20fd37cd0c6d7e6b2984180":
                    return "1.00.030";
                case "a8ed1613d47d1b5064300ff070484528ebb20a3b":
                    return "1.11.000";
                case "ed680e9a13f593e7a80a69ee1035d956ab62212b":
                    return "1.3x";
                case "66d8589343e00fa3e11bbf462e38c6f502515bea":
                    return "1.30.010";
                case "5751ae2ee805d31227cfe7680f3c8be4ab8945a3":
                    return "1.40";

                // drvmgt.dll
                case "d31725ff99be44bc1bfff171f4c4705f786b8e91":
                    return "1.1x-1.3x";
                case "87c0da1b52681fa8052a915e85699738993bea72":
                    return "1.11.000";
                case "8e41db1c60bbac631b06ad4f94adb4214a0e65dc":
                    return "1.4x";
                case "04ed7ac39fe7a6fab497a498cbcff7da19bf0556":
                    return "1.5x-2.40";
                case "5198da51184ca9f3a8096c6136f645b454a85f6c":
                    return "2.30.030";
                case "1437c8c149917c76f741c6dbee6b6b0cc0664f13":
                    return "2.40.010"; // Also 4.60.000, might be a fluke
                case "27d5e7f7eee1f22ebdaa903a9e58a7fdb50ef82c":
                    return "2.51-2.60";
                case "88c7aa6e91c9ba5f2023318048e3c3571088776f":
                    return "2.70";
                case "ea6e24b1f306391cd78a1e7c2f2c0c31828ef004":
                    return "2.80";
                case "e21ff43c2e663264d6cb11fbbc31eb1dcee42b1a":
                case "b824ed257946eee93f438b25c855e9dde7a3671a":
                    return "2.90-3.10";
                case "ecb341ab36c5b3b912f568d347368a6a2def8d5f":
                    return "3.15-3.20";
                case "7c5ab9bdf965b70e60b99086519327168f43f362":
                    return "4.00.002";
                case "a5247ec0ec50b8f470c93bf23e3f2514c402d5ad":
                    return "4.60.000";

                // secdrv.sys
                case "b64ad3ec82f2eb9fb854512cb59c25a771322181":
                    return "1.11.000";
                case "ebf69b0a96adfc903b7e486708474dc864cc0c7c":
                    return "1.40.004";
                case "f68a1370660f8b94f896bbba8dc6e47644d19092":
                    return "2.30";
                case "60bc8c3222081bf76466c521474d63714afd43cd":
                    return "2.40";
                case "08ceca66432278d8c4e0f448436b77583c3c61c8":
                    return "2.50";
                case "10080eb46bf76ac9cf9ea74372cfa4313727f0ca":
                    return "2.51";
                case "832d359a6de191c788b0e61e33f3d01f8d793d3c":
                    return "2.70";
                case "afcfaac945a5b47712719a5e6a7eb69e36a5a6e0":
                case "cb24fbe8aa23a49e95f3c83fb15123ffb01f43f4":
                    return "2.80";
                case "0383b69f98d0a9c0383c8130d52d6b431c79ac48":
                    return "2.90";
                case "d7c9213cc78ff57f2f655b050c4d5ac065661aa9":
                    return "3.20";
                case "fc6fedacc21a7244975b8f410ff8673285374cc2":
                    return "4.00.002"; // Also 4.60.000, might be a fluke
                case "2d9f54f35f5bacb8959ef3affdc3e4209a4629cb":
                    return "1-4";

                default:
                    return null;
            }
        }
    
        private string CheckSectionForProtection(string file, bool includeDebug, PortableExecutable pex, string sectionName)
        {
            // This subtract is needed because BoG_ starts before the section
            var sectionRaw = pex.ReadRawSection(sectionName, first: true, offset: -64);
            if (sectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
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
                };

                return MatchUtil.GetFirstMatch(file, sectionRaw, matchers, includeDebug);
            }

            return null;
        }
    }
}
