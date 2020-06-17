namespace BurnOutSharp.ProtectionType
{
    public class KeyLock
    {
        public static string CheckContents(string fileContent)
        {
            string check = "KEY-LOCK COMMAND";
            if (fileContent.Contains(check))
                return $"Key-Lock (Dongle) (Index {fileContent.IndexOf(check)})";

            return null;
        }
    }
}
