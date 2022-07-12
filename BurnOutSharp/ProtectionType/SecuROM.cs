using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Investigate SecuROM for Macintosh
    public class SecuROM : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (!string.IsNullOrWhiteSpace(name) && name.Contains("SecuROM PA"))
                return $"SecuROM PA v{Utilities.GetInternalVersion(pex)}";

            // Get the matrosch section, if it exists
            bool matroschSection = pex.ContainsSection("matrosch", exact: true);
            if (matroschSection)
                return $"SecuROM Matroschka Package";

            bool dsstextSection = pex.ContainsSection(".dsstext", exact: true);
            if (dsstextSection)
                return $"SecuROM 8.03.03+";

            // Get the .securom section, if it exists
            bool securomSection = pex.ContainsSection(".securom", exact: true);
            if (securomSection)
                return $"SecuROM {GetV7Version(pex)}";

            // Get the .sll section, if it exists
            bool sllSection = pex.ContainsSection(".sll", exact: true);
            if (sllSection)
                return $"SecuROM SLL Protected (for SecuROM v8.x)";

            // Search after the last section
            if (pex.OverlayRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // AddD + (char)0x03 + (char)0x00 + (char)0x00 + (char)0x00)
                    new ContentMatchSet(new byte?[] { 0x41, 0x64, 0x64, 0x44, 0x03, 0x00, 0x00, 0x00 }, GetV4Version, "SecuROM"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.OverlayRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the sections 5+, if they exist (example names: .fmqyrx, .vcltz, .iywiak)
            for (int i = 4; i < sections.Length; i++)
            {
                var nthSection = sections[i];
                string nthSectionName = nthSection.NameString;
                if (nthSection != null && nthSectionName != ".idata" && nthSectionName != ".rsrc")
                {
                    var nthSectionData = pex.ReadRawSection(nthSectionName, first: true);
                    if (nthSectionData != null)
                    {
                        var matchers = new List<ContentMatchSet>
                        {
                            // (char)0xCA + (char)0xDD + (char)0xDD + (char)0xAC + (char)0x03
                            new ContentMatchSet(new byte?[] { 0xCA, 0xDD, 0xDD, 0xAC, 0x03 }, GetV5Version, "SecuROM"),
                        };

                        string match = MatchUtil.GetFirstMatch(file, nthSectionData, matchers, includeDebug);
                        if (!string.IsNullOrWhiteSpace(match))
                            return match;
                    }
                }
            }

            // Get the .rdata section, if it exists
            if (pex.ResourceDataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // drm_pagui_doit -- shortened from "_and_play.dll + (char)0x00 + drm_pagui_doit"
                    new ContentMatchSet(new byte?[]
                    {
                        0x64, 0x72, 0x6D, 0x5F, 0x70, 0x61, 0x67, 0x75,
                        0x69, 0x5F, 0x64, 0x6F, 0x69, 0x74
                    }, Utilities.GetInternalVersion, "SecuROM Product Activation"),

                    // TODO: Re-enable this one if the above undermatches somehow
                    // // S + (char)0x00 + e + (char)0x00 + c + (char)0x00 + u + (char)0x00 + R + (char)0x00 + O + (char)0x00 + M + (char)0x00 +   + (char)0x00 + P + (char)0x00 + A + (char)0x00
                    // new ContentMatchSet(new byte?[]
                    // {
                    //     0x53, 0x00, 0x65, 0x00, 0x63, 0x00, 0x75, 0x00,
                    //     0x52, 0x00, 0x4F, 0x00, 0x4D, 0x00, 0x20, 0x00,
                    //     0x50, 0x00, 0x41, 0x00
                    // }, Utilities.GetInternalVersion, "SecuROM Product Activation"),

                    // The following 2 checks are unique:
                    // Both have the identifier found within `.rdata` but the version is within `.data`
                    // So, we use the placeholder "WHITELABEL" to see if we got a match

                    // /secuexp
                    new ContentMatchSet(new byte?[] { 0x2F, 0x73, 0x65, 0x63, 0x75, 0x65, 0x78, 0x70 }, "WHITELABEL"),

                    // SecuExp.exe
                    new ContentMatchSet(new byte?[] { 0x53, 0x65, 0x63, 0x75, 0x45, 0x78, 0x70 }, "WHITELABEL"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.ResourceDataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                {
                    if (match.StartsWith("WHITELABEL"))
                        return $"SecuROM {GetV8WhiteLabelVersion(pex)} (White Label)";

                    return match;
                }
            }

            // Get the .cms_d and .cms_t sections, if they exist -- TODO: Confirm if both are needed or either/or is fine
            bool cmsdSection = pex.ContainsSection(".cmd_d", true);
            bool cmstSection = pex.ContainsSection(".cms_t", true);
            if (cmsdSection || cmstSection)
                return $"SecuROM 1-3";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // TODO: Verify if these are OR or AND
                new PathMatchSet(new PathMatch("CMS16.DLL", useEndsWith: true), "SecuROM"),
                new PathMatchSet(new PathMatch("CMS_95.DLL", useEndsWith: true), "SecuROM"),
                new PathMatchSet(new PathMatch("CMS_NT.DLL", useEndsWith: true), "SecuROM"),
                new PathMatchSet(new PathMatch("CMS32_95.DLL", useEndsWith: true), "SecuROM"),
                new PathMatchSet(new PathMatch("CMS32_NT.DLL", useEndsWith: true), "SecuROM"),

                // TODO: Verify if these are OR or AND
                new PathMatchSet(new PathMatch("SINTF32.DLL", useEndsWith: true), "SecuROM New"),
                new PathMatchSet(new PathMatch("SINTF16.DLL", useEndsWith: true), "SecuROM New"),
                new PathMatchSet(new PathMatch("SINTFNT.DLL", useEndsWith: true), "SecuROM New"),

                // TODO: Find more samples of this for different versions
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("securom_v7_01.bak", useEndsWith: true),
                    new PathMatch("securom_v7_01.dat", useEndsWith: true),
                    new PathMatch("securom_v7_01.tmp", useEndsWith: true),
                }, "SecuROM 7.01"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("CMS16.DLL", useEndsWith: true), "SecuROM"),
                new PathMatchSet(new PathMatch("CMS_95.DLL", useEndsWith: true), "SecuROM"),
                new PathMatchSet(new PathMatch("CMS_NT.DLL", useEndsWith: true), "SecuROM"),
                new PathMatchSet(new PathMatch("CMS32_95.DLL", useEndsWith: true), "SecuROM"),
                new PathMatchSet(new PathMatch("CMS32_NT.DLL", useEndsWith: true), "SecuROM"),

                new PathMatchSet(new PathMatch("SINTF32.DLL", useEndsWith: true), "SecuROM New"),
                new PathMatchSet(new PathMatch("SINTF16.DLL", useEndsWith: true), "SecuROM New"),
                new PathMatchSet(new PathMatch("SINTFNT.DLL", useEndsWith: true), "SecuROM New"),

                new PathMatchSet(new PathMatch("securom_v7_01.bak", useEndsWith: true), "SecuROM 7.01"),
                new PathMatchSet(new PathMatch("securom_v7_01.dat", useEndsWith: true), "SecuROM 7.01"),
                new PathMatchSet(new PathMatch("securom_v7_01.tmp", useEndsWith: true), "SecuROM 7.01"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string GetV4Version(string file, byte[] fileContent, List<int> positions)
        {
            int index = positions[0] + 8; // Begin reading after "AddD"
            char version = (char)fileContent[index];
            index += 2;

            string subVersion = Encoding.ASCII.GetString(fileContent, index, 2);
            index += 3;

            string subSubVersion = Encoding.ASCII.GetString(fileContent, index, 2);
            index += 3;

            string subSubSubVersion = Encoding.ASCII.GetString(fileContent, index, 4);

            if (!char.IsNumber(version))
                return "(very old, v3 or less)";

            return $"{version}.{subVersion}.{subSubVersion}.{subSubSubVersion}";
        }

        public static string GetV5Version(string file, byte[] fileContent, List<int> positions)
        {
            int index = positions[0] + 8; // Begin reading after "ÊÝÝ¬"
            byte version = (byte)(fileContent[index] & 0x0F);
            index += 2;

            byte[] subVersion = new byte[2];
            subVersion[0] = (byte)(fileContent[index] ^ 36);
            index++;
            subVersion[1] = (byte)(fileContent[index] ^ 28);
            index += 2;

            byte[] subSubVersion = new byte[2];
            subSubVersion[0] = (byte)(fileContent[index] ^ 42);
            index++;
            subSubVersion[0] = (byte)(fileContent[index] ^ 8);
            index += 2;

            byte[] subSubSubVersion = new byte[4];
            subSubSubVersion[0] = (byte)(fileContent[index] ^ 16);
            index++;
            subSubSubVersion[1] = (byte)(fileContent[index] ^ 116);
            index++;
            subSubSubVersion[2] = (byte)(fileContent[index] ^ 34);
            index++;
            subSubSubVersion[3] = (byte)(fileContent[index] ^ 22);

            if (version == 0 || version > 9)
                return string.Empty;

            return $"{version}.{subVersion[0]}{subVersion[1]}.{subSubVersion[0]}{subSubVersion[1]}.{subSubSubVersion[0]}{subSubSubVersion[1]}{subSubSubVersion[2]}{subSubSubVersion[3]}";
        }

        // These live in the MS-DOS stub, for some reason
        private static string GetV7Version(PortableExecutable pex)
        {
            int index = 172; // 64 bytes for DOS stub, 236 bytes in total
            byte[] bytes = new ReadOnlySpan<byte>(pex.DOSStubHeader.ExecutableData, index, 4).ToArray();

            //SecuROM 7 new and 8
            if (bytes[3] == 0x5C) // if (bytes[0] == 0xED && bytes[3] == 0x5C {
            {
                return $"{bytes[0] ^ 0xEA}.{bytes[1] ^ 0x2C:00}.{bytes[2] ^ 0x8:0000}";
            }

            // SecuROM 7 old
            else
            {
                index = 58; // 64 bytes for DOS stub, 122 bytes in total
                bytes = new ReadOnlySpan<byte>(pex.DOSStubHeader.ExecutableData, index, 2).ToArray();
                return $"7.{bytes[0] ^ 0x10:00}.{bytes[1] ^ 0x10:0000}"; //return "7.01-7.10"
            }
        }

        private static string GetV8WhiteLabelVersion(PortableExecutable pex)
        {
            // If we don't have a data section, we default to generic
            if (pex.DataSectionRaw == null)
                return "8";

            // Search .data for the version indicator
            var matcher = new ContentMatch(new byte?[]
            {
                0x29, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                0x82, 0xD8, 0x0C, 0xAC
            });

            (bool success, int position) = matcher.Match(pex.DataSectionRaw);

            // If we can't find the string, we default to generic
            if (!success)
                return "8";

            byte[] bytes = new ReadOnlySpan<byte>(pex.DataSectionRaw, position + 0xAC, 3).ToArray();
            return $"{bytes[0] ^ 0xCA}.{bytes[1] ^ 0x39:00}.{bytes[2] ^ 0x51:0000}";
        }
    }
}
