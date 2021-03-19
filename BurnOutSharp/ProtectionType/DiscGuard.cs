using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class DiscGuard : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("IOSLINK.VXD", StringComparison.OrdinalIgnoreCase))
                && files.Any(f => Path.GetFileName(f).Equals("IOSLINK.DLL", StringComparison.OrdinalIgnoreCase))
                && files.Any(f => Path.GetFileName(f).Equals("IOSLINK.SYS", StringComparison.OrdinalIgnoreCase)))
            {
                return "DiscGuard";
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("IOSLINK.VXD", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("IOSLINK.DLL", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("IOSLINK.SYS", StringComparison.OrdinalIgnoreCase))
            {
                return "DiscGuard";
            }
            
            return null;
        }
    }
}
