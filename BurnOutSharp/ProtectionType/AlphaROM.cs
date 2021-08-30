using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class AlphaROM : IContentCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                // SETTEC
                new ContentMatchSet(new byte?[] { 0x53, 0x45, 0x54, 0x54, 0x45, 0x43 }, "Alpha-ROM"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false) => null;
    }
}
