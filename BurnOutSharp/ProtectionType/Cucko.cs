namespace BurnOutSharp.ProtectionType
{
    public class Cucko
    {
        public static string CheckContents(string fileContent)
        {
            // TODO: Verify this doesn't over-match
            string check = "EASTL";
            if (fileContent.Contains(check))
                return $"Cucko (EA Custom) (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
