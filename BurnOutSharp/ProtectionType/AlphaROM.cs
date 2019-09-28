namespace BurnOutSharp.ProtectionType
{
    public class AlphaROM
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("SETTEC"))
                return "Alpha-ROM";

            return null;
        }
    }
}
