using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if NET462_OR_GREATER || NETCOREAPP
using System.Text;
#endif
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using BinaryObjectScanner.FileType;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Utilities;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;
using static BinaryObjectScanner.Utilities.Dictionary;

namespace BinaryObjectScanner
{
    public class Scanner
    {
        #region Options

        /// <inheritdoc cref="Options.ScanArchives"/>
        public bool ScanArchives => _options?.ScanArchives ?? false;

        /// <inheritdoc cref="Options.ScanContents"/>
        public bool ScanContents => _options?.ScanContents ?? false;

        /// <inheritdoc cref="Options.ScanGameEngines"/>
        public bool ScanGameEngines => _options?.ScanGameEngines ?? false;

        /// <inheritdoc cref="Options.ScanPackers"/>
        public bool ScanPackers => _options?.ScanPackers ?? false;

        /// <inheritdoc cref="Options.ScanPaths"/>
        public bool ScanPaths => _options?.ScanPaths ?? false;

        /// <inheritdoc cref="Options.IncludeDebug"/>
        public bool IncludeDebug => _options?.IncludeDebug ?? false;

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
        public Scanner(bool scanArchives, bool scanContents, bool scanGameEngines, bool scanPackers, bool scanPaths, bool includeDebug, IProgress<ProtectionProgress>? fileProgress = null)
        {
            this._options = new Options
            {
                ScanArchives = scanArchives,
                ScanContents = scanContents,
                ScanGameEngines = scanGameEngines,
                ScanPackers = scanPackers,
                ScanPaths = scanPaths,
                IncludeDebug = includeDebug,
            };

            this._fileProgress = fileProgress;

#if NET462_OR_GREATER || NETCOREAPP
            // Register the codepages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        #region Scanning

        /// <summary>
        /// Scan a single path and get all found protections
        /// </summary>
        /// <param name="path">Path to scan</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
#if NET20 || NET35
        public Dictionary<string, Queue<string>>? GetProtections(string path)
#else
        public ConcurrentDictionary<string, ConcurrentQueue<string>>? GetProtections(string path)
#endif
        {
            return GetProtections([path]);
        }

        /// <summary>
        /// Scan the list of paths and get all found protections
        /// </summary>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
#if NET20 || NET35
        public Dictionary<string, Queue<string>>? GetProtections(List<string>? paths)
#else
        public ConcurrentDictionary<string, ConcurrentQueue<string>>? GetProtections(List<string>? paths)
#endif
        {
            // If we have no paths, we can't scan
            if (paths == null || !paths.Any())
                return null;

            // Set a starting starting time for debug output
            DateTime startTime = DateTime.UtcNow;

            // Checkpoint
            this._fileProgress?.Report(new ProtectionProgress(null, 0, null));

            // Temp variables for reporting
            string tempFilePath = Path.GetTempPath();
            string tempFilePathWithGuid = Path.Combine(tempFilePath, Guid.NewGuid().ToString());

            // Loop through each path and get the returned values
#if NET20 || NET35
            var protections = new Dictionary<string, Queue<string>>();
#else
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
#endif
            foreach (string path in paths)
            {
                // Directories scan each internal file individually
                if (Directory.Exists(path))
                {
                    // Enumerate all files at first for easier access
                    var files = IOExtensions.SafeEnumerateFiles(path, "*", SearchOption.AllDirectories).ToList();

                    // Scan for path-detectable protections
                    if (ScanPaths)
                    {
                        var directoryPathProtections = Handler.HandlePathChecks(path, files);
                        AppendToDictionary(protections, directoryPathProtections);
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
                        this._fileProgress?.Report(new ProtectionProgress(reportableFileName, i / (float)files.Count, "Checking file" + (file != reportableFileName ? " from archive" : string.Empty)));

                        // Scan for path-detectable protections
                        if (ScanPaths)
                        {
                            var filePathProtections = Handler.HandlePathChecks(file, files: null);
                            AppendToDictionary(protections, filePathProtections);
                        }

                        // Scan for content-detectable protections
                        var fileProtections = GetInternalProtections(file);
                        if (fileProtections != null && fileProtections.Any())
                        {
                            foreach (string key in fileProtections.Keys)
                            {
                                if (!protections.ContainsKey(key))
#if NET20 || NET35
                                    protections[key] = new Queue<string>();
#else
                                    protections[key] = new ConcurrentQueue<string>();
#endif

                                protections[key].AddRange(fileProtections[key]);
                            }
                        }

                        // Checkpoint
                        protections.TryGetValue(file, out var fullProtectionList);
                        var fullProtection = fullProtectionList != null && fullProtectionList.Any() ? string.Join(", ", [.. fullProtectionList]) : null;
                        this._fileProgress?.Report(new ProtectionProgress(reportableFileName, (i + 1) / (float)files.Count, fullProtection ?? string.Empty));
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
                    this._fileProgress?.Report(new ProtectionProgress(reportableFileName, 0, "Checking file" + (path != reportableFileName ? " from archive" : string.Empty)));

                    // Scan for path-detectable protections
                    if (ScanPaths)
                    {
                        var filePathProtections = Handler.HandlePathChecks(path, files: null);
                        AppendToDictionary(protections, filePathProtections);
                    }

                    // Scan for content-detectable protections
                    var fileProtections = GetInternalProtections(path);
                    if (fileProtections != null && fileProtections.Any())
                    {
                        foreach (string key in fileProtections.Keys)
                        {
                            if (!protections.ContainsKey(key))
#if NET20 || NET35
                                protections[key] = new Queue<string>();
#else
                                protections[key] = new ConcurrentQueue<string>();
#endif

                            protections[key].AddRange(fileProtections[key]);
                        }
                    }

                    // Checkpoint
                    protections.TryGetValue(path, out var fullProtectionList);
                    var fullProtection = fullProtectionList != null && fullProtectionList.Any() ? string.Join(", ", [.. fullProtectionList]) : null;
                    this._fileProgress?.Report(new ProtectionProgress(reportableFileName, 1, fullProtection ?? string.Empty));
                }

                // Throw on an invalid path
                else
                {
                    Console.WriteLine($"{path} is not a directory or file, skipping...");
                    //throw new FileNotFoundException($"{path} is not a directory or file, skipping...");
                }
            }

            // Clear out any empty keys
            ClearEmptyKeys(protections);

            // If we're in debug, output the elasped time to console
            if (IncludeDebug)
                Console.WriteLine($"Time elapsed: {DateTime.UtcNow.Subtract(startTime)}");

            return protections;
        }

        /// <summary>
        /// Get the content-detectable protections associated with a single path
        /// </summary>
        /// <param name="file">Path to the file to scan</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
#if NET20 || NET35
        private Dictionary<string, Queue<string>>? GetInternalProtections(string file)
#else
        private ConcurrentDictionary<string, ConcurrentQueue<string>>? GetInternalProtections(string file)
#endif
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
                if (IncludeDebug) Console.WriteLine(ex);

#if NET20 || NET35
                var protections = new Dictionary<string, Queue<string>>();
#else
                var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
#endif
                AppendToDictionary(protections, file, IncludeDebug ? ex.ToString() : "[Exception opening file, please try again]");
                ClearEmptyKeys(protections);
                return protections;
            }
        }

