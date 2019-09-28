namespace BurnOutSharp.ProtectionType
{
    public class CodeLock
    {
        public static string CheckContents(string fileContent)
        {
            // TODO: Verify if these are OR or AND
            if (fileContent.Contains("icd1" + (char)0x00)
                || fileContent.Contains("icd2" + (char)0x00)
                || fileContent.Contains("CODE-LOCK.OCX"))
            {
                return "Code Lock";
            }

            return null;
        }
    }
}
