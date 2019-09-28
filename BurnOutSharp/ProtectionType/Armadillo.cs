namespace BurnOutSharp.ProtectionType
{
    public class Armadillo
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains(".nicode" + (char)0x00)
                || fileContent.Contains("ARMDEBUG"))
                return "Armadillo";

            return null;
        }
    }
}
