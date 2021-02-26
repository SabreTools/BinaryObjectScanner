using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Key2AudioXS : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("SDKHM.EXE", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("SDKHM.DLL", StringComparison.OrdinalIgnoreCase)))
                {
                    return "key2AudioXS";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("SDKHM.EXE", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("SDKHM.DLL", StringComparison.OrdinalIgnoreCase))
                {
                    return "key2AudioXS";
                }
            }

            return null;
        }
    }
}
