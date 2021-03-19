using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Bitpool : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("bitpool.rsc", StringComparison.OrdinalIgnoreCase)))
                return "Bitpool";

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("bitpool.rsc", StringComparison.OrdinalIgnoreCase))
                return "Bitpool";
            
            return null;
        }
    }
}
