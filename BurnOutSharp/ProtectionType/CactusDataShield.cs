using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BurnOutSharp.ProtectionType
{
    public class CactusDataShield
    {
        public static string CheckContents(string file)
        {
            if (Path.GetFileName(file) == "CDSPlayer.app")
            {
                using (var sr = new StreamReader(file, Encoding.Default))
                {
                    return "Cactus Data Shield " + sr.ReadLine().Substring(3) + "(" + sr.ReadLine() + ")";
                }
            }

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("CDSPlayer.app", StringComparison.OrdinalIgnoreCase)))
                {
                    string file = files.First(f => Path.GetFileName(f).Equals("CDSPlayer.app", StringComparison.OrdinalIgnoreCase));
                    string protection = CheckContents(file);
                    if (!string.IsNullOrWhiteSpace(protection))
                        return protection;
                }
                else if (files.Any(f => Path.GetFileName(f).Equals("yucca.cds", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("wmmp.exe", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("PJSTREAM.DLL", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("CACTUSPJ.exe", StringComparison.OrdinalIgnoreCase)))
                {
                    return "Cactus Data Shield 200";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("CDSPlayer.app", StringComparison.OrdinalIgnoreCase))
                {
                    string protection = CheckContents(path);
                    if (!string.IsNullOrWhiteSpace(protection))
                        return protection;
                }
                else if (Path.GetFileName(path).Equals("yucca.cds", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("wmmp.exe", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("PJSTREAM.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("CACTUSPJ.exe", StringComparison.OrdinalIgnoreCase))
                {
                    return "Cactus Data Shield 200";
                }
            }

            return null;
        }
    }
}
