using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Interfaces;
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

            // Check the export directory table, if it exists
            if (pex.ExportTable?.ExportDirectoryTable != null)
            {
                string version = GetPEExportDirectoryVersion(pex);
                if (!string.IsNullOrWhiteSpace(version))
                    return $"WinZip SFX {version}";
            }

            // Get the _winzip_ section, if it exists
            if (pex.ContainsSection("_winzip_", exact: true))
                return "WinZip SFX Unknown Version (32-bit)";

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
        /// Get the version from the PE export directory table value combinations
        /// </summary>
        /// TODO: Research to see if the versions are embedded elsewhere in these files
        private string GetPEExportDirectoryVersion(PortableExecutable pex)
        {
            string sfxFileName = pex.ExportTable.ExportDirectoryTable.Name;
            uint sfxTimeDateStamp = pex.ExportTable.ExportDirectoryTable.TimeDateStamp;
            string assemblyVersion = pex.AssemblyVersion ?? "Unknown Version";

            // Standard
            if (sfxFileName == "VW95SE.SFX" || sfxFileName == "ST32E.SFX"
                || sfxFileName == "WZIPSE32.exe" || sfxFileName == "SI32LPG.SFX"
                || sfxFileName == "ST32E.WZE")
            {
                switch (sfxTimeDateStamp)
                {
                    case 842636344:
                        return "2.0 (32-bit)";
                    case 865370756:
                        return "2.1 RC2 (32-bit)";
                    case 869059925:
                        return "2.1 (32-bit)";
                    case 979049321:
                        return "2.2.4003";
                    case 1149714685:
                        return "3.0.7158";
                    case 1185211734:
                        return "3.1.7556";
                    case 1185211920:
                        return "3.1.7556";
                    case 1235490556:
                        return "4.0.8421";
                    case 1235490757:
                        return "4.0.8421";
                    case 1235490687:
                        return "4.0.8421"; // 3.1.8421.0, SI32LPG?
                    case 1257193383:
                        return "4.0.8672"; // 3.1.8672.0
                    case 1257193543:
                        return "4.0.8672";
                    case 1470410848:
                        return "4.0.12218"; // 4.0.1221.0
                    default:
                        return $"{assemblyVersion} (32-bit)";
                }
            }

            // Personal Edition
            if (sfxFileName == "VW95LE.SFX" || sfxFileName == "PE32E.SFX"
                || sfxFileName == "wzsepe32.exe" || sfxFileName == "SI32PE.SFX"
                || sfxFileName == "SI32LPE.SFX")
            {
                switch (sfxTimeDateStamp)
                {
                    case 845061601:
                        return "Personal Edition (32-bit)"; // TODO: Find version
                    case 868303343:
                        return "Personal Edition (32-bit)"; // TODO: Find version
                    case 868304170:
                        return "Personal Edition (32-bit)"; // TODO: Find version
                    case 906039079:
                        return "Personal Edition 2.2.1260 (32-bit)";
                    case 906040543:
                        return "Personal Edition 2.2.1260 (32-bit)";
                    case 908628435:
                        return "Personal Edition 2.2.1285 (32-bit)";
                    case 908628785:
                        return "Personal Edition 2.2.1285 (32-bit)";
                    case 956165981:
                        return "Personal Edition 2.2.3063";
                    case 956166038:
                        return "Personal Edition 2.2.3063";
                    case 1006353695:
                        return "Personal Edition 2.2.4325";
                    case 1006353714:
                        return "Personal Edition 2.2.4325"; // 8.1.0.0
                    case 1076515698:
                        return "Personal Edition 2.2.6028";
                    case 1076515784:
                        return "Personal Edition 2.2.6028"; // 9.0.6028.0
                    case 1092688561:
                        return "Personal Edition 2.2.6224";
                    case 1092688645:
                        return "Personal Edition 2.2.6224"; // 9.0.6224.0
                    case 1125074095:
                        return "Personal Edition 2.2.6604";
                    case 1125074162:
                        return "Personal Edition 2.2.6604"; // 10.0.6604.0
                    case 1130153399:
                        return "Personal Edition 2.2.6663";
                    case 1130153428:
                        return "Personal Edition 2.2.6663"; // 10.0.6663.0
                    case 1149714176:
                        return "Personal Edition 3.0.7158";
                    case 1163137967:
                        return "Personal Edition 3.0.7305";
                    case 1163137994:
                        return "Personal Edition 3.0.7313"; // 11.0.7313.0
                    case 1176345383:
                        return "Personal Edition 3.0.7452";
                    case 1176345423:
                        return "Personal Edition 3.1.7466"; // 11.1.7466.0
                    case 1184106698:
                        return "Personal Edition 3.1.7556";
                    case 1207280880:
                        return "Personal Edition 4.0.8060"; // 2.3.7382.0
                    case 1207280892:
                        return "Personal Edition 4.0.8094"; // 11.2.8094.0
                    case 1220904506:
                        return "Personal Edition 4.0.8213"; // 2.3.7382.0
                    case 1220904518:
                        return "Personal Edition 4.0.8252"; // 12.0.8252.0
                    case 1235490648:
                        return "Personal Edition 4.0.8421"; // 3.1.8421.0
                    case 1242049399:
                        return "Personal Edition 4.0.8497"; // 12.1.8497.0
                    case 1257193469:
                        return "Personal Edition 4.0.8672"; // 3.1.8672.0, SI32LPE?
                    default:
                        return $"Personal Edition {assemblyVersion} (32-bit)";
                }
            }

            // Software Installation
            else if (sfxFileName == "VW95SRE.SFX" || sfxFileName == "SI32E.SFX"
                || sfxFileName == "SI32E.WZE")
            {
                switch (sfxTimeDateStamp)
                {
                    case 842636381:
                        return "Software Installation 2.0 (32-bit)";
                    case 865370800:
                        return "Software Installation 2.1 RC2 (32-bit)";
                    case 869059963:
                        return "Software Installation 2.1 (32-bit)";
                    case 893107697:
                        return "Software Installation 2.2.1110 (32-bit)";
                    case 952007369:
                        return "Software Installation 2.2.3063";
                    case 1006352634:
                        return "Software Installation 2.2.4325"; // +Personal Edition?
                    case 979049345:
                        return "Software Installation 2.2.4403";
                    case 1026227373:
                        return "Software Installation 2.2.5196"; // +Personal Edition?
                    case 1090582390:
                        return "Software Installation 2.2.6202"; // +Personal Edition?
                    case 1149714757:
                        return "Software Installation 3.0.7158";
                    case 1154357628:
                        return "Software Installation 3.0.7212";
                    case 1175234637:
                        return "Software Installation 3.0.7454";
                    case 1185211802:
                        return "Software Installation 3.1.7556";
                    case 1470410906:
                        return "Software Installation 4.0.12218"; // 4.0.1221.0
                    default:
                        return $"Software Installation {assemblyVersion} (32-bit)";
                }
            }

            switch (sfxFileName)
            {
                // Standard
                case "VW95SE.SFX":
                    return "Unknown Version (32-bit)"; // TODO: Find starting version
                case "ST32E.SFX":
                    return "Unknown Version (32-bit)"; // TODO: Find starting version
                case "WZIPSE32.exe":
                    return "Unknown Version (32-bit)"; // TODO: Find starting version
                case "SI32LPG.SFX":
                    return "Unknown Version (32-bit)"; // TODO: Find starting version
                case "ST32E.WZE":
                    return "Unknown Version (32-bit)"; // TODO: Find starting version

                // Personal Edition
                case "VW95LE.SFX":
                    return "Unknown Version before Personal Edition Build 1285 (32-bit)";
                case "PE32E.SFX":
                    return "Unknown Version after Personal Edition Build 1285 (32-bit)";
                case "wzsepe32.exe":
                    return "Unknown Version Personal Edition (32-bit)"; // TODO: Find starting version
                case "SI32PE.SFX":
                    return "Unknown Version Personal Edition (32-bit)"; // TODO: Find starting version
                case "SI32LPE.SFX":
                    return "Unknown Version Personal Edition (32-bit)"; // TODO: Find starting version

                // Software Installation
                case "VW95SRE.SFX":
                    return "Unknown Version before Software Installation 2.1 (32-bit)";
                case "SI32E.SFX":
                    return "Unknown Version after Software Installation 2.1 (32-bit)";
                case "SI32E.WZE":
                    return "Unknown Version Software Installation (32-bit)"; // TODO: Find starting version
            }

            return null;
        }
    }
}
