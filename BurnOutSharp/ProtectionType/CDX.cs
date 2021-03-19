using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CDX : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("CHKCDX16.DLL", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("CHKCDX32.DLL", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("CHKCDXNT.DLL", StringComparison.OrdinalIgnoreCase)))
            {
                return "CD-X";
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("CHKCDX16.DLL", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("CHKCDX32.DLL", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("CHKCDXNT.DLL", StringComparison.OrdinalIgnoreCase))
            {
                return "CD-X";
            }
            
            return null;
        }
    }
}
