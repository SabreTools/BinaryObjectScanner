using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class TZCopyProtector : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("_742893.016", StringComparison.OrdinalIgnoreCase)))
                return "TZCopyProtector";

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("_742893.016", StringComparison.OrdinalIgnoreCase))
                return "TZCopyProtector";

            return null;
        }
    }
}
