using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // Created by IndigoRose (creators of Setup Factory), primarily to be used to create autorun menus for various media.
    // Official website: https://www.autoplay.org/
    // TODO: Add extraction
    public class AutoPlayMediaStudio : IExtractable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.Model.SectionTable;
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
#if NET48
        public string Extract(string file, bool includeDebug)
#else
        public string? Extract(string file, bool includeDebug)
#endif
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(Stream stream, string file, bool includeDebug)
#else
        public string? Extract(Stream stream, string file, bool includeDebug)
#endif
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
            version = pex.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return version;

            return "(Unknown Version)";
        }
    }
}