        /// <summary>
        /// Get the content-detectable protections associated with a single path
        /// </summary>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
#if NET20 || NET35
        private Dictionary<string, Queue<string>>? GetInternalProtections(string fileName, Stream stream)
#else
        private ConcurrentDictionary<string, ConcurrentQueue<string>>? GetInternalProtections(string fileName, Stream stream)
#endif
        {
            // Quick sanity check before continuing
            if (stream == null || !stream.CanRead || !stream.CanSeek)
                return null;

            // Initialize the protections found
#if NET20 || NET35
            var protections = new Dictionary<string, Queue<string>>();
#else
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
#endif

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
                WrapperType fileType = WrapperFactory.GetFileType(magic, extension);
                if (fileType == WrapperType.UNKNOWN)
                    return null;

                #region Non-Archive File Types

                // Create a detectable for the given file type
                var detectable = Factory.CreateDetectable(fileType);

                // If we're scanning file contents
                if (detectable != null && ScanContents)
                {
                    // If we have an executable, it needs to bypass normal handling
                    if (detectable is Executable executable)
                    {
                        executable.IncludeGameEngines = ScanGameEngines;
                        executable.IncludePackers = ScanPackers;
                        var subProtections = ProcessExecutable(executable, fileName, stream);
                        if (subProtections != null)
                            AppendToDictionary(protections, subProtections);
                    }

                    // Otherwise, use the default implementation
                    else
                    {
                        var subProtections = Handler.HandleDetectable(detectable, fileName, stream, IncludeDebug);
                        if (subProtections != null)
                            AppendToDictionary(protections, fileName, subProtections);
                    }

                    var subProtection = detectable.Detect(stream, fileName, IncludeDebug);
                    if (!string.IsNullOrEmpty(subProtection))
                    {
                        // If we have an indicator of multiple protections
                        if (subProtection.Contains(';'))
                        {
                            var splitProtections = subProtection!.Split(';');
                            AppendToDictionary(protections, fileName, splitProtections);
                        }
                        else
                        {
                            AppendToDictionary(protections, fileName, subProtection!);
                        }
                    }
                }

                #endregion

                #region Archive File Types

                // Create an extractable for the given file type
                var extractable = Factory.CreateExtractable(fileType);

                // If we're scanning archives
                if (extractable != null && ScanArchives)
                {
                    var subProtections = Handler.HandleExtractable(extractable, fileName, stream, this);
                    if (subProtections != null)
                        AppendToDictionary(protections, subProtections);
                }

                #endregion
            }
            catch (Exception ex)
            {
                if (IncludeDebug) Console.WriteLine(ex);
                AppendToDictionary(protections, fileName, IncludeDebug ? ex.ToString() : "[Exception opening file, please try again]");
            }

