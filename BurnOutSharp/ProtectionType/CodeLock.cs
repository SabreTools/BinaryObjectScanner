using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CodeLock : IContentCheck
    {
        // TODO: Verify if these are OR or AND
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // icd1 + (char)0x00
                new ContentMatchSet(new byte?[] { 0x69, 0x63, 0x64, 0x31, 0x00 }, "Code Lock"),

                // icd2 + (char)0x00
                new ContentMatchSet(new byte?[] { 0x69, 0x63, 0x64, 0x32, 0x00 }, "Code Lock"),

                // CODE-LOCK.OCX
                new ContentMatchSet(new byte?[]
                {
                    0x43, 0x4F, 0x44, 0x45, 0x2D, 0x4C, 0x4F, 0x43,
                    0x4B, 0x2E, 0x4F, 0x43, 0x58
                }, "Code Lock"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includePosition);
        }
    }
}
