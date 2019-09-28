namespace BurnOutSharp.ProtectionType
{
    public class ActiveMARK
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("TMSAMVOF"))
            {
                return "ActiveMARK";
            }

            else if (fileContent.Contains(" " + (char)0xC2 + (char)0x16 + (char)0x00 + (char)0xA8 + (char)0xC1 + (char)0x16
                + (char)0x00 + (char)0xB8 + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0x86 + (char)0xC8 + (char)0x16 + (char)0x0
                + (char)0x9A + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0x10 + (char)0xC2 + (char)0x16 + (char)0x00))
            {
                return "ActiveMARK 5";
            }

            return null;
        }
    }
}
