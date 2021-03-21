using System.Collections.Generic;

namespace BurnOutSharp.ProtectionType
{
    public class SVKProtector : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var mappings = new Dictionary<byte?[], string>
            {
                // ?SVKP + (char)0x00 + (char)0x00
                [new byte?[] { 0x3F, 0x53, 0x56, 0x4B, 0x50, 0x00, 0x00 }] = "SVK Protector",
            };

            return Utilities.GetContentMatches(fileContent, mappings, includePosition);
        }
    }
}
