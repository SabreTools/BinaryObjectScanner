using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class IndyVCD : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("INDYVCD.AX", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("INDYMP3.idt", StringComparison.OrdinalIgnoreCase)))
                {
                    return "IndyVCD";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("INDYVCD.AX", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("INDYMP3.idt", StringComparison.OrdinalIgnoreCase))
                {
                    return "IndyVCD";
                }
            }

            return null;
        }
    }
}
