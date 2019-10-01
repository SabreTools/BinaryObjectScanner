using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CactusDataShield
    {
        public static string CheckContents(string file)
        {
            if (Path.GetFileName(file) == "CDSPlayer.app")
            {
                using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
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
                if (files.Count(f => Path.GetFileName(f).Equals("CDSPlayer.app", StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    string file = files.First(f => Path.GetFileName(f).Equals("CDSPlayer.app", StringComparison.OrdinalIgnoreCase));
                    string protection = CheckContents(file);
                    if (!string.IsNullOrWhiteSpace(protection))
                        return protection;
                }
                else if (files.Count(f => Path.GetFileName(f).Equals("yucca.cds", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("wmmp.exe", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("PJSTREAM.DLL", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("CACTUSPJ.exe", StringComparison.OrdinalIgnoreCase)) > 0)
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
