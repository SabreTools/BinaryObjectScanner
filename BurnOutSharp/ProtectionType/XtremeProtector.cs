namespace BurnOutSharp.ProtectionType
{
    public class XtremeProtector
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("XPROT   "))
                return "Xtreme-Protector";

            return null;
        }
    }
}
