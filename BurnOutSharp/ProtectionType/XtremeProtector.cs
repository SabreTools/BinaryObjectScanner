namespace BurnOutSharp.ProtectionType
{
    public class XtremeProtector
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // "XPROT   "
            byte[] check = new byte[] { 0x58, 0x50, 0x52, 0x4F, 0x54, 0x20, 0x20, 0x20 };
            if (fileContent.Contains(check, out int position))
                return "Xtreme-Protector" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
