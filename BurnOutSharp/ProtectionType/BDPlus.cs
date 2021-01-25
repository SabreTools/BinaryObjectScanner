using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class BDPlus
    {
         // TODO: Detect version
         public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (!isDirectory)
                return null;

            if (Directory.Exists(Path.Combine(path, "BDSVM")))
            {
                return "BD+";
            }

            return null;
        }
    }
}