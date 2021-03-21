using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SafeLock : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "SafeLock"
            byte[] check = new byte[] { 0x53, 0x61, 0x66, 0x65, 0x4C, 0x6F, 0x63, 0x6B };
            if (fileContent.FirstPosition(check, out int position))
                return "SafeLock" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("SafeLock.dat", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("SafeLock.001", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("SafeLock.128", StringComparison.OrdinalIgnoreCase)))
            {
                return "SafeLock";
            }
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("SafeLock.dat", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("SafeLock.001", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("SafeLock.128", StringComparison.OrdinalIgnoreCase))
            {
                return "SafeLock";
            }

            return null;
        }
    }
}
