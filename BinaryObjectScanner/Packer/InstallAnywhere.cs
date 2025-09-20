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
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.FileDescription;

            if (name.OptionalStartsWith("InstallAnywhere Self Extractor", StringComparison.OrdinalIgnoreCase))
                return $"InstallAnywhere {GetVersion(exe)}";

            name = exe.ProductName;

            if (name.OptionalStartsWith("InstallAnywhere", StringComparison.OrdinalIgnoreCase))
                return $"InstallAnywhere {GetVersion(exe)}";

            return null;
        }

        private static string GetVersion(PortableExecutable exe)
        {
            // Check the internal versions
            var version = exe.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return version!;

            return "(Unknown Version)";
        }
    }
}