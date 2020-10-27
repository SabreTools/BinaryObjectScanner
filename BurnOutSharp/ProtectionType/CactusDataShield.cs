using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BurnOutSharp.ProtectionType
{
    public class CactusDataShield
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                string appPath = files.FirstOrDefault(f => Path.GetFileName(f).Equals("CDSPlayer.app", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(appPath))
                {
                    string version = GetVersion(appPath);
                    if (!string.IsNullOrWhiteSpace(version))
                        return $"Cactus Data Shield {version}";
                }
                
                if (files.Any(f => Path.GetFileName(f).Equals("yucca.cds", StringComparison.OrdinalIgnoreCase))
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
                    string version = GetVersion(path);
                    if (!string.IsNullOrWhiteSpace(version))
                        return $"Cactus Data Shield {version}";
                }
                
                if (Path.GetFileName(path).Equals("yucca.cds", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("wmmp.exe", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("PJSTREAM.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("CACTUSPJ.exe", StringComparison.OrdinalIgnoreCase))
                {
                    return "Cactus Data Shield 200";
                }
            }

            return null;
        }

        private static string GetVersion(string path)
        {
            if (!File.Exists(path))
                return null;

            try
            {
                // TODO: Can we do anything with the contents? Take a look at CDSPlayer.app
                using (var sr = new StreamReader(path, Encoding.Default))
                {
                    return sr.ReadLine().Substring(3) + "(" + sr.ReadLine() + ")";
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
