using System.Collections.Generic;

namespace BurnOutSharp.ProtectionType
{
    public class CengaProtectDVD : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var mappings = new Dictionary<byte?[], string>
            {
                // .cenega
                [new byte?[] { 0x2E, 0x63, 0x65, 0x6E, 0x65, 0x67, 0x61 }] = "Cenega ProtectDVD",
            };

            return Utilities.GetContentMatches(fileContent, mappings, includePosition);
        }
    }
}
