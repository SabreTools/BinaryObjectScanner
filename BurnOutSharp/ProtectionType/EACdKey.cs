namespace BurnOutSharp.ProtectionType
{
    public class EACdKey
    {
        public static string CheckContents(string fileContent)
        {
            string check = "ereg.ea-europe.com";
            if (fileContent.Contains(check))
                return $"EA CdKey Registration Module (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
