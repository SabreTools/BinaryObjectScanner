using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.FileType;
using BurnOutSharp.ProtectionType;

namespace BurnOutSharp
{
    public class Scanner
    {
        /// <summary>
        /// Optional progress callback during scanning
        /// </summary>
        public IProgress<ProtectionProgress> FileProgress { get; set; } = null;

        /// <summary>
        /// Determines whether the byte position of found protection is included or not
        /// </summary>
        public bool IncludePosition { get; set; } = false;

        /// <summary>
        /// Determines whether all files are scanned or just executables are
        /// </summary>
        public bool ScanAllFiles { get; set; } = false;

        /// <summary>
        /// Determines whether archives are decompressed and scanned
        /// </summary>
        public bool ScanArchives { get; set; } = true;

        /// <summary>
        /// Determines if packers are counted as detected protections or not
        /// </summary>
        public bool ScanPackers { get; set; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileProgress">Optional progress callback</param>
        public Scanner(IProgress<ProtectionProgress> fileProgress = null)
        {
            FileProgress = fileProgress;
        }

        /// <summary>
        /// Scan a single path and get all found protections
        /// </summary>
        /// <param name="path">Path to scan</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        public Dictionary<string, List<string>> GetProtections(string path)
        {
            return GetProtections(new List<string> { path });
        }

        /// <summary>
        /// Scan the list of paths and get all found protections
        /// </summary>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        public Dictionary<string, List<string>> GetProtections(List<string> paths)
        {
            // If we have no paths, we can't scan
            if (paths == null || !paths.Any())
                return null;

            // Checkpoint
            FileProgress?.Report(new ProtectionProgress(null, 0, null));

            // Temp variables for reporting
            string tempFilePath = Path.GetTempPath();
            string tempFilePathWithGuid = Path.Combine(tempFilePath, Guid.NewGuid().ToString());

            // Loop through each path and get the returned values
            var protections = new Dictionary<string, List<string>>();
            foreach (string path in paths)
            {
                // Directories scan each internal file individually
                if (Directory.Exists(path))
                {
                    // Enumerate all files at first for easier access
                    var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).ToList();

                    // Scan for path-detectable protections
                    var directoryPathProtections = GetPathProtections(path, files);
                    Utilities.AppendToDictionary(protections, directoryPathProtections);

                    // Scan each file in directory separately
                    for (int i = 0; i < files.Count(); i++)
                    {
                        // Get the current file
                        string file = files.ElementAt(i);

                        // Get the reportable file name
                        string reportableFileName = file;
                        if (reportableFileName.StartsWith(tempFilePath))
                            reportableFileName = reportableFileName.Substring(tempFilePathWithGuid.Length);

                        // Checkpoint
                        FileProgress?.Report(new ProtectionProgress(reportableFileName, i / (float)files.Count(), "Checking file" + (file != reportableFileName ? " from archive" : string.Empty)));

                        // Scan for path-detectable protections
                        var filePathProtections = GetPathProtections(file);
                        Utilities.AppendToDictionary(protections, filePathProtections);

                        // Scan for content-detectable protections
                        var fileProtections = GetInternalProtections(file);
                        if (fileProtections != null && fileProtections.Any())
                        {
                            foreach (string key in fileProtections.Keys)
                            {
                                if (!protections.ContainsKey(key))
                                    protections[key] = new List<string>();

                                protections[key].AddRange(fileProtections[key]);
                            }
                        }

                        // Checkpoint
                        protections.TryGetValue(file, out List<string> fullProtectionList);
                        string fullProtection = (fullProtectionList != null && fullProtectionList.Any() ? string.Join(", ", fullProtectionList) : null);
                        FileProgress?.Report(new ProtectionProgress(reportableFileName, (i + 1) / (float)files.Count(), fullProtection ?? string.Empty));
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
                    var filePathProtections = GetPathProtections(path);
                    Utilities.AppendToDictionary(protections, filePathProtections);

                    // Scan for content-detectable protections
                    var fileProtections = GetInternalProtections(path);
                    if (fileProtections != null && fileProtections.Any())
                    {
                        foreach (string key in fileProtections.Keys)
                        {
                            if (!protections.ContainsKey(key))
                                protections[key] = new List<string>();

                            protections[key].AddRange(fileProtections[key]);
                        }
                    }

                    // Checkpoint
                    protections.TryGetValue(path, out List<string> fullProtectionList);
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

            return protections;
        }

        /// <summary>
        /// Get the path-detectable protections associated with a single path
        /// </summary>
        /// <param name="path">Path of the directory or file to scan</param>
        /// <param name="files">Files contained within if the path is a directory</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private Dictionary<string, List<string>> GetPathProtections(string path, List<string> files = null)
        {
            List<string> protections = new List<string>();
            string protection;

            // If we have a directory, get the files in the directory for searching
            bool isDirectory = false;
            if (Directory.Exists(path))
                isDirectory = true;

            // AACS
            protection = new AACS().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Alpha-DVD
            protection = new AlphaDVD().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Bitpool
            protection = new Bitpool().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // ByteShield
            protection = new ByteShield().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Cactus Data Shield
            protection = new CactusDataShield().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CD-Cops
            protection = new CDCops().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CD-Lock
            protection = new CDLock().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CD-Protector
            protection = new CDProtector().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CD-X
            protection = new CDX().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            /*
            // CopyKiller
            protection = new CopyKiller().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);
            */

            // DiscGuard
            protection = new DiscGuard().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // DVD Crypt
            protection = new DVDCrypt().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // DVD-Movie-PROTECT
            protection = new DVDMoviePROTECT().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // FreeLock
            protection = new FreeLock().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Games for Windows - Live
            protection = new GFWL().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Hexalock AutoLock
            protection = new HexalockAutoLock().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Impulse Reactor
            protection = new ImpulseReactor().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // IndyVCD
            protection = new IndyVCD().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Key2Audio XS
            protection = new Key2AudioXS().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // LaserLock
            protection = new LaserLock().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // MediaCloQ
            protection = new MediaCloQ().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // MediaMax CD3
            protection = new MediaMaxCD3().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Origin
            protection = new Origin().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Protect DVD-Video
            protection = new ProtectDVDVideo().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SafeCast
            protection = new SafeCast().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SafeDisc
            protection = new SafeDisc().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SafeDisc Lite
            protection = new SafeDiscLite().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SafeLock
            protection = new SafeLock().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SecuROM
            protection = new SecuROM().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SmartE
            protection = new SmartE().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SoftLock
            protection = new SoftLock().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SolidShield
            protection = new SolidShield().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // StarForce
            protection = new StarForce().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Steam
            protection = new Steam().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // TAGES
            protection = new Tages().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // TZCopyProtector
            protection = new TZCopyProtector().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Uplay
            protection = new Uplay().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // VOB ProtectCD/DVD
            protection = new VOBProtectCDDVD().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Winlock
            protection = new Winlock().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // WTM CD Protect
            protection = new WTMCDProtect().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // XCP
            protection = new XCP().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Zzxzz
            protection = new Zzxzz().CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Create and return the dictionary
            return new Dictionary<string, List<string>>
            {
                [path] = protections
            };
        }

        /// <summary>
        /// Get the content-detectable protections associated with a single path
        /// </summary>
        /// <param name="file">Path to the file to scan</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private Dictionary<string, List<string>> GetInternalProtections(string file)
        {
            // Quick sanity check before continuing
            if (!File.Exists(file))
                return null;

            // Initialze the protections found
            var protections = new Dictionary<string, List<string>>();

            // Get the extension for certain checks
            string extension = Path.GetExtension(file).ToLower().TrimStart('.');

            // Open the file and begin scanning
            using (FileStream fs = File.OpenRead(file))
            {
                // Get the first 16 bytes for matching
                byte[] magic = new byte[16];
                try
                {
                    fs.Read(magic, 0, 16);
                    fs.Seek(-16, SeekOrigin.Current);
                }
                catch
                {
                    // We don't care what the issue was, we can't read or seek the file
                    return null;
                }

                #region Non-Archive File Types

                // Executable
                if (ScanAllFiles || new Executable().ShouldScan(magic))
                {
                    var subProtections = new Executable().Scan(this, fs, file);
                    Utilities.AppendToDictionary(protections, subProtections);
                }

                // Text-based files
                if (ScanAllFiles || new Textfile().ShouldScan(magic, extension))
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

            // Clear out any empty keys
            Utilities.ClearEmptyKeys(protections);

            return protections;
        }
    }
}
