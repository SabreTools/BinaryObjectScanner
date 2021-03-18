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
                    string file = files.FirstOrDefault(f => Path.GetFileName(f).Equals("MKBROM.AACS", StringComparison.OrdinalIgnoreCase));
                    int? version = GetVersion(file);
                    return (version == null ? "AACS (Unknown Version)" : $"AACS Version {version}");
                }
                if (files.Any(f => f.Contains(Path.Combine("AACS", "MKB_RO.inf"))))
                {
                    string file = files.FirstOrDefault(f => Path.GetFileName(f).Equals("MKB_RO.inf", StringComparison.OrdinalIgnoreCase));
                    int? version = GetVersion(file);
                    return (version == null ? "AACS (Unknown Version)" : $"AACS Version {version}");
                }
            }
            else
            {
                string filename = Path.GetFileName(path);
                if (filename.Equals("MKBROM.AACS", StringComparison.OrdinalIgnoreCase))
                {
                    int? version = GetVersion(path);
                    return (version == null ? "AACS (Unknown Version)" : $"AACS Version {version}");
                }
                if (filename.Equals("MKB_RO.inf", StringComparison.OrdinalIgnoreCase))
                {
                    int? version = GetVersion(path);
                    return (version == null ? "AACS (Unknown Version)" : $"AACS Version {version}");
                } 
            }

            return null;
        }
        private static int? GetVersion(string path)
        {
            if (!File.Exists(path))
                return null;
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
