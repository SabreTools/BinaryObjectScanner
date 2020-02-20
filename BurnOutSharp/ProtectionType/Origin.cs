using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Origin
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("OriginSetup.exe", StringComparison.OrdinalIgnoreCase)))
                    return "Origin";
            }
            else
            {
                if (Path.GetFileName(path).Equals("OriginSetup.exe", StringComparison.OrdinalIgnoreCase))
                    return "Origin";
            }

            return null;
        }
    }
}
