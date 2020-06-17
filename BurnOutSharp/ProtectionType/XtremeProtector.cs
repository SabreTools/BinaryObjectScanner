namespace BurnOutSharp.ProtectionType
{
    public class XtremeProtector
    {
        public static string CheckContents(string fileContent)
        {
            string check = "XPROT   ";
            if (fileContent.Contains(check))
                return $"Xtreme-Protector (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
