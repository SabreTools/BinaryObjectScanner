using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction, which is possible but the only tools available that can
    // do this seem to be Universal Extractor 2 and InstallExplorer (https://totalcmd.net/plugring/InstallExplorer.html)
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class SetupFactory : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.LegalCopyright;

            // Known to detect versions 7.0.5.1 - 9.1.0.0
            if (name.OptionalStartsWith("Setup Engine", StringComparison.OrdinalIgnoreCase))
                return $"Setup Factory {GetVersion(exe)}";

            name = exe.ProductName;

            if (name.OptionalStartsWith("Setup Factory", StringComparison.OrdinalIgnoreCase))
                return $"Setup Factory {GetVersion(exe)}";

            name = exe.FileDescription;

            // Known to detect version 5.0.1 - 6.0.1.3
            if (name.OptionalStartsWith("Setup Factory", StringComparison.OrdinalIgnoreCase))
                return $"Setup Factory {GetVersion(exe)}";

            // Longer version of the check that can be used if false positves become an issue:
            // "Setup Factory is a trademark of Indigo Rose Corporation"

            return null;
        }

        private static string GetVersion(PortableExecutable exe)
        {
            // Check the product version explicitly
            var version = exe.ProductVersion;
            if (!string.IsNullOrEmpty(version))
                return version!;

            // Check the internal versions
            version = exe.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return version!;

            return "(Unknown Version)";
        }
    }
}
