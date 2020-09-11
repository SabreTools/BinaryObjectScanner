namespace BurnOutSharp.ProtectionType
{
    public class PECompact
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // "PEC2"
            byte[] check = new byte[] { 0x50, 0x45, 0x43, 0x32 };
            if (fileContent.Contains(check, out int position))
                return "PE Compact 2" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
