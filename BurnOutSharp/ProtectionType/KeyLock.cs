using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class KeyLock : IContentCheck
    {
        /// <summary>
        /// Set of all ContentMatchSets for this protection
        /// </summary>
        private static List<ContentMatchSet> contentMatchers = new List<ContentMatchSet>
        {
            // KEY-LOCK COMMAND
            new ContentMatchSet(new byte?[]
            {
                0x4B, 0x45, 0x59, 0x2D, 0x4C, 0x4F, 0x43, 0x4B,
                0x20, 0x43, 0x4F, 0x4D, 0x4D, 0x41, 0x4E, 0x44
            }, "Key-Lock (Dongle)"),
        };

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            return MatchUtil.GetFirstMatch(file, fileContent, contentMatchers, includePosition);
        }
    }
}
