using System;
using System.Collections.Generic;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ProtectionType
{
    public class CDKey : IContentCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                // I + (char)0x00 + n + (char)0x00 + t + (char)0x00 + e + (char)0x00 + r + (char)0x00 + n + (char)0x00 + a + (char)0x00 + l + (char)0x00 + N + (char)0x00 + a + (char)0x00 + m + (char)0x00 + e + (char)0x00 +  + (char)0x00 +  + (char)0x00 + C + (char)0x00 + D + (char)0x00 + K + (char)0x00 + e + (char)0x00 + y + (char)0x00
                new ContentMatchSet(new byte?[]
                {
                    0x49, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x65, 0x00,
                    0x72, 0x00, 0x6E, 0x00, 0x61, 0x00, 0x6C, 0x00,
                    0x4E, 0x00, 0x61, 0x00, 0x6D, 0x00, 0x65, 0x00,
                    0x00, 0x00, 0x43, 0x00, 0x44, 0x00, 0x4B, 0x00,
                    0x65, 0x00, 0x79, 0x00
                }, Utilities.GetFileVersion, "CD-Key / Serial"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            // PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            // var sections = pex?.SectionTable;
            // if (sections == null)
            //     return null;
            
            // TODO: Implement resource finding instead of using the built in methods
            // Assembly information lives in the .rsrc section
            // I need to find out how to navigate the resources in general
            // as well as figure out the specific resources for both
            // file info and MUI (XML) info. Once I figure this out,
            // that also opens the doors to easier assembly XML checks.

            var fvinfo = Utilities.GetFileVersionInfo(file);

            string name = fvinfo?.InternalName?.Trim();
            if (!string.IsNullOrWhiteSpace(name) && name.Equals("CDKey", StringComparison.OrdinalIgnoreCase))
                return "CD-Key / Serial";

            return null;
        }
    }
}
