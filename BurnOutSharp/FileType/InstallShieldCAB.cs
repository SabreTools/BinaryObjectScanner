using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;
using BurnOutSharp.Interfaces;
using UnshieldSharp.Cabinet;
using static BurnOutSharp.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// InstallShield cabinet file
    /// </summary>
    public class InstallShieldCAB : IScannable
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

                    InstallShieldCabinet cabfile = InstallShieldCabinet.Open(file);
                    for (int i = 0; i < cabfile.FileCount; i++)
                    {
                        // If an individual entry fails
                        try
                        {
                            // Check if the file is valid first
                            if (!cabfile.FileIsValid(i))
                                continue;

                            string filename = cabfile.FileName(i);
                            string tempFile;
                            try
                            {
                                tempFile = Path.Combine(tempPath, filename);
                            }
                            catch
                            {
                                tempFile = Path.Combine(tempPath, $"BAD_FILENAME{i}");
                            }

                            cabfile.FileSave(i, tempFile);
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
