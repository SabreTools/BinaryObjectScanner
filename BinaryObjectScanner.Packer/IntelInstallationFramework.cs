using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction, seems to primarily use MSZip compression.
    public class IntelInstallationFramework : IExtractable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
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
#if NET48
        public string Extract(string file, bool includeDebug)
#else
        public string? Extract(string file, bool includeDebug)
#endif
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(Stream stream, string file, bool includeDebug)
#else
        public string? Extract(Stream stream, string file, bool includeDebug)
#endif
        {
            return null;
        }
    }
}
