namespace BurnOutSharp.ProtectionType
{
    public class XtremeProtector : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "XPROT   "
            byte[] check = new byte[] { 0x58, 0x50, 0x52, 0x4F, 0x54, 0x20, 0x20, 0x20 };
            if (fileContent.FirstPosition(check, out int position))
                return "Xtreme-Protector" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
