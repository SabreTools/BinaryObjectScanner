using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class PSXAntiModchip
    {
        public static string CheckContents(string file, string fileContent)
        {
            if (fileContent.Contains("     SOFTWARE TERMINATED\nCONSOLE MAY HAVE BEEN MODIFIED\n     CALL 1-888-780-7690"))
                return "PlayStation Anti-modchip (English)";
            else if (fileContent.Contains("強制終了しました。\n本体が改造されている\nおそれがあります。"))
                return "PlayStation Anti-modchip (Japanese)";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Where(s => s.ToLowerInvariant().EndsWith(".cnf")).Count() > 0)
                {
                    foreach (string file in files)
                    {
                        using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var sr = new StreamReader(fs))
                        {
                            string fileContent = sr.ReadToEnd();
                            string protection = CheckContents(path, fileContent);
                            if (!string.IsNullOrWhiteSpace(protection))
                                return protection;
                        }
                    }
                }
            }
            else
            {
                using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    string fileContent = sr.ReadToEnd();
                    string protection = CheckContents(path, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        return protection;
                }
            }            

            return null;
        }
    }
}
