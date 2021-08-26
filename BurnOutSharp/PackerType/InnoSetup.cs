using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class InnoSetup : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                // Inno Setup Setup Data (
                new ContentMatchSet(new byte?[]
                {
                    0x49, 0x6E, 0x6E, 0x6F, 0x20, 0x53, 0x65, 0x74,
                    0x75, 0x70, 0x20, 0x53, 0x65, 0x74, 0x75, 0x70,
                    0x20, 0x44, 0x61, 0x74, 0x61, 0x20, 0x28
                }, GetVersion, "Inno Setup"),

                // Inno
                new ContentMatchSet(
                    new ContentMatch(new byte?[] { 0x49, 0x6E, 0x6E, 0x6F }, start: 0x30, end: 0x31),
                    GetOldVersion,
                    "Inno Setup"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false) => null;

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
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
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }

        public static string GetOldVersion(string file, byte[] fileContent, List<int> positions)
        {
            var matchers = new List<ContentMatchSet>
            {
                // "rDlPtS02" + (char)0x87 + "eVx"
                new ContentMatchSet(new byte?[] { 0x72, 0x44, 0x6C, 0x50, 0x74, 0x53, 0x30, 0x32, 0x87, 0x65, 0x56, 0x78 }, "1.2.16 or earlier"),
            };

            string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, false);
            return match ?? "Unknown 1.X"; 
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            try
            {
                int index = positions[0];
                index += 23;

                var versionBytes = new ReadOnlySpan<byte>(fileContent, index, 16).ToArray();
                var onlyVersion = versionBytes.TakeWhile(b => b != ')').ToArray();

                index += onlyVersion.Length + 2;
                var unicodeBytes = new ReadOnlySpan<byte>(fileContent, index, 3).ToArray();
                string version = Encoding.ASCII.GetString(onlyVersion);

                if (unicodeBytes.SequenceEqual(new byte[] { 0x28, 0x75, 0x29 }))
                    return (version + " (Unicode)");

                return version;
            }
            catch
            {
                return "(Unknown Version)";
            }
        }
    }
}
