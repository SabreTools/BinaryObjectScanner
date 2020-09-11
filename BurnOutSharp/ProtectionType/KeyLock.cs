namespace BurnOutSharp.ProtectionType
{
    public class KeyLock
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // "KEY-LOCK COMMAND"
            byte[] check = new byte[] { 0x4B, 0x45, 0x59, 0x2D, 0x4C, 0x4F, 0x43, 0x4B, 0x20, 0x43, 0x4F, 0x4D, 0x4D, 0x41, 0x4E, 0x44 };
            if (fileContent.Contains(check, out int position))
                return $"Key-Lock (Dongle)" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
