using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Not matching all SolidShield Wrapper v1 (See JackKeane)
    // TODO: Not matching all SolidShield Wrapper v1 (See NFS11)
    public class SolidShield : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // TODO: Investigate ".pseudo" section found in "tvdm.dll" in Redump entry 68166.

            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (name?.StartsWith("DVM Library", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield {Utilities.GetInternalVersion(pex)}";

            else if (name?.StartsWith("Solidshield Activation Library", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield Core.dll {Utilities.GetInternalVersion(pex)}";

            else if (name?.StartsWith("Activation Manager", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield Activation Manager Module {GetInternalVersion(pex)}";

            // Found in "tvdm.dll" in Redump entry 68166.
            else if (name?.StartsWith("Solidshield Library", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield {GetInternalVersion(pex)}";

            name = pex.ProductName;
            if (name?.StartsWith("Solidshield Activation Library", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield Core.dll {Utilities.GetInternalVersion(pex)}";

            else if (name?.StartsWith("Solidshield Library", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield Core.dll {Utilities.GetInternalVersion(pex)}";

            else if (name?.StartsWith("Activation Manager", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield Activation Manager Module {GetInternalVersion(pex)}";

            // Found in "tvdm.dll" in Redump entry 68166.
            else if (name?.StartsWith("Solidshield Library", StringComparison.OrdinalIgnoreCase) == true)
                return $"SolidShield {GetInternalVersion(pex)}";

            // Get the .init section, if it exists
            var initSectionRaw = pex.ReadRawSection(".init", first: true);
            if (initSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // (char)0xAD + (char)0xDE + (char)0xFE + (char)0xCA
                    new ContentMatchSet(new byte?[] { 0xAD, 0xDE, 0xFE, 0xCA }, GetVersionPlusTages, "SolidShield"),

                    // (char)0xEF + (char)0xBE + (char)0xAD + (char)0xDE
                    new ContentMatchSet(new byte?[] { 0xEF, 0xBE, 0xAD, 0xDE }, GetExeWrapperVersion, "SolidShield EXE Wrapper"),

                    // dvm.dll
                    new ContentMatchSet(new byte?[] { 0x64, 0x76, 0x6D, 0x2E, 0x64, 0x6C, 0x6C }, "SolidShield EXE Wrapper v1"),
                };

                string match = MatchUtil.GetFirstMatch(file, initSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the wrapper resource, if it exists
            var resource = pex.FindResource(dataContains: "B\0I\0N\0" + (char)0x07 + "\0I\0D\0R\0_\0S\0G\0T\0");
            if (resource != null)
                return "SolidShield EXE Wrapper v1";

            // Search the last two available sections
            var sectionNames = pex.GetSectionNames();
            for (int i = (sectionNames.Length >= 2 ? sectionNames.Length - 2 : 0); i < sectionNames.Length; i++)
            {
                var nthSectionRaw = pex.ReadRawSection(sectionNames[i], first: false);
                if (nthSectionRaw != null)
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // Solidshield
                        new ContentMatchSet(new byte?[]
                        {
                            0x53, 0x6F, 0x6C, 0x69, 0x64, 0x73, 0x68, 0x69,
                            0x65, 0x6C, 0x64
                        }, GetVersion, "SolidShield EXE Wrapper"),
                    };

                    string match = MatchUtil.GetFirstMatch(file, nthSectionRaw, matchers, includeDebug);
                    if (!string.IsNullOrWhiteSpace(match))
                        return match;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Redump entry 68166.
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}tdvm.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}tdvm.vds", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch("vfs20.dll", useEndsWith: true), "SolidShield"),

                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}dvm.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}hc.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch("solidshield-cd.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch("c11prot.dll", useEndsWith: true), "SolidShield"),
            };

            // TODO: Verify if these are OR or AND
            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}dvm.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch($"{Path.DirectorySeparatorChar}hc.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch("solidshield-cd.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch("c11prot.dll", useEndsWith: true), "SolidShield"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string GetExeWrapperVersion(string file, byte[] fileContent, List<int> positions)
        {
            int position = positions[0];
            var id1 = new ArraySegment<byte>(fileContent, position + 5, 3);
            var id2 = new ArraySegment<byte>(fileContent, position + 16, 4);

            if (id1.SequenceEqual(new byte[] { 0x00, 0x00, 0x00 }) && id2.SequenceEqual(new byte[] { 0x00, 0x10, 0x00, 0x00 }))
                return "v1";
            else if (id1.SequenceEqual(new byte[] { 0x2E, 0x6F, 0x26 }) && id2.SequenceEqual(new byte[] { 0xDB, 0xC5, 0x20, 0x3A })) // new byte[] { 0xDB, 0xC5, 0x20, 0x3A, 0xB9 }
                return "v2"; // TODO: Verify against other SolidShield 2 discs
            
            return null;
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            int index = positions[0] + 12; // Begin reading after "Solidshield"
            char version = (char)fileContent[index];
            index++;
            index++;
            char subVersion = (char)fileContent[index];
            index++;
            index++;
            char subSubVersion = (char)fileContent[index];
            index++;
            index++;
            char subSubSubVersion = (char)fileContent[index];

            if (!char.IsNumber(version))
                return null;
            
            return $"{version}.{subVersion}.{subSubVersion}.{subSubSubVersion}";
        }
    
        public static string GetVersionPlusTages(string file, byte[] fileContent, List<int> positions)
        {
            int position = positions[0];
            var id1 = new ArraySegment<byte>(fileContent, position + 4, 3);
            var id2 = new ArraySegment<byte>(fileContent, position + 15, 4);

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
                byte?[] check2 = new byte?[]
                {
                    0x54, 0x61, 0x67, 0x65, 0x73, 0x53, 0x65, 0x74,
                    0x75, 0x70, 0x30, 0x08, 0x01, 0x00, 0x46, 0x69,
                    0x6C, 0x65, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F,
                    0x6E, 0x00, 0x00, 0x00, 0x00
                };
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

            return null;
        }
    
        private static string GetInternalVersion(PortableExecutable pex)
        {
            string companyName = pex.CompanyName?.ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(companyName) && (companyName.Contains("solidshield") || companyName.Contains("tages")))
                return Utilities.GetInternalVersion(pex);
            
            return null;
        }
    }
}
