using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction
    // TODO: Add version checking, if possible
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class Armadillo : IPortableExecutableCheck, IScannable
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .nicode section, if it exists
            bool nicodeSection = pex.ContainsSection(".nicode", exact: true);
            if (nicodeSection)
                return "Armadillo";

            // Loop through all "extension" sections -- usually .data1 or .text1
            foreach (var sectionName in pex.SectionNames.Where(s => s != null && s.EndsWith("1")))
            {
                var sectionRaw = pex.GetFirstSectionData(sectionName);
                if (sectionRaw != null)
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // ARMDEBUG
                        new ContentMatchSet(new byte?[] { 0x41, 0x52, 0x4D, 0x44, 0x45, 0x42, 0x55, 0x47 }, $"Armadillo"),
                    };

                    string match = MatchUtil.GetFirstMatch(file, sectionRaw, matchers, includeDebug);
                    if (!string.IsNullOrWhiteSpace(match))
                        return match;
                }
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
