using System;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class DVDCops
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // "DVD-Cops,  ver. "
            byte[] check = new byte[] { 0x44, 0x56, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73, 0x2C, 0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20 };
            if (fileContent.Contains(check, out int position))
                return $"DVD-Cops {GetVersion(fileContent, position)}" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        private static string GetVersion(byte[] fileContent, int position)
        {
            char[] version = new ArraySegment<byte>(fileContent, position + 15, 4).Select(b => (char)b).ToArray();
            if (version[0] == 0x00)
                return "";

            return new string(version);
        }
    }
}
