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
                return version == null ? "BD+ (Unknown Version)" : $"BD+ Version {version}";
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
                return version == null ? "BD+ (Unknown Version)" : $"BD+ Version {version}";
            }
            
            return null;
        }

        /// <remarks>Version detection logic from libbdplus was used to implement this</remarks>
        private static string GetVersion(string path)
        {
            try
            {
                using (var fs = File.OpenRead(path))
                {
                    fs.Seek(0x0d, SeekOrigin.Begin);
                    int year1 = fs.ReadByte();
                    int year2 = fs.ReadByte();
                    int month = fs.ReadByte();
                    int day = fs.ReadByte();

                    int year = (year1 << 8) | year2;

                    // If the result isn't a valid date, report it as an unknown version
                    if (year < 2006 || year > 2100 || month < 1 || month > 12 || day < 1 || day > 31)
                    {
                        return null;
                    }

                    return $"{year}/{month}/{day}";
                }
            }
            catch
            {
                return null;
            }
        }
    }
}

