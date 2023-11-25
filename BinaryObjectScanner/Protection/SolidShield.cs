﻿using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.ProtectionType
{
    // TODO: Not matching all SolidShield Wrapper v1 (See JackKeane)
    // TODO: Not matching all SolidShield Wrapper v1 (See NFS11)
    public class SolidShield : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // TODO: Investigate ".pseudo" section found in "tvdm.dll" in Redump entry 68166.

            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.FileDescription;
            if (name?.StartsWith("DVM Library", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield {pex.GetInternalVersion()}";

            else if (name?.StartsWith("Solidshield Activation Library", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield Core.dll {pex.GetInternalVersion()}";

            else if (name?.StartsWith("Activation Manager", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield Activation Manager Module {GetInternalVersion(pex)}";

            // Found in "tvdm.dll" in Redump entry 68166.
            else if (name?.StartsWith("Solidshield Library", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield {GetInternalVersion(pex)}";

            name = pex.ProductName;
            if (name?.StartsWith("Solidshield Activation Library", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield Core.dll {pex.GetInternalVersion()}";

            else if (name?.StartsWith("Solidshield Library", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield Core.dll {pex.GetInternalVersion()}";

            else if (name?.StartsWith("Activation Manager", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield Activation Manager Module {GetInternalVersion(pex)}";

            // Found in "tvdm.dll" in Redump entry 68166.
            else if (name?.StartsWith("Solidshield Library", StringComparison.OrdinalIgnoreCase) == true)
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
            var wrapperResources = pex.FindResourceByNamedType("BIN, IDR_SGT");
            if (wrapperResources != null && wrapperResources.Any())
                return "SolidShield EXE Wrapper v1";

            // Search the last two available sections
            for (int i = (pex.SectionNames != null && pex.SectionNames.Length >= 2 ? pex.SectionNames.Length - 2 : 0); i < (pex.SectionNames?.Length ?? 0); i++)
            {
                // Get the nth section strings, if they exist
                var strs = pex.GetSectionStrings(i);
                if (strs != null)
                {
                    var str = strs.FirstOrDefault(s => s.Contains("Solidshield "));
                    if (str != null)
                        return $"SolidShield EXE Wrapper {str.Substring("Solidshield ".Length)}";
                }
            }

            // Get the import directory table, if it exists
            if (pex.Model.ImportTable?.ImportDirectoryTable != null)
            {
                bool match = pex.Model.ImportTable.ImportDirectoryTable.Any(idte => idte?.Name == "dvm.dll");
                if (match)
                    return "SolidShield EXE Wrapper v1";

                match = pex.Model.ImportTable.ImportDirectoryTable.Any(idte => idte?.Name == "activation.x86.dll");
                if (match)
                    return "SolidShield EXE Wrapper v2";

                match = pex.Model.ImportTable.ImportDirectoryTable.Any(idte => idte?.Name == "activation.x64.dll");
                if (match)
                    return "SolidShield EXE Wrapper v2";
            }

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
                // Found in Redump entry 68166.
                new(new FilePathMatch("tdvm.dll"), "SolidShield"),
                new(new FilePathMatch("tdvm.vds"), "SolidShield"),
                new(new PathMatch("vfs20.dll", useEndsWith: true), "SolidShield"),

                new(new FilePathMatch("dvm.dll"), "SolidShield"),
                new(new FilePathMatch("hc.dll"), "SolidShield"),
                new(new PathMatch("solidshield-cd.dll", useEndsWith: true), "SolidShield"),
                new(new PathMatch("c11prot.dll", useEndsWith: true), "SolidShield"),
            };

            // TODO: Verify if these are OR or AND
            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("dvm.dll"), "SolidShield"),
                new(new FilePathMatch("hc.dll"), "SolidShield"),
                new(new PathMatch("solidshield-cd.dll", useEndsWith: true), "SolidShield"),
                new(new PathMatch("c11prot.dll", useEndsWith: true), "SolidShield"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string? GetExeWrapperVersion(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

            int position = positions[0];

#if NET20 || NET35 || NET40
            byte[] id1 = new byte[3];
            Array.Copy(fileContent, position + 5, id1, 0, 3);
            byte[] id2 = new byte[4];
            Array.Copy(fileContent, position + 16, id2, 0, 3);
#else
            var id1 = new ArraySegment<byte>(fileContent, position + 5, 3);
            var id2 = new ArraySegment<byte>(fileContent, position + 16, 4);
#endif

            if (id1.SequenceEqual(new byte[] { 0x00, 0x00, 0x00 }) && id2.SequenceEqual(new byte[] { 0x00, 0x10, 0x00, 0x00 }))
                return "v1";
            else if (id1.SequenceEqual(new byte[] { 0x2E, 0x6F, 0x26 }) && id2.SequenceEqual(new byte[] { 0xDB, 0xC5, 0x20, 0x3A })) // new byte[] { 0xDB, 0xC5, 0x20, 0x3A, 0xB9 }
                return "v2"; // TODO: Verify against other SolidShield 2 discs

            if (id1.SequenceEqual(new byte[] { 0x00, 0x00, 0x00 }) && id2.SequenceEqual(new byte[] { 0x00, 0x10, 0x00, 0x00 }))
                return "v1";
            else if (id1.SequenceEqual(new byte[] { 0x2E, 0x6F, 0x26 }) && id2.SequenceEqual(new byte[] { 0xDB, 0xC5, 0x20, 0x3A })) // new byte[] { 0xDB, 0xC5, 0x20, 0x3A, 0xB9 }
                return "v2"; // TODO: Verify against other SolidShield 2 discs

            return string.Empty;
        }

        public static string? GetVersionPlusTages(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

            int position = positions[0];
#if NET20 || NET35 || NET40
            byte[] id1 = new byte[3];
            Array.Copy(fileContent, position + 4, id1, 0, 3);
            byte[] id2 = new byte[4];
            Array.Copy(fileContent, position + 15, id2, 0, 3);
#else
            var id1 = new ArraySegment<byte>(fileContent, position + 4, 3);
            var id2 = new ArraySegment<byte>(fileContent, position + 15, 4);
#endif

            if ((fileContent[position + 3] == 0x04 || fileContent[position + 3] == 0x05)
                && id1.SequenceEqual(new byte[] { 0x00, 0x00, 0x00 })
                && id2.SequenceEqual(new byte[] { 0x00, 0x10, 0x00, 0x00 }))
            {
                return "2 (SolidShield v2 EXE Wrapper)";
            }
            else if (id1.SequenceEqual(new byte[] { 0x00, 0x00, 0x00 })
                && id2.SequenceEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }))
            {
                // "T" + (char)0x00 + "a" + (char)0x00 + "g" + (char)0x00 + "e" + (char)0x00 + "s" + (char)0x00 + "S" + (char)0x00 + "e" + (char)0x00 + "t" + (char)0x00 + "u" + (char)0x00 + "p" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "0" + (char)0x00 + (char)0x8 + (char)0x00 + (char)0x1 + (char)0x0 + "F" + (char)0x00 + "i" + (char)0x00 + "l" + (char)0x00 + "e" + (char)0x00 + "V" + (char)0x00 + "e" + (char)0x00 + "r" + (char)0x00 + "s" + (char)0x00 + "i" + (char)0x00 + "o" + (char)0x00 + "n" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00
                byte?[] check2 =
                [
                    0x54, 0x61, 0x67, 0x65, 0x73, 0x53, 0x65, 0x74,
                    0x75, 0x70, 0x30, 0x08, 0x01, 0x00, 0x46, 0x69,
                    0x6C, 0x65, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F,
                    0x6E, 0x00, 0x00, 0x00, 0x00
                ];
                if (fileContent.FirstPosition(check2, out int position2))
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
