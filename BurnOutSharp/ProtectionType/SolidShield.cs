using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class SolidShield : IContentCheck, IPathCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static readonly List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // D + (char)0x00 + V + (char)0x00 + M + (char)0x00 +   + (char)0x00 + L + (char)0x00 + i + (char)0x00 + b + (char)0x00 + r + (char)0x00 + a + (char)0x00 + r + (char)0x00 + y + (char)0x00
            new ContentMatchSet(new byte?[]
            {
                0x44, 0x00, 0x56, 0x00, 0x4D, 0x00, 0x20, 0x00,
                0x4C, 0x00, 0x69, 0x00, 0x62, 0x00, 0x72, 0x00,
                0x61, 0x00, 0x72, 0x00, 0x79, 0x00
            }, Utilities.GetFileVersion, "SolidShield"),

            // S + (char)0x00 + o + (char)0x00 + l + (char)0x00 + i + (char)0x00 + d + (char)0x00 + s + (char)0x00 + h + (char)0x00 + i + (char)0x00 + e + (char)0x00 + l + (char)0x00 + d + (char)0x00 +   + (char)0x00 + L + (char)0x00 + i + (char)0x00 + b + (char)0x00 + r + (char)0x00 + a + (char)0x00 + r + (char)0x00 + y + (char)0x00
            new ContentMatchSet(new byte?[]
            {
                0x53, 0x00, 0x6F, 0x00, 0x6C, 0x00, 0x69, 0x00,
                0x64, 0x00, 0x73, 0x00, 0x68, 0x00, 0x69, 0x00,
                0x65, 0x00, 0x6C, 0x00, 0x64, 0x00, 0x20, 0x00,
                0x4C, 0x00, 0x69, 0x00, 0x62, 0x00, 0x72, 0x00,
                0x61, 0x00, 0x72, 0x00, 0x79, 0x00
            }, GetFileVersion, "SolidShield Core.dll"),

            // S + (char)0x00 + o + (char)0x00 + l + (char)0x00 + i + (char)0x00 + d + (char)0x00 + s + (char)0x00 + h + (char)0x00 + i + (char)0x00 + e + (char)0x00 + l + (char)0x00 + d + (char)0x00 +   + (char)0x00 + A + (char)0x00 + c + (char)0x00 + t + (char)0x00 + i + (char)0x00 + v + (char)0x00 + a + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00 +   + (char)0x00 + L + (char)0x00 + i + (char)0x00 + b + (char)0x00 + r + (char)0x00 + a + (char)0x00 + r + (char)0x00 + y + (char)0x00
            new ContentMatchSet(new byte?[]
            {
                0x53, 0x00, 0x6F, 0x00, 0x6C, 0x00, 0x69, 0x00,
                0x64, 0x00, 0x73, 0x00, 0x68, 0x00, 0x69, 0x00,
                0x65, 0x00, 0x6C, 0x00, 0x64, 0x00, 0x20, 0x00,
                0x41, 0x00, 0x63, 0x00, 0x74, 0x00, 0x69, 0x00,
                0x76, 0x00, 0x61, 0x00, 0x74, 0x00, 0x69, 0x00,
                0x6F, 0x00, 0x6E, 0x00, 0x20, 0x00, 0x4C, 0x00,
                0x69, 0x00, 0x62, 0x00, 0x72, 0x00, 0x61, 0x00,
                0x72, 0x00, 0x79, 0x00
            }, GetFileVersion, "SolidShield Core.dll"),

            // (char)0xEF + (char)0xBE + (char)0xAD + (char)0xDE
            new ContentMatchSet(new byte?[] { 0xEF, 0xBE, 0xAD, 0xDE }, GetExeWrapperVersion, "SolidShield"),

            // A + (char)0x00 + c + (char)0x00 + t + (char)0x00 + i + (char)0x00 + v + (char)0x00 + a + (char)0x00 + t + (char)0x00 + i + (char)0x00 + o + (char)0x00 + n + (char)0x00 +   + (char)0x00 + M + (char)0x00 + a + (char)0x00 + n + (char)0x00 + a + (char)0x00 + g + (char)0x00 + e + (char)0x00 + r + (char)0x00
            new ContentMatchSet(new byte?[]
            {
                0x41, 0x00, 0x63, 0x00, 0x74, 0x00, 0x69, 0x00,
                0x76, 0x00, 0x61, 0x00, 0x74, 0x00, 0x69, 0x00,
                0x6f, 0x00, 0x6e, 0x00, 0x20, 0x00, 0x4d, 0x00,
                0x61, 0x00, 0x6e, 0x00, 0x61, 0x00, 0x67, 0x00,
                0x65, 0x00, 0x72, 0x00
            }, GetFileVersion, "SolidShield Activation Manager Module"),

            // dvm.dll
            new ContentMatchSet(new byte?[] { 0x64, 0x76, 0x6D, 0x2E, 0x64, 0x6C, 0x6C }, "SolidShield EXE Wrapper"),

            // (char)0xAD + (char)0xDE + (char)0xFE + (char)0xCA
            new ContentMatchSet(new byte?[] { 0xAD, 0xDE, 0xFE, 0xCA }, GetVersionPlusTages, "SolidShield"),

            // Solidshield
            new ContentMatchSet(new byte?[]
            {
                0x53, 0x6F, 0x6C, 0x69, 0x64, 0x73, 0x68, 0x69,
                0x65, 0x6C, 0x64
            }, GetVersion, "SolidShield"),

            // B + (char)0x00 + I + (char)0x00 + N + (char)0x00 + (char)0x7 + (char)0x00 + I + (char)0x00 + D + (char)0x00 + R + (char)0x00 + _ + (char)0x00 + S + (char)0x00 + G + (char)0x00 + T + (char)0x00
            new ContentMatchSet(new byte?[]
            {
                0x42, 0x00, 0x49, 0x00, 0x4E, 0x00, 0x07, 0x00,
                0x49, 0x00, 0x44, 0x00, 0x52, 0x00, 0x5F, 0x00,
                0x53, 0x00, 0x47, 0x00, 0x54, 0x00
            }, "SolidShield"),
        };

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("dvm.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch("hc.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch("solidshield-cd.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch("c11prot.dll", useEndsWith: true), "SolidShield"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("dvm.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch("hc.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch("solidshield-cd.dll", useEndsWith: true), "SolidShield"),
                new PathMatchSet(new PathMatch("c11prot.dll", useEndsWith: true), "SolidShield"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        public static string GetExeWrapperVersion(string file, byte[] fileContent, List<int> positions)
        {
            var id1 = new ArraySegment<byte>(fileContent, positions[0] + 5, 3);
            var id2 = new ArraySegment<byte>(fileContent, positions[0] + 16, 4);

            if (id1.SequenceEqual(new byte[] { 0x00, 0x00, 0x00 }) && id2.SequenceEqual(new byte[] { 0x00, 0x10, 0x00, 0x00 }))
                return "1 (SolidShield EXE Wrapper)";
            else if (id1.SequenceEqual(new byte[] { 0x2E, 0x6F, 0x26 }) && id2.SequenceEqual(new byte[] { 0xDB, 0xC5, 0x20, 0x3A, 0xB9 }))
                return "2 (SolidShield v2 EXE Wrapper)"; // TODO: Verify against other SolidShield 2 discs
            
            return null;
        }

        public static string GetFileVersion(string file, byte[] fileContent, List<int> positions)
        {
            string companyName = string.Empty;
            if (file != null)
                companyName = FileVersionInfo.GetVersionInfo(file).CompanyName.ToLower();

            if (companyName.Contains("solidshield") || companyName.Contains("tages"))
                return Utilities.GetFileVersion(file);
            
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
                byte?[] check2 = new byte?[] { 0x54, 0x61, 0x67, 0x65, 0x73, 0x53, 0x65, 0x74, 0x75, 0x70, 0x30, 0x08, 0x01, 0x00, 0x46, 0x69, 0x6C, 0x65, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00 };
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
    }
}
