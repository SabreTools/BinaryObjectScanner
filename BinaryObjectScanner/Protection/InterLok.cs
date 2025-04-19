using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class InterLok : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
                // Found in "nfsc_link.exe" in IA item "nfscorigin".
                // Full string: 
                // (: ) InterLok PC v2.0, PACE Anti-Piracy, Copyright (C) 1998, ALL RIGHTS RESERVED
                var match = pex.GetFirstSectionStrings(".rsrc").Find(s => s.Contains("InterLok") && s.Contains("PACE Anti-Piracy"));
                if (match != null)
                    return $"PACE Anti-Piracy InterLok {GetVersion(match)}";
                return null;
        }
        private static string GetVersion(string match)
        {
            match = match.Remove(match.IndexOf(",")).Trim();
            return match.Substring("InterLok ".Length + match.IndexOf("InterLok"));
        }
    }
}



    
    
