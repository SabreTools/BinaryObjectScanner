namespace BurnOutSharp.ProtectionType
{
    public class EXEStealth
    {
        public static string CheckContents(string fileContent)
        {
            string check = "??[[__[[_" + (char)0x00 + "{{" + (char)0x0 + (char)0x00 + "{{" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x0 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "?;??;??";
            if (fileContent.Contains(check))
                return $"EXE Stealth (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
