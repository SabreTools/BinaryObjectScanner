using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class MediaCloQ : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("sunncomm.ico", StringComparison.OrdinalIgnoreCase)))
                return "MediaCloQ";
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("sunncomm.ico", StringComparison.OrdinalIgnoreCase))
                return "MediaCloQ";

            return null;
        }
    }
}
