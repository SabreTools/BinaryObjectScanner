using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.FileType;

namespace BurnOutSharp.ProtectionType
{
    public class XCP
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // INI-like file that can be parsed out
                string xcpDatPath = files.FirstOrDefault(f => Path.GetFileName(f).Equals("VERSION.DAT", StringComparison.OrdinalIgnoreCase))
                    ?? files.FirstOrDefault(f => Path.GetFileName(f).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(xcpDatPath))
                    return GetXCPVersion(xcpDatPath);

                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("go.exe", StringComparison.OrdinalIgnoreCase))) // Path.Combine("contents", "go.exe")
                {
                    return "XCP";
                }
            }
            else
            {
                // INI-like file that can be parsed out
                if (Path.GetFileName(path).Equals("VERSION.DAT", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase))
                {
                    return GetXCPVersion(path);
                }

                if (Path.GetFileName(path).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("go.exe", StringComparison.OrdinalIgnoreCase))
                {
                    return "XCP";
                }
            }

            return null;
        }

        private static string GetXCPVersion(string path)
        {
            try
            {
                var xcpIni = new IniFile(path);
                return xcpIni["XCP.VERSION"];
            }
            catch
            {
               return null;
            }
        }
    }
}
