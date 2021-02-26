using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class DVDCrypt : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("DvdCrypt.pdb", StringComparison.OrdinalIgnoreCase)))
                    return "DVD Crypt";
            }
            else
            {
                if (Path.GetFileName(path).Equals("DvdCrypt.pdb", StringComparison.OrdinalIgnoreCase))
                    return "DVD Crypt";
            }

            return null;
        }
    }
}
