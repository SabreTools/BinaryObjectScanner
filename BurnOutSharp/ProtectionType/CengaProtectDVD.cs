namespace BurnOutSharp.ProtectionType
{
    public class CengaProtectDVD
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains(".cenega"))
                return "Cenega ProtectDVD";

            return null;
        }
    }
}
