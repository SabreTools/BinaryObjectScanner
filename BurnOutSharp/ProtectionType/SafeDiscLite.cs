using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SafeDiscLite
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Count(f => Path.GetFileName(f).Equals("00000001.LT1", StringComparison.OrdinalIgnoreCase)) > 0)
                    return "SafeDisc Lite";
            }
            else
            {
                if (Path.GetFileName(path).Equals("00000001.LT1", StringComparison.OrdinalIgnoreCase))
                    return "SafeDisc Lite";
            }

            return null;
        }
    }
}
