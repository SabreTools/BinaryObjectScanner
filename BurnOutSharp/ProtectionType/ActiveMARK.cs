using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how to get version numbers
    public class ActiveMARK : IContentCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug)
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                {
                    // " " + (char)0xC2 + (char)0x16 + (char)0x00 + (char)0xA8 + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0xB8 + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0x86 + (char)0xC8 + (char)0x16 + (char)0x00 + (char)0x9A + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0x10 + (char)0xC2 + (char)0x16 + (char)0x00
                    new ContentMatchSet(new byte?[]
                    {
                        0x20, 0xC2, 0x16, 0x00, 0xA8, 0xC1, 0x16, 0x00,
                        0xB8, 0xC1, 0x16, 0x00, 0x86, 0xC8, 0x16, 0x00,
                        0x9A, 0xC1, 0x16, 0x00, 0x10, 0xC2, 0x16, 0x00
                    }, "ActiveMARK 5 (Unconfirmed - Please report to us on Github)"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
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

            // Get "REGISTRY, AMINTERNETPROTOCOL" resource items
            var resources = pex.FindResourceByNamedType("REGISTRY, AMINTERNETPROTOCOL");
            if (resources.Any())
            {
                bool match = resources.Where(r => r != null)
                    .Select(r => Encoding.ASCII.GetString(r))
                    .Any(r => r.Contains("ActiveMARK"));
                if (match)
                    return "ActiveMARK";
            }

            // TODO: Add entry point checks from https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt

            // Get the overlay data, if it exists
            if (pex.OverlayStrings != null)
            {
                if (pex.OverlayStrings.Any(s => s.Contains("TMSAMVOH")))
                    return "ActiveMARK";
            }

            // Get the last .bss section strings, if they exist
            List<string> strs = pex.GetLastSectionStrings(".bss");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("TMSAMVOF")))
                    return "ActiveMARK";
            }

            return null;
        }
    }
}
