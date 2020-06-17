namespace BurnOutSharp.ProtectionType
{
    public class CodeLock
    {
        public static string CheckContents(string fileContent)
        {
            // TODO: Verify if these are OR or AND
            string check = "icd1" + (char)0x00;
            if (fileContent.Contains(check))
                return $"Code Lock (Index {fileContent.IndexOf(check)})";

            check = "icd2" + (char)0x00;
            if (fileContent.Contains(check))
                return $"Code Lock (Index {fileContent.IndexOf(check)})";

            check = ".ldr";
            if (fileContent.Contains(check))
                return $"Code Lock (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
