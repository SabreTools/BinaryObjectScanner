using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class FreeLock
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Count(f => Path.GetFileName(f).Equals("FREELOCK.IMG", StringComparison.OrdinalIgnoreCase)) > 0)
                    return "FreeLock";
            }
            else
            {
                if (Path.GetFileName(path).Equals("FREELOCK.IMG", StringComparison.OrdinalIgnoreCase))
                    return "FreeLock";
            }

            return null;
        }
    }
}
