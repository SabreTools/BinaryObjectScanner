using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Utilities;
using HLLib.Directory;
using HLLib.Packages;
using static BurnOutSharp.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Various Valve archive formats
    /// </summary>
    public class Valve : IScannable
    {
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

        // TODO: Add stream opening support
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the Valve archive itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Get the package type
                byte[] magic = stream.ReadBytes(16);
                PackageType packageType = Package.GetPackageType(magic);
                if (packageType == PackageType.HL_PACKAGE_NONE)
                    return null;

                // Create a new package from the file
                var pkg = Package.CreatePackage(packageType);
                FileModeFlags mode = FileModeFlags.HL_MODE_READ | FileModeFlags.HL_MODE_WRITE | FileModeFlags.HL_MODE_NO_FILEMAPPING | FileModeFlags.HL_MODE_VOLATILE;
                bool opened = pkg.Open(file, mode, overwriteFiles: true);
                if (!opened)
                    return null;

                // Create the root directory
                var rootDirectory = pkg.GetRoot();

                // Extract all files
                rootDirectory.Extract(tempPath, readEncrypted: true, overwrite: true);

                // Close the package explicitly
                pkg.Close();

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch (Exception ex)
                {
                    if (scanner.IncludeDebug) Console.WriteLine(ex);
                }

                // Remove temporary path references
                StripFromKeys(protections, tempPath);

                return protections;
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
