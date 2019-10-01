using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class XCP
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Count(f => Path.GetFileName(f).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("go.exe", StringComparison.OrdinalIgnoreCase)) > 0) // Path.Combine("contents", "go.exe")
                {
                    return "XCP";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("go.exe", StringComparison.OrdinalIgnoreCase))
                {
                    return "XCP";
                }
            }

            return null;
        }
    }
}
