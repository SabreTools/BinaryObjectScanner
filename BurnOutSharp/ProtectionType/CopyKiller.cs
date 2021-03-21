using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CopyKiller : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "Tom Commander"
            byte[] check = new byte[] { 0x54, 0x6F, 0x6D, 0x20, 0x43, 0x6F, 0x6D, 0x6D, 0x61, 0x6E, 0x64, 0x65, 0x72 };
            if (fileContent.FirstPosition(check, out int position))
                return "CopyKiller" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: The following checks are overly broad and should be refined
            // if (files.Any(f => Path.GetFileName(f).Equals("Autorun.dat", StringComparison.OrdinalIgnoreCase)))
            //     return "CopyKiller";

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            // TODO: The following checks are overly broad and should be refined
            // if (Path.GetFileName(path).Equals("Autorun.dat", StringComparison.OrdinalIgnoreCase))
            //     return "CopyKiller";
            
            return null;
        }
    }
}
