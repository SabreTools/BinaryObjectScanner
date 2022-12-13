﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BurnOutSharp.FileType;
using BurnOutSharp.Tools;

namespace BurnOutSharp
{
    public class Scanner
    {
        #region Options

        /// <summary>
        /// Determines whether archives are decompressed and scanned
        /// </summary>
        public bool ScanArchives { get; private set; }

        /// <summary>
        /// Determines if packers are counted as detected protections or not
        /// </summary>
        public bool ScanPackers { get; private set; }

        /// <summary>
        /// Determines if debug information is output or not
        /// </summary>
        public bool IncludeDebug { get; private set; }

        #endregion

        /// <summary>
        /// Optional progress callback during scanning
        /// </summary>
        private readonly IProgress<ProtectionProgress> fileProgress;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scanArchives">Enable scanning archive contents</param>
        /// <param name="scanPackers">Enable including packers in output</param>
        /// <param name="includeDebug">Enable including debug information</param>
        /// <param name="fileProgress">Optional progress callback</param>
        public Scanner(bool scanArchives, bool scanPackers, bool includeDebug, IProgress<ProtectionProgress> fileProgress = null)
        {
            ScanArchives = scanArchives;
            ScanPackers = scanPackers;
            IncludeDebug = includeDebug;
            this.fileProgress = fileProgress;

            // Register the codepages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        #region Scanning

        /// <summary>
        /// Scan a single path and get all found protections
        /// </summary>
        /// <param name="path">Path to scan</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> GetProtections(string path)
        {
            return GetProtections(new List<string> { path });
        }

        /// <summary>
        /// Scan the list of paths and get all found protections
        /// </summary>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> GetProtections(List<string> paths)
        {
            // If we have no paths, we can't scan
            if (paths == null || !paths.Any())
                return null;

            // Set a starting starting time for debug output
            DateTime startTime = DateTime.UtcNow;

            // Checkpoint
            this.fileProgress?.Report(new ProtectionProgress(null, 0, null));

            // Temp variables for reporting
            string tempFilePath = Path.GetTempPath();
            string tempFilePathWithGuid = Path.Combine(tempFilePath, Guid.NewGuid().ToString());

            // Loop through each path and get the returned values
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
            foreach (string path in paths)
            {
                // Directories scan each internal file individually
                if (Directory.Exists(path))
                {
                    // Enumerate all files at first for easier access
                    var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).ToList();

                    // Scan for path-detectable protections
                    var directoryPathProtections = GetDirectoryPathProtections(path, files);
                    Utilities.AppendToDictionary(protections, directoryPathProtections);

                    // Scan each file in directory separately
                    for (int i = 0; i < files.Count; i++)
                    {
                        // Get the current file
                        string file = files.ElementAt(i);

                        // Get the reportable file name
                        string reportableFileName = file;
                        if (reportableFileName.StartsWith(tempFilePath))
                            reportableFileName = reportableFileName.Substring(tempFilePathWithGuid.Length);

                        // Checkpoint
                        this.fileProgress?.Report(new ProtectionProgress(reportableFileName, i / (float)files.Count, "Checking file" + (file != reportableFileName ? " from archive" : string.Empty)));

                        // Scan for path-detectable protections
                        var filePathProtections = GetFilePathProtections(file);
                        Utilities.AppendToDictionary(protections, filePathProtections);

                        // Scan for content-detectable protections
                        var fileProtections = GetInternalProtections(file);
                        if (fileProtections != null && fileProtections.Any())
                        {
                            foreach (string key in fileProtections.Keys)
                            {
                                if (!protections.ContainsKey(key))
                                    protections[key] = new ConcurrentQueue<string>();

                                protections[key].AddRange(fileProtections[key]);
                            }
                        }

                        // Checkpoint
                        protections.TryGetValue(file, out ConcurrentQueue<string> fullProtectionList);
                        string fullProtection = (fullProtectionList != null && fullProtectionList.Any() ? string.Join(", ", fullProtectionList) : null);
                        this.fileProgress?.Report(new ProtectionProgress(reportableFileName, (i + 1) / (float)files.Count, fullProtection ?? string.Empty));
                    }
                }

                // Scan a single file by itself
                else if (File.Exists(path))
                {
                    // Get the reportable file name
                    string reportableFileName = path;
                    if (reportableFileName.StartsWith(tempFilePath))
                        reportableFileName = reportableFileName.Substring(tempFilePathWithGuid.Length);

                    // Checkpoint
                    this.fileProgress?.Report(new ProtectionProgress(reportableFileName, 0, "Checking file" + (path != reportableFileName ? " from archive" : string.Empty)));

                    // Scan for path-detectable protections
                    var filePathProtections = GetFilePathProtections(path);
                    Utilities.AppendToDictionary(protections, filePathProtections);

                    // Scan for content-detectable protections
                    var fileProtections = GetInternalProtections(path);
                    if (fileProtections != null && fileProtections.Any())
                    {
                        foreach (string key in fileProtections.Keys)
                        {
                            if (!protections.ContainsKey(key))
                                protections[key] = new ConcurrentQueue<string>();

                            protections[key].AddRange(fileProtections[key]);
                        }
                    }

                    // Checkpoint
                    protections.TryGetValue(path, out ConcurrentQueue<string> fullProtectionList);
                    string fullProtection = (fullProtectionList != null && fullProtectionList.Any() ? string.Join(", ", fullProtectionList) : null);
                    this.fileProgress?.Report(new ProtectionProgress(reportableFileName, 1, fullProtection ?? string.Empty));
                }

                // Throw on an invalid path
                else
                {
                    Console.WriteLine($"{path} is not a directory or file, skipping...");
                    //throw new FileNotFoundException($"{path} is not a directory or file, skipping...");
                }
            }

            // Clear out any empty keys
            Utilities.ClearEmptyKeys(protections);

            // If we're in debug, output the elasped time to console
            if (IncludeDebug)
                Console.WriteLine($"Time elapsed: {DateTime.UtcNow.Subtract(startTime)}");

            return protections;
        }

