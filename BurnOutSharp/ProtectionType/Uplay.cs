using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Uplay : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("UplayInstaller.exe", StringComparison.OrdinalIgnoreCase)))
                    return "Uplay";
            }
            else
            {
                if (Path.GetFileName(path).Equals("UplayInstaller.exe", StringComparison.OrdinalIgnoreCase))
                    return "Uplay";
            }

            return null;
        }
    }
}
