namespace BurnOutSharp.ProtectionType
{
    public class CodeLock
    {
        // TODO: Verify if these are OR or AND
        public static string CheckContents(byte[] fileContent)
        {
            // "icd1" + (char)0x00
            byte[] check = new byte[] { 0x69, 0x63, 0x64, 0x31, 0x00 };
            if (fileContent.Contains(check, out int position))
                return $"Code Lock (Index {position})";

            // "icd2" + (char)0x00
            check = new byte[] { 0x69, 0x63, 0x64, 0x32, 0x00 };
            if (fileContent.Contains(check, out position))
                return $"Code Lock (Index {position})";

            // "CODE-LOCK.OCX"
            check = new byte[] { 0x43, 0x4F, 0x44, 0x45, 0x2D, 0x4C, 0x4F, 0x43, 0x4B, 0x2E, 0x4F, 0x43, 0x58 };
            if (fileContent.Contains(check, out position))
                return $"Code Lock (Index {position})";

            return null;
        }
    }
}
