using System.Collections.Generic;

namespace BurnOutSharp.ProtectionType
{
    public class AlphaROM : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var mappings = new Dictionary<byte?[], string>
            {
                // SETTEC
                [new byte?[] { 0x53, 0x45, 0x54, 0x54, 0x45, 0x43 }] = "Alpha-ROM",
            };

            return Utilities.GetContentMatches(fileContent, mappings, includePosition);
        }
    }
}
