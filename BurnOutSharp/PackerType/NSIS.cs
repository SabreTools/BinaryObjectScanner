using System;
using System.Linq;
using System.Text;

namespace BurnOutSharp.PackerType
{
    public class NSIS
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // Nullsoft Install System
            byte[] check = new byte[] { 0x4e, 0x75, 0x6c, 0x6c, 0x73, 0x6f, 0x66, 0x74, 0x20, 0x49, 0x6e, 0x73, 0x74, 0x61, 0x6c, 0x6c, 0x20, 0x53, 0x79, 0x73, 0x74, 0x65, 0x6d };
            if (fileContent.Contains(check, out int position))
            {
                string version = GetVersion(fileContent, position);
                return $"NSIS {version}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            // NullsoftInst
            check = new byte[] { 0x4e, 0x75, 0x6c, 0x6c, 0x73, 0x6f, 0x66, 0x74, 0x49, 0x6e, 0x73, 0x74 };
            if (fileContent.Contains(check, out position))
            {
                return $"NSIS" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            return null;
        }

        private static string GetVersion(byte[] fileContent, int index)
        {
            try
            {
                index += 24;
                if (fileContent[index] != 'v')
                    return "(Unknown Version)";

                var versionBytes = new ReadOnlySpan<byte>(fileContent, index, 16).ToArray();
                var onlyVersion = versionBytes.TakeWhile(b => b != '<').ToArray();
                return Encoding.ASCII.GetString(onlyVersion);
            }
            catch
            {
                return "(Unknown Version)";
            }
        }
    }
}