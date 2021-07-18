using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Matching;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

namespace BurnOutSharp.PackerType
{
    public class WinZipSFX : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<ContentMatchSet>
            {
                // WinZip Self-Extractor
                new ContentMatchSet(new byte?[]
                {
                    0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70, 0x20, 0x53,
                    0x65, 0x6C, 0x66, 0x2D, 0x45, 0x78, 0x74, 0x72,
                    0x61, 0x63, 0x74, 0x6F, 0x72
                }, GetVersion, "WinZip SFX"),

                // _winzip_
                new ContentMatchSet(new byte?[] { 0x5F, 0x77, 0x69, 0x6E, 0x7A, 0x69, 0x70, 0x5F }, GetVersion, "WinZip SFX"),
            };

            return MatchUtil.GetFirstMatch(file, fileContent, matchers, includePosition);
        }

        // TODO: Find a way to generically detect 2.X versions and improve exact version detection for SFX PE versions bundled with WinZip 11+

        /// <inheritdoc/>
        public ConcurrentDictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the zip file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Should be using stream instead of file, but stream fails to extract anything. My guess is that the executable portion of the archive is causing stream to fail, but not file.
                using (ZipArchive zipFile = ZipArchive.Open(file))
                {
                    foreach (var entry in zipFile.Entries)
                    {
                        // If an individual entry fails
                        try
                        {
                            // If we have a directory, skip it
                            if (entry.IsDirectory)
                                continue;

                            string tempFile = Path.Combine(tempPath, entry.Key);
                            entry.WriteToFile(tempFile);
                        }
                        catch { }
                    }
                }

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch { }

                // Remove temporary path references
                Utilities.StripFromKeys(protections, tempPath);

                return protections;
            }
            catch { }

            return null;
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            // Check the manifest version first
            string version = Utilities.GetManifestVersion(fileContent);
            string description = Utilities.GetManifestDescription(fileContent);
            if (description != "WinZip Self-Extractor")
                return GetV2Version(file, fileContent);
            if (!string.IsNullOrEmpty(version))
                return GetV3PlusVersion(file, fileContent, version);
            
            // Assume an earlier version
            return GetV2Version(file, fileContent);
        }

        private static string GetV2Version(string file, byte[] fileContent)
        {
            var matchers = new List<ContentMatchSet>
            {
                #region 16-bit NE Header Checks

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0x86, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                    0x00, 0x20, 0x00, 0x40, 0xE6, 0x2B, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                    0x64, 0x00, 0x6C, 0x00, 0xB8, 0x44, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "2.0 (MS-DOS/16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0x86, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                    0x00, 0x20, 0x00, 0x40, 0x74, 0x31, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                    0x64, 0x00, 0x6C, 0x00, 0x98, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "2.0 (16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0x80, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                    0x00, 0x20, 0x00, 0x40, 0xA0, 0x24, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03, 0x00,
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                    0x64, 0x00, 0x6A, 0x00, 0x92, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "Compact 2.0 (16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0xCD, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x03, 0x00,
                    0x00, 0x20, 0x00, 0x40, 0xFA, 0x36, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x05, 0x00,
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x97, 0x00,
                    0xA3, 0x00, 0xAD, 0x00, 0xDF, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "Software Installation 2.0 (16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0x86, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                    0x00, 0x20, 0x00, 0x40, 0x86, 0x33, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                    0x64, 0x00, 0x6C, 0x00, 0xC8, 0x43, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "2.1 RC2 (MS-DOS/16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0xBE, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x03, 0x00,
                    0x00, 0x20, 0x00, 0x40, 0x56, 0x3E, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x90, 0x00,
                    0x9C, 0x00, 0xA4, 0x00, 0xD0, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "2.1 RC2 (16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0x80, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                    0x00, 0x20, 0x00, 0x40, 0x84, 0x2B, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03, 0x00,
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                    0x64, 0x00, 0x6A, 0x00, 0x92, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "Compact 2.1 RC2 (16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0xBE, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x03, 0x00,
                    0x00, 0x20, 0x00, 0x40, 0xAC, 0x43, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x90, 0x00,
                    0x9C, 0x00, 0xA4, 0x00, 0xD0, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "Software Installation 2.1 RC2 (16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0x86, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                    0x00, 0x20, 0x00, 0x3A, 0x96, 0x33, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                    0x64, 0x00, 0x6C, 0x00, 0xC8, 0x43, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "2.1 (MS-DOS/16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0xBE, 0x00, 0x02, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x03, 0x00,
                    0x00, 0x20, 0x00, 0x3A, 0x7E, 0x3E, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x90, 0x00,
                    0x9C, 0x00, 0xA4, 0x00, 0xD0, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "2.1 (16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0x80, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                    0x00, 0x20, 0x00, 0x3A, 0x90, 0x2B, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03, 0x00,
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                    0x64, 0x00, 0x6A, 0x00, 0x92, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "Compact 2.1 (16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0xBE, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x03, 0x00,
                    0x00, 0x20, 0x00, 0x3A, 0x08, 0x44, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x90, 0x00,
                    0x9C, 0x00, 0xA4, 0x00, 0xD0, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "Software Installation 2.1 (16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0x86, 0x00, 0x02, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00, 
                    0x00, 0x20, 0x00, 0x40, 0x7C, 0x31, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00, 
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00, 
                    0x64, 0x00, 0x6C, 0x00, 0x98, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 
                }, "Personal Edition (16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0xBE, 0x00, 0x02, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x03, 0x00, 
                    0x00, 0x20, 0x00, 0x3C, 0x7C, 0x3E, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00, 
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x90, 0x00, 
                    0x9C, 0x00, 0xA4, 0x00, 0xD0, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 
                }, "Personal Edition 32-bit (16-bit)"),

                new ContentMatchSet(new byte?[]
                {
                    0x4E, 0x45, 0x11, 0x20, 0xC6, 0x00, 0x02, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x03, 0x00, 
                    0xDC, 0x43, 0x08, 0x27, 0xDC, 0x4A, 0x01, 0x00,
                    0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x05, 0x00, 
                    0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x90, 0x00, 
                    0x9C, 0x00, 0xA6, 0x00, 0xD8, 0x01, 0x00, 0x00,
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
                }, "Personal Edition 32-bit Build 1260/1285 (16-bit)"),

                // WZ-SE-01
                new ContentMatchSet(new byte?[]
                {
                    0x57, 0x5A, 0x2D, 0x53, 0x45, 0x2D, 0x30, 0x31
                }, "Unknow Version (16-bit)"),

                #endregion

                #region 32-bit SFX Header Checks

                // .............8�92....�P..............�P..�P..�P..VW95SE.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x38, 0x9C, 0x39,
                    0x32, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x56, 0x57, 0x39, 0x35, 0x53, 0x45, 0x2E,
                    0x53, 0x46, 0x58,
                }, "2.0 (32-bit)"),

                // .............]�92....�P..............�P..�P..�P..VW95SRE.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x9C, 0x39,
                    0x32, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x56, 0x57, 0x39, 0x35, 0x53, 0x52, 0x45,
                    0x2E, 0x53, 0x46, 0x58,
                }, "Software Installation 2.0 (32-bit)"),

                // .............���3....�P..............�P..�P..�P..VW95SE.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x84, 0x82, 0x94,
                    0x33, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x56, 0x57, 0x39, 0x35, 0x53, 0x45, 0x2E,
                    0x53, 0x46, 0x58,
                }, "2.1 RC2 (32-bit)"),

                // .............���3....�P..............�P..�P..�P..VW95SRE.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0xB0, 0x82, 0x94,
                    0x33, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x56, 0x57, 0x39, 0x35, 0x53, 0x52, 0x45,
                    0x2E, 0x53, 0x46, 0x58,
                }, "Software Installation 2.1 RC2 (32-bit)"),

                // .............U��3....�P..............�P..�P..�P..VW95SE.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x55, 0xCD, 0xCC,
                    0x33, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x56, 0x57, 0x39, 0x35, 0x53, 0x45, 0x2E,
                    0x53, 0x46, 0x58,
                }, "2.1 (32-bit)"),

                // .............{��3....�P..............�P..�P..�P..VW95SRE.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0xCD, 0xCC,
                    0x33, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x56, 0x57, 0x39, 0x35, 0x53, 0x52, 0x45,
                    0x2E, 0x53, 0x46, 0x58,
                }, "Software Installation 2.1 (32-bit)"),

                // .............ñ½;5....ˆ`..............ˆ`..ˆ`..ˆ`..SI32E.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0xF1, 0xBD, 0x3B, 
                    0x35, 0x00, 0x00, 0x00, 0x00, 0x88, 0x60, 0x00,
                    0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x60, 0x00, 
                    0x00, 0x88, 0x60, 0x00, 0x00, 0x88, 0x60, 0x00,
                    0x00, 0x53, 0x49, 0x33, 0x32, 0x45, 0x2E, 0x53, 
                    0x46, 0x58, 
                }, "Software Installation 2.2.1110 (32-bit)"),

                // .............á.^2....ˆP..............ˆP..ˆP..ˆP..VW95LE.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0xE1, 0x9D, 0x5E, 
                    0x32, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00, 
                    0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x56, 0x57, 0x39, 0x35, 0x4C, 0x45, 0x2E, 
                    0x53, 0x46, 0x58, 
                }, "Personal Edition (32-bit)"),

                // .............ïAÁ3....ˆP..............ˆP..ˆP..ˆP..VW95LE.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0xEF, 0x41, 0xC1, 
                    0x33, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00, 
                    0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                    0x00, 0x56, 0x57, 0x39, 0x35, 0x4C, 0x45, 0x2E, 
                    0x53, 0x46, 0x58, 
                }, "Personal Edition 32-bit (32-bit)"),

                // .............'..6....ˆ`..............ˆ`..ˆ`..ˆ`..PE32E.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x27, 0x0F, 0x01, 
                    0x36, 0x00, 0x00, 0x00, 0x00, 0x88, 0x60, 0x00,
                    0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x60, 0x00, 
                    0x00, 0x88, 0x60, 0x00, 0x00, 0x88, 0x60, 0x00,
                    0x00, 0x50, 0x45, 0x33, 0x32, 0x45, 0x2E, 0x53, 
                    0x46, 0x58,
                }, "Personal Edition 32-bit Build 1260 (32-bit)"),

                // .............Ó‘(6....ˆ`..............ˆ`..ˆ`..ˆ`..PE32E.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0xD3, 0x91, 0x28, 
                    0x36, 0x00, 0x00, 0x00, 0x00, 0x88, 0x60, 0x00,
                    0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x60, 0x00, 
                    0x00, 0x88, 0x60, 0x00, 0x00, 0x88, 0x60, 0x00,
                    0x00, 0x50, 0x45, 0x33, 0x32, 0x45, 0x2E, 0x53, 
                    0x46, 0x58, 
                }, "Personal Edition 32-bit Build 1285 (32-bit)"),

                // ......]ïý8....˜z..............˜z..˜z..˜z..PE32E.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x5D, 0xEF, 
                    0xFD, 0x38, 0x00, 0x00, 0x00, 0x00, 0x98, 0x7A, 
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x98, 0x7A, 
                    0x00, 0x00, 0x98, 0x7A, 0x00, 0x00, 0x98, 0x7A, 
                    0x00, 0x00, 0x50, 0x45, 0x33, 0x32, 0x45, 0x2E,
                    0x53, 0x46, 0x58, 
                }, "Personal Edition 32-bit Build 3063"),

                // ...................½û;....ˆj..............ˆj..ˆj..ˆj..PE32E.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x1F, 0xBD, 0xFB, 0x3B, 0x00, 0x00,
                    0x00, 0x00, 0x88, 0x6A, 0x00, 0x00, 0x01, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x88, 0x6A, 0x00, 0x00, 0x88, 0x6A,
                    0x00, 0x00, 0x88, 0x6A, 0x00, 0x00, 0x50, 0x45, 
                    0x33, 0x32, 0x45, 0x2E, 0x53, 0x46, 0x58,
                }, "Personal Edition 32-bit Build 4325"),

                // ................rS*@....Xƒ..............Xƒ..Xƒ..Xƒ..PE32E.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x72, 0x53, 0x2A, 0x40, 0x00, 0x00, 0x00, 0x00,
                    0x58, 0x83, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x58, 0x83, 0x00, 0x00, 0x58, 0x83, 0x00, 0x00,
                    0x58, 0x83, 0x00, 0x00, 0x50, 0x45, 0x33, 0x32, 
                    0x45, 0x2E, 0x53, 0x46, 0x58, 
                }, "Personal Edition 32-bit Build 6028"),

                // ................±.!A....Xƒ..............Xƒ..Xƒ..Xƒ..PE32E.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0xB1, 0x1A, 0x21, 0x41, 0x00, 0x00, 0x00, 0x00,
                    0x58, 0x83, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x58, 0x83, 0x00, 0x00, 0x58, 0x83, 0x00, 0x00,
                    0x58, 0x83, 0x00, 0x00, 0x50, 0x45, 0x33, 0x32, 
                    0x45, 0x2E, 0x53, 0x46, 0x58,
                }, "Personal Edition 32-bit Build 6224"),

                // ................¯D.C....x„..............x„..x„..x„..PE32E.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0xAF, 0x44, 0x0F, 0x43, 0x00, 0x00, 0x00, 0x00,
                    0x78, 0x84, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x78, 0x84, 0x00, 0x00, 0x78, 0x84, 0x00, 0x00,
                    0x78, 0x84, 0x00, 0x00, 0x50, 0x45, 0x33, 0x32, 
                    0x45, 0x2E, 0x53, 0x46,
                }, "Personal Edition 32-bit Build 6604"),

                //................·Å\C....x„..............x„..x„..x„..PE32E.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0xB7, 0xC5, 0x5C, 0x43, 0x00, 0x00, 0x00, 0x00,
                    0x78, 0x84, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x78, 0x84, 0x00, 0x00, 0x78, 0x84, 0x00, 0x00,
                    0x78, 0x84, 0x00, 0x00, 0x50, 0x45, 0x33, 0x32, 
                    0x45, 0x2E, 0x53, 0x46, 0x58, 
                }, "Personal Edition 32-bit Build 6663"),

                // VW95SE.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x56, 0x57, 0x39, 0x35, 0x53, 0x45, 0x2E, 0x53, 
                    0x46, 0x58,
                }, "Unknown Version (32-bit)"),

                // VW95SRE.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x56, 0x57, 0x39, 0x35, 0x53, 0x52, 0x45, 0x2E, 
                    0x53, 0x46, 0x58,
                }, "Unknown Version Software Installation (32-bit)"),

                // VW95LE.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x56, 0x57, 0x39, 0x35, 0x4C, 0x45, 0x2E, 0x53, 
                    0x46, 0x58, 
                }, "Unknown Version before build 1285 Personal Edition (32-bit)"),

                // PE32E.SFX
                new ContentMatchSet(new byte?[]
                {
                    0x50, 0x45, 0x33, 0x32, 0x45, 0x2E, 0x53, 0x46, 
                    0x58, 
                }, "Unknown Version after 1285 Personal Edition (32-bit)"),

                #endregion

                #region 32-bit PE Header Checks

                new ContentMatchSet(new byte?[]
                {
                    0x50, 0x45, 0x00, 0x00, 0x4C, 0x01, 0x05, 0x00, 
                    0xC9, 0x7A, 0xBE, 0x38, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0xE0, 0x00, 0x0F, 0x01,
                    0x0B, 0x01, 0x05, 0x0A, 0x00, 0x5C, 0x00, 0x00, 
                    0x00, 0x4C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x71, 0x3E, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                    0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00,
                }, "2.2.3063"),

                new ContentMatchSet(new byte?[]
                {
                    0x50, 0x45, 0x00, 0x00, 0x4C, 0x01, 0x05, 0x00,
                    0x69, 0x1B, 0x5B, 0x3A, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0xE0, 0x00, 0x0F, 0x01,
                    0x0B, 0x01, 0x05, 0x0A, 0x00, 0x4A, 0x00, 0x00,
                    0x00, 0x2A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0xD8, 0x39, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                    0x00, 0x60, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00,
                }, "2.2.4003"),

                new ContentMatchSet(new byte?[]
                {
                    0x50, 0x45, 0x00, 0x00, 0x4C, 0x01, 0x05, 0x00,
                    0x81, 0x1B, 0x5B, 0x3A, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0xE0, 0x00, 0x0F, 0x01,
                    0x0B, 0x01, 0x05, 0x0A, 0x00, 0x56, 0x00, 0x00,
                    0x00, 0x2A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x8F, 0x3F, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                    0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00,
                }, "Software Installation 2.2.4003"),

                new ContentMatchSet(new byte?[]
                {
                    0x50, 0x45, 0x00, 0x00, 0x4C, 0x01, 0x05, 0x00, 
                    0xFA, 0xB8, 0xFB, 0x3B, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0xE0, 0x00, 0x0F, 0x01,
                    0x0B, 0x01, 0x06, 0x00, 0x00, 0x60, 0x00, 0x00, 
                    0x00, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0xF0, 0x3E, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                    0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 
                    0x00, 0x10, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                }, "2.2.4325"),

                new ContentMatchSet(new byte?[]
                {
                    0x50, 0x45, 0x00, 0x00, 0x4C, 0x01, 0x05, 0x00, 
                    0xAD, 0xFC, 0x2A, 0x3D, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0xE0, 0x00, 0x0F, 0x01,
                    0x0B, 0x01, 0x07, 0x00, 0x00, 0x70, 0x00, 0x00, 
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x54, 0x45, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                    0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 
                    0x00, 0x10, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, }, "2.2.5196"),

                new ContentMatchSet(new byte?[]
                {
                    0x50, 0x45, 0x00, 0x00, 0x4C, 0x01, 0x05, 0x00, 
                    0x76, 0xF7, 0x00, 0x41, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0xE0, 0x00, 0x0F, 0x01,
                    0x0B, 0x01, 0x07, 0x00, 0x00, 0x70, 0x00, 0x00, 
                    0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x03, 0x46, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                    0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 
                }, "2.2.6202"),

                #endregion

                // WinZip Self-Extractor header corrupt.
                new ContentMatchSet(new byte?[]
                {
                    0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70, 0x20, 0x53, 
                    0x65, 0x6C, 0x66, 0x2D, 0x45, 0x78, 0x74, 0x72, 
                    0x61, 0x63, 0x74, 0x6F, 0x72, 0x20, 0x68, 0x65,
                    0x61, 0x64, 0x65, 0x72, 0x20, 0x63, 0x6F, 0x72, 
                    0x72, 0x75, 0x70, 0x74, 0x2E, 
                }, "Unknown 2.x version"),

                // winzip\shell\open\command
                new ContentMatchSet(new byte?[]
                {
                    0x77, 0x69, 0x6E, 0x7A, 0x69, 0x70, 0x5C, 0x73, 
                    0x68, 0x65, 0x6C, 0x6C, 0x5C, 0x6F, 0x70, 0x65, 
                    0x6E, 0x5C, 0x63, 0x6F, 0x6D, 0x6D, 0x61, 0x6E,
                    0x64,
                }, "Unknown 2.x version"),
            };

            string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, false);
            return match ?? null;
        }
    
        private static string GetV3PlusVersion(string file, byte[] fileContent, string version)
        {
            // Some version strings don't exactly match the public version number
            switch (version)
            {
                case "2.3.6594.0":
                    return "Personal Edition Build 6604";
                case "2.3.6602.0":
                    return "Personal Edition Build 6663";
                case "2.3.7305.0":
                    return "Personal Edition Build 7305";
                case "2.3.7382.0":
                    return "Personal Edition Build 7452+";
                case "3.0.7158.0":
                    return "3.0.7158";
                case "3.0.7454.0":
                    return "3.0.7454+";
                case "3.0.7212.0":
                    return "3.0.7212";
                case "3.1.7556.0":
                    return "3.1.7556";
                case "3.1.8421.0":
                    return "4.0.8421";
                case "4.0.8421.0":
                    return "4.0.8421";
                case "3.1.8672.0":
                    return "4.0.8672";
                case "4.0.1221.0":
                    return "4.0.12218";
                default:
                    return $"(Unknown - internal version {version})";
            }
        }
    }
}
