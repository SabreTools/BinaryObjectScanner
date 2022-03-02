using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class KeyLock : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                {
                    // KEY-LOCK COMMAND
                    new ContentMatchSet(new byte?[]
                    {
                        0x4B, 0x45, 0x59, 0x2D, 0x4C, 0x4F, 0x43, 0x4B,
                        0x20, 0x43, 0x4F, 0x4D, 0x4D, 0x41, 0x4E, 0x44
                    }, "Key-Lock (Dongle)"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }
    }
}
