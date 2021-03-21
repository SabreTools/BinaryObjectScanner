namespace BurnOutSharp.ProtectionType
{
    public class AlphaROM : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "SETTEC"
            byte[] check = new byte[] { 0x53, 0x45, 0x54, 0x54, 0x45, 0x43 };
            if (fileContent.FirstPosition(check, out int position))
                return "Alpha-ROM" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
