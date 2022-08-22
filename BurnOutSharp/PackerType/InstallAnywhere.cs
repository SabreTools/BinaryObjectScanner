using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction, which may be possible with the current libraries but needs to be investigated further.
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class InstallAnywhere : IPortableExecutableCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = pex.FileDescription;
            if (name?.StartsWith("InstallAnywhere Self Extractor", StringComparison.OrdinalIgnoreCase) == true)
                return $"InstallAnywhere {GetVersion(pex)}";

            name = pex.ProductName;
            if (name?.StartsWith("InstallAnywhere", StringComparison.OrdinalIgnoreCase) == true)
                return $"InstallAnywhere {GetVersion(pex)}";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }

        private string GetVersion(PortableExecutable pex)
        {
            // Check the internal versions
            string version = Utilities.GetInternalVersion(pex);
            if (!string.IsNullOrEmpty(version))
                return version;

            return "(Unknown Version)";
        }
    }
}