            // Clear out any empty keys
            ClearEmptyKeys(protections);

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
#if NET20 || NET35
        private Dictionary<string, Queue<string>>? ProcessExecutable(Executable executable, string fileName, Stream stream)
#else
        private ConcurrentDictionary<string, ConcurrentQueue<string>>? ProcessExecutable(Executable executable, string fileName, Stream stream)
#endif
        {
            // Try to create a wrapper for the proper executable type
            var wrapper = WrapperFactory.CreateExecutableWrapper(stream);
            if (wrapper == null)
                return null;

            // Create the output dictionary
#if NET20 || NET35
            var protections = new Dictionary<string, Queue<string>>();
#else
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
#endif

            // Only use generic content checks if we're in debug mode
            if (IncludeDebug)
            {
                var subProtections = executable.RunContentChecks(fileName, stream, IncludeDebug);
                if (subProtections != null)
                    AppendToDictionary(protections, fileName, subProtections.Values.ToArray());
            }

            if (wrapper is MSDOS mz)
            {
                var subProtections = executable.RunMSDOSExecutableChecks(fileName, stream, mz, IncludeDebug);
                if (subProtections == null)
                    return protections;

                // Append the returned values
                AppendToDictionary(protections, fileName, subProtections.Values.ToArray());

                // If we have any extractable packers
                var extractedProtections = HandleExtractableProtections(subProtections.Keys, fileName, mz);
                if (extractedProtections != null)
                    AppendToDictionary(protections, extractedProtections);
            }
            else if (wrapper is LinearExecutable lex)
            {
                var subProtections = executable.RunLinearExecutableChecks(fileName, stream, lex, IncludeDebug);
                if (subProtections == null)
                    return protections;

                // Append the returned values
                AppendToDictionary(protections, fileName, subProtections.Values.ToArray());

                // If we have any extractable packers
                var extractedProtections = HandleExtractableProtections(subProtections.Keys, fileName, lex);
                if (extractedProtections != null)
                    AppendToDictionary(protections, extractedProtections);
            }
            else if (wrapper is NewExecutable nex)
            {
                var subProtections = executable.RunNewExecutableChecks(fileName, stream, nex, IncludeDebug);
                if (subProtections == null)
                    return protections;

                // Append the returned values
                AppendToDictionary(protections, fileName, subProtections.Values.ToArray());

                // If we have any extractable packers
                var extractedProtections = HandleExtractableProtections(subProtections.Keys, fileName, nex);
                if (extractedProtections != null)
                    AppendToDictionary(protections, extractedProtections);
            }
            else if (wrapper is PortableExecutable pex)
            {
                var subProtections = executable.RunPortableExecutableChecks(fileName, stream, pex, IncludeDebug);
                if (subProtections == null)
                    return protections;

                // Append the returned values
                AppendToDictionary(protections, fileName, subProtections.Values.ToArray());

                // If we have any extractable packers
                var extractedProtections = HandleExtractableProtections(subProtections.Keys, fileName, pex);
                if (extractedProtections != null)
                    AppendToDictionary(protections, extractedProtections);
            }

            return protections;
        }

