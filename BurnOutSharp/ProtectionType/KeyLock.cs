namespace BurnOutSharp.ProtectionType
{
    public class KeyLock
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("KEY-LOCK COMMAND"))
                return "Key-Lock (Dongle)";

            return null;
        }
    }
}
