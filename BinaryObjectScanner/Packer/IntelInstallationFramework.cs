using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction, seems to primarily use MSZip compression.
    public class IntelInstallationFramework : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            var name = pex.FileDescription;
            if (name.OptionalEquals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.OptionalEquals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase))
            {
                return $"Intel Installation Framework {pex.GetInternalVersion()}";
            }

            name = pex.ProductName;
            if (name.OptionalEquals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.OptionalEquals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase))
            {
                return $"Intel Installation Framework {pex.GetInternalVersion()}";
            }

            return null;
        }
    }
}