        /// <summary>
        /// Handle extractable protections, such as executable packers
        /// </summary>
        /// <param name="classes">Set of classes returned from Exectuable scans</param>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="mz">MSDOS to scan the contents of</param>
        /// <returns>Set of protections found from extraction, null on error</returns>
#if NET20 || NET35
        private Dictionary<string, Queue<string>>? HandleExtractableProtections<T>(Dictionary<T, string>.KeyCollection? classes, string fileName, MSDOS mz)
#else
        private ConcurrentDictionary<string, ConcurrentQueue<string>>? HandleExtractableProtections(IEnumerable<object>? classes, string fileName, MSDOS mz)
#endif
        {
            // If we have an invalid set of classes
            if (classes == null || !classes.Any())
                return null;

            // Create the output dictionary
#if NET20 || NET35
            var protections = new Dictionary<string, Queue<string>>();
#else
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
#endif

            // If we have any extractable packers
            var extractables = classes.Where(c => c is IExtractableMSDOSExecutable).Select(c => c as IExtractableMSDOSExecutable);
#if NET20 || NET35
            foreach (var extractable in extractables)
#else
            Parallel.ForEach(extractables, extractable =>
#endif
            {
                // If we have an invalid extractable somehow
                if (extractable == null)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // Get the protection for the class, if possible
                var extractedProtections = Handler.HandleExtractable(extractable, fileName, mz, this);
                if (extractedProtections != null)
                    AppendToDictionary(protections, extractedProtections);
#if NET20 || NET35
            }
#else
            });
#endif

            return protections;
        }

        /// <summary>
        /// Handle extractable protections, such as executable packers
        /// </summary>
        /// <param name="classes">Set of classes returned from Exectuable scans</param>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="lex">LinearExecutable to scan the contents of</param>
        /// <returns>Set of protections found from extraction, null on error</returns>
#if NET20 || NET35
        private Dictionary<string, Queue<string>>? HandleExtractableProtections<T>(Dictionary<T, string>.KeyCollection? classes, string fileName, LinearExecutable lex)
#else
        private ConcurrentDictionary<string, ConcurrentQueue<string>>? HandleExtractableProtections(IEnumerable<object>? classes, string fileName, LinearExecutable lex)
#endif
        {
            // If we have an invalid set of classes
            if (classes == null || !classes.Any())
                return null;

            // Create the output dictionary
#if NET20 || NET35
            var protections = new Dictionary<string, Queue<string>>();
#else
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
#endif

            // If we have any extractable packers
            var extractables = classes.Where(c => c is IExtractableLinearExecutable).Select(c => c as IExtractableLinearExecutable);
#if NET20 || NET35
            foreach (var extractable in extractables)
#else
            Parallel.ForEach(extractables, extractable =>
#endif
            {
                // If we have an invalid extractable somehow
                if (extractable == null)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // Get the protection for the class, if possible
                var extractedProtections = Handler.HandleExtractable(extractable, fileName, lex, this);
                if (extractedProtections != null)
                    AppendToDictionary(protections, extractedProtections);
#if NET20 || NET35
            }
#else
            });
#endif

