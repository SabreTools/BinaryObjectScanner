using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction, seems to primarily use MSZip compression.
    public class IntelInstallationFramework : IExtractableExecutable<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name= pex.FileDescription;
            if (name?.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase) == true
                || name?.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase) == true)
            {
                return $"Intel Installation Framework {pex.GetInternalVersion()}";
            }

            name = pex.ProductName;
            if (name?.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase) == true
                || name?.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase) == true)
            {
                return $"Intel Installation Framework {pex.GetInternalVersion()}";
            }

            return null;
        }

        /// <inheritdoc/>
        public bool Extract(string file, PortableExecutable pex, string outDir, bool includeDebug)
        {
            return false;
        }
    }
}
