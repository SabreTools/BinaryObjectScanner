using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, seems to primarily use MSZip compression.
    public class IntelInstallationFramework : IPortableExecutableCheck, IScannable
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
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }
    }
}