            return protections;
        }

        /// <summary>
        /// Handle extractable protections, such as executable packers
        /// </summary>
        /// <param name="classes">Set of classes returned from Exectuable scans</param>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="nex">NewExecutable to scan the contents of</param>
        /// <returns>Set of protections found from extraction, null on error</returns>
#if NET20 || NET35
        private Dictionary<string, Queue<string>>? HandleExtractableProtections<T>(Dictionary<T, string>.KeyCollection? classes, string fileName, NewExecutable nex)
#else
        private ConcurrentDictionary<string, ConcurrentQueue<string>>? HandleExtractableProtections(IEnumerable<object>? classes, string fileName, NewExecutable nex)
#endif
        {
            // If we have an invalid set of classes
            if (classes == null || !classes.Any())
                return null;

            // Create the output dictionary
#if NET20 || NET35
            var protections = new Dictionary<string, Queue<string>>();
#else
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
#endif

            // If we have any extractable packers
            var extractables = classes.Where(c => c is IExtractableNewExecutable).Select(c => c as IExtractableNewExecutable);
#if NET20 || NET35
            foreach (var extractable in extractables)
#else
            Parallel.ForEach(extractables, extractable =>
#endif
            {
                // If we have an invalid extractable somehow
                if (extractable == null)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // Get the protection for the class, if possible
                var extractedProtections = Handler.HandleExtractable(extractable, fileName, nex, this);
                if (extractedProtections != null)
                    AppendToDictionary(protections, extractedProtections);
#if NET20 || NET35
            }
#else
            });
#endif

            return protections;
        }

        /// <summary>
        /// Handle extractable protections, such as executable packers
        /// </summary>
        /// <param name="classes">Set of classes returned from Exectuable scans</param>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="pex">PortableExecutable to scan the contents of</param>
        /// <returns>Set of protections found from extraction, null on error</returns>
#if NET20 || NET35
        private Dictionary<string, Queue<string>>? HandleExtractableProtections<T>(Dictionary<T, string>.KeyCollection? classes, string fileName, PortableExecutable pex)
#else
        private ConcurrentDictionary<string, ConcurrentQueue<string>>? HandleExtractableProtections(IEnumerable<object>? classes, string fileName, PortableExecutable pex)
#endif
        {
            // If we have an invalid set of classes
            if (classes == null || !classes.Any())
                return null;

            // Create the output dictionary
#if NET20 || NET35
            var protections = new Dictionary<string, Queue<string>>();
#else
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
#endif

            // If we have any extractable packers
            var extractables = classes.Where(c => c is IExtractablePortableExecutable).Select(c => c as IExtractablePortableExecutable);
#if NET20 || NET35
            foreach (var extractable in extractables)
#else
            Parallel.ForEach(extractables, extractable =>
#endif
            {
                // If we have an invalid extractable somehow
                if (extractable == null)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // Get the protection for the class, if possible
                var extractedProtections = Handler.HandleExtractable(extractable, fileName, pex, this);
                if (extractedProtections != null)
                    AppendToDictionary(protections, extractedProtections);
#if NET20 || NET35
            }
#else
            });
#endif

            return protections;
        }

        #endregion
    }
}
