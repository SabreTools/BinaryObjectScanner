using System.Text.RegularExpressions;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class InterLok : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Get the .rsrc section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".rsrc");
            if (strs != null)
            {
                // Found in "nfsc_link.exe" in IA item "nfscorigin".
                // Full string:
                // (: ) InterLok PC v2.0, PACE Anti-Piracy, Copyright (C) 1998, ALL RIGHTS RESERVED
                var match = strs.Find(s => s.Contains("InterLok") && s.Contains("PACE Anti-Piracy"));
                if (match != null)
                    return $"PACE Anti-Piracy InterLok {GetVersion(match)}";
            }

            return null;
        }

        private static string GetVersion(string match)
        {
            var versionMatch = Regex.Match(match, @"(?<=InterLok )(.*?)(?=,)");
            if (versionMatch.Success)
                return versionMatch.Value;

            return "(Unknown Version - Please report to us on GitHub)";
        }
    }
}
