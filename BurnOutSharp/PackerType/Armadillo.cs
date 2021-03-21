using System.Collections.Generic;

namespace BurnOutSharp.PackerType
{
    public class Armadillo : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var mappings = new Dictionary<byte?[], string>
            {
                // .nicode + (char)0x00
                [new byte?[] { 0x2E, 0x6E, 0x69, 0x63, 0x6F, 0x64, 0x65, 0x00 }] = "Armadillo",

                // ARMDEBUG
                [new byte?[] { 0x41, 0x52, 0x4D, 0x44, 0x45, 0x42, 0x55, 0x47 }] = "Armadillo",
            };

            return Utilities.GetContentMatches(fileContent, mappings, includePosition);
        }
    }
}
