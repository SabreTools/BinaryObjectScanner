using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class AACS : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            if (isDirectory)
            {
                if (files.Any(f => f.Contains(Path.Combine("aacs", "VTKF000.AACS")))
                    && files.Any(f => f.Contains(Path.Combine("AACS", "CPSUnit00001.cci"))))
                {
                    return "AACS";
                }
            }
            else
            {
                string filename = Path.GetFileName(path);
                if (filename.Equals("VTKF000.AACS", StringComparison.OrdinalIgnoreCase)
                    || filename.Equals("CPSUnit00001.cci", StringComparison.OrdinalIgnoreCase))
                {
                    return "AACS";
                }
            }

            return null;
        }
    }
}
