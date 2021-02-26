using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class FreeLock : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("FREELOCK.IMG", StringComparison.OrdinalIgnoreCase)))
                    return "FreeLock";
            }
            else
            {
                if (Path.GetFileName(path).Equals("FREELOCK.IMG", StringComparison.OrdinalIgnoreCase))
                    return "FreeLock";
            }

            return null;
        }
    }
}
