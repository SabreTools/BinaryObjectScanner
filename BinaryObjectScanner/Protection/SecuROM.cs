using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // TODO: Investigate SecuROM for Macintosh
    public class SecuROM : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (name?.Contains("SecuROM PA") == true)
                return $"SecuROM Product Activation v{pex.GetInternalVersion()}";

            name = pex.OriginalFilename;
            if (name?.Equals("paul_dll_activate_and_play.dll") == true)
                return $"SecuROM Product Activation v{pex.GetInternalVersion()}";

            name = pex.ProductName;
            if (name?.Contains("SecuROM Activate & Play") == true)
                return $"SecuROM Product Activation v{pex.GetInternalVersion()}";

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
            if (pex.OverlayStrings != null)
            {
                if (pex.OverlayStrings.Any(s => s == "AddD"))
                    return $"SecuROM {GetV4Version(pex)}";
            }

            // Get the sections 5+, if they exist (example names: .fmqyrx, .vcltz, .iywiak)
            for (int i = 4; i < sections.Length; i++)
            {
                var nthSection = sections[i];
                if (nthSection == null)
                    continue;

#if NET20 || NET35 || NET40 || NET452
                string nthSectionName = Encoding.UTF8.GetString(nthSection.Name ?? []).TrimEnd('\0');
#else
                string nthSectionName = Encoding.UTF8.GetString(nthSection.Name ?? Array.Empty<byte>()).TrimEnd('\0');
#endif
                if (nthSectionName != ".idata" && nthSectionName != ".rsrc")
                {
                    var nthSectionData = pex.GetFirstSectionData(nthSectionName);
                    if (nthSectionData == null)
                        continue;

                    var matchers = new List<ContentMatchSet>
                    {
                        // (char)0xCA + (char)0xDD + (char)0xDD + (char)0xAC + (char)0x03
                        new ContentMatchSet(new byte?[] { 0xCA, 0xDD, 0xDD, 0xAC, 0x03 }, GetV5Version, "SecuROM"),
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
                if (strs.Any(s => s.Contains("/secuexp")))
                    return $"SecuROM {GetV8WhiteLabelVersion(pex)} (White Label)";
                else if (strs.Any(s => s.Contains("SecuExp.exe")))
                    return $"SecuROM {GetV8WhiteLabelVersion(pex)} (White Label)";
            }

            // Get the .cms_d and .cms_t sections, if they exist -- TODO: Confirm if both are needed or either/or is fine
            bool cmsdSection = pex.ContainsSection(".cmd_d", true);
            bool cmstSection = pex.ContainsSection(".cms_t", true);
            if (cmsdSection || cmstSection)
                return $"SecuROM 1-3";

            return null;
        }

        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
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
        public string? CheckFilePath(string path)
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

        private static string GetV4Version(PortableExecutable pex)
        {
            int index = 8; // Begin reading after "AddD"
            char version = (char)pex.OverlayData![index];
            index += 2;

            string subVersion = Encoding.ASCII.GetString(pex.OverlayData, index, 2);
            index += 3;

            string subSubVersion = Encoding.ASCII.GetString(pex.OverlayData, index, 2);
            index += 3;

            string subSubSubVersion = Encoding.ASCII.GetString(pex.OverlayData, index, 4);

            if (!char.IsNumber(version))
                return "(very old, v3 or less)";

            return $"{version}.{subVersion}.{subSubVersion}.{subSubSubVersion}";
        }

        public static string? GetV5Version(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

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
#if NETFRAMEWORK
            byte[] bytes = new byte[4];
            Array.Copy(pex.StubExecutableData, index, bytes, 0, 4);
#else
            byte[] bytes = new ReadOnlySpan<byte>(pex.StubExecutableData, index, 4).ToArray();
#endif

            //SecuROM 7 new and 8
            if (bytes[3] == 0x5C) // if (bytes[0] == 0xED && bytes[3] == 0x5C {
            {
                return $"{bytes[0] ^ 0xEA}.{bytes[1] ^ 0x2C:00}.{bytes[2] ^ 0x8:0000}";
            }

            // SecuROM 7 old
            else
            {
                index = 58; // 64 bytes for DOS stub, 122 bytes in total
#if NETFRAMEWORK
                bytes = new byte[2];
                Array.Copy(pex.StubExecutableData, index, bytes, 0, 2);
#else
                bytes = new ReadOnlySpan<byte>(pex.StubExecutableData, index, 2).ToArray();
#endif
                return $"7.{bytes[0] ^ 0x10:00}.{bytes[1] ^ 0x10:0000}"; //return "7.01-7.10"
            }
        }

        private static string GetV8WhiteLabelVersion(PortableExecutable pex)
        {
            // Get the .data/DATA section, if it exists
            var dataSectionRaw = pex.GetFirstSectionData(".data") ?? pex.GetFirstSectionData("DATA");
            if (dataSectionRaw == null)
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

            (bool success, int position) = matcher.Match(dataSectionRaw);

            // If we can't find the string, we default to generic
            if (!success)
                return "8";

#if NETFRAMEWORK
                byte[] bytes = new byte[3];
                Array.Copy(dataSectionRaw, position + 0xAC, bytes, 0, 3);
#else
                byte[] bytes = new ReadOnlySpan<byte>(dataSectionRaw, position + 0xAC, 3).ToArray();
#endif

            return $"{bytes[0] ^ 0xCA}.{bytes[1] ^ 0x39:00}.{bytes[2] ^ 0x51:0000}";
        }
    }
}
