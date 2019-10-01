using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class ByteShield
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Count(f => Path.GetFileName(f).Equals("Byteshield.dll", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetExtension(f).Trim('.').Equals("bbz", StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    return "ByteShield";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("Byteshield.dll", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("bbz", StringComparison.OrdinalIgnoreCase))
                {
                    return "ByteShield";
                }
            }

            return null;
        }
    }
}
