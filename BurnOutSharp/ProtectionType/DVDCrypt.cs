using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class DVDCrypt : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("DvdCrypt.pdb", StringComparison.OrdinalIgnoreCase)))
                return "DVD Crypt";

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("DvdCrypt.pdb", StringComparison.OrdinalIgnoreCase))
                return "DVD Crypt";
            
            return null;
        }
    }
}