        /// <summary>
        /// Get the path-detectable protections associated with a single path
        /// </summary>
        /// <param name="path">Path of the directory to scan</param>
        /// <param name="files">Files contained within</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private ConcurrentDictionary<string, ConcurrentQueue<string>> GetDirectoryPathProtections(string path, List<string> files)
        {
            // Create an empty queue for protections
            var protections = new ConcurrentQueue<string>();

            // Preprocess the list of files
            files = files.Select(f => f.Replace('\\', '/')).ToList();

            // Iterate through all path checks
            Parallel.ForEach(ScanningClasses.PathCheckClasses, pathCheckClass =>
            {
                ConcurrentQueue<string> protection = pathCheckClass.CheckDirectoryPath(path, files);
                if (protection != null)
                    protections.AddRange(protection);
            });

            // Create and return the dictionary
            return new ConcurrentDictionary<string, ConcurrentQueue<string>>
            {
                [path] = protections
            };
        }

        /// <summary>
        /// Get the path-detectable protections associated with a single path
        /// </summary>
        /// <param name="path">Path of the file to scan</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private ConcurrentDictionary<string, ConcurrentQueue<string>> GetFilePathProtections(string path)
        {
            // Create an empty queue for protections
            var protections = new ConcurrentQueue<string>();

            // Iterate through all path checks
            Parallel.ForEach(ScanningClasses.PathCheckClasses, pathCheckClass =>
            {
                string protection = pathCheckClass.CheckFilePath(path.Replace("\\", "/"));
                if (!string.IsNullOrWhiteSpace(protection))
                    protections.Enqueue(protection);
            });

            // Create and return the dictionary
            return new ConcurrentDictionary<string, ConcurrentQueue<string>>
            {
                [path] = protections
            };
        }

        /// <summary>
        /// Get the content-detectable protections associated with a single path
        /// </summary>
        /// <param name="file">Path to the file to scan</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private ConcurrentDictionary<string, ConcurrentQueue<string>> GetInternalProtections(string file)
        {
            // Quick sanity check before continuing
            if (!File.Exists(file))
                return null;

            // Open the file and begin scanning
            try
            {
                using (FileStream fs = File.OpenRead(file))
                {
                    return GetInternalProtections(file, fs);
                }
            }
            catch (Exception ex)
            {
                if (IncludeDebug) Console.WriteLine(ex);

                var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
                Utilities.AppendToDictionary(protections, file, IncludeDebug ? ex.ToString() : "[Exception opening file, please try again]");
                Utilities.ClearEmptyKeys(protections);
                return protections;
            }
        }

