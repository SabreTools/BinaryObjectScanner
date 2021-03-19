using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class AlphaDVD : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("PlayDVD.exe", StringComparison.OrdinalIgnoreCase)))
                return "Alpha-DVD";

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("PlayDVD.exe", StringComparison.OrdinalIgnoreCase))
                return "Alpha-DVD";
            
            return null;
        }
    }
}
