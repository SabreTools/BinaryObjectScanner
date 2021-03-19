using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SafeCast : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("cdac11ba.exe", StringComparison.OrdinalIgnoreCase)))
                return "SafeCast";
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("cdac11ba.exe", StringComparison.OrdinalIgnoreCase))
                return "SafeCast";

            return null;
        }
    }
}
