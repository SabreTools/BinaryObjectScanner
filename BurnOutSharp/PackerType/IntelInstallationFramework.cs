using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, seems to primarily use MSZip compression.
    public class IntelInstallationFramework : IExtractable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (name?.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase) == true
                || name?.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase) == true)
            {
                return $"Intel Installation Framework {Tools.Utilities.GetInternalVersion(pex)}";
            }

            name = pex.ProductName;
            if (name?.Equals("Intel(R) Installation Framework", StringComparison.OrdinalIgnoreCase) == true
                || name?.Equals("Intel Installation Framework", StringComparison.OrdinalIgnoreCase) == true)
            {
                return $"Intel Installation Framework {Tools.Utilities.GetInternalVersion(pex)}";
            }

            return null;
        }

        /// <inheritdoc/>
        public string Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string Extract(Stream stream, string file, bool includeDebug)
        {
            return null;
        }
    }
}
