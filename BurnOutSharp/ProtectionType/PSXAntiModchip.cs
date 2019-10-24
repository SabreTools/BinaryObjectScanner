using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BurnOutSharp.ProtectionType
{
    public class PSXAntiModchip
    {
        public static string CheckContents(string file, string fileContent)
        {
            // TODO: Detect Red Hand protection
            if (fileContent.Contains("     SOFTWARE TERMINATED\nCONSOLE MAY HAVE BEEN MODIFIED\n     CALL 1-888-780-7690"))
                return "PlayStation Anti-modchip (English)";

            if (fileContent.Contains("強制終了しました。\n本体が改造されている\nおそれがあります。"))
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
                        // Load the current file content
                        string fileContent = null;
                        using (StreamReader sr = new StreamReader(file, Encoding.Default))
                        {
                            fileContent = sr.ReadToEnd();
                        }

                        string protection = CheckContents(path, fileContent);
                        if (!string.IsNullOrWhiteSpace(protection))
                            return protection;
                    }
                }
            }
            else
            {
                // Load the current file content
                string fileContent = null;
                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    fileContent = sr.ReadToEnd();
                }

                string protection = CheckContents(path, fileContent);
                if (!string.IsNullOrWhiteSpace(protection))
                    return protection;
            }            

            return null;
        }
    }
}
