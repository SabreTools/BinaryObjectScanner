namespace BurnOutSharp.ProtectionType
{
    public class AlphaROM
    {
        public static string CheckContents(byte[] fileContent)
        {
            // "SETTEC"
            byte[] check = new byte[] { 0x53, 0x45, 0x54, 0x54, 0x45, 0x43 };
            if (fileContent.Contains(check, out int position))
                return $"Alpha-ROM (Index {position})";

            return null;
        }
    }
}
