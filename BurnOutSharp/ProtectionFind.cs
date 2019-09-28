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
using System.Text;
using System.Threading;
using BurnOutSharp.ProtectionType;
using LibMSPackN;
using UnshieldSharp;

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
                // Try using just the path first to get protection info
                string pathProtection = ScanPath(path, false);
                if (!string.IsNullOrWhiteSpace(pathProtection))
                    protections[path] = pathProtection;

                // Now check to see if the file contains any additional information
                string contentProtection = ScanInFile(path)?.Replace("" + (char)0x00, "");
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

                    // Now check to see if the file contains any additional information
                    string contentProtection = ScanInFile(file)?.Replace("" + (char)0x00, "");
                    if (!string.IsNullOrWhiteSpace(contentProtection))
                        protections[file] = contentProtection;

                    // Checkpoint
                    progress?.Report(new FileProtection(file, i / files.Count(), contentProtection));
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
        private static string ScanPath(string path, bool isDirectory)
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

            // PSX Anti-modchip
            protection = PSXAntiModchip.CheckPath(path, files, isDirectory);
            if (!string.IsNullOrWhiteSpace(protection))
                protections.Add(protection);

            // SafeCast
            protection = SafeCast.CheckPath(path, files, isDirectory);
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
        private static string ScanInFile(string file)
        {
            // Get the extension for certain checks
            string extension = Path.GetExtension(file).ToLower().TrimStart('.');

            // Read the first 8 bytes to get the file type
            string magic = "";
            try
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
                {
                    magic = new string(br.ReadChars(8));
                }
            }
            catch
            {
                // We don't care what the issue was, we can't open the file
                return null;
            }

            // Files can be protected in multiple ways
            List<string> protections = new List<string>();

            #region Executable Content Checks

            // Windows Executable and DLL
            if (magic.StartsWith("MZ"))
            {
                try
                {
                    // Load the current file and check for specialty strings first
                    StreamReader sr = new StreamReader(file, Encoding.Default);
                    string protection;
                    string fileContent = sr.ReadToEnd();
                    sr.Close();

                    // 3PLock
                    protection = ThreePLock.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // ActiveMARK
                    protection = ActiveMARK.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // Alpha-ROM
                    protection = AlphaROM.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // Armadillo
                    protection = Armadillo.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // CD-Cops
                    protection = CDCops.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // CD-Lock
                    protection = CDLock.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // CDSHiELD SE
                    protection = CDSHiELDSE.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // CD Check
                    protection = CDCheck.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // Cenega ProtectDVD
                    protection = CengaProtectDVD.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // Code Lock
                    protection = CodeLock.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // CopyKiller
                    protection = CopyKiller.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // Cucko (EA Custom)
                    protection = Cucko.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // dotFuscator
                    protection = dotFuscator.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // DVD-Cops
                    protection = DVDCops.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // EA CdKey Registration Module
                    protection = EACdKey.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // EXE Stealth
                    protection = EXEStealth.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // Games for Windows - Live
                    protection = GFWL.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // Impulse Reactor
                    protection = ImpulseReactor.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // Inno Setup
                    protection = InnoSetup.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // JoWooD X-Prot
                    protection = JoWooDXProt.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // Key-Lock (Dongle)
                    protection = KeyLock.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // LaserLock
                    protection = LaserLock.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // PE Compact
                    protection = PECompact.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // ProtectDisc
                    protection = ProtectDisc.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // Ring PROTECH
                    protection = RingPROTECH.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // SafeDisc / SafeCast
                    protection = SafeDisc.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // SafeLock
                    protection = SafeLock.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // SecuROM
                    protection = SecuROM.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // SmartE
                    protection = SmartE.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // SolidShield
                    protection = SolidShield.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // StarForce
                    protection = StarForce.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // SVK Protector
                    protection = SVKProtector.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // Sysiphus / Sysiphus DVD
                    protection = Sysiphus.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // TAGES
                    protection = Tages.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // VOB ProtectCD/DVD
                    protection = VOBProtectCDDVD.CheckContents(file, fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // WTM CD Protect
                    protection = WTMCDProtect.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);

                    // Xtreme-Protector
                    protection = XtremeProtector.CheckContents(fileContent);
                    if (!string.IsNullOrWhiteSpace(protection))
                        protections.Add(protection);
                }
                catch { }
            }

            #endregion

            #region Textfile Content Checks

            if (magic.StartsWith("{\rtf") // Rich Text File
                || magic.StartsWith("" + (char)0xd0 + (char)0xcf + (char)0x11 + (char)0xe0 + (char)0xa1 + (char)0xb1 + (char)0x1a + (char)0xe1) // Microsoft Office File (old)
                || extension == "txt") // Generic textfile (no header)
            {
                try
                {
                    StreamReader sr = File.OpenText(file);
                    string FileContent = sr.ReadToEnd().ToLower();
                    sr.Close();

                    // CD-Key
                    if (FileContent.Contains("a valid serial number is required")
                        || FileContent.Contains("serial number is located"))
                    {
                        protections.Add("CD-Key / Serial");
                    }
                }
                catch
                {
                    // We don't care what the error was
                }
                // No-op
            }

            #endregion

            #region Archive Content Checks

            // 7-zip
            if (magic.StartsWith("7z" + (char)0xbc + (char)0xaf + (char)0x27 + (char)0x1c))
            {
                // No-op
            }

            // InstallShield CAB
            else if (magic.StartsWith("ISc"))
            {
                try
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    Directory.CreateDirectory(tempPath);

                    UnshieldCabinet cabfile = UnshieldCabinet.Open(file);
                    for (int i = 0; i < cabfile.FileCount; i++)
                    {
                        string tempFileName = Path.Combine(tempPath, cabfile.FileName(i));
                        if (cabfile.FileSave(i, tempFileName))
                        {
                            string protection = ScanInFile(tempFileName);
                            try
                            {
                                File.Delete(tempFileName);
                            }
                            catch { }

                            if (!string.IsNullOrEmpty(protection))
                                protections.Add(protection);
                        }
                    }

                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch { }
                }
                catch { }
            }

            // Microsoft CAB
            else if (magic.StartsWith("MSCF"))
            {
                try
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    Directory.CreateDirectory(tempPath);

                    using (MSCabinet cabfile = new MSCabinet(file))
                    {
                        foreach (var sub in cabfile.GetFiles())
                        {
                            string tempfile = Path.Combine(tempPath, sub.Filename);
                            sub.ExtractTo(tempfile);
                            string protection = ScanInFile(tempfile);
                            File.Delete(tempfile);

                            if (!string.IsNullOrEmpty(protection))
                                protections.Add(protection);
                        }

                        try
                        {
                            Directory.Delete(tempPath, true);
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // PKZIP
            else if (magic.StartsWith("PK" + (char)03 + (char)04)
                || magic.StartsWith("PK" + (char)05 + (char)06)
                || magic.StartsWith("PK" + (char)07 + (char)08))
            {
                // No-op
            }

            // RAR
            else if (magic.StartsWith("Rar!"))
            {
                // No-op
            }

            #endregion

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