        /// <summary>
        /// Get the content-detectable protections associated with a single path
        /// </summary>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private ConcurrentDictionary<string, ConcurrentQueue<string>> GetInternalProtections(string fileName, Stream stream)
        {
            // Quick sanity check before continuing
            if (stream == null || !stream.CanRead || !stream.CanSeek)
                return null;

            // Initialize the protections found
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

            // Get the extension for certain checks
            string extension = Path.GetExtension(fileName).ToLower().TrimStart('.');

            // Open the file and begin scanning
            try
            {
                // Get the first 16 bytes for matching
                byte[] magic = new byte[16];
                try
                {
                    stream.Read(magic, 0, 16);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception ex)
                {
                    if (IncludeDebug) Console.WriteLine(ex);

                    return null;
                }

                // Get the file type either from magic number or extension
                SupportedFileType fileType = Utilities.GetFileType(magic);
                if (fileType == SupportedFileType.UNKNOWN)
                    fileType = Utilities.GetFileType(extension);

                // If we still got unknown, just return null
                if (fileType == SupportedFileType.UNKNOWN)
                    return null;

                // Create a scannable for the given file type
                var scannable = Utilities.CreateScannable(fileType);
                if (scannable == null)
                    return null;

                #region Non-Archive File Types

                // Executable
                if (scannable is Executable)
                {
                    var subProtections = scannable.Scan(this, stream, fileName);
                    Utilities.AppendToDictionary(protections, subProtections);
                }

                // PLJ
                if (scannable is PLJ)
                {
                    var subProtections = scannable.Scan(this, stream, fileName);
                    Utilities.AppendToDictionary(protections, subProtections);
                }

                // SFFS
                if (scannable is SFFS)
                {
                    var subProtections = scannable.Scan(this, stream, fileName);
                    Utilities.AppendToDictionary(protections, subProtections);
                }

                // Text-based files
                if (scannable is Textfile)
                {
                    var subProtections = scannable.Scan(this, stream, fileName);
                    Utilities.AppendToDictionary(protections, subProtections);
                }

                #endregion

                #region Archive File Types

                // If we're scanning archives, we have a few to try out
                if (ScanArchives)
                {
                    // 7-Zip archive
                    if (scannable is SevenZip)
                    {
                        var subProtections = scannable.Scan(this, stream, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // BFPK archive
                    if (scannable is BFPK)
                    {
                        var subProtections = scannable.Scan(this, stream, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // BZip2
                    if (scannable is BZip2)
                    {
                        var subProtections = scannable.Scan(this, stream, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // GZIP
                    if (scannable is GZIP)
                    {
                        var subProtections = scannable.Scan(this, stream, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // InstallShield Archive V3 (Z)
                    if (fileName != null && scannable is InstallShieldArchiveV3)
                    {
                        var subProtections = scannable.Scan(this, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // InstallShield Cabinet
                    if (fileName != null && scannable is InstallShieldCAB)
                    {
                        var subProtections = scannable.Scan(this, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // Microsoft Cabinet
                    if (fileName != null && scannable is MicrosoftCAB)
                    {
                        var subProtections = scannable.Scan(this, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // MSI
                    if (fileName != null && scannable is MSI)
                    {
                        var subProtections = scannable.Scan(this, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // MPQ archive
                    if (fileName != null && scannable is MPQ)
                    {
                        var subProtections = scannable.Scan(this, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // PKZIP archive (and derivatives)
                    if (scannable is PKZIP)
                    {
                        var subProtections = scannable.Scan(this, stream, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // RAR archive
                    if (scannable is RAR)
                    {
                        var subProtections = scannable.Scan(this, stream, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // Tape Archive
                    if (scannable is TapeArchive)
                    {
                        var subProtections = scannable.Scan(this, stream, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // Valve archive formats
                    if (fileName != null && scannable is Valve)
                    {
                        var subProtections = scannable.Scan(this, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // XZ
                    if (scannable is XZ)
                    {
                        var subProtections = scannable.Scan(this, stream, fileName);
                        Utilities.PrependToKeys(subProtections, fileName);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                if (IncludeDebug) Console.WriteLine(ex);

                Utilities.AppendToDictionary(protections, fileName, IncludeDebug ? ex.ToString() : "[Exception opening file, please try again]");
            }

            // Clear out any empty keys
            Utilities.ClearEmptyKeys(protections);

            return protections;
        }

        #endregion
    }
}
