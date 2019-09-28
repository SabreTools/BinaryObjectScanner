using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CDProtector
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Count(f => Path.GetFileName(f).Equals("_cdp16.dat", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("_cdp16.dll", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("_cdp32.dat", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("_cdp32.dll", StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    return "CD-Protector";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("_cdp16.dat", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("_cdp16.dll", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("_cdp32.dat", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("_cdp32.dll", StringComparison.OrdinalIgnoreCase))
                {
                    return "CD-Protector";
                }
            }

            return null;
        }
    }
}
