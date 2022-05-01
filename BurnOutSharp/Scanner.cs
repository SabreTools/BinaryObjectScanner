using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BurnOutSharp.FileType;
using BurnOutSharp.Tools;

namespace BurnOutSharp
{
    public class Scanner
    {
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

        /// <summary>
        /// Optional progress callback during scanning
        /// </summary>
        public IProgress<ProtectionProgress> FileProgress { get; private set; }

        /// <summary>
        /// Cache for all IPathCheck types
        /// </summary>
        private static readonly IEnumerable<IPathCheck> pathCheckClasses = InitPathCheckClasses();

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
            FileProgress = fileProgress;
        }

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
            FileProgress?.Report(new ProtectionProgress(null, 0, null));

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
                        FileProgress?.Report(new ProtectionProgress(reportableFileName, i / (float)files.Count, "Checking file" + (file != reportableFileName ? " from archive" : string.Empty)));

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
                        FileProgress?.Report(new ProtectionProgress(reportableFileName, (i + 1) / (float)files.Count, fullProtection ?? string.Empty));
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
                    FileProgress?.Report(new ProtectionProgress(reportableFileName, 0, "Checking file" + (path != reportableFileName ? " from archive" : string.Empty)));

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
                    FileProgress?.Report(new ProtectionProgress(reportableFileName, 1, fullProtection ?? string.Empty));
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
            Parallel.ForEach(pathCheckClasses, pathCheckClass =>
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
            Parallel.ForEach(pathCheckClasses, pathCheckClass =>
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

            // Initialize the protections found
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

            // Get the extension for certain checks
            string extension = Path.GetExtension(file).ToLower().TrimStart('.');

            // Open the file and begin scanning
            try
            {
                using (FileStream fs = File.OpenRead(file))
                {
                    // Get the first 16 bytes for matching
                    byte[] magic = new byte[16];
                    try
                    {
                        fs.Read(magic, 0, 16);
                        fs.Seek(0, SeekOrigin.Begin);
                    }
                    catch
                    {
                        // We don't care what the issue was, we can't read or seek the file
                        return null;
                    }

                    #region Non-Archive File Types

                    // Executable
                    if (new Executable().ShouldScan(magic))
                    {
                        var subProtections = new Executable().Scan(this, fs, file);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    // Text-based files
                    if (new Textfile().ShouldScan(magic, extension))
                    {
                        var subProtections = new Textfile().Scan(this, fs, file);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }

                    #endregion

                    #region Archive File Types

                    // If we're scanning archives, we have a few to try out
                    if (ScanArchives)
                    {
                        // 7-Zip archive
                        if (new SevenZip().ShouldScan(magic))
                        {
                            var subProtections = new SevenZip().Scan(this, fs, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // BFPK archive
                        if (new BFPK().ShouldScan(magic))
                        {
                            var subProtections = new BFPK().Scan(this, fs, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // BZip2
                        if (new BZip2().ShouldScan(magic))
                        {
                            var subProtections = new BZip2().Scan(this, fs, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // GZIP
                        if (new GZIP().ShouldScan(magic))
                        {
                            var subProtections = new GZIP().Scan(this, fs, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // InstallShield Archive V3 (Z)
                        if (file != null && new InstallShieldArchiveV3().ShouldScan(magic))
                        {
                            var subProtections = new InstallShieldArchiveV3().Scan(this, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // InstallShield Cabinet
                        if (file != null && new InstallShieldCAB().ShouldScan(magic))
                        {
                            var subProtections = new InstallShieldCAB().Scan(this, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // Microsoft Cabinet
                        if (file != null && new MicrosoftCAB().ShouldScan(magic))
                        {
                            var subProtections = new MicrosoftCAB().Scan(this, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // MSI
                        if (file != null && new MSI().ShouldScan(magic))
                        {
                            var subProtections = new MSI().Scan(this, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // MPQ archive
                        if (file != null && new MPQ().ShouldScan(magic))
                        {
                            var subProtections = new MPQ().Scan(this, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // PKZIP archive (and derivatives)
                        if (new PKZIP().ShouldScan(magic))
                        {
                            var subProtections = new PKZIP().Scan(this, fs, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // RAR archive
                        if (new RAR().ShouldScan(magic))
                        {
                            var subProtections = new RAR().Scan(this, fs, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // Tape Archive
                        if (new TapeArchive().ShouldScan(magic))
                        {
                            var subProtections = new TapeArchive().Scan(this, fs, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // Valve archive formats
                        if (file != null && new Valve().ShouldScan(magic))
                        {
                            var subProtections = new Valve().Scan(this, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }

                        // XZ
                        if (new XZ().ShouldScan(magic))
                        {
                            var subProtections = new XZ().Scan(this, fs, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Utilities.AppendToDictionary(protections, file, "[Exception opening file, please try again]");
            }
            

            // Clear out any empty keys
            Utilities.ClearEmptyKeys(protections);

            return protections;
        }
    
        /// <summary>
        /// Initialize all IPathCheck implementations
        /// </summary>
        private static IEnumerable<IPathCheck> InitPathCheckClasses()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && t.GetInterface(nameof(IPathCheck)) != null)
                .Select(t => Activator.CreateInstance(t) as IPathCheck);
        }
    }
}
