using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction - https://github.com/dscharrer/InnoExtract
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class InnoSetup : INewExecutableCheck, IPortableExecutableCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // Check we have a valid executable
            if (nex == null)
                return null;
            
            // Check for "Inno" in the reserved words
            if (nex.Stub_Reserved2[4] == 0x6E49 && nex.Stub_Reserved2[5] == 0x6F6E)
            {
                string version = GetOldVersion(file, nex);
                if (!string.IsNullOrWhiteSpace(version))
                    return $"Inno Setup {version}";
                
                return "Inno Setup (Unknown Version)";
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;
            
            // Get the .data/DATA section, if it exists
            var dataSectionRaw = pex.GetFirstSectionData(".data") ?? pex.GetFirstSectionData("DATA");
            if (dataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // Inno Setup Setup Data (
                    new ContentMatchSet(new byte?[]
                    {
                        0x49, 0x6E, 0x6E, 0x6F, 0x20, 0x53, 0x65, 0x74,
                        0x75, 0x70, 0x20, 0x53, 0x65, 0x74, 0x75, 0x70,
                        0x20, 0x44, 0x61, 0x74, 0x61, 0x20, 0x28
                    }, GetVersion, "Inno Setup"),
                };

                string match = MatchUtil.GetFirstMatch(file, dataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

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

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
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
                    return version + " (Unicode)";

                return version;
            }
            catch
            {
                return "(Unknown Version)";
            }
        }

        private static string GetOldVersion(string file, NewExecutable nex)
        {
            // TODO: Don't read entire file
            // TODO: Only 64 bytes at the end of the file is needed
            var data = nex.ReadArbitraryRange();
            if (data != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // "rDlPtS02" + (char)0x87 + "eVx"
                    new ContentMatchSet(new byte?[] { 0x72, 0x44, 0x6C, 0x50, 0x74, 0x53, 0x30, 0x32, 0x87, 0x65, 0x56, 0x78 }, "1.2.16 or earlier"),
                };

                return MatchUtil.GetFirstMatch(file, data, matchers, false) ?? "Unknown 1.X";
            }
            
            return "Unknown 1.X"; 
        }
    }
}
