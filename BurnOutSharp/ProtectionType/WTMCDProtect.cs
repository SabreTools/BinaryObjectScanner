using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class WTMCDProtect
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // "WTM76545"
            byte[] check = new byte[] { 0x57, 0x54, 0x4D, 0x37, 0x36, 0x35, 0x34, 0x35 };
            if (fileContent.Contains(check, out int position))
                return "WTM CD Protect" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetExtension(f).Trim('.').Equals("IMP", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("imp.dat", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("wtmfiles.dat", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("Viewer.exe", StringComparison.OrdinalIgnoreCase)))
                {
                    return "WTM CD Protect";
                }
            }
            else
            {
                if (Path.GetExtension(path).Trim('.').Equals("IMP", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("imp.dat", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("wtmfiles.dat", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("Viewer.exe", StringComparison.OrdinalIgnoreCase))
                {
                    return "WTM CD Protect";
                }
            }

            return null;
        }
    }
}
