using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SafeCast
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("cdac11ba.exe", StringComparison.OrdinalIgnoreCase)))
                    return "SafeCast";
            }
            else
            {
                if (Path.GetFileName(path).Equals("cdac11ba.exe", StringComparison.OrdinalIgnoreCase))
                    return "SafeCast";
            }

            return null;
        }
    }
}
