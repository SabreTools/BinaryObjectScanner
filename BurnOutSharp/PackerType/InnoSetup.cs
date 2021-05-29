using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class InnoSetup : IContentCheck, IScannable
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static readonly List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // Inno
            new ContentMatchSet(
                new ContentMatch(new byte?[] { 0x49, 0x6E, 0x6E, 0x6F }, start: 0x30, end: 0x31),
                GetVersion,
                "Inno Setup"),

            new ContentMatchSet(new byte?[]
            {
                0x49, 0x6E, 0x6E, 0x6F, 0x20, 0x53, 0x65, 0x74, 0x75, 0x70, 0x20, 0x53,
                0x65, 0x74, 0x75, 0x70, 0x20, 0x44, 0x61, 0x74, 0x61, 0x20, 0x28
            }, GetVersion, "Inno Setup"),
        };

        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }

        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        // TOOO: Add Inno Setup extraction
        // https://github.com/dscharrer/InnoExtract
        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            byte[] signature = new ArraySegment<byte>(fileContent, 0x30, 12).ToArray();

            // "rDlPtS02" + (char)0x87 + "eVx"
            if (signature.SequenceEqual( new byte[] { 0x72, 0x44, 0x6C, 0x50, 0x74, 0x53, 0x30, 0x32, 0x87, 0x65, 0x56, 0x78 }))
                return "1.2.10";

            // "rDlPtS04" + (char)0x87 + "eVx"
            else if (signature.SequenceEqual(new byte[] { 0x72, 0x44, 0x6C, 0x50, 0x74, 0x53, 0x30, 0x34, 0x87, 0x65, 0x56, 0x78 }))
                return "4.0.0";

            // "rDlPtS05" + (char)0x87 + "eVx"
            else if (signature.SequenceEqual(new byte[] { 0x72, 0x44, 0x6C, 0x50, 0x74, 0x53, 0x30, 0x35, 0x87, 0x65, 0x56, 0x78 }))
                return "4.0.3";

            // "rDlPtS06" + (char)0x87 + "eVx"
            else if (signature.SequenceEqual(new byte[] { 0x72, 0x44, 0x6C, 0x50, 0x74, 0x53, 0x30, 0x36, 0x87, 0x65, 0x56, 0x78 }))
                return "4.0.10";

            // "rDlPtS07" + (char)0x87 + "eVx"
            else if (signature.SequenceEqual(new byte[] { 0x72, 0x44, 0x6C, 0x50, 0x74, 0x53, 0x30, 0x37, 0x87, 0x65, 0x56, 0x78 }))
                return "4.1.6";

            // "rDlPtS" + (char)0xcd + (char)0xe6 + (char)0xd7 + "{" + (char)0x0b + "*"
            else if (signature.SequenceEqual(new byte[] { 0x72, 0x44, 0x6C, 0x50, 0x74, 0x53, 0xCD, 0xE6, 0xD7, 0x7B, 0x0b, 0x2A }))
                return "5.1.5";

            // "nS5W7dT" + (char)0x83 + (char)0xaa + (char)0x1b + (char)0x0f + "j"
            else if (signature.SequenceEqual(new byte[] { 0x6E, 0x53, 0x35, 0x57, 0x37, 0x64, 0x54, 0x83, 0xAA, 0x1B, 0x0F, 0x6A }))
                return "5.1.5";

            try
            {
                int index = positions[0];
                index += 22;
                if (fileContent[index] != '(')
                    return "(Unknown Version)";
                index += 1;

                var versionBytes = new ReadOnlySpan<byte>(fileContent, index, 16).ToArray();
                var onlyVersion = versionBytes.TakeWhile(b => b != ')').ToArray();
                return Encoding.ASCII.GetString(onlyVersion);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
