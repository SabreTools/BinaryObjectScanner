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

            // TODO: This doesn't properly grab the File Description for secdrv.sys and I'm not sure why.
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("Macrovision SECURITY Driver", StringComparison.OrdinalIgnoreCase))
                return $"SafeDisc Security Driver {GetSecDrvExecutableVersion(pex)}";

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

            // TODO: Add entry point check
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/Safedisc.2.sg

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // TODO: Investigate if these DLLs are viable to be hashed to provide a rough version range.
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

                new PathMatchSet(new PathMatch("00000002.TMP", useEndsWith: true), "SafeDisc 2+"),

                new PathMatchSet(new PathMatch("DPLAYERX.DLL", useEndsWith: true), GetDPlayerXVersion, "SafeDisc (dplayerx.dll)"),
                new PathMatchSet(new PathMatch("drvmgt.dll", useEndsWith: true), GetDrvmgtVersion, "SafeDisc (drvmgt.dll)"),
                new PathMatchSet(new PathMatch("secdrv.sys", useEndsWith: true), GetSecdrvVersion, "SafeDisc (secdrv.sys)"),

                // Found in Redump entries 28810 and 30555.
                new PathMatchSet(new PathMatch("mcp.dll", useEndsWith: true), "SafeDisc (Version 1.45.011-1.50.020)"),

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
                // TODO: Investigate if these DLLs are viable to be hashed to provide a rough version range.
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("CLCD16.DLL", useEndsWith: true),
                    new PathMatch("CLCD32.DLL", useEndsWith: true),
                    new PathMatch("CLOKSPL.EXE", useEndsWith: true),
                }, "SafeDisc 1/Lite"),

                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("00000001.TMP", useEndsWith: true),
                }, "SafeDisc"),

                new PathMatchSet(new PathMatch("00000002.TMP", useEndsWith: true), "SafeDisc 2+"),

                new PathMatchSet(new PathMatch("DPLAYERX.DLL", useEndsWith: true), GetDPlayerXVersion, "SafeDisc (dplayerx.dll)"),
                new PathMatchSet(new PathMatch("drvmgt.dll", useEndsWith: true), GetDrvmgtVersion, "SafeDisc (drvmgt.dll)"),
                new PathMatchSet(new PathMatch("secdrv.sys", useEndsWith: true), GetSecdrvVersion, "SafeDisc (secdrv.sys)"),

                // Found in Redump entries 28810 and 30555.
                new PathMatchSet(new PathMatch("mcp.dll", useEndsWith: true), "SafeDisc (Version 1.45.011-1.50.020)"),

                // TODO: Add extra detection of DIAG.exe, which is used by versions 4.50.000-4.70.000. This isn't particularly pressing, as it is already detected as a SafeDisc EXE, complete with version string.

                // Found in Redump entry 58990.
                new PathMatchSet(new PathMatch("SafediskSplash.bmp", useEndsWith: true), "SafeDisc"),

                new PathMatchSet(new PathMatch("00000001.LT1", useEndsWith: true), "SafeDisc Lite"),

                new PathMatchSet(".SafeDiscDVD.bundle", "SafeDisc for Macintosh"),

                new PathMatchSet(new PathMatch("cdac11ba.exe", useEndsWith: true), "SafeCast"),
                new PathMatchSet(new PathMatch("cdac14ba.dll", useEndsWith: true), "SafeCast"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string Get320to4xVersion(string file, byte[] fileContent, List<int> positions) => "3.20-4.xx [version expunged]";

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

        // TODO: Investigate alternatives to size checks for this file.
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

            // The file "drvmgt.dll" has been found to be incredibly consistent between versions, with the vast majority of files based on hash corresponding 1:1 with the SafeDisc version used according to the EXE.
            string sha1 = BurnOutSharp.Tools.Utilities.GetFileSHA1(firstMatchedString);
            switch (sha1)
            {
                // Found in Redump entries 29073 and 31149.
                case "33434590D7DE4EEE2C35FCC98B0BF141F422B26D":
                    return "1.06.000";
                // Found in Redump entries 9718 and 46756.
                case "D5E4C99CDCA8091EC8010FCB96C5534A8BE35B43":
                    return "1.07.000";
                // Found in Redump entries 12885 and 66210.
                case "412067F80F6B644EDFB25932EA34A7E92AD4FC21":
                    return "1.09.000";
                // Found in Redump entries 37523 and 66586.
                case "87C0DA1B52681FA8052A915E85699738993BEA72":
                    return "1.11.000";
                // Found in Redump entries 21154 and 37982. 
                case "3569FE747311265FDC83CBDF13361B4E06484725":
                    return "1.20.000";
                // Found in Redump entry 37920.
                case "89795A34A2CAD4602713524365F710948F7367D0":
                    return "1.20.001";
                // Found in Redump entries 31526 and 55080.
                case "D31725FF99BE44BC1BFFF171F4C4705F786B8E91":
                    return "1.30.010";
                // Found in Redump entries 9617 and 49552.
                case "2A86168FE8EFDFC31ABCC7C5D55A1A693F2483CD":
                    return "1.35.000";
                // Found in Redump entries 2595 and 30121.
                case "8E41DB1C60BBAC631B06AD4F94ADB4214A0E65DC":
                    return "1.40.004";
                // Found in Redump entries 44350 and 63323.
                case "833EA803FB5B0FA22C9CF4DD840F0B7DE31D24FE":
                    return "1.41.000";
                // Found in Redump entries 37832 and 42091.
                case "1A9C8F6A5BD68F23CA0C8BCB890991AB214F14E0":
                    return "1.41.001";
                // Found in Redump entries 30555 and 55078.
                case "0BF4574813EA92FEE373481CA11DF220B6C4F61A":
                    return "1.45.011";
                // Found in Redump entries 28810 and 62935.
                case "812121D39D6DCC4947E177240655903DEC4DA15A":
                    return "1.50.020";
                // Found in Redump entries 72195 and 73502.
                case "04ED7AC39FE7A6FAB497A498CBCFF7DA19BF0556":
                    return "2.05.030";
                // Found in Redump entries 38541 and 59462 and 81096.
                case "0AB8330A33E188A29E8CE1EA9625AA5935D7E8CE":
                    return "2.10.030";
                // Found in Redump entries 55823 and 79476.
                case "5198DA51184CA9F3A8096C6136F645B454A85F6C":
                    return "2.30.030";
                // Found in Redump entries 15312 and 48863.
                case "414CAC2BE3D9BE73796D51A15076A5A323ABBF2C":
                    return "2.30.031";
                // Found in Redump entries 9819 and 53658. 
                case "126DCA2317DA291CBDE13A91B3FE47BA4719446A":
                    return "2.30.033";
                // Found in Redump entries 9846 and 65642.
                case "1437C8C149917C76F741C6DBEE6B6B0CC0664F13":
                    return "2.40.010";
                // Found in Redump entries 23786 and 37478.
                case "10FAD492991C251C9C9394A2B746C1BF48A18173":
                    return "2.40.011";
                // Found in Redump entries 30022 and 75014.
                case "94267BB97C418A6AA22C1354E38136F889EB0B6A":
                    return "2.51.020";
                // Found in Redump entries 31666 and 66852.
                case "27D5E7F7EEE1F22EBDAA903A9E58A7FDB50EF82C":
                    return "2.51.021";
                // Found in Redump entries 2064 and 47047.
                case "F346F4D0CAB4775041AD692A6A49C47D34D46571":
                    return "2.60.052";
                // Found in Redump entries 13048 and 35385.
                case "88C7AA6E91C9BA5F2023318048E3C3571088776F":
                    return "2.70.030";
                // Found in Redump entries 48101 and 64198.
                case "544EE77889092129E9818B5086E19197E5771C7F":
                    return "2.72.000";
                // Found in Redump entries 32783 and 72743.
                case "EA6E24B1F306391CD78A1E7C2F2C0C31828EF004":
                    return "2.80.010";
                // Found in Redump entries 39273 and 59351.
                case "1BF885FDEF8A1824C34C10E2729AD133F70E1516":
                    return "2.80.011";
                // Found in Redump entries 52606 and 62505.
                case "B824ED257946EEE93F438B25C855E9DDE7A3671A":
                    return "2.90.040";
                // Found in Redump entries 13230 and 68204.
                case "CDA56FD150C9E9A19D7AF484621352122431F029":
                    return "3.10.020";
                // Found in Redump entries 36511 and 74338.
                case "E5504C4C31561D38C1F626C851A8D06212EA13E0":
                    return "3.15.010";
                // Found in Redump entries 15383 and 35512. 
                case "AABA7B3EF08E80BC55282DA3C3B7AA598379D385":
                    return "3.15.011";
                // Found in Redump entries 58625, 75782, and 84586.
                // The presence of any drvmgt.dll file at all is notably missing in several games with SafeDisc versions 3.20.020-3.20.024, including Redump entries 20729, 30404, and 56748.
                // TODO: Further investigate versions 3.20.020-3.20.024, and verify that 3.20.024 doesn't use drvmgt.dll at all.
                case "ECB341AB36C5B3B912F568D347368A6A2DEF8D5F":
                    return "3.20.020-3.20.022";
                // Found in Redump entries 15614, 79729, 83408, and 86196.
                // The presence of any drvmgt.dll file at all is notably missing in several games with SafeDisc versions 4.00.001-4.00.003, including Redump entries 33326, 51597, and 67927.
                case "E21FF43C2E663264D6CB11FBBC31EB1DCEE42B1A":
                    return "4.00.000-4.00.003";
                // Found in Redump entry 49677.
                case "7C5AB9BDF965B70E60B99086519327168F43F362":
                    return "4.00.002";
                // Found in Redump entries 46765 and 78980.
                case "A5247EC0EC50B8F470C93BF23E3F2514C402D5AD":
                    return "4.00.002+";
                // Found in Redump entries 74564 and 80776.
                // The presence of any drvmgt.dll file at all is notably missing in several games with SafeDisc versions 4.50.000, including Redump entries 58990 and XXXXXXXX
                case "C658E0B4992903D5E8DD9B235C25CB07EE5BFEEB":
                    return "4.50.000";
                // Found in Redump entry 56320.
                case "84480ABCE4676EEB9C43DFF7C5C49F0D574FAC25":
                    return "4.70.000";
                default:
                    return "Unknown Version";

                // File size of drvmgt.dll and others is a commonly used indicator of SafeDisc version, though it has been found ot not be completely consistent, and is completely replaced by hash checks.
                // 34,816 bytes corresponds to SafeDisc 1.0x
                // 32,256 bytes corresponds to SafeDisc 1.1x-1.3x
                // 31,744 bytes corresponds to SafeDisc 1.4x
                // 34,304 bytes corresponds to SafeDisc 1.5x-2.40
                // 35,840 bytes corresponds to SafeDisc 2.51-2.60
                // 40,960 bytes corresponds to SafeDisc 2.70
                // 23,552 bytes corresponds to SafeDisc 2.80
                // 41,472 bytes corresponds to SafeDisc 2.90-3.10
                // 24,064 bytes corresponds to SafeDisc 3.15-3.20;
            }
        }

        // TODO: Verify these checks and remove any that may not be needed, file version checks should remove the need for any checks for 2.80+.
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
                // File size checks for versions 2.90+ are superceded by executable version checks, which are more accurate. For reference, the previously used file sizes are kept as comments.
                // 12,400 bytes corresponds to SafeDisc 2.90
                // 12,528 bytes corresponds to SafeDisc 3.10-3.15
                // 11,973 bytes corresponds to SafeDisc 3.20

                //  14_304 - Bundled wtih 1.11.000
                //  10_848 - Bundled with 1.40.004
                //  143_68 - UNKNOWN
                // 163_644 - Bundled with 4.00.002, 4.60.000
                default:
                    return "1-4";
            }
        }

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

        private string GetSecDrvExecutableVersion(PortableExecutable pex)
        {
            // Different versions of this driver correspond to different SafeDisc versions.
            string version = pex.ProductVersion;
            if (!string.IsNullOrEmpty(version))
            {
                switch (version)
                {
                    // Found to be in Redump entry 32783.
                    case "3.17.0.0":
                        return "3.17.0.0 / SafeDisc 2.80.010-2.80.011";
                    // Found to be in Redump entry 52606.
                    case "3.18.0.0":
                        return "3.18.0.0 / SafeDisc 2.90.010-2.90.040";
                    // Found to be in Redump entry 13230.
                    case "3.19.0.0":
                        return "3.19.0.0 / SafeDisc 3.10.020-3.15.011";
                    // Found to be in Redump entry 58625.
                    case "3.22.0.0":
                        return "3.22.0.0 / SafeDisc 3.20.020-3.20.022";
                    // Found to be in Redump entry 15614.
                    case "4.0.60.0":
                        return "4.0.60.0 / SafeDisc 4.00.000-4.70.000";
                    // Found distributed online, but so far not in a game release.
                    case "4.3.86.0":
                        return "4.3.86.0 / Unknown SafeDisc version";
                    default:
                        return "Unknown Version " + version;
                }
            }

            return "(Unknown Version)";
        }
    }
}
