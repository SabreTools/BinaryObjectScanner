using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction
    // TODO: Verify that all versions are detected
    public class AdvancedInstaller : IPortableExecutableCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .rdata section, if it exists
            if (pex.ResourceDataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // Software\Caphyon\Advanced Installer
                    new ContentMatchSet(new byte?[]
                    {
                        0x53, 0x6F, 0x66, 0x74, 0x77, 0x61, 0x72, 0x65,
                        0x5C, 0x43, 0x61, 0x70, 0x68, 0x79, 0x6F, 0x6E,
                        0x5C, 0x41, 0x64, 0x76, 0x61, 0x6E, 0x63, 0x65,
                        0x64, 0x20, 0x49, 0x6E, 0x73, 0x74, 0x61, 0x6C,
                        0x6C, 0x65, 0x72
                    }, "Caphyon Advanced Installer"),
                };

                return MatchUtil.GetFirstMatch(file, pex.ResourceDataSectionRaw, matchers, includeDebug);
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
