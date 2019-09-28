using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class WTMCDProtect
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains("WTM76545"))
                return "WTM CD Protect";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Count(f => Path.GetExtension(f).Trim('.').Equals("IMP", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("imp.dat", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("wtmfiles.dat", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("Viewer.exe", StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    return "WTM CD Protect";
                }
            }
            else
            {
                if (Path.GetExtension(path).Trim('.').Equals("IMP", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("imp.dat", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("wtmfiles.dat", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("Viewer.exe", StringComparison.OrdinalIgnoreCase))
                {
                    return "WTM CD Protect";
                }
            }

            return null;
        }
    }
}
