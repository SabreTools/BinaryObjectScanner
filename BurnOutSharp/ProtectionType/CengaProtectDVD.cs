namespace BurnOutSharp.ProtectionType
{
    public class CengaProtectDVD
    {
        public static string CheckContents(string fileContent)
        {
            string check = ".cenega";
            if (fileContent.Contains(check))
                return $"Cenega ProtectDVD (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
