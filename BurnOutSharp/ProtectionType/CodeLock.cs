namespace BurnOutSharp.ProtectionType
{
    public class CodeLock : IContentCheck
    {
        // TODO: Verify if these are OR or AND
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "icd1" + (char)0x00
            byte[] check = new byte[] { 0x69, 0x63, 0x64, 0x31, 0x00 };
            if (fileContent.Contains(check, out int position))
                return "Code Lock" + (includePosition ? $" (Index {position})" : string.Empty);

            // "icd2" + (char)0x00
            check = new byte[] { 0x69, 0x63, 0x64, 0x32, 0x00 };
            if (fileContent.Contains(check, out position))
                return "Code Lock" + (includePosition ? $" (Index {position})" : string.Empty);

            // "CODE-LOCK.OCX"
            check = new byte[] { 0x43, 0x4F, 0x44, 0x45, 0x2D, 0x4C, 0x4F, 0x43, 0x4B, 0x2E, 0x4F, 0x43, 0x58 };
            if (fileContent.Contains(check, out position))
                return "Code Lock" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
