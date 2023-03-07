using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using BurnOutSharp.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.PackerType
{
    // CrackProof is a packer/obfuscator created by Japanese company HyperTech (https://www.hypertech.co.jp/products/windows/).
    // It is known to be used along with other DRM, such as Shury2 (Redump entry 97135) and BDL.
    // https://www.reddit.com/r/riseofincarnates/comments/m3vbnm/subreddit_revival_does_anyone_still_have_rise_of/
    // https://steamcommunity.com/app/310950/discussions/0/4224890554455490819/
    // https://github.com/horsicq/Detect-It-Easy/blob/63a1aa8bb23ca02d8a7fd5936db8dbc5c5d52dea/db/PE/HyperTech%20Crackproof.2.sg
    public class HyperTechCrackProof : IPortableExecutableCheck, IScannable
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // This check may be overly limiting, as it excludes the sample provided to DiE (https://github.com/horsicq/Detect-It-Easy/issues/102).
            // TODO: Find further samples and invesitgate if the "peC" section is only present on specific versions.
            bool peCSection = pex.ContainsSection("peC", exact: true);
            bool importTableMatch = (pex.ImportTable?.ImportDirectoryTable?.Any(idte => idte.Name == "KeRnEl32.dLl") ?? false);

            if (peCSection && importTableMatch)
                return "HyperTech CrackProof";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
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
