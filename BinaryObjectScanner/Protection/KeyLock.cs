using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Content;

namespace BinaryObjectScanner.Protection
{
    public class KeyLock : IContentCheck
    {
        /// <inheritdoc/>
        public string? CheckContents(string file, byte[] fileContent, bool includeDebug)
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                {
                    // KEY-LOCK COMMAND
                    new(new byte?[]
                    {
                        0x4B, 0x45, 0x59, 0x2D, 0x4C, 0x4F, 0x43, 0x4B,
                        0x20, 0x43, 0x4F, 0x4D, 0x4D, 0x41, 0x4E, 0x44
                    }, "Key-Lock (Dongle) (Unconfirmed - Please report to us on Github)"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }
    }
}
