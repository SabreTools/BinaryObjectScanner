using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SafeDiscLite : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("00000001.LT1", StringComparison.OrdinalIgnoreCase)))
                return "SafeDisc Lite";
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("00000001.LT1", StringComparison.OrdinalIgnoreCase))
                return "SafeDisc Lite";

            return null;
        }
    }
}
