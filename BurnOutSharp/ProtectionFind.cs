//this file is part of BurnOut
//Copyright (C)2005-2010 Gernot Knippen
//Ported code with augments Copyright (C)2018 Matt Nadareski
//
//This program is free software; you can redistribute it and/or
//modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation; either
//version 2 of the License, or (at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU General Public License for more details.
//
//You can get a copy of the GNU General Public License
//by writing to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BurnOutSharp.FileType;
using BurnOutSharp.ProtectionType;

namespace BurnOutSharp
{
    public static class ProtectionFind
    {
        /// <summary>
        /// Scan a path to find any known copy protection(s)
        /// </summary>
        /// <param name="path">Path to scan for protection(s)</param>
        /// <param name="progress">Optional progress indicator that will return a float in the range from 0 to 1</param>
        /// <returns>Dictionary of filename to protection mappings, if possible</returns>
        public static Dictionary<string, string> Scan(string path, IProgress<FileProtection> progress = null)
        {
            var protections = new Dictionary<string, string>();

            // Checkpoint
            progress?.Report(new FileProtection(null, 0, null));

            // If we have a file
            if (File.Exists(path))
            {
                // Try using just the file first to get protection info
                string fileProtection = ScanPath(path, false);
                if (!string.IsNullOrWhiteSpace(fileProtection))
                    protections[path] = fileProtection;

                // Now check to see if the file contains any additional information
                string contentProtection = ScanContent(path)?.Replace("" + (char)0x00, "");
                if (!string.IsNullOrWhiteSpace(contentProtection))
                {
                    if (protections.ContainsKey(path))
                        protections[path] += $", {contentProtection}";
                    else
                        protections[path] = contentProtection;
                }

                // Checkpoint
                progress?.Report(new FileProtection(path, 1, contentProtection));
            }
            // If we have a directory
            else if (Directory.Exists(path))
            {
                // Get the lists of files to be used
                var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);

                // Try using just the path first to get protection info
                string pathProtection = ScanPath(path, true);
                if (!string.IsNullOrWhiteSpace(pathProtection))
                    protections[path] = pathProtection;

                // Loop through all files and scan their contents
                for (int i = 0; i < files.Count(); i++)
                {
                    // Get the current file
                    string file = files.ElementAt(i);

                    // Try using just the file first to get protection info
                    string fileProtection = ScanPath(file, false);
                    if (!string.IsNullOrWhiteSpace(fileProtection))
                        protections[file] = fileProtection;

                    // Now check to see if the file contains any additional information
                    string contentProtection = ScanContent(file)?.Replace("" + (char)0x00, "");
                    if (!string.IsNullOrWhiteSpace(contentProtection))
                    {
                        if (protections.ContainsKey(file))
                            protections[file] += $", {contentProtection}";
                        else
                            protections[file] = contentProtection;
                    }

                    // Checkpoint
                    progress?.Report(new FileProtection(file, i / (float)files.Count(), contentProtection));
                }
            }

            // If we have an empty list, we need to take care of that
            if (protections.Count(p => !string.IsNullOrWhiteSpace(p.Value)) == 0)
            {
                protections = new Dictionary<string, string>();
            }

            return protections;
        }

        /// <summary>
        /// Scan a path for indications of copy protection
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isDirectory"></param>
        /// <returns></returns>
        public static string ScanPath(string path, bool isDirectory)
        {
            List<string> protections = new List<string>();
            string protection;

            // If we have a directory, get the files in the directory for searching
            IEnumerable<string> files = null;
            if (isDirectory)
                files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);

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

