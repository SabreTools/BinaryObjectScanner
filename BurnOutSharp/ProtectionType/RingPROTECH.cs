namespace BurnOutSharp.ProtectionType
{
    public class RingPROTECH
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // (char)0x00 + "Allocator" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00
            byte[] check = new byte[] { 0x00, 0x41, 0x6C, 0x6C, 0x6F, 0x63, 0x61, 0x74, 0x6F, 0x72, 0x00, 0x00, 0x00, 0x00 };
            if (fileContent.Contains(check, out int position))
                return "Ring PROTECH" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
