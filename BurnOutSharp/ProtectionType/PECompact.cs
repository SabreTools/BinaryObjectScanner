namespace BurnOutSharp.ProtectionType
{
    public class PECompact
    {
        public static string CheckContents(byte[] fileContent)
        {
            // "PEC2"
            byte[] check = new byte[] { 0x50, 0x45, 0x43, 0x32 };
            if (fileContent.Contains(check, out int position))
                return $"PE Compact 2 (Index {position})";

            return null;
        }
    }
}
