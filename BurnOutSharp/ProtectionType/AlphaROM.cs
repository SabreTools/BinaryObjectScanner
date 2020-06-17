namespace BurnOutSharp.ProtectionType
{
    public class AlphaROM
    {
        public static string CheckContents(string fileContent)
        {
            string check = "SETTEC";
            if (fileContent.Contains(check))
                return $"Alpha-ROM (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
