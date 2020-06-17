namespace BurnOutSharp.ProtectionType
{
    public class dotFuscator
    {
        public static string CheckContents(string fileContent)
        {
            string check = "DotfuscatorAttribute";
            if (fileContent.Contains(check))
                return $"dotFuscator (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
