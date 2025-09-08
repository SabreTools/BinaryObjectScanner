using System;
using System.Collections.Generic;
using System.Text;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Content;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // TODO: Investigate SecuROM for Macintosh
    public class SecuROM : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Check if executable is a Securom PA Module
            var paModule = CheckProductActivation(exe);
            if (paModule != null)
            {
                return paModule;
            }
            
            // Get the matrosch section, if it exists
            if (exe.ContainsSection("matrosch", exact: true))
                return $"SecuROM Matroschka Package";

            // Get the rcpacker section, if it exists
            if (exe.ContainsSection("rcpacker", exact: true))
                return $"SecuROM Release Control";

            if (exe.ContainsSection(".dsstext", exact: true))
                return $"SecuROM 8.03.03+";

            // Get the .securom section, if it exists
            if (exe.ContainsSection(".securom", exact: true))
                return $"SecuROM {GetV7Version(exe)}";

            // Get the .sll section, if it exists
            if (exe.ContainsSection(".sll", exact: true))
                return $"SecuROM SLL Protected (for SecuROM v8.x)";

            // Search after the last section
            if (exe.OverlayStrings != null)
            {
                if (exe.OverlayStrings.Exists(s => s == "AddD"))
                    return $"SecuROM {GetV4Version(exe)}";
            }

            // Get the sections 5+, if they exist (example names: .fmqyrx, .vcltz, .iywiak)
            var sections = exe.Model.SectionTable ?? [];
            for (int i = 4; i < sections.Length; i++)
            {
                var nthSection = sections[i];
                if (nthSection == null)
                    continue;

                string nthSectionName = Encoding.ASCII.GetString(nthSection.Name ?? []).TrimEnd('\0');
                if (nthSectionName != ".idata" && nthSectionName != ".rsrc")
                {
                    var nthSectionData = exe.GetFirstSectionData(nthSectionName);
                    if (nthSectionData == null)
                        continue;

                    var matchers = new List<ContentMatchSet>
                    {
                        // (char)0xCA + (char)0xDD + (char)0xDD + (char)0xAC + (char)0x03
                        new(new byte?[] { 0xCA, 0xDD, 0xDD, 0xAC, 0x03 }, GetV5Version, "SecuROM"),
                    };

                    var match = MatchUtil.GetFirstMatch(file, nthSectionData, matchers, includeDebug);
                    if (!string.IsNullOrEmpty(match))
                        return match;
                }
            }

            // Get the .rdata section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                // Both have the identifier found within `.rdata` but the version is within `.data`
                if (strs.Exists(s => s.Contains("/secuexp")))
                    return $"SecuROM {GetV8WhiteLabelVersion(exe)} (White Label)";
                else if (strs.Exists(s => s.Contains("SecuExp.exe")))
                    return $"SecuROM {GetV8WhiteLabelVersion(exe)} (White Label)";
            }

            // Get the .cms_d and .cms_t sections, if they exist -- TODO: Confirm if both are needed or either/or is fine
            if (exe.ContainsSection(".cmd_d", true) || exe.ContainsSection(".cms_t", true))
                return $"SecuROM 1-3";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // TODO: Verify if these are OR or AND
                new(new FilePathMatch("CMS16.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS_95.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS_NT.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS32_95.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS32_NT.DLL"), "SecuROM"),

                // TODO: Verify if these are OR or AND
                new(new FilePathMatch("SINTF32.DLL"), "SecuROM New"),
                new(new FilePathMatch("SINTF16.DLL"), "SecuROM New"),
                new(new FilePathMatch("SINTFNT.DLL"), "SecuROM New"),

                // TODO: Find more samples of this for different versions
                new(new List<PathMatch>
                {
                    new FilePathMatch("securom_v7_01.bak"),
                    new FilePathMatch("securom_v7_01.dat"),
                    new FilePathMatch("securom_v7_01.tmp"),
                }, "SecuROM 7.01"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("CMS16.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS_95.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS_NT.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS32_95.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS32_NT.DLL"), "SecuROM"),

                new(new FilePathMatch("SINTF32.DLL"), "SecuROM New"),
                new(new FilePathMatch("SINTF16.DLL"), "SecuROM New"),
                new(new FilePathMatch("SINTFNT.DLL"), "SecuROM New"),

                new(new FilePathMatch("securom_v7_01.bak"), "SecuROM 7.01"),
                new(new FilePathMatch("securom_v7_01.dat"), "SecuROM 7.01"),
                new(new FilePathMatch("securom_v7_01.tmp"), "SecuROM 7.01"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        private static string GetV4Version(PortableExecutable exe)
        {
            int index = 8; // Begin reading after "AddD"
            char major = (char)exe.OverlayData![index];
            index += 2;

            string minor = Encoding.ASCII.GetString(exe.OverlayData, index, 2);
            index += 3;

            string patch = Encoding.ASCII.GetString(exe.OverlayData, index, 2);
            index += 3;

            string revision = Encoding.ASCII.GetString(exe.OverlayData, index, 4);

            if (!char.IsNumber(major))
                return "(very old, v3 or less)";

            return $"{major}.{minor}.{patch}.{revision}";
        }

        private static string? GetV5Version(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

            int index = positions[0] + 8; // Begin reading after "ÊÝÝ¬"
            byte major = (byte)(fileContent[index] & 0x0F);
            index += 2;

            byte[] minor = new byte[2];
            minor[0] = (byte)(fileContent[index] ^ 36);
            index++;
            minor[1] = (byte)(fileContent[index] ^ 28);
            index += 2;

            byte[] patch = new byte[2];
            patch[0] = (byte)(fileContent[index] ^ 42);
            index++;
            patch[1] = (byte)(fileContent[index] ^ 8);
            index += 2;

            byte[] revision = new byte[4];
            revision[0] = (byte)(fileContent[index] ^ 16);
            index++;
            revision[1] = (byte)(fileContent[index] ^ 116);
            index++;
            revision[2] = (byte)(fileContent[index] ^ 34);
            index++;
            revision[3] = (byte)(fileContent[index] ^ 22);

            if (major == 0 || major > 9)
                return string.Empty;

            return $"{major}.{minor[0]}{minor[1]}.{patch[0]}{patch[1]}.{revision[0]}{revision[1]}{revision[2]}{revision[3]}";
        }

        // These live in the MS-DOS stub, for some reason
        private static string GetV7Version(PortableExecutable exe)
        {
            // If SecuROM is stripped, the MS-DOS stub might be shorter.
            // We then know that SecuROM -was- there, but we don't know what exact version.
            if (exe.StubExecutableData == null)
                return "7 remnants";

            //SecuROM 7 new and 8 -- 64 bytes for DOS stub, 236 bytes in total
            int index = 172;
            if (exe.StubExecutableData.Length >= 176 && exe.StubExecutableData[index + 3] == 0x5C)
            {
                int major = exe.StubExecutableData[index + 0] ^ 0xEA;
                int minor = exe.StubExecutableData[index + 1] ^ 0x2C;
                int patch = exe.StubExecutableData[index + 2] ^ 0x08;

                return $"{major}.{minor:00}.{patch:0000}";
            }

            // SecuROM 7 old -- 64 bytes for DOS stub, 122 bytes in total
            index = 58;
            if (exe.StubExecutableData.Length >= 62)
            {
                int minor = exe.StubExecutableData[index + 0] ^ 0x10;
                int patch = exe.StubExecutableData[index + 1] ^ 0x10;

                //return "7.01-7.10"
                return $"7.{minor:00}.{patch:0000}";
            }

            // If SecuROM is stripped, the MS-DOS stub might be shorter.
            // We then know that SecuROM -was- there, but we don't know what exact version.
            return "7 remnants";
        }

        private static string GetV8WhiteLabelVersion(PortableExecutable exe)
        {
            // Get the .data/DATA section, if it exists
            var dataSectionRaw = exe.GetFirstSectionData(".data") ?? exe.GetFirstSectionData("DATA");
            if (dataSectionRaw == null)
                return "8";

            // Search .data for the version indicator
            var matcher = new ContentMatch(
            [
                0x29, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                0x82, 0xD8, 0x0C, 0xAC
            ]);

            int position = matcher.Match(dataSectionRaw);

            // If we can't find the string, we default to generic
            if (position < 0)
                return "8";

            int major = dataSectionRaw[position + 0xAC + 0] ^ 0xCA;
            int minor = dataSectionRaw[position + 0xAC + 1] ^ 0x39;
            int patch = dataSectionRaw[position + 0xAC + 2] ^ 0x51;

            return $"{major}.{minor:00}.{patch:0000}";
        }


        /// <summary>
        /// Helper method to check if a given PortableExecutable is a SecuROM PA module.
        /// </summary>
        private static string? CheckProductActivation(PortableExecutable exe)
        {
            var name = exe.FileDescription;
            if (name.OptionalContains("SecuROM PA"))
                return $"SecuROM Product Activation v{exe.GetInternalVersion()}";

            name = exe.InternalName;
            
            // Checks if ProductName isn't drEAm to organize custom module checks at the end.
            if (name.OptionalEquals("paul.dll", StringComparison.OrdinalIgnoreCase) ^ exe.ProductName.OptionalEquals("drEAm"))
                    return $"SecuROM Product Activation v{exe.GetInternalVersion()}";
            else if (name.OptionalEquals("paul_dll_activate_and_play.dll"))
                return $"SecuROM Product Activation v{exe.GetInternalVersion()}";
            else if (name.OptionalEquals("paul_dll_preview_and_review.dll"))
                return $"SecuROM Product Activation v{exe.GetInternalVersion()}";

            name = exe.OriginalFilename;
            if (name.OptionalEquals("paul_dll_activate_and_play.dll"))
                return $"SecuROM Product Activation v{exe.GetInternalVersion()}";

            name = exe.ProductName;
            if (name.OptionalContains("SecuROM Activate & Play"))
                return $"SecuROM Product Activation v{exe.GetInternalVersion()}";

            // Custom Module Checks
            
            if (exe.ProductName.OptionalEquals("drEAm")) 
                return $"SecuROM Product Activation v{exe.GetInternalVersion()} - EA Game Authorization Management";
            
            // Fallback for PA if none of the above occur, in the case of companies that used their own modified PA
            // variants. PiD refers to this as "SecuROM Modified PA Module".
            // Found in Redump entries 111997 (paul.dll) and 56373+56374 (AurParticleSystem.dll). The developers of 
            // both, Softstar and Aurogon respectively(?), seem to have some connection, and use similar-looking
            // modified PA. It probably has its own name like EA's GAM, but I don't currently know what that would be. 
            // Regardless, even if these are given their own named variant later, this check should remain in order to
            // catch other modified PA variants (this would have also caught EA GAM, for example) and to match PiD's 
            // detection abilities.
            // TODO: Decide whether to get internal version or not in the future.
            name = exe.ExportTable?.ExportNameTable?.Strings?[0];
            if (name.OptionalEquals("drm_pagui_doit"))
                return $"SecuROM Product Activation - Modified";
            
            return null;
        }
    }
}
