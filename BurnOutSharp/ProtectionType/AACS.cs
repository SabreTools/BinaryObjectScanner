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
                if (files.Any(f => f.Contains(Path.Combine("AACS", "MKBROM.AACS"))))
                {
                    string versionPathHDDVD = files.FirstOrDefault(f => Path.GetFileName(f).Equals("MKBROM.AACS", StringComparison.OrdinalIgnoreCase));
                    int? version = GetVersion(versionPathHDDVD);
                    if (version == null)
                        return "AACS (Unknown Version)";
                    return $"AACS Version {version}";
                }
                if (files.Any(f => f.Contains(Path.Combine("AACS", "MKB_RO.inf"))))
                {
                    string versionPathBD = files.FirstOrDefault(f => Path.GetFileName(f).Equals("MKB_RO.inf", StringComparison.OrdinalIgnoreCase));
                    int? version = GetVersion(versionPathBD);
                    if (version == null)
                        return "AACS (Unknown Version)";
                    return $"AACS Version {version}";
                }
            }
            else
            {
                string filename = Path.GetFileName(path);
                if (filename.Equals("MKBROM.AACS", StringComparison.OrdinalIgnoreCase))
                {
                    string versionPathHDDVD = path;
                    int? version = GetVersion(versionPathHDDVD);
                    if (version == null)
                        return "AACS (Unknown Version)";
                    return $"AACS Version {version}";
                }
                if (filename.Equals("MKB_RO.inf", StringComparison.OrdinalIgnoreCase))
                {
                    string versionPathBD = path;
                    int? version = GetVersion(versionPathBD);
                    if (version == null)
                        return "AACS (Unknown Version)";
                    return $"AACS Version {version}";
                } 
            }

            return null;
        }
        private static int? GetVersion(string path)
        {
            try
            {
                using (var fs = File.OpenRead(path))
                {
                    fs.Seek(0xB, SeekOrigin.Begin);
                    int version = fs.ReadByte();
                    return version;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
