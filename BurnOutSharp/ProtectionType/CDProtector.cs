using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CDProtector : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("_cdp16.dat", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("_cdp16.dll", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("_cdp32.dat", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("_cdp32.dll", StringComparison.OrdinalIgnoreCase)))
            {
                return "CD-Protector";
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("_cdp16.dat", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("_cdp16.dll", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("_cdp32.dat", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("_cdp32.dll", StringComparison.OrdinalIgnoreCase))
            {
                return "CD-Protector";
            }
            
            return null;
        }
    }
}
