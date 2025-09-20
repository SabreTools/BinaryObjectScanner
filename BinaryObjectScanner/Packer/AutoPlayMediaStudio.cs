using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // Created by IndigoRose (creators of Setup Factory), primarily to be used to create autorun menus for various media.
    // Official website: https://www.autoplay.org/
    // TODO: Add extraction
    public class AutoPlayMediaStudio : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Known to detect versions 5.0.0.3 - 8.1.0.0
            string? name = exe.ProductName;
            if (name.OptionalStartsWith("AutoPlay Media Studio", StringComparison.OrdinalIgnoreCase))
                return $"AutoPlay Media Studio {GetVersion(exe)}";

            // TODO: Currently too vague, may be re-enabled in the future
            // name  = Utilities.GetLegalCopyright(exe);
            // if (name.OptionalStartsWith("Runtime Engine", StringComparison.OrdinalIgnoreCase))
            //     return $"AutoPlay Media Studio {GetVersion(exe)}";

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
