using System;
using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class Zzxzz
    {
        public static string CheckPath(string path, bool isDirectory)
        {
            if (isDirectory)
            {
                if (File.Exists(Path.Combine(path, "Zzxzz", "Zzz.aze")))
                    return "Zzxzz";

                else if (Directory.Exists(Path.Combine(path, "Zzxzz")))
                    return "Zzxzz";
            }
            else
            {
                string filename = Path.GetFileName(path);
                if (filename.Equals("Zzz.aze", StringComparison.OrdinalIgnoreCase))
                    return "Zzxzz";
            }

            return null;
        }
    }
}
