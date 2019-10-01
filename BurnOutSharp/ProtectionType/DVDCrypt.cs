using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class DVDCrypt
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Count(f => Path.GetFileName(f).Equals("DvdCrypt.pdb", StringComparison.OrdinalIgnoreCase)) > 0)
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
