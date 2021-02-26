using System;
using System.Collections.Generic;
using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class Zzxzz : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (File.Exists(Path.Combine(path, "Zzxzz", "Zzz.aze")))
                    return "Zzxzz";

                else if (Directory.Exists(Path.Combine(path, "Zzxzz")))
                    return "Zzxzz";
            }
            else
            {
                string filename = Path.GetFileName(path);
                if (filename.Equals("Zzz.aze", StringComparison.OrdinalIgnoreCase))
                    return "Zzxzz";
            }

            return null;
        }
    }
}
