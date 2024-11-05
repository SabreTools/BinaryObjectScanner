using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.FileType;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner
{
    public class Scanner
    {
        #region Options

        /// <summary>
        /// Options object for configuration
        /// </summary>
        private readonly Options _options;

        #endregion

        /// <summary>
        /// Optional progress callback during scanning
        /// </summary>
        private readonly IProgress<ProtectionProgress>? _fileProgress;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scanArchives">Enable scanning archive contents</param>
        /// <param name="scanContents">Enable including content detections in output</param>
        /// <param name="scanGameEngines">Enable including game engines in output</param>
        /// <param name="scanPackers">Enable including packers in output</param>
        /// <param name="scanPaths">Enable including path detections in output</param>
        /// <param name="includeDebug">Enable including debug information</param>
        /// <param name="fileProgress">Optional progress callback</param>
        public Scanner(bool scanArchives,
            bool scanContents,
            bool scanGameEngines,
            bool scanPackers,
            bool scanPaths,
            bool includeDebug,
            IProgress<ProtectionProgress>? fileProgress = null)
        {
            _options = new Options
            {
                ScanArchives = scanArchives,
                ScanContents = scanContents,
                ScanGameEngines = scanGameEngines,
                ScanPackers = scanPackers,
                ScanPaths = scanPaths,
                IncludeDebug = includeDebug,
            };

            _fileProgress = fileProgress;

#if NET462_OR_GREATER || NETCOREAPP
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
        public ProtectionDictionary? GetProtections(string path)
            => GetProtections([path]);

        /// <summary>
        /// Scan the list of paths and get all found protections
        /// </summary>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        public ProtectionDictionary? GetProtections(List<string>? paths)
        {
            // If we have no paths, we can't scan
            if (paths == null || !paths.Any())
                return null;

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
                    var files = IOExtensions.SafeEnumerateFiles(path, "*", SearchOption.AllDirectories).ToList();

                    // Scan for path-detectable protections
                    if (_options.ScanPaths)
                    {
                        var directoryPathProtections = Handler.HandlePathChecks(path, files);
                        protections.Append(directoryPathProtections);
                    }

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
                        _fileProgress?.Report(new ProtectionProgress(reportableFileName, i / (float)files.Count, "Checking file" + (file != reportableFileName ? " from archive" : string.Empty)));

                        // Scan for path-detectable protections
                        if (_options.ScanPaths)
                        {
                            var filePathProtections = Handler.HandlePathChecks(file, files: null);
                            if (filePathProtections != null && filePathProtections.Any())
                                protections.Append(filePathProtections);
                        }

                        // Scan for content-detectable protections
                        var fileProtections = GetInternalProtections(file);
                        if (fileProtections != null && fileProtections.Any())
                            protections.Append(fileProtections);

                        // Checkpoint
                        protections.TryGetValue(file, out var fullProtectionList);
                        var fullProtection = fullProtectionList != null && fullProtectionList.Any() ? string.Join(", ", [.. fullProtectionList]) : null;
                        _fileProgress?.Report(new ProtectionProgress(reportableFileName, (i + 1) / (float)files.Count, fullProtection ?? string.Empty));
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
                    _fileProgress?.Report(new ProtectionProgress(reportableFileName, 0, "Checking file" + (path != reportableFileName ? " from archive" : string.Empty)));

                    // Scan for path-detectable protections
                    if (_options.ScanPaths)
                    {
                        var filePathProtections = Handler.HandlePathChecks(path, files: null);
                        if (filePathProtections != null && filePathProtections.Any())
                            protections.Append(filePathProtections);
                    }

                    // Scan for content-detectable protections
                    var fileProtections = GetInternalProtections(path);
                    if (fileProtections != null && fileProtections.Any())
                        protections.Append(fileProtections);

                    // Checkpoint
                    protections.TryGetValue(path, out var fullProtectionList);
                    var fullProtection = fullProtectionList != null && fullProtectionList.Any() ? string.Join(", ", [.. fullProtectionList]) : null;
                    _fileProgress?.Report(new ProtectionProgress(reportableFileName, 1, fullProtection ?? string.Empty));
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
            if (_options.IncludeDebug)
                Console.WriteLine($"Time elapsed: {DateTime.UtcNow.Subtract(startTime)}");

            return protections;
        }

        /// <summary>
        /// Get the content-detectable protections associated with a single path
        /// </summary>
        /// <param name="file">Path to the file to scan</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private ProtectionDictionary? GetInternalProtections(string file)
        {
            // Quick sanity check before continuing
            if (!File.Exists(file))
                return null;

            // Open the file and begin scanning
            try
            {
                using FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return GetInternalProtections(file, fs);
            }
            catch (Exception ex)
            {
                if (_options.IncludeDebug) Console.WriteLine(ex);

                var protections = new ProtectionDictionary();
                protections.Append(file, _options.IncludeDebug ? ex.ToString() : "[Exception opening file, please try again]");
                protections.ClearEmptyKeys();
                return protections;
            }
        }

        /// <summary>
        /// Get the content-detectable protections associated with a single path
        /// </summary>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private ProtectionDictionary? GetInternalProtections(string fileName, Stream stream)
        {
            // Quick sanity check before continuing
            if (stream == null || !stream.CanRead || !stream.CanSeek)
                return null;

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
                    stream.Read(magic, 0, 16);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception ex)
                {
                    if (_options.IncludeDebug) Console.WriteLine(ex);

                    return null;
                }

                // Get the file type either from magic number or extension
                WrapperType fileType = WrapperFactory.GetFileType(magic, extension);
                if (fileType == WrapperType.UNKNOWN)
                    return null;

                #region Non-Archive File Types

                // Create a detectable for the given file type
                var detectable = Factory.CreateDetectable(fileType);

                // If we're scanning file contents
                if (detectable != null && _options.ScanContents)
                {
                    // If we have an executable, it needs to bypass normal handling
                    if (detectable is Executable executable)
                    {
                        executable.IncludeGameEngines = _options.ScanGameEngines;
                        executable.IncludePackers = _options.ScanPackers;
                        var subProtections = ProcessExecutable(executable, fileName, stream);
                        if (subProtections != null)
                            protections.Append(subProtections);
                    }

                    // Otherwise, use the default implementation
                    else
                    {
                        var subProtections = Handler.HandleDetectable(detectable, fileName, stream, _options.IncludeDebug);
                        if (subProtections != null)
                            protections.Append(fileName, subProtections);
                    }

                    var subProtection = detectable.Detect(stream, fileName, _options.IncludeDebug);
                    if (!string.IsNullOrEmpty(subProtection))
                    {
                        // If we have an indicator of multiple protections
                        if (subProtection.Contains(';'))
                        {
                            var splitProtections = subProtection!.Split(';');
                            protections.Append(fileName, splitProtections);
                        }
                        else
                        {
                            protections.Append(fileName, subProtection!);
                        }
                    }
                }

                #endregion

                #region Archive File Types

                // Create an extractable for the given file type
                var extractable = Factory.CreateExtractable(fileType);

                // If we're scanning archives
                if (extractable != null && _options.ScanArchives)
                {
                    // If the extractable file itself fails
                    try
                    {
                        // Extract and get the output path
                        string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                        bool extracted = extractable.Extract(stream, fileName, tempPath, _options.IncludeDebug);

                        // Collect and format all found protections
                        ProtectionDictionary? subProtections = null;
                        if (extracted)
                            subProtections = GetProtections(tempPath);

                        // If temp directory cleanup fails
                        try
                        {
                            if (Directory.Exists(tempPath))
                                Directory.Delete(tempPath, true);
                        }
                        catch (Exception ex)
                        {
                            if (_options.IncludeDebug) Console.WriteLine(ex);
                        }

                        // Prepare the returned protections
                        subProtections?.StripFromKeys(tempPath);
                        subProtections?.PrependToKeys(fileName);
                        if (subProtections != null)
                            protections.Append(subProtections);
                    }
                    catch (Exception ex)
                    {
                        if (_options.IncludeDebug) Console.WriteLine(ex);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                if (_options.IncludeDebug) Console.WriteLine(ex);
                protections.Append(fileName, _options.IncludeDebug ? ex.ToString() : "[Exception opening file, please try again]");
            }

            // Clear out any empty keys
            protections.ClearEmptyKeys();

            return protections;
        }

        #endregion

        #region Executable Handling

        /// <summary>
        /// Process scanning for an Executable type
        /// </summary>
        /// <param name="executable">Executable instance for processing</param>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <remarks>
        /// Ideally, we wouldn't need to circumvent the proper handling of file types just for Executable,
        /// but due to the complexity of scanning, this is not currently possible.
        /// </remarks>
        private ProtectionDictionary? ProcessExecutable(Executable executable, string fileName, Stream stream)
        {
            // Try to create a wrapper for the proper executable type
            IWrapper? wrapper;
            try
            {
                wrapper = WrapperFactory.CreateExecutableWrapper(stream);
                if (wrapper == null)
                    return null;
            }
            catch (Exception ex)
            {
                if (_options.IncludeDebug) Console.WriteLine(ex);
                return null;
            }

            // Create the output dictionary
            var protections = new ProtectionDictionary();

            // Only use generic content checks if we're in debug mode
            if (_options.IncludeDebug)
            {
                var subProtections = executable.RunContentChecks(fileName, stream, _options.IncludeDebug);
                protections.Append(fileName, subProtections.Values);
            }

            if (wrapper is MSDOS mz)
            {
                // Standard checks
                var subProtections = executable.RunExecutableChecks(fileName, mz, Executable.MSDOSExecutableCheckClasses, _options.IncludeDebug);
                protections.Append(fileName, subProtections.Values);

                // Extractable checks
                var extractedProtections = HandleExtractableProtections(fileName, mz, subProtections.Keys);
                protections.Append(extractedProtections);
            }
            else if (wrapper is LinearExecutable lex)
            {
                // Standard checks
                var subProtections = executable.RunExecutableChecks(fileName, lex, Executable.LinearExecutableCheckClasses, _options.IncludeDebug);
                protections.Append(fileName, subProtections.Values);

                // Extractable checks
                var extractedProtections = HandleExtractableProtections(fileName, lex, subProtections.Keys);
                protections.Append(extractedProtections);
            }
            else if (wrapper is NewExecutable nex)
            {
                // Standard checks
                var subProtections = executable.RunExecutableChecks(fileName, nex, Executable.NewExecutableCheckClasses, _options.IncludeDebug);
                protections.Append(fileName, subProtections.Values);

                // Extractable checks
                var extractedProtections = HandleExtractableProtections(fileName, nex, subProtections.Keys);
                protections.Append(extractedProtections);
            }
            else if (wrapper is PortableExecutable pex)
            {
                // Standard checks
                var subProtections = executable.RunExecutableChecks(fileName, pex, Executable.PortableExecutableCheckClasses, _options.IncludeDebug);
                protections.Append(fileName, subProtections.Values);

                // Extractable checks
                var extractedProtections = HandleExtractableProtections(fileName, pex, subProtections.Keys);
                protections.Append(extractedProtections);
            }

            return protections;
        }

        /// <summary>
        /// Handle extractable protections, such as executable packers
        /// </summary>
        /// <param name="file">Name of the source file of the stream, for tracking</param>
        /// <param name="exe">Executable to scan the contents of</param>
        /// <param name="checks">Set of classes returned from Exectuable scans</param>
        /// <returns>Set of protections found from extraction, null on error</returns>
        private ProtectionDictionary HandleExtractableProtections<T, U>(string file, T exe, IEnumerable<U> checks)
            where T : WrapperBase
            where U : IExecutableCheck<T>
        {
            // Create the output dictionary
            var protections = new ProtectionDictionary();

            // If we have an invalid set of classes
            if (checks == null || !checks.Any())
                return protections;

            // If we have any extractable packers
            var extractables = checks
                .Where(c => c is IExtractableExecutable<T>)
                .Select(c => c as IExtractableExecutable<T>);
            extractables.IterateWithAction(extractable =>
            {
                // If we have an invalid extractable somehow
                if (extractable == null)
                    return;

                // If the extractable file itself fails
                try
                {
                    // Extract and get the output path
                    string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    bool extracted = extractable.Extract(file, exe, tempPath, _options.IncludeDebug);

                    // Collect and format all found protections
                    ProtectionDictionary? subProtections = null;
                    if (extracted)
                        subProtections = GetProtections(tempPath);

                    // If temp directory cleanup fails
                    try
                    {
                        if (Directory.Exists(tempPath))
                            Directory.Delete(tempPath, true);
                    }
                    catch (Exception ex)
                    {
                        if (_options.IncludeDebug) Console.WriteLine(ex);
                    }

                    // Prepare the returned protections
                    subProtections?.StripFromKeys(tempPath);
                    subProtections?.PrependToKeys(file);
                    if (subProtections != null)
                        protections.Append(subProtections);
                }
                catch (Exception ex)
                {
                    if (_options.IncludeDebug) Console.WriteLine(ex);
                }
            });

            return protections;
        }

        #endregion
    }
}
