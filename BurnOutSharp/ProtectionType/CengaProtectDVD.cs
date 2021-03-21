namespace BurnOutSharp.ProtectionType
{
    public class CengaProtectDVD : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // ".cenega"
            byte?[] check = new byte?[] { 0x2E, 0x63, 0x65, 0x6E, 0x65, 0x67, 0x61 };
            if (fileContent.FirstPosition(check, out int position))
                return "Cenega ProtectDVD" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
