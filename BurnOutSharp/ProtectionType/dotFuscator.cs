namespace BurnOutSharp.ProtectionType
{
    public class dotFuscator
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("DotfuscatorAttribute"))
                return "dotFuscator";

            return null;
        }
    }
}
