using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Data;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner
{
    public class Scanner
    {
        #region Instance Variables

        /// <summary>
        /// Determines whether archives are decompressed and scanned
        /// </summary>
        private readonly bool _scanArchives;

        /// <summary>
        /// Determines if content matches are used
        /// </summary>
        private readonly bool _scanContents;

        /// <summary>
        /// Determines if path matches are used
        /// </summary>
        private readonly bool _scanPaths;

        /// <summary>
        /// Determines if subdirectories are scanned
        /// </summary>
        private readonly bool _scanSubdirectories;

        /// <summary>
        /// Determines if debug information is output
        /// </summary>
        private readonly bool _includeDebug;

        /// <summary>
        /// Optional progress callback during scanning
        /// </summary>
        private readonly IProgress<ProtectionProgress>? _fileProgress;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scanArchives">Enable scanning archive contents</param>
        /// <param name="scanContents">Enable including content detections in output</param>
        /// <param name="scanPaths">Enable including path detections in output</param>
        /// <param name="scanSubdirectories">Enable scanning subdirectories</param>
        /// <param name="includeDebug">Enable including debug information</param>
        /// <param name="fileProgress">Optional progress callback</param>
        public Scanner(bool scanArchives,
            bool scanContents,
            bool scanPaths,
            bool scanSubdirectories,
            bool includeDebug,
            IProgress<ProtectionProgress>? fileProgress = null)
        {
            _scanArchives = scanArchives;
            _scanContents = scanContents;
            _scanPaths = scanPaths;
            _scanSubdirectories = scanSubdirectories;
            _includeDebug = includeDebug;
            _fileProgress = fileProgress;

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            // Register the codepages
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif
        }

        #region Scanning

        /// <summary>
        /// Scan a single path and get all found protections
        /// </summary>
        /// <param name="path">Path to scan</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        public Dictionary<string, List<string>> GetProtections(string path)
            => GetProtectionsImpl(path, depth: 0).ToDictionary();

        /// <summary>
        /// Scan the list of paths and get all found protections
        /// </summary>
        /// <param name="paths">Paths to scan</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        public Dictionary<string, List<string>> GetProtections(List<string>? paths)
            => GetProtectionsImpl(paths, depth: 0).ToDictionary();

        /// <summary>
        /// Scan a single path and get all found protections
        /// </summary>
        /// <param name="path">Path to scan</param>
        /// <param name="depth">Depth of the current scanner pertaining to extracted data</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private ProtectionDictionary GetProtectionsImpl(string path, int depth)
            => GetProtectionsImpl([path], depth);

        /// <summary>
        /// Scan a single path and get all found protections
        /// </summary>
        /// <param name="paths">Paths to scan</param>
        /// <param name="depth">Depth of the current scanner pertaining to extracted data</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private ProtectionDictionary GetProtectionsImpl(List<string>? paths, int depth)
        {
            // If we have no paths, we can't scan
            if (paths == null || paths.Count == 0)
                return [];

            // Set a starting starting time for debug output
            DateTime startTime = DateTime.UtcNow;

            // Checkpoint
            _fileProgress?.Report(new ProtectionProgress(null, 0, null));

            // Temp variables for reporting
            string tempFilePath = Path.GetTempPath();
            string tempFilePathWithGuid = Path.Combine(tempFilePath, Guid.NewGuid().ToString());

            // Loop through each path and get the returned values
            var protections = new ProtectionDictionary();
            foreach (string path in paths)
            {
                // Directories scan each internal file individually
                if (Directory.Exists(path))
                {
                    // Enumerate all files at first for easier access
                    SearchOption searchOption = _scanSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    List<string> files = [.. IOExtensions.SafeGetFiles(path, "*", searchOption)];

                    // Scan for path-detectable protections
                    if (_scanPaths)
                    {
                        var directoryPathProtections = HandlePathChecks(path, files);
                        protections.Append(directoryPathProtections);
                    }

                    // Scan each file in directory separately
                    for (int i = 0; i < files.Count; i++)
                    {
                        // Get the current file
                        string file = files[i];

                        // Get the reportable file name
                        string reportableFileName = file;
                        if (reportableFileName.StartsWith(tempFilePath))
                            reportableFileName = reportableFileName.Substring(tempFilePathWithGuid.Length);

                        // Checkpoint
                        _fileProgress?.Report(new ProtectionProgress(reportableFileName, depth, i / (float)files.Count, "Checking file" + (file != reportableFileName ? " from archive" : string.Empty)));

                        // Scan for path-detectable protections
                        if (_scanPaths)
                        {
                            var filePathProtections = HandlePathChecks(file, files: null);
                            if (filePathProtections != null && filePathProtections.Count > 0)
                                protections.Append(filePathProtections);
                        }

                        // Scan for content-detectable protections
                        var fileProtections = GetInternalProtections(file, depth);
                        if (fileProtections != null && fileProtections.Count > 0)
                            protections.Append(fileProtections);

                        // Checkpoint
                        protections.TryGetValue(file, out var fullProtectionList);
                        var fullProtection = fullProtectionList != null && fullProtectionList.Count > 0
                            ? string.Join(", ", [.. fullProtectionList])
                            : null;
                        _fileProgress?.Report(new ProtectionProgress(reportableFileName, depth, (i + 1) / (float)files.Count, fullProtection ?? string.Empty));
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
                    _fileProgress?.Report(new ProtectionProgress(reportableFileName, depth, 0, "Checking file" + (path != reportableFileName ? " from archive" : string.Empty)));

                    // Scan for path-detectable protections
                    if (_scanPaths)
                    {
                        var filePathProtections = HandlePathChecks(path, files: null);
                        if (filePathProtections != null && filePathProtections.Count > 0)
                            protections.Append(filePathProtections);
                    }

                    // Scan for content-detectable protections
                    var fileProtections = GetInternalProtections(path, depth);
                    if (fileProtections != null && fileProtections.Count > 0)
                        protections.Append(fileProtections);

                    // Checkpoint
                    protections.TryGetValue(path, out var fullProtectionList);
                    var fullProtection = fullProtectionList != null && fullProtectionList.Count > 0
                        ? string.Join(", ", [.. fullProtectionList])
                        : null;
                    _fileProgress?.Report(new ProtectionProgress(reportableFileName, depth, 1, fullProtection ?? string.Empty));
                }

                // Throw on an invalid path
                else
                {
                    Console.WriteLine($"{path} is not a directory or file, skipping...");
                    //throw new FileNotFoundException($"{path} is not a directory or file, skipping...");
                }
            }

            // Clear out any empty keys
            protections.ClearEmptyKeys();

            // If we're in debug, output the elasped time to console
            if (_includeDebug)
                Console.WriteLine($"Time elapsed: {DateTime.UtcNow.Subtract(startTime)}");

            return protections;
        }

        /// <summary>
        /// Get the content-detectable protections associated with a single path
        /// </summary>
        /// <param name="file">Path to the file to scan</param>
        /// <param name="depth">Depth of the current scanner pertaining to extracted data</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private ProtectionDictionary GetInternalProtections(string file, int depth)
        {
            // Quick sanity check before continuing
            if (!File.Exists(file))
                return [];

            // Open the file and begin scanning
            try
            {
                using FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return GetInternalProtections(fs.Name, fs, depth);
            }
            catch (Exception ex)
            {
                if (_includeDebug) Console.WriteLine(ex);

                var protections = new ProtectionDictionary();
                protections.Append(file, _includeDebug ? ex.ToString() : "[Exception opening file, please try again]");
                protections.ClearEmptyKeys();
                return protections;
            }
        }

        /// <summary>
        /// Get the content-detectable protections associated with a single path
        /// </summary>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <param name="depth">Depth of the current scanner pertaining to extracted data</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private ProtectionDictionary GetInternalProtections(string fileName, Stream stream, int depth)
        {
            // Quick sanity check before continuing
            if (!stream.CanRead)
            {
                if (_includeDebug) Console.WriteLine($"{fileName} does not have a readable stream, skipping...");
                return [];
            }

            // Initialize the protections found
            var protections = new ProtectionDictionary();

            // Get the extension for certain checks
            string extension = Path.GetExtension(fileName).ToLower().TrimStart('.');

            // Open the file and begin scanning
            try
            {
                // Get the first 16 bytes for matching
                byte[] magic = new byte[16];
                try
                {
                    magic = stream.ReadBytes(16);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception ex)
                {
                    if (_includeDebug) Console.Error.WriteLine(ex);
                    return [];
                }

                // Get the file type either from magic number or extension
                WrapperType fileType = WrapperFactory.GetFileType(magic, extension);
                if (fileType == WrapperType.UNKNOWN)
                {
                    if (_includeDebug) Console.WriteLine($"{fileName} not a scannable file type, skipping...");
                    return [];
                }

                // Get the wrapper, if possible
                var wrapper = WrapperFactory.CreateWrapper(fileType, stream);
                if (wrapper == null)
                {
                    if (_includeDebug) Console.WriteLine($"{fileName} not a scannable file type, skipping...");
                    return [];
                }

                #region Non-Archive File Types

                // Try to scan file contents
                var detectable = CreateDetectable(fileType, wrapper);
                if (_scanContents && detectable != null)
                {
                    var subProtection = detectable.Detect(stream, fileName, _includeDebug);
                    protections.Append(fileName, subProtection);
                }

                #endregion

                #region Archive File Types

                // If we're scanning archives
                if (_scanArchives && wrapper is IExtractable extractable)
                {
                    // If the extractable file itself fails
                    try
                    {
                        // Extract and get the output path
                        string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                        Directory.CreateDirectory(tempPath);
                        bool extracted = extractable.Extract(tempPath, _includeDebug);

                        // Collect and format all found protections
                        ProtectionDictionary? subProtections = null;
                        if (extracted)
                            subProtections = GetProtectionsImpl(tempPath, depth + 1);

                        // If temp directory cleanup fails
                        try
                        {
                            if (Directory.Exists(tempPath))
                                Directory.Delete(tempPath, true);
                        }
                        catch (Exception ex)
                        {
                            if (_includeDebug) Console.Error.WriteLine(ex);
                        }

                        // Prepare the returned protections
                        subProtections?.StripFromKeys(tempPath);
                        subProtections?.PrependToKeys(fileName);
                        if (subProtections != null)
                            protections.Append(subProtections);
                    }
                    catch (Exception ex)
                    {
                        if (_includeDebug) Console.Error.WriteLine(ex);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                if (_includeDebug) Console.WriteLine(ex);
                protections.Append(fileName, _includeDebug ? ex.ToString() : "[Exception opening file, please try again]");
            }

            // Clear out any empty keys
            protections.ClearEmptyKeys();

            return protections;
        }

        #endregion

        #region Path Handling

        /// <summary>
        /// Handle a single path based on all path check implementations
        /// </summary>
        /// <param name="path">Path of the file or directory to check</param>
        /// <param name="scanner">Scanner object to use for options and scanning</param>
        /// <returns>Set of protections in file, null on error</returns>
        private static ProtectionDictionary HandlePathChecks(string path, List<string>? files)
        {
            // Create the output dictionary
            var protections = new ProtectionDictionary();

            // Preprocess the list of files
            files = files?
                .ConvertAll(f => f.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));

            // Iterate through all checks
            StaticChecks.PathCheckClasses.IterateWithAction(checkClass =>
            {
                var subProtections = PerformPathCheck(checkClass, path, files);
                protections.Append(path, subProtections);
            });

            return protections;
        }

        /// <summary>
        /// Handle files based on an IPathCheck implementation
        /// </summary>
        /// <param name="impl">IPathCheck class representing the file type</param>
        /// <param name="path">Path of the file or directory to check</param>
        /// <returns>Set of protections in path, empty on error</returns>
        private static List<string> PerformPathCheck(IPathCheck impl, string? path, List<string>? files)
        {
            // If we have an invalid path
            if (string.IsNullOrEmpty(path))
                return [];

            // Setup the list
            var protections = new List<string>();

            // If we have a file path
            if (File.Exists(path))
            {
                var protection = impl.CheckFilePath(path!);
                if (protection != null)
                    protections.Add(protection);
            }

            // If we have a directory path
            if (Directory.Exists(path) && files != null && files.Count > 0)
            {
                var subProtections = impl.CheckDirectoryPath(path!, files);
                if (subProtections != null)
                    protections.AddRange(subProtections);
            }

            return protections;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Create an instance of a detectable based on file type
        /// </summary>
        private static IDetectable? CreateDetectable(WrapperType fileType, IWrapper? wrapper)
        {
            // Use the wrapper before the type
            switch (wrapper)
            {
                case AACSMediaKeyBlock obj: return new FileType.AACSMediaKeyBlock(obj);
                case BDPlusSVM obj: return new FileType.BDPlusSVM(obj);
                // case CIA obj => new FileType.CIA(obj),
                case LinearExecutable obj: return new FileType.LinearExecutable(obj);
                case MSDOS obj: return new FileType.MSDOS(obj);
                // case N3DS obj: return new FileType.N3DS(obj);
                case NewExecutable obj: return new FileType.NewExecutable(obj);
                case PlayJAudioFile obj: return new FileType.PLJ(obj);
                case PortableExecutable obj: return new FileType.PortableExecutable(obj);
            }

            // Fall back on the file type for types not implemented in Serialization
            return fileType switch
            {
                // WrapperType.CIA => new FileType.CIA(),
                WrapperType.LDSCRYPT => new FileType.LDSCRYPT(),
                // WrapperType.N3DS => new FileType.N3DS(),
                WrapperType.RealArcadeInstaller => new FileType.RealArcadeInstaller(),
                WrapperType.RealArcadeMezzanine => new FileType.RealArcadeMezzanine(),
                WrapperType.SFFS => new FileType.SFFS(),
                WrapperType.Textfile => new FileType.Textfile(),
                _ => null,
            };
        }

        #endregion
    }
}
