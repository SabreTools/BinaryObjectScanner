using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.ExecutableType.Microsoft.Headers;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class LaserLok : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // TODO: Additional checks that may or may not be useful with the below
            //
            // These two can appear separately
            // \LASERLOK\LASERLOK.IN
            // C:\NOMOUSE.SP
            //
            // // :\LASERLOK\LASERLOK.IN + (char)0x00 + C:\NOMOUSE.SP
            // new ContentMatchSet(new byte?[]
            // {
            //     0x3A, 0x5C, 0x5C, 0x4C, 0x41, 0x53, 0x45, 0x52,
            //     0x4C, 0x4F, 0x4B, 0x5C, 0x5C, 0x4C, 0x41, 0x53,
            //     0x45, 0x52, 0x4C, 0x4F, 0x4B, 0x2E, 0x49, 0x4E,
            //     0x00, 0x43, 0x3A, 0x5C, 0x5C, 0x4E, 0x4F, 0x4D,
            //     0x4F, 0x55, 0x53, 0x45, 0x2E, 0x53, 0x50
            // }, "LaserLok 3"),

            // // LASERLOK_INIT + (char)0xC + LASERLOK_RUN + (char)0xE + LASERLOK_CHECK + (char)0xF + LASERLOK_CHECK2 + (char)0xF + LASERLOK_CHECK3
            // new ContentMatchSet(new byte?[]
            // {
            //     0x4C, 0x41, 0x53, 0x45, 0x52, 0x4C, 0x4F, 0x4B,
            //     0x5F, 0x49, 0x4E, 0x49, 0x54, 0x0C, 0x4C, 0x41,
            //     0x53, 0x45, 0x52, 0x4C, 0x4F, 0x4B, 0x5F, 0x52,
            //     0x55, 0x4E, 0x0E, 0x4C, 0x41, 0x53, 0x45, 0x52,
            //     0x4C, 0x4F, 0x4B, 0x5F, 0x43, 0x48, 0x45, 0x43,
            //     0x4B, 0x0F, 0x4C, 0x41, 0x53, 0x45, 0x52, 0x4C,
            //     0x4F, 0x4B, 0x5F, 0x43, 0x48, 0x45, 0x43, 0x4B,
            //     0x32, 0x0F, 0x4C, 0x41, 0x53, 0x45, 0x52, 0x4C,
            //     0x4F, 0x4B, 0x5F, 0x43, 0x48, 0x45, 0x43, 0x4B,
            //     0x33
            // }, "LaserLok 5"),

            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Packed by SPEEnc V2 Asterios Parlamentas.PE
            byte?[] check = new byte?[]
            {
                0x50, 0x61, 0x63, 0x6B, 0x65, 0x64, 0x20, 0x62,
                0x79, 0x20, 0x53, 0x50, 0x45, 0x45, 0x6E, 0x63,
                0x20, 0x56, 0x32, 0x20, 0x41, 0x73, 0x74, 0x65,
                0x72, 0x69, 0x6F, 0x73, 0x20, 0x50, 0x61, 0x72,
                0x6C, 0x61, 0x6D, 0x65, 0x6E, 0x74, 0x61, 0x73,
                0x2E, 0x50, 0x45
            };
            int endDosStub = pex.DOSStubHeader.NewExeHeaderAddr;
            bool containsCheck = fileContent.FirstPosition(check, out int position, start: 0, end: endDosStub);

            // If the .text section doesn't exist, then the second check can't be found
            bool containsCheck2 = false;
            int position2 = -1;

            // Get the .text section, if it exists
            var textSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".text"));
            if (textSection != null)
            {
                int sectionAddr = (int)textSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)textSection.VirtualSize;

                // GetModuleHandleA + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + GetProcAddress + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + LoadLibraryA + (char)0x00 + (char)0x00 + KERNEL32.dll + (char)0x00 + ëy + (char)0x01 + SNIF/MPVI
                byte?[] check2 = new byte?[]
                {
                    0x47, 0x65, 0x74, 0x4D, 0x6F, 0x64, 0x75, 0x6C,
                    0x65, 0x48, 0x61, 0x6E, 0x64, 0x6C, 0x65, 0x41,
                    0x00, 0x00, 0x00, 0x00, 0x47, 0x65, 0x74, 0x50,
                    0x72, 0x6F, 0x63, 0x41, 0x64, 0x64, 0x72, 0x65,
                    0x73, 0x73, 0x00, 0x00, 0x00, 0x00, 0x4C, 0x6F,
                    0x61, 0x64, 0x4C, 0x69, 0x62, 0x72, 0x61, 0x72,
                    0x79, 0x41, 0x00, 0x00, 0x4B, 0x45, 0x52, 0x4E,
                    0x45, 0x4C, 0x33, 0x32, 0x2E, 0x64, 0x6C, 0x6C,
                    0x00, 0xEB, 0x79, 0x01, null, null, null, null,
                };
                containsCheck2 = fileContent.FirstPosition(check2, out position2, start: sectionAddr, end: sectionEnd);
            }

            if (containsCheck && containsCheck2)
                return $"LaserLok {GetVersion(fileContent, position2)} {GetBuild(sections, fileContent, true)}" + (includeDebug ? $" (Index {position}, {position2})" : string.Empty);
            else if (containsCheck && !containsCheck2)
                return $"LaserLok Marathon {GetBuild(sections, fileContent, false)}" + (includeDebug ? $" (Index {position})" : string.Empty);
            else if (!containsCheck && containsCheck2)
                return $"LaserLok {GetVersion(fileContent, --position2)} {GetBuild(sections, fileContent, false)}" + (includeDebug ? $" (Index {position2})" : string.Empty);

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet($"LASERLOK{Path.DirectorySeparatorChar}", "LaserLok"),

                // TODO: Verify if these are OR or AND
                new PathMatchSet(new PathMatch("NOMOUSE.SP", useEndsWith: true), GetVersion16Bit, "LaserLok"),
                new PathMatchSet(new PathMatch("NOMOUSE.COM", useEndsWith: true), "LaserLok"),
                new PathMatchSet(new PathMatch("l16dll.dll", useEndsWith: true), "LaserLok"),
                new PathMatchSet(new PathMatch("laserlok.in", useEndsWith: true), "LaserLok"),
                new PathMatchSet(new PathMatch("laserlok.o10", useEndsWith: true), "LaserLok"),
                new PathMatchSet(new PathMatch("laserlok.o11", useEndsWith: true), "LaserLok"),
                new PathMatchSet(new PathMatch("laserlok.o12", useEndsWith: true), "LaserLok"),
                new PathMatchSet(new PathMatch("laserlok.out", useEndsWith: true), "LaserLok"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("NOMOUSE.SP", useEndsWith: true), GetVersion16Bit, "LaserLok"),

                // TODO: Verify if these are OR or AND
                new PathMatchSet(new PathMatch("NOMOUSE.COM", useEndsWith: true), "LaserLok"),
                new PathMatchSet(new PathMatch("l16dll.dll", useEndsWith: true), "LaserLok"),
                new PathMatchSet(new PathMatch("laserlok.in", useEndsWith: true), "LaserLok"),
                new PathMatchSet(new PathMatch("laserlok.o10", useEndsWith: true), "LaserLok"),
                new PathMatchSet(new PathMatch("laserlok.o11", useEndsWith: true), "LaserLok"),
                new PathMatchSet(new PathMatch("laserlok.o12", useEndsWith: true), "LaserLok"),
                new PathMatchSet(new PathMatch("laserlok.out", useEndsWith: true), "LaserLok"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        private static string GetBuild(SectionHeader[] sections, byte[] fileContent, bool versionTwo)
        {
            var textSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".text"));
            if (textSection == null)
                return "(Build unknown)";

            int sectionAddr = (int)textSection.PointerToRawData;
            int sectionEnd = sectionAddr + (int)textSection.VirtualSize;

            // Unkown + (char)0x00 + Unkown
            byte?[] check = new byte?[]
            {
                0x55, 0x6E, 0x6B, 0x6F, 0x77, 0x6E, 0x00, 0x55,
                0x6E, 0x6B, 0x6F, 0x77, 0x6E
            };
            if (!fileContent.FirstPosition(check, out int position, start: sectionAddr, end: sectionEnd))
                return "(Build unknown)";

            string year, month, day;
            if (versionTwo)
            {
                int index = position + 14;
                day = new string(new ArraySegment<byte>(fileContent, index, 2).Select(b => (char)b).ToArray());
                index += 3;
                month = new string(new ArraySegment<byte>(fileContent, index, 2).Select(b => (char)b).ToArray());
                index += 3;
                year = "20" + new string(new ArraySegment<byte>(fileContent, index, 2).Select(b => (char)b).ToArray());
            }
            else
            {
                int index = position + 13;
                day = new string(new ArraySegment<byte>(fileContent, index, 2).Select(b => (char)b).ToArray());
                index += 3;
                month = new string(new ArraySegment<byte>(fileContent, index, 2).Select(b => (char)b).ToArray());
                index += 3;
                year = "20" + new string(new ArraySegment<byte>(fileContent, index, 2).Select(b => (char)b).ToArray());
            }

            return $"(Build {year}-{month}-{day})";
        }

        private static string GetVersion(byte[] fileContent, int position)
        {
            return new string(new ArraySegment<byte>(fileContent, position + 76, 4).Select(b => (char)b).ToArray());
        }

        public static string GetVersion16Bit(string firstMatchedString, IEnumerable<string> files)
        {
            if (!File.Exists(firstMatchedString))
                return string.Empty;

            using (var fs = File.Open(firstMatchedString, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                return GetVersion16Bit(br.ReadBytes((int)fs.Length));
            }
        }

        private static string GetVersion16Bit(byte[] fileContent)
        {
            char[] version = new ArraySegment<byte>(fileContent, 71, 7).Select(b => (char)b).ToArray();
            if (char.IsNumber(version[0]) && char.IsNumber(version[2]) && char.IsNumber(version[3]))
            {
                if (char.IsNumber(version[5]) && char.IsNumber(version[6]))
                    return $"{version[0]}.{version[2]}{version[3]}.{version[5]}{version[6]}";
                else if (char.IsNumber(version[5]))
                    return $"{version[0]}.{version[2]}{version[3]}.{version[5]}";

                return $"{version[0]}.{version[2]}{version[3]}";
            }

            return string.Empty;
        }
    }
}
