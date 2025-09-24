using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;
using SabreTools.IO.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// Reflexive Arcade Installer
    /// </summary>
    public class ReflexiveArcadeInstaller : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Get the .data/DATA section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".data") ?? exe.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                // <see href="https://www.virustotal.com/gui/file/33b98b675d78b88ed317e7e52dca21ca07bd84e79211294fcec72cab48d11184"/>
                if (strs.Exists(s => s.Contains("ReflexiveArcade")))
                    return "Reflexive Arcade Installer";

                // <see href="https://www.virustotal.com/gui/file/33b98b675d78b88ed317e7e52dca21ca07bd84e79211294fcec72cab48d11184"/>
                if (strs.Exists(s => s.Contains("Reflexive Arcade")))
                    return "Reflexive Arcade Installer";
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("ReflexiveArcade.dll"), "Reflexive Arcade Installer"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("ReflexiveArcade.dll"), "Reflexive Arcade Installer"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
