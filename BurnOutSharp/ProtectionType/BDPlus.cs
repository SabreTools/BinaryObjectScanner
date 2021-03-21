using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class BDPlus : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => f.Contains(Path.Combine("BDSVM", "00000.svm")))
                && files.Any(f => f.Contains(Path.Combine("BDSVM", "BACKUP", "00000.svm"))))
            {
                string file = files.FirstOrDefault(f => Path.GetFileName(f).Equals("00000.svm", StringComparison.OrdinalIgnoreCase));
                string version = GetVersion(file);
                return version == null ? "BD+ (Unknown Version)" : $"BD+ {version}";
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            string filename = Path.GetFileName(path);
            if (filename.Equals("00000.svm", StringComparison.OrdinalIgnoreCase))
            {
                string version = GetVersion(path);
                return version == null ? "BD+ (Unknown Version)" : $"BD+ {version}";
            }
            
            return null;
        }

        /// <remarks>Version detection logic from libbdplus was used to implement this</remarks>
        private static string GetVersion(string path)
        {
            if (!File.Exists(path))
                return null;

            try
            {
                using (var fs = File.OpenRead(path))
                {
                    fs.Seek(0x0D, SeekOrigin.Begin);
                    byte[] date = new byte[4];
                    fs.Read(date, 0, 4); //yymd
                    short year = BitConverter.ToInt16(date, 0);

                    // Do some rudimentary date checking
                    if (year < 2006 || year > 2100 || date[2] < 1 || date[2] > 12 || date[3] < 1 || date[3] > 31)
                        return null;

                    return $"{year}/{date[2]}/{date[3]}";
                }
            }
            catch
            {
                return null;
            }
        }
    }
}

