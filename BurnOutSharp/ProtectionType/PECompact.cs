namespace BurnOutSharp.ProtectionType
{
    public class PECompact
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("PEC2"))
                return "PE Compact 2";

            return null;
        }
    }
}
