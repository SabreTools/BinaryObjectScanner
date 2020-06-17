namespace BurnOutSharp.ProtectionType
{
    public class dotFuscator
    {
        public static string CheckContents(byte[] fileContent)
        {
            // "DotfuscatorAttribute"
            byte[] check = new byte[] { 0x44, 0x6F, 0x74, 0x66, 0x75, 0x73, 0x63, 0x61, 0x74, 0x6F, 0x72, 0x41, 0x74, 0x74, 0x72, 0x69, 0x62, 0x75, 0x74, 0x65 };
            if (fileContent.Contains(check, out int position))
                return $"dotFuscator (Index {position})";

            return null;
        }
    }
}
