using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class GFWL
    {
        public static string CheckContents(byte[] fileContent)
        {
            // "xlive.dll"
            byte[] check = new byte[] { 0x78, 0x6C, 0x69, 0x76, 0x65, 0x2E, 0x64, 0x6C, 0x6C };
            if (fileContent.Contains(check, out int position))
                return $"Games for Windows - Live (Index {position})";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("XLiveRedist.msi", StringComparison.OrdinalIgnoreCase)))
                    return "Games for Windows - Live";
            }
            else
            {
                if (Path.GetFileName(path).Equals("XLiveRedist.msi", StringComparison.OrdinalIgnoreCase))
                    return "Games for Windows - Live";
            }

            return null;
        }
    }
}
