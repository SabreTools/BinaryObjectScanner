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
                    int? version = GetVersionHDDVD(versionPathHDDVD);
                    return $"AACS Version {version}";
                }
                if (files.Any(f => f.Contains(Path.Combine("AACS", "MKB_RO.inf"))))
                {
                    string versionPathBD = files.FirstOrDefault(f => Path.GetFileName(f).Equals("MKB_RO.inf", StringComparison.OrdinalIgnoreCase));
                    int? version = GetVersionBD(versionPathBD);
                    return $"AACS Version {version}";
                }
            }

            return null;
        }
        private static int? GetVersionHDDVD(string path)
        {
            try
            {
                FileStream file = File.OpenRead(path);
                file.Seek(0xB, SeekOrigin.Begin);
                int? version = file.ReadByte();
                return version;
            }
            catch
            {
                return null;
            }
        }
        private static int? GetVersionBD(string path)
        {
            try
            {
                FileStream file = File.OpenRead(path);
                file.Seek(0xB, SeekOrigin.Begin);
                int? version = file.ReadByte();
                return version;
            }
            catch
            {
                return null;
            }
        }
    }
}
