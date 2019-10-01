namespace BurnOutSharp.ProtectionType
{
    public class Cucko
    {
        public static string CheckContents(string fileContent)
        {
            // TODO: Verify this doesn't over-match
            if (fileContent.Contains("EASTL"))
                return "Cucko (EA Custom)";

            return null;
        }
    }
}
