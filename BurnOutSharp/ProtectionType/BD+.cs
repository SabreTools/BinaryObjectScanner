using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class BDPlus : IPathCheck
    {
        // TODO: Figure out how to detect version
        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            // Multiple .svm files should always be present, but as far as I can tell, 00000.svm is the first one made and should be present no matter what.
            if (isDirectory)
            {
                if (files.Any(f => f.Contains(Path.Combine("BDSVM", "00000.svm")))
                    && files.Any(f => f.Contains(Path.Combine("BDSVM", "BACKUP", "00000.svm"))))
                {
                    return "BD+";
                }
            }
            return null;
        }
    }
}

