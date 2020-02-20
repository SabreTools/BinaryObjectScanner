using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class AlphaDVD
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("PlayDVD.exe", StringComparison.OrdinalIgnoreCase)))
                    return "Alpha-DVD";
            }
            else
            {
                if (Path.GetFileName(path).Equals("PlayDVD.exe", StringComparison.OrdinalIgnoreCase))
                    return "Alpha-DVD";
            }

            return null;
        }
    }
}
