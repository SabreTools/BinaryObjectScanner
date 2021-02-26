using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class MediaCloQ : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("sunncomm.ico", StringComparison.OrdinalIgnoreCase)))
                    return "MediaCloQ";
            }
            else
            {
                if (Path.GetFileName(path).Equals("sunncomm.ico", StringComparison.OrdinalIgnoreCase))
                    return "MediaCloQ";
            }

            return null;
        }
    }
}
