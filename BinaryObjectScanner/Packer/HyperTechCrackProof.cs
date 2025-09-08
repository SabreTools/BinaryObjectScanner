using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // CrackProof is a packer/obfuscator created by Japanese company HyperTech (https://www.hypertech.co.jp/products/windows/).
    // It is known to be used along with other DRM, such as Shury2 (Redump entry 97135) and BDL.
    // https://www.reddit.com/r/riseofincarnates/comments/m3vbnm/subreddit_revival_does_anyone_still_have_rise_of/
    // https://steamcommunity.com/app/310950/discussions/0/4224890554455490819/
    // https://github.com/horsicq/Detect-It-Easy/blob/63a1aa8bb23ca02d8a7fd5936db8dbc5c5d52dea/db/PE/HyperTech%20Crackproof.2.sg
    public class HyperTechCrackProof : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // This check may be overly limiting, as it excludes the sample provided to DiE (https://github.com/horsicq/Detect-It-Easy/issues/102).
            // TODO: Find further samples and invesitgate if the "peC" section is only present on specific versions.
            bool importTableMatch = Array.Exists(exe.Model.ImportTable?.ImportDirectoryTable ?? [],
                idte => idte?.Name == "KeRnEl32.dLl");

            if (exe.ContainsSection("peC", exact: true) && importTableMatch)
                return "HyperTech CrackProof";

            return null;
        }
    }
}
