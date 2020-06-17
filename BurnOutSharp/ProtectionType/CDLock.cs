using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CDLock
    {
        public static string CheckContents(string fileContent)
        {
            string check = "2" + (char)0xF2 + (char)0x02 + (char)0x82 + (char)0xC3 + (char)0xBC + (char)0x0B + "$" + (char)0x99 + (char)0xAD + "'C" + (char)0xE4 + (char)0x9D + "st" + (char)0x99 + (char)0xFA + "2$" + (char)0x9D + ")4" + (char)0xFF + "t";
            if (fileContent.Contains(check))
                return $"CD-Lock (Index {fileContent.IndexOf(check)})";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetExtension(f).Trim('.').Equals("AFP", StringComparison.OrdinalIgnoreCase)))
                    return "CD-Lock";
            }
            else
            {
                if (Path.GetExtension(path).Trim('.').Equals("AFP", StringComparison.OrdinalIgnoreCase))
                    return "CD-Lock";
            }

            return null;
        }
    }
}
