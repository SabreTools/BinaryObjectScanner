using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CopyKiller
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("Tom Commander"))
                return "CopyKiller";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("Autorun.dat", StringComparison.OrdinalIgnoreCase)))
                    return "CopyKiller";
            }
            else
            {
                if (Path.GetFileName(path).Equals("Autorun.dat", StringComparison.OrdinalIgnoreCase))
                    return "CopyKiller";
            }

            return null;
        }
    }
}
