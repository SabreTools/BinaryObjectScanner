namespace BurnOutSharp.ProtectionType
{
    public class RingPROTECH
    {
        public static string CheckContents(string fileContent)
        {
            string check = (char)0x00 + "Allocator" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00;
            if (fileContent.Contains(check))
                return $"Ring PROTECH (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
