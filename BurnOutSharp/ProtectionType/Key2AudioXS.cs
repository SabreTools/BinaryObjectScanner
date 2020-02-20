using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Key2AudioXS
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("SDKHM.EXE", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("SDKHM.DLL", StringComparison.OrdinalIgnoreCase)))
                {
                    return "Key2Audio XS";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("SDKHM.EXE", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("SDKHM.DLL", StringComparison.OrdinalIgnoreCase))
                {
                    return "Key2Audio XS";
                }
            }

            return null;
        }
    }
}
