using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Uplay
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Count(f => Path.GetFileName(f).Equals("UplayInstaller.exe", StringComparison.OrdinalIgnoreCase)) > 0)
                    return "Uplay";
            }
            else
            {
                if (Path.GetFileName(path).Equals("UplayInstaller.exe", StringComparison.OrdinalIgnoreCase))
                    return "Uplay";
            }

            return null;
        }
    }
}
