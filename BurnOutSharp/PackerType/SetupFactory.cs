using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    public class SetupFactory : IPEContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckPEContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            string name = Utilities.GetLegalCopyright(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("Setup Factory", StringComparison.OrdinalIgnoreCase))
                return $"Setup Factory {GetVersion(pex)}";

            name = Utilities.GetProductName(pex);
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("Setup Factory", StringComparison.OrdinalIgnoreCase))
                return $"Setup Factory {GetVersion(pex)}";

            // Longer version of the check that can be used if false positves become an issue:
            // "Setup Factory is a trademark of Indigo Rose Corporation"

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
        // TODO: Add extraction, which is possible but the only tools available that can
        // do this seem to be Universal Extractor 2 and InstallExplorer (https://totalcmd.net/plugring/InstallExplorer.html)
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }
    
        private string GetVersion(PortableExecutable pex)
        {
            // Check the manifest version first
            string version = Utilities.GetManifestVersion(pex);
            if (!string.IsNullOrEmpty(version))
                return version;
            
            // Then check the file version
            version = Utilities.GetFileVersion(pex);
            if (!string.IsNullOrEmpty(version))
                return version;

            return "(Unknown Version)";
        }
    }
}
