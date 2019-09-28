using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SmartE
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("BITARTS"))
                return "SmartE";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Count(f => Path.GetFileName(f).Equals("00001.TMP", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("00002.TMP", StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    return "SmartE";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("00001.TMP", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("00002.TMP", StringComparison.OrdinalIgnoreCase))
                {
                    return "SmartE";
                }
            }

            return null;
        }
    }
}
