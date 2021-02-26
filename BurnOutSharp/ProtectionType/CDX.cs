using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CDX : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("CHKCDX16.DLL", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("CHKCDX32.DLL", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("CHKCDXNT.DLL", StringComparison.OrdinalIgnoreCase)))
                {
                    return "CD-X";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("CHKCDX16.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("CHKCDX32.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("CHKCDXNT.DLL", StringComparison.OrdinalIgnoreCase))
                {
                    return "CD-X";
                }
            }

            return null;
        }
    }
}
