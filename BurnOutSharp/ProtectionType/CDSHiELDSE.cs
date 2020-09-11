namespace BurnOutSharp.ProtectionType
{
    public class CDSHiELDSE
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // "~0017.tmp"
            byte[] check = new byte[] { 0x7E, 0x30, 0x30, 0x31, 0x37, 0x2E, 0x74, 0x6D, 0x70 };
            if (fileContent.Contains(check, out int position))
                return "CDSHiELD SE" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
