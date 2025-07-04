using System;
using System.IO;
using System.Text.RegularExpressions;
using BinaryObjectScanner.Interfaces;
using UnshieldSharp;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// InstallShield cabinet file
    /// </summary>
    public class InstallShieldCAB : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string file, string outDir, bool includeDebug)
        {
            if (!File.Exists(file))
                return false;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, outDir, includeDebug);
        }

        /// <inheritdoc/>
        public bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Handles getting full path if relative paths were passed.
            file = Path.GetFullPath(file);
            // Get the name of the first cabinet file or header
            var directory = Path.GetDirectoryName(file);
            string noExtension = Path.GetFileNameWithoutExtension(file);

            bool shouldScanCabinet;
            if (directory == null)
            {
                string filenamePattern = noExtension;
                filenamePattern = new Regex(@"\d+$").Replace(filenamePattern, string.Empty);
                bool cabinetHeaderExists = File.Exists(filenamePattern + "1.hdr");
                shouldScanCabinet = cabinetHeaderExists
                    ? file.Equals(filenamePattern + "1.hdr", StringComparison.OrdinalIgnoreCase)
                    : file.Equals(filenamePattern + "1.cab", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                string filenamePattern = Path.Combine(directory, noExtension);
                filenamePattern = new Regex(@"\d+$").Replace(filenamePattern, string.Empty);
                bool cabinetHeaderExists = File.Exists(Path.Combine(directory, filenamePattern + "1.hdr"));
                shouldScanCabinet = cabinetHeaderExists
                    ? file.Equals(Path.Combine(directory, filenamePattern + "1.hdr"), StringComparison.OrdinalIgnoreCase)
                    : file.Equals(Path.Combine(directory, filenamePattern + "1.cab"), StringComparison.OrdinalIgnoreCase);
            }

            // If we have anything but the first file
            if (!shouldScanCabinet)
                return false;

            try
            {
                if (!File.Exists(file))
                    return false;

                var cabfile = InstallShieldCabinet.Open(file);
                if (cabfile?.HeaderList == null)
                    return false;

                for (int i = 0; i < cabfile.HeaderList.FileCount; i++)
                {
                    try
                    {
                        // Check if the file is valid first
                        if (!cabfile.HeaderList.FileIsValid(i))
                            continue;

                        string tempFile;
                        try
                        {
                            string? filename = cabfile.HeaderList.GetFileName(i);
                            tempFile = Path.Combine(outDir, filename ?? string.Empty);
                        }
                        catch
                        {
                            tempFile = Path.Combine(outDir, $"BAD_FILENAME{i}");
                        }

                        var directoryName = Path.GetDirectoryName(tempFile);
                        if (directoryName != null && !Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);

                        cabfile.FileSave(i, tempFile);
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.WriteLine(ex);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
        }
    }
}
