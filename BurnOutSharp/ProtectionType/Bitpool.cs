using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Bitpool
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("bitpool.rsc", StringComparison.OrdinalIgnoreCase)))
                    return "Bitpool";
            }
            else
            {
                if (Path.GetFileName(path).Equals("bitpool.rsc", StringComparison.OrdinalIgnoreCase))
                    return "Bitpool";
            }

            return null;
        }
    }
}
