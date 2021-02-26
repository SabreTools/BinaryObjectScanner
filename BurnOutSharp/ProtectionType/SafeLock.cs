using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SafeLock : IPathCheck
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // "SafeLock"
            byte[] check = new byte[] { 0x53, 0x61, 0x66, 0x65, 0x4C, 0x6F, 0x63, 0x6B };
            if (fileContent.Contains(check, out int position))
                return "SafeLock" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        /// <inheritdoc/>
        public string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
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
