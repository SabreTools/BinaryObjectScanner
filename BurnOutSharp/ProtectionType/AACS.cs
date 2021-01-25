using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class AACS
    {
	    // TODO: Detect version
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
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
