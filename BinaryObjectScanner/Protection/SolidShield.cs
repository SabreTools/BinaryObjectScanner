using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Content;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // TODO: Not matching all SolidShield Wrapper v1 (See JackKeane)
    // TODO: Not matching all SolidShield Wrapper v1 (See NFS11)
    public class SolidShield : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // TODO: Investigate ".pseudo" section found in "tvdm.dll" in Redump entry 68166.

            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (name.OptionalStartsWith("DVM Library", StringComparison.OrdinalIgnoreCase))
                return $"SolidShield {pex.GetInternalVersion()}";

            else if (name.OptionalStartsWith("Solidshield Activation Library", StringComparison.OrdinalIgnoreCase))
                return $"SolidShield Core.dll {pex.GetInternalVersion()}";

            else if (name.OptionalStartsWith("Activation Manager", StringComparison.OrdinalIgnoreCase))
                return $"SolidShield Activation Manager Module {GetInternalVersion(pex)}";

            // Found in "tvdm.dll" in Redump entry 68166.
            else if (name.OptionalStartsWith("Solidshield Library", StringComparison.OrdinalIgnoreCase))
                return $"SolidShield {GetInternalVersion(pex)}";

            name = pex.ProductName;
            if (name.OptionalStartsWith("Solidshield Activation Library", StringComparison.OrdinalIgnoreCase))
                return $"SolidShield Core.dll {pex.GetInternalVersion()}";

            else if (name.OptionalStartsWith("Solidshield Library", StringComparison.OrdinalIgnoreCase))
                return $"SolidShield Core.dll {pex.GetInternalVersion()}";

            else if (name.OptionalStartsWith("Activation Manager", StringComparison.OrdinalIgnoreCase))
                return $"SolidShield Activation Manager Module {GetInternalVersion(pex)}";

            // Found in "tvdm.dll" in Redump entry 68166.
            else if (name.OptionalStartsWith("Solidshield Library", StringComparison.OrdinalIgnoreCase))
                return $"SolidShield {GetInternalVersion(pex)}";

            // Get the .init section, if it exists
            if (pex.ContainsSection(".init"))
            {
                var initData = pex.GetFirstSectionData(".init");
                if (initData != null)
                {
                    var matchers = new List<ContentMatchSet>
                {
                    // (char)0xAD + (char)0xDE + (char)0xFE + (char)0xCA
                    new(new byte?[] { 0xAD, 0xDE, 0xFE, 0xCA }, GetVersionPlusTages, "SolidShield"),

                    // (char)0xEF + (char)0xBE + (char)0xAD + (char)0xDE
                    new(new byte?[] { 0xEF, 0xBE, 0xAD, 0xDE }, GetExeWrapperVersion, "SolidShield EXE Wrapper"),
                };

                    var match = MatchUtil.GetFirstMatch(file, initData, matchers, includeDebug);
                    if (!string.IsNullOrEmpty(match))
                        return match;
                }
            }

            // Get the wrapper resource, if it exists
            if (pex.FindResourceByNamedType("BIN, IDR_SGT").Count > 0)
                return "SolidShield EXE Wrapper v1";

            // Search the last two available sections
            for (int i = Math.Max(sections.Length - 2, 0); i < sections.Length; i++)
            {
                // Get the nth section strings, if they exist
                var strs = pex.GetSectionStrings(i);
                if (strs != null)
                {
                    var str = strs.Find(s => s.Contains("Solidshield "));
                    if (str != null)
                        return $"SolidShield EXE Wrapper {str.Substring("Solidshield ".Length)}";
                }
            }

            // Get the import directory table, if it exists
            if (pex.Model.ImportTable?.ImportDirectoryTable != null)
            {
                if (Array.Exists(pex.Model.ImportTable.ImportDirectoryTable, idte => idte?.Name == "dvm.dll"))
                    return "SolidShield EXE Wrapper v1";

                if (Array.Exists(pex.Model.ImportTable.ImportDirectoryTable, idte => idte?.Name == "activation.x86.dll"))
                    return "SolidShield EXE Wrapper v2";

                if (Array.Exists(pex.Model.ImportTable.ImportDirectoryTable, idte => idte?.Name == "activation.x64.dll"))
                    return "SolidShield EXE Wrapper v2";
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Redump entry 68166.
                new(new FilePathMatch("tdvm.dll"), "SolidShield"),
                new(new FilePathMatch("tdvm.vds"), "SolidShield"),
                new(new FilePathMatch("vfs20.dll"), "SolidShield"),

                new(new FilePathMatch("dvm.dll"), "SolidShield"),
                new(new FilePathMatch("hc.dll"), "SolidShield"),
                new(new FilePathMatch("solidshield-cd.dll"), "SolidShield"),
                new(new FilePathMatch("c11prot.dll"), "SolidShield"),
            };

            // TODO: Verify if these are OR or AND
            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Redump entry 68166.
                new(new FilePathMatch("tdvm.dll"), "SolidShield"),
                new(new FilePathMatch("tdvm.vds"), "SolidShield"),
                new(new FilePathMatch("vfs20.dll"), "SolidShield"),

                new(new FilePathMatch("dvm.dll"), "SolidShield"),
                new(new FilePathMatch("hc.dll"), "SolidShield"),
                new(new FilePathMatch("solidshield-cd.dll"), "SolidShield"),
                new(new FilePathMatch("c11prot.dll"), "SolidShield"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        private static string? GetExeWrapperVersion(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

            int position = positions[0];

            byte[] id1 = new byte[3];
            Array.Copy(fileContent, position + 5, id1, 0, 3);
            byte[] id2 = new byte[4];
            Array.Copy(fileContent, position + 16, id2, 0, 3);

            if (id1[0] == 0x00 && id1[1] == 0x00 && id1[2] == 0x00
                && id2[0] == 0x00 && id2[1] == 0x10 && id2[2] == 0x00 && id2[3] == 0x00)
            {
                return "v1";
            }
            else if (id1[0] == 0x2E && id1[1] == 0x6F && id1[2] == 0x26
                && id2[0] == 0xDB && id2[1] == 0xC5 && id2[2] == 0x20 && id2[3] == 0x3A) // [0xDB, 0xC5, 0x20, 0x3A, 0xB9]
            {
                return "v2"; // TODO: Verify against other SolidShield 2 discs
            }

            return string.Empty;
        }

        private static string? GetVersionPlusTages(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

            int position = positions[0];

            byte[] id1 = new byte[3];
            Array.Copy(fileContent, position + 4, id1, 0, 3);
            byte[] id2 = new byte[4];
            Array.Copy(fileContent, position + 15, id2, 0, 3);

            if ((fileContent[position + 3] == 0x04 || fileContent[position + 3] == 0x05)
                && id1[0] == 0x00 && id1[1] == 0x00 && id1[2] == 0x00
                && id2[0] == 0x00 && id2[1] == 0x10 && id2[2] == 0x00 && id2[3] == 0x00)
            {
                return "2 (SolidShield v2 EXE Wrapper)";
            }
            else if (id1[0] == 0x00 && id1[1] == 0x00 && id1[2] == 0x00
                && id2[0] == 0x00 && id2[1] == 0x10 && id2[2] == 0x00 && id2[3] == 0x00)
            {
                // "T" + (char)0x00 + "a" + (char)0x00 + "g" + (char)0x00 + "e" + (char)0x00 + "s" + (char)0x00 + "S" + (char)0x00 + "e" + (char)0x00 + "t" + (char)0x00 + "u" + (char)0x00 + "p" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "0" + (char)0x00 + (char)0x8 + (char)0x00 + (char)0x1 + (char)0x0 + "F" + (char)0x00 + "i" + (char)0x00 + "l" + (char)0x00 + "e" + (char)0x00 + "V" + (char)0x00 + "e" + (char)0x00 + "r" + (char)0x00 + "s" + (char)0x00 + "i" + (char)0x00 + "o" + (char)0x00 + "n" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00
                byte?[] check2 =
                [
                    0x54, 0x61, 0x67, 0x65, 0x73, 0x53, 0x65, 0x74,
                    0x75, 0x70, 0x30, 0x08, 0x01, 0x00, 0x46, 0x69,
                    0x6C, 0x65, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F,
                    0x6E, 0x00, 0x00, 0x00, 0x00
                ];

                int position2 = fileContent.FirstPosition(check2);
                if (position2 > -1)
                {
                    position2--; // TODO: Verify this subtract
                    return $"2 + Tagès {fileContent[position2 + 0x38]}.{fileContent[position2 + 0x38 + 4]}.{fileContent[position2 + 0x38 + 8]}.{fileContent[position + 0x38 + 12]}";
                }
                else
                {
                    return "2 (SolidShield v2 EXE Wrapper)";
                }
            }

            return string.Empty;
        }

        private static string GetInternalVersion(PortableExecutable pex)
        {
            var companyName = pex.CompanyName?.ToLowerInvariant();
            if (!string.IsNullOrEmpty(companyName) && (companyName!.Contains("solidshield") || companyName.Contains("tages")))
                return pex.GetInternalVersion() ?? string.Empty;

            return string.Empty;
        }
    }
}
