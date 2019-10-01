using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class GFWL
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("xlive.dll"))
                return "Games for Windows - Live";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Count(f => Path.GetFileName(f).Equals("XLiveRedist.msi", StringComparison.OrdinalIgnoreCase)) > 0)
                    return "Games for Windows - Live";
            }
            else
            {
                if (Path.GetFileName(path).Equals("XLiveRedist.msi", StringComparison.OrdinalIgnoreCase))
                    return "Games for Windows - Live";
            }

            return null;
        }
    }
}
