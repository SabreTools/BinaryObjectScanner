namespace BurnOutSharp.ProtectionType
{
    public class WiseInstaller
    {
        // TODO: Add Wise Installer extraction
        public static string CheckContents(byte[] fileContent)
        {
            // "WiseMain"
            byte[] check = new byte[] { 0x57, 0x69, 0x73, 0x65, 0x4D, 0x61, 0x69, 0x6E };
            if (fileContent.Contains(check, out int position))
                return $"Wise Installation Wizard Module (Index {position})";

            return null;
        }
    }
}
