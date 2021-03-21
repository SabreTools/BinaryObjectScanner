namespace BurnOutSharp.ProtectionType
{
    public class SVKProtector : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "?SVKP" + (char)0x00 + (char)0x00
            byte[] check = new byte[] { 0x3F, 0x53, 0x56, 0x4B, 0x50, 0x00, 0x00 };
            if (fileContent.FirstPosition(check, out int position))
                return "SVK Protector" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
