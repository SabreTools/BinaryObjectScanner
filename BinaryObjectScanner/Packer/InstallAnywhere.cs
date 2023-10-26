using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction, which may be possible with the current libraries but needs to be investigated further.
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class InstallAnywhere : IExtractable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name= pex.FileDescription;
            if (name?.StartsWith("InstallAnywhere Self Extractor", StringComparison.OrdinalIgnoreCase) == true)
                return $"InstallAnywhere {GetVersion(pex)}";

            name = pex.ProductName;
            if (name?.StartsWith("InstallAnywhere", StringComparison.OrdinalIgnoreCase) == true)
                return $"InstallAnywhere {GetVersion(pex)}";

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
        public string? Extract(Stream? stream, string file, bool includeDebug)
#endif
        {
            return null;
        }

        private string GetVersion(PortableExecutable pex)
        {
            // Check the internal versions
            var version = pex.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return version;

            return "(Unknown Version)";
        }
    }
}