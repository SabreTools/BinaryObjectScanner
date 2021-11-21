using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    public class InstallAnywhere : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = Utilities.GetFileDescription(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("InstallAnywhere Self Extractor", StringComparison.OrdinalIgnoreCase))
                return $"InstallAnywhere {GetVersion(pex)}";

            name = Utilities.GetProductName(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("InstallAnywhere", StringComparison.OrdinalIgnoreCase))
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
        // TODO: Add extraction, which may be possible with the current libraries but needs to be investigated further.
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }

        private string GetVersion(PortableExecutable pex)
        {
            // Check the file version first
            string version = Utilities.GetFileVersion(pex);
            if (!string.IsNullOrEmpty(version))
                return version;

            // Then check the manifest version
            version = Utilities.GetManifestVersion(pex);
            if (!string.IsNullOrEmpty(version))
                return version;

            return "(Unknown Version)";
        }
    }
}