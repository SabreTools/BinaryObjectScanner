using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Winlock : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("WinLock.PSX", StringComparison.OrdinalIgnoreCase)))
                return "Winlock";

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("WinLock.PSX", StringComparison.OrdinalIgnoreCase))
                return "Winlock";

            return null;
        }
    }
}
