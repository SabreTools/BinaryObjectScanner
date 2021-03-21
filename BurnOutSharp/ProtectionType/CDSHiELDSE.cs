using System.Collections.Generic;

namespace BurnOutSharp.ProtectionType
{
    public class CDSHiELDSE : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var mappings = new Dictionary<byte?[], string>
            {
                // ~0017.tmp
                [new byte?[] { 0x7E, 0x30, 0x30, 0x31, 0x37, 0x2E, 0x74, 0x6D, 0x70 }] = "CDSHiELD SE",
            };

            return Utilities.GetContentMatches(fileContent, mappings, includePosition);
        }
    }
}
