using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, which should be possible with LibMSPackN, but it refuses to extract due to SFX files lacking the typical CAB identifiers.
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class MicrosoftCABSFX : IPortableExecutableCheck, IScannable
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

            string name = pex.InternalName;
            if (name?.Equals("Wextract", StringComparison.OrdinalIgnoreCase) == true)
                return $"Microsoft CAB SFX {GetVersion(pex)}";

            name = pex.OriginalFileName;
            if (name?.Equals("WEXTRACT.EXE", StringComparison.OrdinalIgnoreCase) == true)
                return $"Microsoft CAB SFX {GetVersion(pex)}";

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // wextract_cleanup
                    new ContentMatchSet(new byte?[]
                    {
                        0x77, 0x65, 0x78, 0x74, 0x72, 0x61, 0x63, 0x74, 
                        0x5F, 0x63, 0x6C, 0x65, 0x61, 0x6E, 0x75, 0x70,
                    }, "Microsoft CAB SFX"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return $"Microsoft CAB SFX {GetVersion(pex)}";
            }
            
            // Get the .text section, if it exists
            if (pex.TextSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    /* This detects a different but similar type of SFX that uses Microsoft CAB files.
                       Further research is needed to see if it's just a different version or entirely separate. */
                    // MSCFu
                    new ContentMatchSet(new byte?[] { 0x4D, 0x53, 0x43, 0x46, 0x75 }, "Microsoft CAB SFX"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.TextSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return $"Microsoft CAB SFX {GetVersion(pex)}";
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
    
        private string GetVersion(PortableExecutable pex)
        {
            // Check the internal versions
            string version = Utilities.GetInternalVersion(pex);
            if (!string.IsNullOrWhiteSpace(version))
                return $"v{version}";

            return string.Empty;
        }
    }
}
