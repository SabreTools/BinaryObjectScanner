namespace BurnOutSharp.ProtectionType
{
    public class SVKProtector
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("?SVKP" + (char)0x00 + (char)0x00))
                return "SVK Protector";

            return null;
        }
    }
}
