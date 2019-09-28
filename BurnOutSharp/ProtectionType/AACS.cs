using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class AACS
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Count(f => f.Contains(Path.Combine("aacs", "VTKF000.AACS"))) > 0
                    && files.Count(f => f.Contains(Path.Combine("AACS", "CPSUnit00001.cci"))) > 0)
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
