namespace BurnOutSharp.ProtectionType
{
    public class RingPROTECH : IContentCheck
    {
        /// <inheritdoc/>
        /// TODO: Investigate as this may be over-matching
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // (char)0x00 + "Allocator" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00
            byte?[] check = new byte?[] { 0x00, 0x41, 0x6C, 0x6C, 0x6F, 0x63, 0x61, 0x74, 0x6F, 0x72, 0x00, 0x00, 0x00, 0x00 };
            if (fileContent.FirstPosition(check, out int position))
                return "Ring PROTECH [Check disc for physical ring]" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