            // Now combine any found protections, or null if empty
            if (protections.Count() == 0)
                return null;
            else
                return string.Join(", ", protections);
        }

        /// <summary>
        /// Scan an individual file for copy protection
        /// </summary>
        /// <param name="file">File path for scanning</param>
        public static string ScanContent(string file)
        {
            using (FileStream fs = File.OpenRead(file))
            {
                return ScanContent(fs, file);
            }
        }

        /// <summary>
        /// Scan an individual stream for copy protection
        /// </summary>
        /// <param name="stream">Generic stream to scan</param>
        /// <param name="file">File path to be used for name checks (optional)</param>
        public static string ScanContent(Stream stream, string file = null)
        {
            // Get the extension for certain checks
            string extension = Path.GetExtension(file).ToLower().TrimStart('.');

            // Assume the first part of the stream is the start of a file
            byte[] magic = new byte[16];
            try
            {
                stream.Read(magic, 0, 16);
                stream.Seek(-16, SeekOrigin.Current);
            }
            catch
            {
                // We don't care what the issue was, we can't read or seek the file
                return null;
            }

            // Files can be protected in multiple ways
            List<string> protections = new List<string>();

            // 7-Zip archive
            if (SevenZip.ShouldScan(magic))
                protections.AddRange(SevenZip.Scan(stream));

            // BFPK archive
            if (BFPK.ShouldScan(magic))
                protections.AddRange(BFPK.Scan(stream));

            // BZip2
            if (BZip2.ShouldScan(magic))
                protections.AddRange(BZip2.Scan(stream));

            // Executable
            if (Executable.ShouldScan(magic))
                protections.AddRange(Executable.Scan(stream, file));

            // GZIP
            if (GZIP.ShouldScan(magic))
                protections.AddRange(GZIP.Scan(stream));

            // InstallShield Cabinet
            if (file != null && InstallShieldCAB.ShouldScan(magic))
                protections.AddRange(InstallShieldCAB.Scan(file));

            // Microsoft Cabinet
            if (file != null && MicrosoftCAB.ShouldScan(magic))
                protections.AddRange(MicrosoftCAB.Scan(file));

            // MPQ archive
            if (file != null && MPQ.ShouldScan(magic))
                protections.AddRange(MPQ.Scan(file));

            // PKZIP archive (and derivatives)
            if (PKZIP.ShouldScan(magic))
                protections.AddRange(PKZIP.Scan(stream));

            // RAR archive
            if (RAR.ShouldScan(magic))
                protections.AddRange(RAR.Scan(stream));

            // Tape Archive
            if (TapeArchive.ShouldScan(magic))
                protections.AddRange(TapeArchive.Scan(stream));

            // Text-based files
            if (Textfile.ShouldScan(magic, extension))
                protections.AddRange(Textfile.Scan(stream));

            // Valve archive formats
            if (file != null && Valve.ShouldScan(magic))
                protections.AddRange(Valve.Scan(file));

            // XZ
            if (XZ.ShouldScan(magic))
                protections.AddRange(XZ.Scan(stream));

            // Return blank if nothing found, or comma-separated list of protections
            if (protections.Count() == 0)
                return string.Empty;
            else
                return string.Join(", ", protections);
        }

        /// <summary>
        /// Scan a disc sector by sector for protection
        /// </summary>
        /// <remarks>
        /// https://stackoverflow.com/questions/8819188/c-sharp-classes-to-undelete-files/8820157#8820157
        /// TODO: Finish implementation
        /// </remarks>
        private static string ScanSectors(char driveLetter, int sectorsize)
        {
            string fsName = Utilities.GetFileSystemName(driveLetter);

            // Gets a handle to the physical disk
            IntPtr hDisk = Utilities.CreateFile($"\\\\.\\{driveLetter}:",
                FileAccess.Read,
                FileShare.ReadWrite,
                IntPtr.Zero,
                FileMode.Open,
                0,
                IntPtr.Zero);

            // If we have a good pointer
            if (hDisk.ToInt32() != -1)
            {
                // Setup vars
                byte[] buffer = new byte[sectorsize];
                IntPtr pt = IntPtr.Zero;
                NativeOverlapped no = new NativeOverlapped();

                // Set initial offset
                Utilities.SetFilePointerEx(
                    hDisk,
                    0,
                    ref pt,
                    Utilities.FileBegin);

                // Read a whole sector
                while (true)
                {
                    buffer = new byte[sectorsize];
                    Utilities.ReadFileEx(
                        hDisk,
                        buffer,
                        (uint)sectorsize,
                        ref no,
                        null);

                    Utilities.SetFilePointerEx(
                        hDisk,
                        sectorsize,
                        ref pt,
                        Utilities.FileCurrent);
                }
            }

            Utilities.CloseHandle(hDisk);

            return null;
        }
    }
}
