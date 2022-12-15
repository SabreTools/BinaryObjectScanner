using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.PackerType
{
    // Created by IndigoRose (creators of Setup Factory), primarily to be used to create autorun menus for various media.
    // Official website: https://www.autoplay.org/
    // TODO: Add extraction
    public class AutoPlayMediaStudio : IPortableExecutableCheck, IScannable
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Known to detect versions 5.0.0.3 - 8.1.0.0
            string name = pex.ProductName;
            if (name?.StartsWith("AutoPlay Media Studio", StringComparison.OrdinalIgnoreCase) == true)
                return $"AutoPlay Media Studio {GetVersion(pex)}";

            // Currently too vague, may be re-enabled in the future
            /*
            name  = Utilities.GetLegalCopyright(pex);
            if (name?.StartsWith("Runtime Engine", StringComparison.OrdinalIgnoreCase) == true)
                return $"AutoPlay Media Studio {GetVersion(pex)}";
                */

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }
    
        private string GetVersion(PortableExecutable pex)
        {
            // Check the product version explicitly
            string version = pex.ProductVersion;
            if (!string.IsNullOrEmpty(version))
                return version;

            // Check the internal versions
            version = Tools.Utilities.GetInternalVersion(pex);
            if (!string.IsNullOrEmpty(version))
                return version;

            return "(Unknown Version)";
        }
    }
}
