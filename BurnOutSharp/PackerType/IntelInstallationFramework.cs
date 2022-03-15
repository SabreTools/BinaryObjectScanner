using System;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, seems to primarily use MSZip compression.
    public class IntelInstallationFramework : IPEContentCheck
    {
        /// <inheritdoc/>
        public string CheckPEContents(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = Utilities.GetFileDescription(pex);
            if (!string.IsNullOrWhiteSpace(name)
                && (name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase)))
            {
                return $"Intel Installation Framework {Utilities.GetFileVersion(pex)}";
            }

            name = Utilities.GetProductName(pex);
            if (!string.IsNullOrWhiteSpace(name)
                && (name.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase)
                || name.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase)))
            {
                return $"Intel Installation Framework {Utilities.GetFileVersion(pex)}";
            }

            return null;
        }
    }
}
