using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction
    public class dotFuscator : IPortableExecutableCheck, IScannable
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .text section, if it exists
            if (pex.ContainsSection(".text"))
            {
                var matchers = new List<ContentMatchSet>
                {
                    // DotfuscatorAttribute
                    new ContentMatchSet(new byte?[]
                    {
                        0x44, 0x6F, 0x74, 0x66, 0x75, 0x73, 0x63, 0x61,
                        0x74, 0x6F, 0x72, 0x41, 0x74, 0x74, 0x72, 0x69,
                        0x62, 0x75, 0x74, 0x65
                    }, "dotFuscator"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.GetFirstSectionData(".text"), matchers, includeDebug);
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
    }
}
