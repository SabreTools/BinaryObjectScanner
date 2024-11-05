using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction, which may be possible with the current libraries but needs to be investigated further.
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class InstallAnywhere : IExtractablePortableExecutable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
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
        public bool Extract(string file, PortableExecutable pex, string outDir, bool includeDebug)
        {
            return false;
        }

        private string GetVersion(PortableExecutable pex)
        {
            // Check the internal versions
            var version = pex.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return version!;

            return "(Unknown Version)";
        }
    }
}