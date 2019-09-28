namespace BurnOutSharp.ProtectionType
{
    public class CDSHiELDSE
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("~0017.tmp"))
                return "CDSHiELD SE";

            return null;
        }
    }
}
