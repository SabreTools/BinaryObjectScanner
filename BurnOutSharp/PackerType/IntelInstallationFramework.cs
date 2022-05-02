using System;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, seems to primarily use MSZip compression.
    public class IntelInstallationFramework : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (!string.IsNullOrWhiteSpace(name)
                && (name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase)))
            {
                return $"Intel Installation Framework {Utilities.GetInternalVersion(pex)}";
            }

            name = pex.ProductName;
            if (!string.IsNullOrWhiteSpace(name)
                && (name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase)))
            {
                return $"Intel Installation Framework {Utilities.GetInternalVersion(pex)}";
            }

            return null;
        }
    }
}
