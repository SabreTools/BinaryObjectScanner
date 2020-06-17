namespace BurnOutSharp.ProtectionType
{
    public class CDSHiELDSE
    {
        public static string CheckContents(string fileContent)
        {
            string check = "~0017.tmp";
            if (fileContent.Contains(check))
                return $"CDSHiELD SE (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
