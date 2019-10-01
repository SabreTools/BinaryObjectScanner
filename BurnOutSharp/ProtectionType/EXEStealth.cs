namespace BurnOutSharp.ProtectionType
{
    public class EXEStealth
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("??[[__[[_" + (char)0x00 + "{{" + (char)0x0
                + (char)0x00 + "{{" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x0
                + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "?;??;??"))
            {
                return "EXE Stealth";
            }

            return null;
        }
    }
}
