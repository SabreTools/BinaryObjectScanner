namespace BurnOutSharp.ProtectionType
{
    public class CengaProtectDVD
    {
        public static string CheckContents(byte[] fileContent)
        {
            // ".cenega"
            byte[] check = new byte[] { 0x2E, 0x63, 0x65, 0x6E, 0x65, 0x67, 0x61 };
            if (fileContent.Contains(check, out int position))
                return $"Cenega ProtectDVD (Index {position})";

            return null;
        }
    }
}
