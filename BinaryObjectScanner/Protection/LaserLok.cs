using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class LaserLok : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // TODO: Add entry point check
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/Laserlok.2.sg

            // TODO: Additional checks that may or may not be useful with the below
            //
            // These two can appear separately
            // \LASERLOK\LASERLOK.IN
            // C:\NOMOUSE.SP
            //
            // // :\LASERLOK\LASERLOK.IN + (char)0x00 + C:\NOMOUSE.SP
            // new(new byte?[]
            // {
            //     0x3A, 0x5C, 0x5C, 0x4C, 0x41, 0x53, 0x45, 0x52,
            //     0x4C, 0x4F, 0x4B, 0x5C, 0x5C, 0x4C, 0x41, 0x53,
            //     0x45, 0x52, 0x4C, 0x4F, 0x4B, 0x2E, 0x49, 0x4E,
            //     0x00, 0x43, 0x3A, 0x5C, 0x5C, 0x4E, 0x4F, 0x4D,
            //     0x4F, 0x55, 0x53, 0x45, 0x2E, 0x53, 0x50
            // }, "LaserLok 3"),

            // // LASERLOK_INIT + (char)0xC + LASERLOK_RUN + (char)0xE + LASERLOK_CHECK + (char)0xF + LASERLOK_CHECK2 + (char)0xF + LASERLOK_CHECK3
            // new(new byte?[]
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
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Packed by SPEEnc V2 Asterios Parlamentas.PE
            byte?[] check =
            [
                0x50, 0x61, 0x63, 0x6B, 0x65, 0x64, 0x20, 0x62,
                0x79, 0x20, 0x53, 0x50, 0x45, 0x45, 0x6E, 0x63,
                0x20, 0x56, 0x32, 0x20, 0x41, 0x73, 0x74, 0x65,
                0x72, 0x69, 0x6F, 0x73, 0x20, 0x50, 0x61, 0x72,
                0x6C, 0x61, 0x6D, 0x65, 0x6E, 0x74, 0x61, 0x73,
                0x2E, 0x50, 0x45
            ];
            int endDosStub = (int)(pex.Model.Stub?.Header?.NewExeHeaderAddr ?? 0);
            int position = -1;
#if NET20
            bool containsCheck = Extensions.FirstPosition(pex.StubExecutableData ?? [], check, out position);
#else
            bool containsCheck = pex.StubExecutableData?.FirstPosition(check, out position) ?? false;
