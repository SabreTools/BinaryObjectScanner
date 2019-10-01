using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SoftLock
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Count(f => Path.GetFileName(f).Equals("SOFTLOCKI.dat", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("SOFTLOCKC.dat", StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    return "SoftLock";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("SOFTLOCKI.dat", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("SOFTLOCKC.dat", StringComparison.OrdinalIgnoreCase))
                {
                    return "SoftLock";
                }
            }

            return null;
        }
    }
}
