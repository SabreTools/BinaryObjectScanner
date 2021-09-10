using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class KeyLock : IContentCheck
    {
        /// <inheritdoc/>
        private List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                // KEY-LOCK COMMAND
                new ContentMatchSet(new byte?[]
                {
                    0x4B, 0x45, 0x59, 0x2D, 0x4C, 0x4F, 0x43, 0x4B,
                    0x20, 0x43, 0x4F, 0x4D, 0x4D, 0x41, 0x4E, 0x44
                }, "Key-Lock (Dongle)"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var contentMatchSets = GetContentMatchSets();
            if (contentMatchSets != null && contentMatchSets.Any())
                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);

            return null;
        }
    }
}
