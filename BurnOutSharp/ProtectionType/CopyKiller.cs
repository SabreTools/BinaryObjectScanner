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
            string check = "Tom Commander";
            if (fileContent.Contains(check))
                return $"CopyKiller (Index {fileContent.IndexOf(check)})";

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
