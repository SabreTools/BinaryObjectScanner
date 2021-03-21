using System.Collections.Generic;

namespace BurnOutSharp.ProtectionType
{
    public class XtremeProtector : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var mappings = new Dictionary<byte?[], string>
            {
                // XPROT   
                [new byte?[] { 0x58, 0x50, 0x52, 0x4F, 0x54, 0x20, 0x20, 0x20 }] = "Xtreme-Protector",
            };

            return Utilities.GetContentMatches(fileContent, mappings, includePosition);
        }
    }
}
