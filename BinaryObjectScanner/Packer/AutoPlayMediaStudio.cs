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
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Known to detect versions 5.0.0.3 - 8.1.0.0
            var name = pex.ProductName;
            if (name.OptionalStartsWith("AutoPlay Media Studio", StringComparison.OrdinalIgnoreCase))
                return $"AutoPlay Media Studio {GetVersion(pex)}";

            // TODO: Currently too vague, may be re-enabled in the future
            // name  = Utilities.GetLegalCopyright(pex);
            // if (name.OptionalStartsWith("Runtime Engine", StringComparison.OrdinalIgnoreCase))
            //     return $"AutoPlay Media Studio {GetVersion(pex)}";

            return null;
        }

        private static string GetVersion(PortableExecutable pex)
        {
            // Check the product version explicitly
            var version = pex.ProductVersion;
            if (!string.IsNullOrEmpty(version))
                return version!;

            // Check the internal versions
            version = pex.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return version!;

            return "(Unknown Version)";
        }
    }
}
