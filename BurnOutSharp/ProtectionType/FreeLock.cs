using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class FreeLock : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("FREELOCK.IMG", StringComparison.OrdinalIgnoreCase)))
                return "FreeLock";
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("FREELOCK.IMG", StringComparison.OrdinalIgnoreCase))
                return "FreeLock";

            return null;
        }
    }
}
