using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BurnOutSharp.Interfaces;
using UnshieldSharp.Archive;
using static BurnOutSharp.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// InstallShield archive v3
    /// </summary>
    public class InstallShieldArchiveV3 : IScannable
    {
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

        // TODO: Add stream opening support
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // Get the name of the first cabinet file or header
            string directory = Path.GetDirectoryName(file);
            string noExtension = Path.GetFileNameWithoutExtension(file);
            string filenamePattern = Path.Combine(directory, noExtension);
            filenamePattern = new Regex(@"\d+$").Replace(filenamePattern, string.Empty);

            bool cabinetHeaderExists = File.Exists(Path.Combine(directory, filenamePattern + "1.hdr"));
            bool shouldScanCabinet = cabinetHeaderExists
                ? file.Equals(Path.Combine(directory, filenamePattern + "1.hdr"), StringComparison.OrdinalIgnoreCase)
                : file.Equals(Path.Combine(directory, filenamePattern + "1.cab"), StringComparison.OrdinalIgnoreCase);

            // If we have the first file
            if (shouldScanCabinet)
            {
                // If the cab file itself fails
                try
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    Directory.CreateDirectory(tempPath);

                    UnshieldSharp.Archive.InstallShieldArchiveV3 archive = new UnshieldSharp.Archive.InstallShieldArchiveV3(file);
                    foreach (CompressedFile cfile in archive.Files.Select(kvp => kvp.Value))
                    {
                        // If an individual entry fails
                        try
                        {
                            string tempFile = Path.Combine(tempPath, cfile.FullPath);
                            if (!Directory.Exists(Path.GetDirectoryName(tempFile)))
                                Directory.CreateDirectory(Path.GetDirectoryName(tempFile));

                            (byte[] fileContents, string error) = archive.Extract(cfile.FullPath);
                            if (!string.IsNullOrWhiteSpace(error))
                                continue;

                            using (FileStream fs = File.OpenWrite(tempFile))
                            {
                                fs.Write(fileContents, 0, fileContents.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (scanner.IncludeDebug) Console.WriteLine(ex);
                        }
                    }

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
            }

            return null;
        }
    }
}
