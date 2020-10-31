using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.FileType;
using BurnOutSharp.ProtectionType;

namespace BurnOutSharp
{
    // TODO: Use the file progress everywhere
    // TODO: Re-enable direct stream scanning
    // TODO: Should FileTypes be exposed directly as well so the scans can be exposed easier?
    public class Scanner
    {
        /// <summary>
        /// Optional progress callback during scanning
        /// </summary>
        public IProgress<FileProtection> FileProgress { get; set; } = null;

        /// <summary>
        /// List of paths that will be scanned with this object
        /// </summary>
        public List<string> Paths { get; set; } = new List<string>();

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
        /// Constructor
        /// </summary>
        /// <param name="path">Path to create a scanner for</param>
        /// <param name="fileProgress">Optional progress callback</param>
        public Scanner(string path, IProgress<FileProtection> fileProgress = null)
        {
            Paths = new List<string> { path };
            FileProgress = fileProgress;
        }

        /// <summary>
        /// Scan the list of paths and get all found protections
        /// </summary>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        public Dictionary<string, List<string>> GetProtections()
        {
            // If we have no paths, we can't scan
            if (Paths == null || !Paths.Any())
                return null;

            // Loop through each path and get the returned values
            var protections = new Dictionary<string, List<string>>();
            foreach (string path in Paths)
            {
                // Directories scan each internal file individually
                if (Directory.Exists(path))
                {
                    // Enumerate all files at first for easier access
                    var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).ToList();

                    // Scan for path-detectable protections
                    var directoryPathProtections = GetPathProtections(path, files);
                    if (directoryPathProtections != null && directoryPathProtections.Any())
                    {
                        foreach (string key in directoryPathProtections.Keys)
                        {
                            if (!protections.ContainsKey(key))
                                protections[key] = new List<string>();

                            protections[key].AddRange(directoryPathProtections[key]);
                        }
                    }

                    // Scan each file in directory separately
                    foreach (string file in files)
                    {
                        // Scan for path-detectable protections
                        var filePathProtections = GetPathProtections(file);
                        if (filePathProtections != null && filePathProtections.Any())
                        {
                            foreach (string key in filePathProtections.Keys)
                            {
                                if (!protections.ContainsKey(key))
                                    protections[key] = new List<string>();

                                protections[key].AddRange(filePathProtections[key]);
                            }
                        }

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
                    }    
                }

                // Scan a single file by itself
                else if (File.Exists(path))
                {
                    // Scan for path-detectable protections
                    var filePathProtections = GetPathProtections(path);
                    if (filePathProtections != null && filePathProtections.Any())
                    {
                        foreach (string key in filePathProtections.Keys)
                        {
                            if (!protections.ContainsKey(key))
                                protections[key] = new List<string>();

                            protections[key].AddRange(filePathProtections[key]);
                        }
                    }

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
                }

                // Throw on an invalid path
                else
                {
                    throw new FileNotFoundException($"{path} is not a directory or file, skipping...");
                }
            }

            return protections;
        }

