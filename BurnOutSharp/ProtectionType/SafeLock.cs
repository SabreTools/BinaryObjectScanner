using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SafeLock
    {
        public static string CheckContents(string fileContent)
        {
            string check = "SafeLock";
            if (fileContent.Contains(check))
                return $"SafeLock (Index {fileContent.IndexOf(check)})";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("SafeLock.dat", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("SafeLock.001", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("SafeLock.128", StringComparison.OrdinalIgnoreCase)))
                {
                    return "SafeLock";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("SafeLock.dat", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("SafeLock.001", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("SafeLock.128", StringComparison.OrdinalIgnoreCase))
                {
                    return "SafeLock";
                }
            }

            return null;
        }
    }
}
