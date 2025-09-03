using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction, which may be possible with the current libraries but needs to be investigated further.
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class InstallAnywhere : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            var name = pex.FileDescription;
            if (name.OptionalStartsWith("InstallAnywhere Self Extractor", StringComparison.OrdinalIgnoreCase))
                return $"InstallAnywhere {GetVersion(pex)}";

            name = pex.ProductName;
            if (name.OptionalStartsWith("InstallAnywhere", StringComparison.OrdinalIgnoreCase))
                return $"InstallAnywhere {GetVersion(pex)}";

            return null;
        }

        private static string GetVersion(PortableExecutable pex)
        {
            // Check the internal versions
            var version = pex.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return version!;

            return "(Unknown Version)";
        }
    }
}