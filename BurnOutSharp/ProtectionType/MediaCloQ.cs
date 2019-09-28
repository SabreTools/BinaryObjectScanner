using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class MediaCloQ
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Count(f => Path.GetFileName(f).Equals("sunncomm.ico", StringComparison.OrdinalIgnoreCase)) > 0)
                    return "MediaCloQ";
            }
            else
            {
                if (Path.GetFileName(path).Equals("sunncomm.ico", StringComparison.OrdinalIgnoreCase))
                    return "MediaCloQ";
            }

            return null;
        }
    }
}
