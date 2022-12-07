using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;
using BurnOutSharp.Wrappers;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

namespace BurnOutSharp.PackerType
{
    public class WinZipSFX : INewExecutableCheck, IPortableExecutableCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // Check we have a valid executable
            if (nex == null)
                return null;

            // If the resident-name table doesnt exist
            if (nex.ResidentNameTable == null)
                return null;

            // Check for the WinZip name string
            bool winZipNameFound = nex.ResidentNameTable.Where(rnte => rnte?.NameString != null)
                .Select(rnte => Encoding.ASCII.GetString(rnte.NameString))
                .Any(s => s.Contains("WZ-SE-01"));

            // If we didn't find it
            if (!winZipNameFound)
                return null;

            // Try to get a known version
            string version = GetNEHeaderVersion(nex);
            if (!string.IsNullOrWhiteSpace(version))
                return $"WinZip SFX {version}";

            return $"WinZip SFX Unknown Version (16-bit)";
        }

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .rdata section, if it exists
            if (pex.ContainsSection(".rdata"))
            {
                string version = GetSFXSectionDataVersion(file, pex.GetFirstSectionData(".rdata"), includeDebug);
                if (!string.IsNullOrWhiteSpace(version))
                    return $"WinZip SFX {version}";
            }

            // Get the _winzip_ section, if it exists
            if (pex.ContainsSection("_winzip_", exact: true))
            {
                string version = GetPEHeaderVersion(pex);
                if (!string.IsNullOrWhiteSpace(version))
                    return $"WinZip SFX {version}";

                version = GetAdjustedManifestVersion(pex);
                if (!string.IsNullOrWhiteSpace(version))
                    return $"WinZip SFX {version}";

                return "WinZip SFX Unknown Version (32-bit)";
            }

            #region Unknown Version checks

            // Get the .rdata section, if it exists
            if (pex.ContainsSection(".rdata"))
            {
                string version = GetSFXSectionDataUnknownVersion(file, pex.GetFirstSectionData(".rdata"), includeDebug);
                if (!string.IsNullOrWhiteSpace(version))
                    return $"WinZip SFX {version}";
            }

            // Get the .data/DATA section, if it exists
            var dataSectionRaw = pex.GetFirstSectionData(".data") ?? pex.GetFirstSectionData("DATA");
            if (dataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // WinZip Self-Extractor header corrupt.
                    new ContentMatchSet(new byte?[]
                    {
                        0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70, 0x20, 0x53,
                        0x65, 0x6C, 0x66, 0x2D, 0x45, 0x78, 0x74, 0x72,
                        0x61, 0x63, 0x74, 0x6F, 0x72, 0x20, 0x68, 0x65,
                        0x61, 0x64, 0x65, 0x72, 0x20, 0x63, 0x6F, 0x72,
                        0x72, 0x75, 0x70, 0x74, 0x2E,
                    }, "Unknown Version (32-bit)"),

                    // winzip\shell\open\command
                    new ContentMatchSet(new byte?[]
                    {
                        0x77, 0x69, 0x6E, 0x7A, 0x69, 0x70, 0x5C, 0x73,
                        0x68, 0x65, 0x6C, 0x6C, 0x5C, 0x6F, 0x70, 0x65,
                        0x6E, 0x5C, 0x63, 0x6F, 0x6D, 0x6D, 0x61, 0x6E,
                        0x64,
                    }, "Unknown Version (32-bit)"),
                };

                string version = MatchUtil.GetFirstMatch(file, pex.GetFirstSectionData(".data"), matchers, false);
                if (!string.IsNullOrWhiteSpace(version))
                {
                    // Try to grab the value from the manifest, if possible
                    string manifestVersion = GetAdjustedManifestVersion(pex);
                    if (!string.IsNullOrWhiteSpace(manifestVersion))
                        return $"WinZip SFX {manifestVersion}";

                    return $"WinZip SFX {version}";
                }
            }

            #endregion

            return null;
        }

        // TODO: Find a way to generically detect 2.X versions and improve exact version detection for SFX PE versions bundled with WinZip 11+

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
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
                        catch (Exception ex)
                        {
                            if (scanner.IncludeDebug) Console.WriteLine(ex);
                        }
                    }
                }

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch (Exception ex)
                {
                    if (scanner.IncludeDebug) Console.WriteLine(ex);
                }

                // Remove temporary path references
                Utilities.StripFromKeys(protections, tempPath);

                return protections;
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }

        /// <summary>
        /// Get the version from the assembly manifest, correcting where possible
        /// </summary>
        private static string GetAdjustedManifestVersion(PortableExecutable pex)
        {
            // Get the manifest information, if possible
            string description = pex.AssemblyDescription;
            string version = pex.AssemblyVersion;

            // Either an incorrect description or empty version mean we can't match
            if (description != "WinZip Self-Extractor")
                return null;
            else if (string.IsNullOrEmpty(version))
                return null;

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

        /// <summary>
        /// Get the version from the NE header value combinations
        /// </summary>
        /// TODO: Reduce the checks to only the ones that differ between versions
        /// TODO: Research to see if the versions are embedded elsewhere in these files
        private string GetNEHeaderVersion(NewExecutable nex)
        {
            #region 2.0 Variants

            // 2.0 (MS-DOS/16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x0086
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x4000
                && nex.InitialCSIPSetting == 0x00012BE6
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0004
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0058
                && nex.ModuleReferenceTableOffset == 0x0064
                && nex.ImportedNamesTableOffset == 0x006C
                && nex.NonResidentNamesTableOffset == 0x000044B8
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "2.0 (MS-DOS/16-bit)";

            // 2.0 (16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x0086
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x4000
                && nex.InitialCSIPSetting == 0x00013174
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0004
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0058
                && nex.ModuleReferenceTableOffset == 0x0064
                && nex.ImportedNamesTableOffset == 0x006C
                && nex.NonResidentNamesTableOffset == 0x00000198
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "2.0 (16-bit)";

            // Compact 2.0 (16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x0080
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x4000
                && nex.InitialCSIPSetting == 0x000124A0
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0003
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0058
                && nex.ModuleReferenceTableOffset == 0x0064
                && nex.ImportedNamesTableOffset == 0x006A
                && nex.NonResidentNamesTableOffset == 0x00000192
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "Compact 2.0 (16-bit)";

            // Software Installation 2.0 (16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x00CD
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x4000
                && nex.InitialCSIPSetting == 0x000136FA
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0005
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0097
                && nex.ModuleReferenceTableOffset == 0x00A3
                && nex.ImportedNamesTableOffset == 0x00AD
                && nex.NonResidentNamesTableOffset == 0x000001DF
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "Software Installation 2.0 (16-bit)";

            #endregion

            #region 2.1 RC2 Variants

            // 2.1 RC2 (MS-DOS/16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x0086
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x4000
                && nex.InitialCSIPSetting == 0x00013386
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0004
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0058
                && nex.ModuleReferenceTableOffset == 0x0064
                && nex.ImportedNamesTableOffset == 0x006C
                && nex.NonResidentNamesTableOffset == 0x000043C8
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "2.1 RC2 (MS-DOS/16-bit)";

            // 2.1 RC2 (16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x00BE
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x4000
                && nex.InitialCSIPSetting == 0x00013E56
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0004
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0090
                && nex.ModuleReferenceTableOffset == 0x009C
                && nex.ImportedNamesTableOffset == 0x00A4
                && nex.NonResidentNamesTableOffset == 0x000001D0
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "2.1 RC2 (16-bit)";

            // Compact 2.1 RC2 (16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x0080
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x4000
                && nex.InitialCSIPSetting == 0x00012B84
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0003
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0058
                && nex.ModuleReferenceTableOffset == 0x0064
                && nex.ImportedNamesTableOffset == 0x006A
                && nex.NonResidentNamesTableOffset == 0x00000192
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "Compact 2.1 RC2 (16-bit)";

            // Software Installation 2.1 RC2 (16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x00BE
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x4000
                && nex.InitialCSIPSetting == 0x000143AC
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0004
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0090
                && nex.ModuleReferenceTableOffset == 0x009C
                && nex.ImportedNamesTableOffset == 0x00A4
                && nex.NonResidentNamesTableOffset == 0x000001D0
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "Software Installation 2.1 RC2 (16-bit)";

            #endregion

            #region 2.1 Variants

            // 2.1 (MS-DOS/16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x0086
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x3A00
                && nex.InitialCSIPSetting == 0x00013396
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0004
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0058
                && nex.ModuleReferenceTableOffset == 0x0064
                && nex.ImportedNamesTableOffset == 0x006C
                && nex.NonResidentNamesTableOffset == 0x000043C8
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "2.1 (MS-DOS/16-bit)";

            // 2.1 (16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x00BE
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x3A00
                && nex.InitialCSIPSetting == 0x00013E7E
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0004
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0090
                && nex.ModuleReferenceTableOffset == 0x009C
                && nex.ImportedNamesTableOffset == 0x00A4
                && nex.NonResidentNamesTableOffset == 0x000001D0
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "2.1 (16-bit)";

            // Compact 2.1 (16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x0080
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x3A00
                && nex.InitialCSIPSetting == 0x00012B90
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0003
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0058
                && nex.ModuleReferenceTableOffset == 0x0064
                && nex.ImportedNamesTableOffset == 0x006A
                && nex.NonResidentNamesTableOffset == 0x00000192
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "Compact 2.1 (16-bit)";

            // Software Installation 2.1 (16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x00BE
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x3A00
                && nex.InitialCSIPSetting == 0x00014408
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0004
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0090
                && nex.ModuleReferenceTableOffset == 0x009C
                && nex.ImportedNamesTableOffset == 0x00A4
                && nex.NonResidentNamesTableOffset == 0x000001D0
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "Software Installation 2.1 (16-bit)";

            #endregion

            #region Misc. Variants

            // Personal Edition (16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x0086
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x4000
                && nex.InitialCSIPSetting == 0x0001317C
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0004
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0058
                && nex.ModuleReferenceTableOffset == 0x0064
                && nex.ImportedNamesTableOffset == 0x006C
                && nex.NonResidentNamesTableOffset == 0x00000198
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "Personal Edition (16-bit)";

            // Personal Edition 32-bit (16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x00BE
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x2000
                && nex.InitialStackAlloc == 0x3C00
                && nex.InitialCSIPSetting == 0x00013E7C
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0004
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0090
                && nex.ModuleReferenceTableOffset == 0x009C
                && nex.ImportedNamesTableOffset == 0x00A4
                && nex.NonResidentNamesTableOffset == 0x000001D0
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "Personal Edition 32-bit (16-bit)";

            // Personal Edition 32-bit Build 1260/1285 (16-bit)
            if (nex.LinkerVersion == 0x11
                && nex.LinkerRevision == 0x20
                && nex.EntryTableOffset == 0x00C6
                && nex.EntryTableSize == 0x0002
                && nex.CrcChecksum == 0x00000000
                && nex.FlagWord == (Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | Models.NewExecutable.HeaderFlag.FullScreen
                    | Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.AutomaticDataSegmentNumber == 0x0003
                && nex.InitialHeapAlloc == 0x43DC
                && nex.InitialStackAlloc == 0x2708
                && nex.InitialCSIPSetting == 0x00014ADC
                && nex.InitialSSSPSetting == 0x00030000
                && nex.FileSegmentCount == 0x0003
                && nex.ModuleReferenceTableSize == 0x0005
                && nex.NonResidentNameTableSize == 0x004B
                && nex.SegmentTableOffset == 0x0040
                && nex.ResourceTableOffset == 0x0058
                && nex.ResidentNameTableOffset == 0x0090
                && nex.ModuleReferenceTableOffset == 0x009C
                && nex.ImportedNamesTableOffset == 0x00A6
                && nex.NonResidentNamesTableOffset == 0x000001D8
                && nex.MovableEntriesCount == 0x0000
                && nex.SegmentAlignmentShiftCount == 0x0001
                && nex.ResourceEntriesCount == 0x0000
                && nex.TargetOperatingSystem == Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.AdditionalFlags == 0x00
                && nex.ReturnThunkOffset == 0x0000
                && nex.SegmentReferenceThunkOffset == 0x0000
                && nex.MinCodeSwapAreaSize == 0x0000
                && nex.WindowsSDKRevision == 0x00
                && nex.WindowsSDKVersion == 0x03)
                return "Personal Edition 32-bit Build 1260/1285 (16-bit)";

            #endregion

            return null;
        }

        /// <summary>
        /// Get the version from the PE header value combinations
        /// </summary>
        /// TODO: Reduce the checks to only the ones that differ between versions
        /// TODO: Research to see if the versions are embedded elsewhere in these files
        private string GetPEHeaderVersion(PortableExecutable pex)
        {
            // 2.2.3063
            if (pex.Machine == Models.PortableExecutable.MachineType.IMAGE_FILE_MACHINE_I386
                && pex.NumberOfSections == 0x0005
                && pex.TimeDateStamp == 0x38BE7AC9
                && pex.PointerToSymbolTable == 0x00000000
                && pex.NumberOfSymbols == 0x00000000
                && pex.SizeOfOptionalHeader == 0x00E0
                && (ushort)pex.Characteristics == 0x010F

                && pex.OH_Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
                && pex.OH_MajorLinkerVersion == 0x05
                && pex.OH_MinorLinkerVersion == 0x0A
                && pex.OH_SizeOfCode == 0x00005C00
                && pex.OH_SizeOfInitializedData == 0x00004C00
                && pex.OH_SizeOfUninitializedData == 0x00000000
                && pex.OH_AddressOfEntryPoint == 0x00003E71
                && pex.OH_BaseOfCode == 0x00001000
                && pex.OH_BaseOfData == 0x00007000
                && pex.OH_ImageBase == 0x00400000)
                return "2.2.3063";

            // 2.2.4003
            if (pex.Machine == Models.PortableExecutable.MachineType.IMAGE_FILE_MACHINE_I386
                && pex.NumberOfSections == 0x0005
                && pex.TimeDateStamp == 0x3A5B1B69
                && pex.PointerToSymbolTable == 0x00000000
                && pex.NumberOfSymbols == 0x00000000
                && pex.SizeOfOptionalHeader == 0x00E0
                && (ushort)pex.Characteristics == 0x010F

                && pex.OH_Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
                && pex.OH_MajorLinkerVersion == 0x05
                && pex.OH_MinorLinkerVersion == 0x0A
                && pex.OH_SizeOfCode == 0x00004A00
                && pex.OH_SizeOfInitializedData == 0x00002A00
                && pex.OH_SizeOfUninitializedData == 0x00000000
                && pex.OH_AddressOfEntryPoint == 0x000039D8
                && pex.OH_BaseOfCode == 0x00001000
                && pex.OH_BaseOfData == 0x00006000
                && pex.OH_ImageBase == 0x00400000)
                return "2.2.4003";

            // Software Installation 2.2.4003
            if (pex.Machine == Models.PortableExecutable.MachineType.IMAGE_FILE_MACHINE_I386
                && pex.NumberOfSections == 0x0005
                && pex.TimeDateStamp == 0x3A5B1B81
                && pex.PointerToSymbolTable == 0x00000000
                && pex.NumberOfSymbols == 0x00000000
                && pex.SizeOfOptionalHeader == 0x00E0
                && (ushort)pex.Characteristics == 0x010F

                && pex.OH_Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
                && pex.OH_MajorLinkerVersion == 0x05
                && pex.OH_MinorLinkerVersion == 0x0A
                && pex.OH_SizeOfCode == 0x00005600
                && pex.OH_SizeOfInitializedData == 0x00002A00
                && pex.OH_SizeOfUninitializedData == 0x00000000
                && pex.OH_AddressOfEntryPoint == 0x00003F8F
                && pex.OH_BaseOfCode == 0x00001000
                && pex.OH_BaseOfData == 0x00007000
                && pex.OH_ImageBase == 0x00400000)
                return "Software Installation 2.2.4003";

            // 2.2.4325
            if (pex.Machine == Models.PortableExecutable.MachineType.IMAGE_FILE_MACHINE_I386
                && pex.NumberOfSections == 0x0005
                && pex.TimeDateStamp == 0x3BFBB8FA
                && pex.PointerToSymbolTable == 0x00000000
                && pex.NumberOfSymbols == 0x00000000
                && pex.SizeOfOptionalHeader == 0x00E0
                && (ushort)pex.Characteristics == 0x010F

                && pex.OH_Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
                && pex.OH_MajorLinkerVersion == 0x06
                && pex.OH_MinorLinkerVersion == 0x00
                && pex.OH_SizeOfCode == 0x00006000
                && pex.OH_SizeOfInitializedData == 0x0000F000
                && pex.OH_SizeOfUninitializedData == 0x00000000
                && pex.OH_AddressOfEntryPoint == 0x00003EF0
                && pex.OH_BaseOfCode == 0x00001000
                && pex.OH_BaseOfData == 0x00007000
                && pex.OH_ImageBase == 0x00400000
                && pex.OH_SectionAlignment == 0x00001000
                && pex.OH_FileAlignment == 0x00001000)
                return "2.2.4325";

            // 2.2.5196
            if (pex.Machine == Models.PortableExecutable.MachineType.IMAGE_FILE_MACHINE_I386
                && pex.NumberOfSections == 0x0005
                && pex.TimeDateStamp == 0x3D2AFCAD
                && pex.PointerToSymbolTable == 0x00000000
                && pex.NumberOfSymbols == 0x00000000
                && pex.SizeOfOptionalHeader == 0x00E0
                && (ushort)pex.Characteristics == 0x010F

                && pex.OH_Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
                && pex.OH_MajorLinkerVersion == 0x07
                && pex.OH_MinorLinkerVersion == 0x00
                && pex.OH_SizeOfCode == 0x00007000
                && pex.OH_SizeOfInitializedData == 0x00010000
                && pex.OH_SizeOfUninitializedData == 0x00000000
                && pex.OH_AddressOfEntryPoint == 0x00004554
                && pex.OH_BaseOfCode == 0x00001000
                && pex.OH_BaseOfData == 0x00008000
                && pex.OH_ImageBase == 0x00400000
                && pex.OH_SectionAlignment == 0x00001000
                && pex.OH_FileAlignment == 0x00001000)
                return "2.2.5196";

            // 2.2.6202
            if (pex.Machine == Models.PortableExecutable.MachineType.IMAGE_FILE_MACHINE_I386
                && pex.NumberOfSections == 0x0005
                && pex.TimeDateStamp == 0x4100F776
                && pex.PointerToSymbolTable == 0x00000000
                && pex.NumberOfSymbols == 0x00000000
                && pex.SizeOfOptionalHeader == 0x00E0
                && (ushort)pex.Characteristics == 0x010F

                && pex.OH_Magic == Models.PortableExecutable.OptionalHeaderMagicNumber.PE32
                && pex.OH_MajorLinkerVersion == 0x07
                && pex.OH_MinorLinkerVersion == 0x00
                && pex.OH_SizeOfCode == 0x00007000
                && pex.OH_SizeOfInitializedData == 0x00010000
                && pex.OH_SizeOfUninitializedData == 0x00000000
                && pex.OH_AddressOfEntryPoint == 0x00004603
                && pex.OH_BaseOfCode == 0x00001000
                && pex.OH_BaseOfData == 0x00008000
                && pex.OH_ImageBase == 0x00400000)
                return "2.2.6202";

            return null;
        }

        /// <summary>
        /// Get the version from the .rdata SFX header data
        /// </summary>
        /// TODO: Research to see if the versions are embedded elsewhere in these files
        private string GetSFXSectionDataVersion(string file, byte[] sectionContent, bool includeDebug)
        {
            var matchers = new List<ContentMatchSet>
            {
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
            };

            return MatchUtil.GetFirstMatch(file, sectionContent, matchers, includeDebug);
        }

        /// <summary>
        /// Get the unknown version from the .rdata SFX header data
        /// </summary>
        private string GetSFXSectionDataUnknownVersion(string file, byte[] sectionContent, bool includeDebug)
        {
            var matchers = new List<ContentMatchSet>
            {
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
            };

            return MatchUtil.GetFirstMatch(file, sectionContent, matchers, includeDebug);
        }
    }
}
