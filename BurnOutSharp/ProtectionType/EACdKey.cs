namespace BurnOutSharp.ProtectionType
{
    public class EACdKey
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("ereg.ea-europe.com"))
                return "EA CdKey Registration Module";

            return null;
        }
    }
}
