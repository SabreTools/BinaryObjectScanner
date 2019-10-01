using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class MediaMaxCD3
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Count(f => Path.GetFileName(f).Equals("LaunchCd.exe", StringComparison.OrdinalIgnoreCase)) > 0)
                    return "MediaMax CD3";
            }
            else
            {
                if (Path.GetFileName(path).Equals("LaunchCd.exe", StringComparison.OrdinalIgnoreCase))
                    return "MediaMax CD3";
            }

            return null;
        }
    }
}
