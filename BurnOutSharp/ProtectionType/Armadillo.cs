namespace BurnOutSharp.ProtectionType
{
    public class Armadillo
    {
        public static string CheckContents(string fileContent)
        {
            string check = ".nicode" + (char)0x00;
            if (fileContent.Contains(check))
                return $"Armadillo (Index {fileContent.IndexOf(check)})";

            check = "ARMDEBUG";
            if (fileContent.Contains(check))
                return $"Armadillo (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
