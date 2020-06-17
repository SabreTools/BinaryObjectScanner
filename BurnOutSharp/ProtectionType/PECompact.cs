namespace BurnOutSharp.ProtectionType
{
    public class PECompact
    {
        public static string CheckContents(string fileContent)
        {
            string check = "PEC2";
            if (fileContent.Contains(check))
                return $"PE Compact 2 (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
