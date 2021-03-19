using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Uplay : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("UplayInstaller.exe", StringComparison.OrdinalIgnoreCase)))
                return "Uplay";

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("UplayInstaller.exe", StringComparison.OrdinalIgnoreCase))
                return "Uplay";

            return null;
        }
    }
}
