using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
#if NETSTANDARD2_0
using WixToolset.Dtf.Compression;
using WixToolset.Dtf.Compression.Cab;
#elif NET6_0_OR_GREATER
using LibMSPackSharp;
using LibMSPackSharp.CABExtract;
#endif
using static BurnOutSharp.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Microsoft cabinet file
    /// </summary>
    /// <remarks>Specification available at <see href="http://download.microsoft.com/download/5/0/1/501ED102-E53F-4CE0-AA6B-B0F93629DDC6/Exchange/%5BMS-CAB%5D.pdf"/></remarks>
    public partial class MicrosoftCAB : IScannable
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

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
#if NET6_0_OR_GREATER
            // If the cab file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Create the decompressor
                var decompressor = Library.CreateCABDecompressor(null);
                decompressor.Debug = scanner.IncludeDebug;

                // Open the cab file
                var cabFile = decompressor.Open(file);
                if (cabFile == null)
                {
                    if (scanner.IncludeDebug) Console.WriteLine($"Error occurred opening of '{file}': {decompressor.Error}");
                    return null;
                }

                // If we have a previous CAB and it exists, don't try scanning
                string directory = Path.GetDirectoryName(file);
                if (!string.IsNullOrWhiteSpace(cabFile.PreviousCabinetName))
                {
                    if (File.Exists(Path.Combine(directory, cabFile.PreviousCabinetName)))
                        return null;
                }

                // If there are additional next CABs, add those
                string fileName = Path.GetFileName(file);
                CABExtract.LoadSpanningCabinets(cabFile, fileName);

                // Loop through the found internal files
                var sub = cabFile.Files;
                while (sub != null)
                {
                    // If an individual entry fails
                    try
                    {
                        // The trim here is for some very odd and stubborn files
                        string tempFile = Path.Combine(tempPath, sub.Filename.TrimEnd('\0', ' ', '.'));
                        Error error = decompressor.Extract(sub, tempFile);
                        if (error != Error.MSPACK_ERR_OK)
                        {
                            if (scanner.IncludeDebug) Console.WriteLine($"Error occurred during extraction of '{sub.Filename}': {error}");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (scanner.IncludeDebug) Console.WriteLine(ex);
                    }

                    sub = sub.Next;
                }

                // Destroy the decompressor
                Library.DestroyCABDecompressor(decompressor);

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
#else
            // If the cab file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                CabInfo cabInfo = new CabInfo(file);
                cabInfo.Unpack(tempPath);

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
#endif
        }
    }
}