        /// <summary>
        /// Get the path-detectable protections associated with a single path
        /// </summary>
        /// <param name="path">Path of the directory or file to scan</param>
        /// <param name="files">Files contained within if the path is a directory</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        public Dictionary<string, List<string>> GetPathProtections(string path, List<string> files = null)
        {
            List<string> protections = new List<string>();
            string protection;

            // If we have a directory, get the files in the directory for searching
            bool isDirectory = false;
            if (Directory.Exists(path))
                isDirectory = true;

            // AACS
            protection = AACS.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Alpha-DVD
            protection = AlphaDVD.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Bitpool
            protection = Bitpool.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // ByteShield
            protection = ByteShield.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Cactus Data Shield
            protection = CactusDataShield.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CD-Cops
            protection = CDCops.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CD-Lock
            protection = CDLock.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CD-Protector
            protection = CDProtector.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // CD-X
            protection = CDX.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            /*
            // CopyKiller
            protection = CopyKiller.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);
            */

            // DiscGuard
            protection = DiscGuard.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // DVD Crypt
            protection = DVDCrypt.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // DVD-Movie-PROTECT
            protection = DVDMoviePROTECT.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // FreeLock
            protection = FreeLock.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Games for Windows - Live
            protection = GFWL.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Hexalock AutoLock
            protection = HexalockAutoLock.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Impulse Reactor
            protection = ImpulseReactor.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // IndyVCD
            protection = IndyVCD.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Key2Audio XS
            protection = Key2AudioXS.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // LaserLock
            protection = LaserLock.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // MediaCloQ
            protection = MediaCloQ.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // MediaMax CD3
            protection = MediaMaxCD3.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Origin
            protection = Origin.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Protect DVD-Video
            protection = ProtectDVDVideo.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SafeCast
            protection = SafeCast.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SafeDisc
            protection = SafeDisc.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SafeDisc Lite
            protection = SafeDiscLite.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SafeLock
            protection = SafeLock.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SecuROM
            protection = SecuROM.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SmartE
            protection = SmartE.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SoftLock
            protection = SoftLock.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SolidShield
            protection = SolidShield.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // StarForce
            protection = StarForce.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Steam
            protection = Steam.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // TAGES
            protection = Tages.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // TZCopyProtector
            protection = TZCopyProtector.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Uplay
            protection = Uplay.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // VOB ProtectCD/DVD
            protection = VOBProtectCDDVD.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Winlock
            protection = Winlock.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // WTM CD Protect
            protection = WTMCDProtect.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // XCP
            protection = XCP.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // Zzxzz
            protection = Zzxzz.CheckPath(path, isDirectory);
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
                if (ScanAllFiles || Executable.ShouldScan(magic))
                {
                    var subProtections = Executable.Scan(fs, file, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // Text-based files
                if (ScanAllFiles || Textfile.ShouldScan(magic, extension))
                {
                    var subProtections = Executable.Scan(fs, file, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                #endregion

                #region Archive File Types

                // 7-Zip archive
                if (SevenZip.ShouldScan(magic))
                {
                    var subProtections = SevenZip.Scan(fs, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // BFPK archive
                if (BFPK.ShouldScan(magic))
                {
                    var subProtections = BFPK.Scan(fs, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // BZip2
                if (BZip2.ShouldScan(magic))
                {
                    var subProtections = BZip2.Scan(fs, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // GZIP
                if (GZIP.ShouldScan(magic))
                {
                    var subProtections = GZIP.Scan(fs, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // InstallShield Cabinet
                if (file != null && InstallShieldCAB.ShouldScan(magic))
                {
                    var subProtections = InstallShieldCAB.Scan(file, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // Microsoft Cabinet
                if (file != null && MicrosoftCAB.ShouldScan(magic))
                {
                    var subProtections = MicrosoftCAB.Scan(file, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // MSI
                if (file != null && MSI.ShouldScan(magic))
                {
                    var subProtections = MSI.Scan(file, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // MPQ archive
                if (file != null && MPQ.ShouldScan(magic))
                {
                    var subProtections = MPQ.Scan(file, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // PKZIP archive (and derivatives)
                if (PKZIP.ShouldScan(magic))
                {
                    var subProtections = PKZIP.Scan(fs, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // RAR archive
                if (RAR.ShouldScan(magic))
                {
                    var subProtections = RAR.Scan(fs, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // Tape Archive
                if (TapeArchive.ShouldScan(magic))
                {
                    var subProtections = TapeArchive.Scan(fs, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // Valve archive formats
                if (file != null && Valve.ShouldScan(magic))
                {
                    var subProtections = Valve.Scan(file, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                // XZ
                if (XZ.ShouldScan(magic))
                {
                    var subProtections = XZ.Scan(fs, IncludePosition);
                    if (!protections.ContainsKey(file))
                        protections[file] = new List<string>();

                    protections[file] = subProtections;
                }

                #endregion
            }

            return protections;
        }
    }
}
