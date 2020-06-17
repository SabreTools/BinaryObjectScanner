namespace BurnOutSharp.ProtectionType
{
    public class EACdKey
    {
        public static string CheckContents(byte[] fileContent)
        {
            // "ereg.ea-europe.com"
            byte[] check = new byte[] { 0x65, 0x72, 0x65, 0x67, 0x2E, 0x65, 0x61, 0x2D, 0x65, 0x75, 0x72, 0x6F, 0x70, 0x65, 0x2E, 0x63, 0x6F, 0x6D };
            if (fileContent.Contains(check, out int position))
                return $"EA CdKey Registration Module (Index {position})";

            return null;
        }
    }
}
