using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class TZCopyProtector
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("_742893.016", StringComparison.OrdinalIgnoreCase)))
                    return "TZCopyProtector";
            }
            else
            {
                if (Path.GetFileName(path).Equals("_742893.016", StringComparison.OrdinalIgnoreCase))
                    return "TZCopyProtector";
            }

            return null;
        }
    }
}