#endif

            // Check the executable tables
            bool containsCheck2 = Array.Exists(pex.Model.ImportTable?.HintNameTable ?? [], hnte => hnte?.Name == "GetModuleHandleA")
                && Array.Exists(pex.Model.ImportTable?.HintNameTable ?? [], hnte => hnte?.Name == "GetProcAddress")
                && Array.Exists(pex.Model.ImportTable?.HintNameTable ?? [], hnte => hnte?.Name == "LoadLibraryA")
                && Array.Exists(pex.Model.ImportTable?.ImportDirectoryTable ?? [], idte => idte?.Name == "KERNEL32.dll");

            int position2 = -1;

            // Get the .text section, if it exists
            if (containsCheck2 && pex.ContainsSection(".text"))
            {
                // GetModuleHandleA + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + GetProcAddress + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + LoadLibraryA + (char)0x00 + (char)0x00 + KERNEL32.dll + (char)0x00 + ëy + (char)0x01 + SNIF/MPVI
                byte?[] check2 =
                [
                    0x47, 0x65, 0x74, 0x4D, 0x6F, 0x64, 0x75, 0x6C,
                    0x65, 0x48, 0x61, 0x6E, 0x64, 0x6C, 0x65, 0x41,
                    0x00, 0x00, 0x00, 0x00, 0x47, 0x65, 0x74, 0x50,
                    0x72, 0x6F, 0x63, 0x41, 0x64, 0x64, 0x72, 0x65,
                    0x73, 0x73, 0x00, 0x00, 0x00, 0x00, 0x4C, 0x6F,
                    0x61, 0x64, 0x4C, 0x69, 0x62, 0x72, 0x61, 0x72,
                    0x79, 0x41, 0x00, 0x00, 0x4B, 0x45, 0x52, 0x4E,
                    0x45, 0x4C, 0x33, 0x32, 0x2E, 0x64, 0x6C, 0x6C,
                    0x00, 0xEB, 0x79, 0x01, null, null, null, null,
                ];
#if NET20
                containsCheck2 = Extensions.FirstPosition(pex.GetFirstSectionData(".text") ?? [], check2, out position2);
#else
                containsCheck2 = pex.GetFirstSectionData(".text")?.FirstPosition(check2, out position2) ?? false;
#endif
            }
            else
            {
                // Otherwise, we reset the containsCheck2 value
                containsCheck2 = false;

            }

            if (containsCheck && containsCheck2)
                return $"LaserLok {GetVersion(pex.GetFirstSectionData(".text"), position2)} {GetBuild(pex.GetFirstSectionData(".text"), true)} [Check disc for physical ring]" + (includeDebug ? $" (Index {position}, {position2})" : string.Empty);
            else if (containsCheck && !containsCheck2)
                return $"LaserLok Marathon {GetBuild(pex.GetFirstSectionData(".text"), false)} [Check disc for physical ring]" + (includeDebug ? $" (Index {position})" : string.Empty);
            else if (!containsCheck && containsCheck2)
                return $"LaserLok {GetVersion(pex.GetFirstSectionData(".text"), --position2)} {GetBuild(pex.GetFirstSectionData(".text"), false)} [Check disc for physical ring]" + (includeDebug ? $" (Index {position2})" : string.Empty);

            return null;
        }

        /// <inheritdoc/>
        public IEnumerable<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new($"LASERLOK{Path.DirectorySeparatorChar}", "LaserLok [Check disc for physical ring]"),

                // TODO: Verify if these are OR or AND
                new(new FilePathMatch("NOMOUSE.SP"), GetVersion16Bit, "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("NOMOUSE.COM"), "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("l16dll.dll"), "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("laserlok.in"), "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("laserlok.o10"), "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("laserlok.o11"), "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("laserlok.o12"), "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("laserlok.out"), "LaserLok [Check disc for physical ring]"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("NOMOUSE.SP"), GetVersion16Bit, "LaserLok [Check disc for physical ring]"),

                // TODO: Verify if these are OR or AND
                new(new FilePathMatch("NOMOUSE.COM"), "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("l16dll.dll"), "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("laserlok.in"), "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("laserlok.o10"), "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("laserlok.o11"), "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("laserlok.o12"), "LaserLok [Check disc for physical ring]"),
                new(new FilePathMatch("laserlok.out"), "LaserLok [Check disc for physical ring]"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        private static string GetBuild(byte[]? sectionContent, bool versionTwo)
        {
            if (sectionContent == null)
                return "(Build unknown)";

            // Unkown + (char)0x00 + Unkown
            byte?[] check =
            [
                0x55, 0x6E, 0x6B, 0x6F, 0x77, 0x6E, 0x00, 0x55,
                0x6E, 0x6B, 0x6F, 0x77, 0x6E
            ];
#if NET20
            if (!Extensions.FirstPosition(sectionContent, check, out int position))
#else
            if (!sectionContent.FirstPosition(check, out int position))
#endif
                return "(Build unknown)";

            string year, month, day;
            if (versionTwo)
            {
                int index = position + 14;

                byte[] temp = new byte[2];
                Array.Copy(sectionContent, index, temp, 0, 2);
                day = new string(Array.ConvertAll(temp, b => (char)b));
                index += 3;
                Array.Copy(sectionContent, index, temp, 0, 2);
                month = new string(Array.ConvertAll(temp, b => (char)b));
                index += 3;
                Array.Copy(sectionContent, index, temp, 0, 2);
                year = "20" + new string(Array.ConvertAll(temp, b => (char)b));
            }
            else
            {
                int index = position + 13;

                byte[] temp = new byte[2];
                Array.Copy(sectionContent, index, temp, 0, 2);
                day = new string(Array.ConvertAll(temp, b => (char)b));
                index += 3;
                Array.Copy(sectionContent, index, temp, 0, 2);
                month = new string(Array.ConvertAll(temp, b => (char)b));
                index += 3;
                Array.Copy(sectionContent, index, temp, 0, 2);
                year = "20" + new string(Array.ConvertAll(temp, b => (char)b));
            }

            return $"(Build {year}-{month}-{day})";
        }

        private static string? GetVersion(byte[]? sectionContent, int position)
        {
            // If we have invalid data
            if (sectionContent == null)
                return null;

            byte[] temp = new byte[4];
            Array.Copy(sectionContent, position + 76, temp, 0, 4);
            return new string(Array.ConvertAll(temp, b => (char)b));
        }

        public static string? GetVersion16Bit(string firstMatchedString, IEnumerable<string>? files)
        {
            if (!File.Exists(firstMatchedString))
                return string.Empty;

            using var fs = File.Open(firstMatchedString, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var br = new BinaryReader(fs);
            return GetVersion16Bit(br.ReadBytes((int)fs.Length));
        }

        private static string GetVersion16Bit(byte[] fileContent)
        {
            byte[] temp = new byte[7];
            Array.Copy(fileContent, 71, temp, 0, 7);
            char[] version = Array.ConvertAll(temp, b => (char)b);

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
