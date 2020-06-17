namespace BurnOutSharp.ProtectionType
{
    public class SVKProtector
    {
        public static string CheckContents(string fileContent)
        {
            string check = "?SVKP" + (char)0x00 + (char)0x00;
            if (fileContent.Contains(check))
                return $"SVK Protector (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
