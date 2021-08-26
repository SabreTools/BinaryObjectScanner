using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDCheck : IContentCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                // MGS CDCheck
                new ContentMatchSet(new byte?[]
                {
                    0x4D, 0x47, 0x53, 0x20, 0x43, 0x44, 0x43, 0x68,
                    0x65, 0x63, 0x6B
                }, "Microsoft Game Studios CD Check"),

                // CDCheck
                new ContentMatchSet(new byte?[] { 0x43, 0x44, 0x43, 0x68, 0x65, 0x63, 0x6B }, "Executable-Based CD Check"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false) => null;

        // These content checks are too broad to be useful
        private static string CheckContentsBroad(string file, byte[] fileContent, bool includeDebug = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // GetDriveType
                new ContentMatchSet(new byte?[]
                {
                    0x47, 0x65, 0x74, 0x44, 0x72, 0x69, 0x76, 0x65,
                    0x54, 0x79, 0x70, 0x65
                }, "Executable-Based CD Check"),

                // GetVolumeInformation
                new ContentMatchSet(new byte?[]
                {
                    0x47, 0x65, 0x74, 0x56, 0x6F, 0x6C, 0x75, 0x6D,
                    0x65, 0x49, 0x6E, 0x66, 0x6F, 0x72, 0x6D, 0x61,
                    0x74, 0x69, 0x6F, 0x6E
                }, "Executable-Based CD Check"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
        }
    }
}
