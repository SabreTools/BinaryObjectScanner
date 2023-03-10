using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, which may be possible with the current libraries but needs to be investigated further.
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class InstallAnywhere : IExtractable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (name?.StartsWith("InstallAnywhere Self Extractor", StringComparison.OrdinalIgnoreCase) == true)
                return $"InstallAnywhere {GetVersion(pex)}";

            name = pex.ProductName;
            if (name?.StartsWith("InstallAnywhere", StringComparison.OrdinalIgnoreCase) == true)
                return $"InstallAnywhere {GetVersion(pex)}";

            return null;
        }

        /// <inheritdoc/>
        public string Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string Extract(Stream stream, string file, bool includeDebug)
        {
            return null;
        }

        private string GetVersion(PortableExecutable pex)
        {
            // Check the internal versions
            string version = pex.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return version;

            return "(Unknown Version)";
        }
    }
}