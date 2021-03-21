namespace BurnOutSharp.PackerType
{
    public class PECompact : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "PEC2"
            byte[] check = new byte[] { 0x50, 0x45, 0x43, 0x32 };
            if (fileContent.FirstPosition(check, out int position, end: 2048))
                return "PE Compact 2" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
