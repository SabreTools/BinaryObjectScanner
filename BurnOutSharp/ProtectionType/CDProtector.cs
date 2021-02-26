using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CDProtector : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("_cdp16.dat", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("_cdp16.dll", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("_cdp32.dat", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("_cdp32.dll", StringComparison.OrdinalIgnoreCase)))
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
