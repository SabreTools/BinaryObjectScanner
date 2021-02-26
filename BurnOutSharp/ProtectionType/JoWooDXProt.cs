using System;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class JoWooDXProt : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // ".ext    "
            byte[] check = new byte[] { 0x2E, 0x65, 0x78, 0x74, 0x20, 0x20, 0x20, 0x20 };
            if (fileContent.Contains(check, out int position))
            {
                // "kernel32.dll" + (char)0x00 + (char)0x00 + (char)0x00 + "VirtualProtect"
                byte[] check2 = new byte[] { 0x6B, 0x65, 0x72, 0x6E, 0x65, 0x6C, 0x33, 0x32, 0x2E, 0x64, 0x6C, 0x6C, 0x00, 0x00, 0x00, 0x56, 0x69, 0x72, 0x74, 0x75, 0x61, 0x6C, 0x50, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74 };
                if (fileContent.Contains(check2, out int position2))
                    return $"JoWooD X-Prot {GetVersion(fileContent, --position2)}" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
                else
                    return $"JoWooD X-Prot v1" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            // "@HC09    "
            check = new byte[] { 0x40, 0x48, 0x43, 0x30, 0x39, 0x20, 0x20, 0x20, 0x20 };
            if (fileContent.Contains(check, out position))
                return $"JoWooD X-Prot v2" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        private static string GetVersion(byte[] fileContent, int position)
        {
            char[] version = new ArraySegment<byte>(fileContent, position + 67, 8).Select(b => (char)b).ToArray();
            return $"{version[0]}.{version[2]}.{version[4]}.{version[6]}{version[7]}";
        }
    }
}
