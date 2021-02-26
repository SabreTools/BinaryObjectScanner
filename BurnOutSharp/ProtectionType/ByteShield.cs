using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class ByteShield : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("Byteshield.dll", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetExtension(f).Trim('.').Equals("bbz", StringComparison.OrdinalIgnoreCase)))
                {
                    return "ByteShield";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("Byteshield.dll", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("bbz", StringComparison.OrdinalIgnoreCase))
                {
                    return "ByteShield";
                }
            }

            return null;
        }
    }
}
