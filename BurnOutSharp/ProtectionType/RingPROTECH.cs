namespace BurnOutSharp.ProtectionType
{
    public class RingPROTECH
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains((char)0x00 + "Allocator" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00))
                return "Ring PROTECH";

            return null;
        }
    }
}
