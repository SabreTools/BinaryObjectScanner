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
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (name.OptionalContains("SecuROM PA"))
                return $"SecuROM Product Activation v{pex.GetInternalVersion()}";

            name = pex.InternalName;
            if (name.OptionalEquals("paul.dll"))
                return $"SecuROM Product Activation v{pex.GetInternalVersion()}";
            else if (name.OptionalEquals("paul_dll_activate_and_play.dll"))
                return $"SecuROM Product Activation v{pex.GetInternalVersion()}";
            else if (name.OptionalEquals("paul_dll_preview_and_review.dll"))
                return $"SecuROM Product Activation v{pex.GetInternalVersion()}";

            name = pex.OriginalFilename;
            if (name.OptionalEquals("paul_dll_activate_and_play.dll"))
                return $"SecuROM Product Activation v{pex.GetInternalVersion()}";

            name = pex.ProductName;
            if (name.OptionalContains("SecuROM Activate & Play"))
                return $"SecuROM Product Activation v{pex.GetInternalVersion()}";

            // Get the matrosch section, if it exists
            if (pex.ContainsSection("matrosch", exact: true))
                return $"SecuROM Matroschka Package";

            if (pex.ContainsSection(".dsstext", exact: true))
                return $"SecuROM 8.03.03+";

            // Get the .securom section, if it exists
            if (pex.ContainsSection(".securom", exact: true))
                return $"SecuROM {GetV7Version(pex)}";

            // Get the .sll section, if it exists
            if (pex.ContainsSection(".sll", exact: true))
                return $"SecuROM SLL Protected (for SecuROM v8.x)";

            // Search after the last section
            if (pex.OverlayStrings != null)
            {
                if (pex.OverlayStrings.Exists(s => s == "AddD"))
                    return $"SecuROM {GetV4Version(pex)}";
            }

            // Get the sections 5+, if they exist (example names: .fmqyrx, .vcltz, .iywiak)
            for (int i = 4; i < sections.Length; i++)
            {
                var nthSection = sections[i];
                if (nthSection == null)
                    continue;

                string nthSectionName = Encoding.ASCII.GetString(nthSection.Name ?? []).TrimEnd('\0');
                if (nthSectionName != ".idata" && nthSectionName != ".rsrc")
                {
                    var nthSectionData = pex.GetFirstSectionData(nthSectionName);
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
            var strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                // Both have the identifier found within `.rdata` but the version is within `.data`
                if (strs.Exists(s => s.Contains("/secuexp")))
                    return $"SecuROM {GetV8WhiteLabelVersion(pex)} (White Label)";
                else if (strs.Exists(s => s.Contains("SecuExp.exe")))
                    return $"SecuROM {GetV8WhiteLabelVersion(pex)} (White Label)";
            }

            // Get the .cms_d and .cms_t sections, if they exist -- TODO: Confirm if both are needed or either/or is fine
            if (pex.ContainsSection(".cmd_d", true) || pex.ContainsSection(".cms_t", true))
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

        private static string GetV4Version(PortableExecutable pex)
        {
            int index = 8; // Begin reading after "AddD"
            char major = (char)pex.OverlayData![index];
            index += 2;

            string minor = Encoding.ASCII.GetString(pex.OverlayData, index, 2);
            index += 3;

            string patch = Encoding.ASCII.GetString(pex.OverlayData, index, 2);
            index += 3;

            string revision = Encoding.ASCII.GetString(pex.OverlayData, index, 4);

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
        private static string GetV7Version(PortableExecutable pex)
        {
            // If SecuROM is stripped, the MS-DOS stub might be shorter.
            // We then know that SecuROM -was- there, but we don't know what exact version.
            if (pex.StubExecutableData == null)
                return "7 remnants";

            //SecuROM 7 new and 8 -- 64 bytes for DOS stub, 236 bytes in total
            int index = 172;
            if (pex.StubExecutableData.Length >= 176 && pex.StubExecutableData[index + 3] == 0x5C)
            {
                int major = pex.StubExecutableData[index + 0] ^ 0xEA;
                int minor = pex.StubExecutableData[index + 1] ^ 0x2C;
                int patch = pex.StubExecutableData[index + 2] ^ 0x08;

                return $"{major}.{minor:00}.{patch:0000}";
            }

            // SecuROM 7 old -- 64 bytes for DOS stub, 122 bytes in total
            index = 58;
            if (pex.StubExecutableData.Length >= 62)
            {
                int minor = pex.StubExecutableData[index + 0] ^ 0x10;
                int patch = pex.StubExecutableData[index + 1] ^ 0x10;

                //return "7.01-7.10"
                return $"7.{minor:00}.{patch:0000}";
            }

            // If SecuROM is stripped, the MS-DOS stub might be shorter.
            // We then know that SecuROM -was- there, but we don't know what exact version.
            return "7 remnants";
        }

        private static string GetV8WhiteLabelVersion(PortableExecutable pex)
        {
            // Get the .data/DATA section, if it exists
            var dataSectionRaw = pex.GetFirstSectionData(".data") ?? pex.GetFirstSectionData("DATA");
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
    }
}
