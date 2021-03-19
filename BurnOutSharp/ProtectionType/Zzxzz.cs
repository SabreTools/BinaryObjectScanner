using System;
using System.Collections.Generic;
using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class Zzxzz : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (File.Exists(Path.Combine(path, "Zzxzz", "Zzz.aze")))
                return "Zzxzz";

            else if (Directory.Exists(Path.Combine(path, "Zzxzz")))
                return "Zzxzz";

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            string filename = Path.GetFileName(path);
            if (filename.Equals("Zzz.aze", StringComparison.OrdinalIgnoreCase))
                return "Zzxzz";

            return null;
        }
    }
}
