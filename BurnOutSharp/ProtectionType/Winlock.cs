using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Winlock : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("WinLock.PSX", StringComparison.OrdinalIgnoreCase)))
                    return "Winlock";
            }
            else
            {
                if (Path.GetFileName(path).Equals("WinLock.PSX", StringComparison.OrdinalIgnoreCase))
                    return "Winlock";
            }

            return null;
        }
    }
}
