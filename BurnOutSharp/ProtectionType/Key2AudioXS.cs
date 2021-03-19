using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Key2AudioXS : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("SDKHM.EXE", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("SDKHM.DLL", StringComparison.OrdinalIgnoreCase)))
            {
                return "key2AudioXS";
            }
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("SDKHM.EXE", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("SDKHM.DLL", StringComparison.OrdinalIgnoreCase))
            {
                return "key2AudioXS";
            }

            return null;
        }
    }
}
