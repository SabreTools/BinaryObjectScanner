using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CDCops : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "CD-Cops,  ver. "
            byte[] check = new byte[] { 0x43, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73, 0x2C, 0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20 };
            if (fileContent.Contains(check, out int position))
                return $"CD-Cops {GetVersion(fileContent, position)}" + (includePosition ? $" (Index {position})" : string.Empty);

            // ".grand" + (char)0x00
            check = new byte[] { 0x2E, 0x67, 0x72, 0x61, 0x6E, 0x64, 0x00};
            if (fileContent.Contains(check, out position))
                return "CD-Cops" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        /// <inheritdoc/>
        public string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("CDCOPS.DLL", StringComparison.OrdinalIgnoreCase))
                    && (files.Any(f => Path.GetExtension(f).Trim('.').Equals("GZ_", StringComparison.OrdinalIgnoreCase))
                        || files.Any(f => Path.GetExtension(f).Trim('.').Equals("W_X", StringComparison.OrdinalIgnoreCase))
                        || files.Any(f => Path.GetExtension(f).Trim('.').Equals("Qz", StringComparison.OrdinalIgnoreCase))
                        || files.Any(f => Path.GetExtension(f).Trim('.').Equals("QZ_", StringComparison.OrdinalIgnoreCase))))
                {
                    return "CD-Cops";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("CDCOPS.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("GZ_", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("W_X", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("Qz", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("QZ_", StringComparison.OrdinalIgnoreCase))
                {
                    return "CD-Cops";
                }
            }

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
