using System;
using System.IO;
using System.Linq;
using System.Text;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
#endif

namespace BinaryObjectScanner.Packer
{
    public class WinZipSFX : IExtractableNewExecutable, IExtractablePortableExecutable, INewExecutableCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // If the resident-name table doesnt exist
            if (nex.Model.ResidentNameTable == null)
                return null;

            // Check for the WinZip name string
            bool winZipNameFound = nex.Model.ResidentNameTable
                .Select(rnte => rnte?.NameString == null ? string.Empty : Encoding.ASCII.GetString(rnte.NameString))
                .Any(s => s.Contains("WZ-SE-01"));

            // If we didn't find it
            if (!winZipNameFound)
                return null;

            // Try to get a known version
            var version = GetNEHeaderVersion(nex);
            if (!string.IsNullOrEmpty(version))
                return $"WinZip SFX {version}";

            return $"WinZip SFX Unknown Version (16-bit)";
        }

        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Check the export directory table, if it exists
            if (pex.Model.ExportTable?.ExportDirectoryTable != null)
            {
                var version = GetPEExportDirectoryVersion(pex);
                if (!string.IsNullOrEmpty(version))
                    return $"WinZip SFX {version}";
            }

            // Get the _winzip_ section, if it exists
            if (pex.ContainsSection("_winzip_", exact: true))
                return "WinZip SFX Unknown Version (32-bit)";

            return null;
        }

        // TODO: Find a way to generically detect 2.X versions and improve exact version detection for SFX PE versions bundled with WinZip 11+

        /// <inheritdoc/>
        public string? Extract(string file, NewExecutable nex, bool includeDebug)
            => Extract(file, includeDebug);

        /// <inheritdoc/>
        public string? Extract(string file, PortableExecutable pex, bool includeDebug)
            => Extract(file, includeDebug);

        /// <summary>
        /// Handle common extraction between executable types
        /// </summary>
        /// <inheritdoc/>
        public string? Extract(string file, bool includeDebug)
        {
#if NET462_OR_GREATER || NETCOREAPP
            try
            {
                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (ZipArchive zipFile = ZipArchive.Open(file))
                {
                    foreach (var entry in zipFile.Entries)
                    {
                        try
                        {
                            // If we have a directory, skip it
                            if (entry.IsDirectory)
                                continue;

                            // If we have a partial entry due to an incomplete multi-part archive, skip it
                            if (!entry.IsComplete)
                                continue;

                            string tempFile = Path.Combine(tempPath, entry.Key);
                            var directoryName = Path.GetDirectoryName(tempFile);
                            if (directoryName != null && !Directory.Exists(directoryName))
                                Directory.CreateDirectory(directoryName);
                            entry.WriteToFile(tempFile);
                        }
                        catch (Exception ex)
                        {
                            if (includeDebug) Console.WriteLine(ex);
                        }
                    }
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
#else
            return null;
#endif
        }

        /// <summary>
        /// Get the version from the NE header value combinations
        /// </summary>
        /// TODO: Reduce the checks to only the ones that differ between versions
        /// TODO: Research to see if the versions are embedded elsewhere in these files
        private static string? GetNEHeaderVersion(NewExecutable nex)
        {
            #region 2.0 Variants

            // 2.0 (MS-DOS/16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x0086
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x4000
                && nex.Model.Header?.InitialCSIPSetting == 0x00012BE6
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0004
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0058
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x0064
                && nex.Model.Header?.ImportedNamesTableOffset == 0x006C
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x000044B8
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "2.0 (MS-DOS/16-bit)";

            // 2.0 (16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x0086
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x4000
                && nex.Model.Header?.InitialCSIPSetting == 0x00013174
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0004
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0058
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x0064
                && nex.Model.Header?.ImportedNamesTableOffset == 0x006C
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x00000198
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "2.0 (16-bit)";

            // Compact 2.0 (16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x0080
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x4000
                && nex.Model.Header?.InitialCSIPSetting == 0x000124A0
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0003
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0058
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x0064
                && nex.Model.Header?.ImportedNamesTableOffset == 0x006A
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x00000192
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "Compact 2.0 (16-bit)";

            // Software Installation 2.0 (16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x00CD
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x4000
                && nex.Model.Header?.InitialCSIPSetting == 0x000136FA
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0005
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0097
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x00A3
                && nex.Model.Header?.ImportedNamesTableOffset == 0x00AD
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x000001DF
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "Software Installation 2.0 (16-bit)";

            #endregion

            #region 2.1 RC2 Variants

            // 2.1 RC2 (MS-DOS/16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x0086
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x4000
                && nex.Model.Header?.InitialCSIPSetting == 0x00013386
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0004
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0058
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x0064
                && nex.Model.Header?.ImportedNamesTableOffset == 0x006C
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x000043C8
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "2.1 RC2 (MS-DOS/16-bit)";

            // 2.1 RC2 (16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x00BE
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x4000
                && nex.Model.Header?.InitialCSIPSetting == 0x00013E56
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0004
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0090
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x009C
                && nex.Model.Header?.ImportedNamesTableOffset == 0x00A4
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x000001D0
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "2.1 RC2 (16-bit)";

            // Compact 2.1 RC2 (16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x0080
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x4000
                && nex.Model.Header?.InitialCSIPSetting == 0x00012B84
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0003
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0058
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x0064
                && nex.Model.Header?.ImportedNamesTableOffset == 0x006A
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x00000192
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "Compact 2.1 RC2 (16-bit)";

            // Software Installation 2.1 RC2 (16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x00BE
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x4000
                && nex.Model.Header?.InitialCSIPSetting == 0x000143AC
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0004
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0090
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x009C
                && nex.Model.Header?.ImportedNamesTableOffset == 0x00A4
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x000001D0
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "Software Installation 2.1 RC2 (16-bit)";

            #endregion

            #region 2.1 Variants

            // 2.1 (MS-DOS/16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x0086
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x3A00
                && nex.Model.Header?.InitialCSIPSetting == 0x00013396
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0004
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0058
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x0064
                && nex.Model.Header?.ImportedNamesTableOffset == 0x006C
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x000043C8
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "2.1 (MS-DOS/16-bit)";

            // 2.1 (16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x00BE
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x3A00
                && nex.Model.Header?.InitialCSIPSetting == 0x00013E7E
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0004
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0090
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x009C
                && nex.Model.Header?.ImportedNamesTableOffset == 0x00A4
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x000001D0
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "2.1 (16-bit)";

            // Compact 2.1 (16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x0080
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x3A00
                && nex.Model.Header?.InitialCSIPSetting == 0x00012B90
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0003
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0058
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x0064
                && nex.Model.Header?.ImportedNamesTableOffset == 0x006A
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x00000192
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "Compact 2.1 (16-bit)";

            // Software Installation 2.1 (16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x00BE
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x3A00
                && nex.Model.Header?.InitialCSIPSetting == 0x00014408
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0004
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0090
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x009C
                && nex.Model.Header?.ImportedNamesTableOffset == 0x00A4
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x000001D0
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "Software Installation 2.1 (16-bit)";

            #endregion

            #region Misc. Variants

            // Personal Edition (16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x0086
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.ProtectedModeOnly
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x4000
                && nex.Model.Header?.InitialCSIPSetting == 0x0001317C
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0004
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0058
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x0064
                && nex.Model.Header?.ImportedNamesTableOffset == 0x006C
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x00000198
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "Personal Edition (16-bit)";

            // Personal Edition 32-bit (16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x00BE
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x2000
                && nex.Model.Header?.InitialStackAlloc == 0x3C00
                && nex.Model.Header?.InitialCSIPSetting == 0x00013E7C
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0004
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0090
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x009C
                && nex.Model.Header?.ImportedNamesTableOffset == 0x00A4
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x000001D0
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "Personal Edition 32-bit (16-bit)";

            // Personal Edition 32-bit Build 1260/1285 (16-bit)
            if (nex.Model.Header?.LinkerVersion == 0x11
                && nex.Model.Header?.LinkerRevision == 0x20
                && nex.Model.Header?.EntryTableOffset == 0x00C6
                && nex.Model.Header?.EntryTableSize == 0x0002
                && nex.Model.Header?.CrcChecksum == 0x00000000
                && nex.Model.Header?.FlagWord == (SabreTools.Models.NewExecutable.HeaderFlag.MULTIPLEDATA
                    | SabreTools.Models.NewExecutable.HeaderFlag.FullScreen
                    | SabreTools.Models.NewExecutable.HeaderFlag.WindowsPMCompatible)
                && nex.Model.Header?.AutomaticDataSegmentNumber == 0x0003
                && nex.Model.Header?.InitialHeapAlloc == 0x43DC
                && nex.Model.Header?.InitialStackAlloc == 0x2708
                && nex.Model.Header?.InitialCSIPSetting == 0x00014ADC
                && nex.Model.Header?.InitialSSSPSetting == 0x00030000
                && nex.Model.Header?.FileSegmentCount == 0x0003
                && nex.Model.Header?.ModuleReferenceTableSize == 0x0005
                && nex.Model.Header?.NonResidentNameTableSize == 0x004B
                && nex.Model.Header?.SegmentTableOffset == 0x0040
                && nex.Model.Header?.ResourceTableOffset == 0x0058
                && nex.Model.Header?.ResidentNameTableOffset == 0x0090
                && nex.Model.Header?.ModuleReferenceTableOffset == 0x009C
                && nex.Model.Header?.ImportedNamesTableOffset == 0x00A6
                && nex.Model.Header?.NonResidentNamesTableOffset == 0x000001D8
                && nex.Model.Header?.MovableEntriesCount == 0x0000
                && nex.Model.Header?.SegmentAlignmentShiftCount == 0x0001
                && nex.Model.Header?.ResourceEntriesCount == 0x0000
                && nex.Model.Header?.TargetOperatingSystem == SabreTools.Models.NewExecutable.OperatingSystem.WINDOWS
                && nex.Model.Header?.AdditionalFlags == 0x00
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.ReturnThunkOffset == 0x0000
                && nex.Model.Header?.MinCodeSwapAreaSize == 0x0000
                && nex.Model.Header?.WindowsSDKRevision == 0x00
                && nex.Model.Header?.WindowsSDKVersion == 0x03)
                return "Personal Edition 32-bit Build 1260/1285 (16-bit)";

            #endregion

            return null;
        }

        /// <summary>
        /// Get the version from the PE export directory table value combinations
        /// </summary>
        /// TODO: Research to see if the versions are embedded elsewhere in these files
        private static string? GetPEExportDirectoryVersion(PortableExecutable pex)
        {
            string sfxFileName = pex.Model.ExportTable?.ExportDirectoryTable?.Name ?? string.Empty;
            uint sfxTimeDateStamp = pex.Model.ExportTable?.ExportDirectoryTable?.TimeDateStamp ?? uint.MaxValue;
            string assemblyVersion = pex.AssemblyVersion ?? "Unknown Version";

            // Standard
            if (sfxFileName == "VW95SE.SFX" || sfxFileName == "ST32E.SFX"
                || sfxFileName == "WZIPSE32.exe" || sfxFileName == "SI32LPG.SFX"
                || sfxFileName == "ST32E.WZE")
            {
                return sfxTimeDateStamp switch
                {
                    842636344 => "2.0 (32-bit)",
                    865370756 => "2.1 RC2 (32-bit)",
                    869059925 => "2.1 (32-bit)",
                    979049321 => "2.2.4003",
                    1149714685 => "3.0.7158",
                    1185211734 => "3.1.7556",
                    1185211920 => "3.1.7556",
                    1235490556 => "4.0.8421",
                    1235490757 => "4.0.8421",
                    1235490687 => "4.0.8421",// 3.1.8421.0, SI32LPG?
                    1257193383 => "4.0.8672",// 3.1.8672.0
                    1257193543 => "4.0.8672",
                    1470410848 => "4.0.12218",// 4.0.1221.0
                    _ => $"{assemblyVersion} (32-bit)",
                };
            }

            // Personal Edition
            if (sfxFileName == "VW95LE.SFX" || sfxFileName == "PE32E.SFX"
                || sfxFileName == "wzsepe32.exe" || sfxFileName == "SI32PE.SFX"
                || sfxFileName == "SI32LPE.SFX")
            {
                return sfxTimeDateStamp switch
                {
                    845061601 => "Personal Edition (32-bit)",// TODO: Find version
                    868303343 => "Personal Edition (32-bit)",// TODO: Find version
                    868304170 => "Personal Edition (32-bit)",// TODO: Find version
                    906039079 => "Personal Edition 2.2.1260 (32-bit)",
                    906040543 => "Personal Edition 2.2.1260 (32-bit)",
                    908628435 => "Personal Edition 2.2.1285 (32-bit)",
                    908628785 => "Personal Edition 2.2.1285 (32-bit)",
                    956165981 => "Personal Edition 2.2.3063",
                    956166038 => "Personal Edition 2.2.3063",
                    1006353695 => "Personal Edition 2.2.4325",
                    1006353714 => "Personal Edition 2.2.4325",// 8.1.0.0
                    1076515698 => "Personal Edition 2.2.6028",
                    1076515784 => "Personal Edition 2.2.6028",// 9.0.6028.0
                    1092688561 => "Personal Edition 2.2.6224",
                    1092688645 => "Personal Edition 2.2.6224",// 9.0.6224.0
                    1125074095 => "Personal Edition 2.2.6604",
                    1125074162 => "Personal Edition 2.2.6604",// 10.0.6604.0
                    1130153399 => "Personal Edition 2.2.6663",
                    1130153428 => "Personal Edition 2.2.6663",// 10.0.6663.0
                    1149714176 => "Personal Edition 3.0.7158",
                    1163137967 => "Personal Edition 3.0.7305",
                    1163137994 => "Personal Edition 3.0.7313",// 11.0.7313.0
                    1176345383 => "Personal Edition 3.0.7452",
                    1176345423 => "Personal Edition 3.1.7466",// 11.1.7466.0
                    1184106698 => "Personal Edition 3.1.7556",
                    1207280880 => "Personal Edition 4.0.8060",// 2.3.7382.0
                    1207280892 => "Personal Edition 4.0.8094",// 11.2.8094.0
                    1220904506 => "Personal Edition 4.0.8213",// 2.3.7382.0
                    1220904518 => "Personal Edition 4.0.8252",// 12.0.8252.0
                    1235490648 => "Personal Edition 4.0.8421",// 3.1.8421.0
                    1242049399 => "Personal Edition 4.0.8497",// 12.1.8497.0
                    1257193469 => "Personal Edition 4.0.8672",// 3.1.8672.0, SI32LPE?
                    _ => $"Personal Edition {assemblyVersion} (32-bit)",
                };
            }

            // Software Installation
            else if (sfxFileName == "VW95SRE.SFX" || sfxFileName == "SI32E.SFX"
                || sfxFileName == "SI32E.WZE")
            {
                return sfxTimeDateStamp switch
                {
                    842636381 => "Software Installation 2.0 (32-bit)",
                    865370800 => "Software Installation 2.1 RC2 (32-bit)",
                    869059963 => "Software Installation 2.1 (32-bit)",
                    893107697 => "Software Installation 2.2.1110 (32-bit)",
                    952007369 => "Software Installation 2.2.3063",
                    1006352634 => "Software Installation 2.2.4325",// +Personal Edition?
                    979049345 => "Software Installation 2.2.4403",
                    1026227373 => "Software Installation 2.2.5196",// +Personal Edition?
                    1090582390 => "Software Installation 2.2.6202",// +Personal Edition?
                    1149714757 => "Software Installation 3.0.7158",
                    1154357628 => "Software Installation 3.0.7212",
                    1175234637 => "Software Installation 3.0.7454",
                    1185211802 => "Software Installation 3.1.7556",
                    1470410906 => "Software Installation 4.0.12218",// 4.0.1221.0
                    _ => $"Software Installation {assemblyVersion} (32-bit)",
                };
            }

            return sfxFileName switch
            {
                // Standard
                "VW95SE.SFX" => "Unknown Version (32-bit)",// TODO: Find starting version
                "ST32E.SFX" => "Unknown Version (32-bit)",// TODO: Find starting version
                "WZIPSE32.exe" => "Unknown Version (32-bit)",// TODO: Find starting version
                "SI32LPG.SFX" => "Unknown Version (32-bit)",// TODO: Find starting version
                "ST32E.WZE" => "Unknown Version (32-bit)",// TODO: Find starting version
                                                          
                // Personal Edition
                "VW95LE.SFX" => "Unknown Version before Personal Edition Build 1285 (32-bit)",
                "PE32E.SFX" => "Unknown Version after Personal Edition Build 1285 (32-bit)",
                "wzsepe32.exe" => "Unknown Version Personal Edition (32-bit)",// TODO: Find starting version
                "SI32PE.SFX" => "Unknown Version Personal Edition (32-bit)",// TODO: Find starting version
                "SI32LPE.SFX" => "Unknown Version Personal Edition (32-bit)",// TODO: Find starting version
                                                                             
                // Software Installation
                "VW95SRE.SFX" => "Unknown Version before Software Installation 2.1 (32-bit)",
                "SI32E.SFX" => "Unknown Version after Software Installation 2.1 (32-bit)",
                "SI32E.WZE" => "Unknown Version Software Installation (32-bit)",// TODO: Find starting version
                _ => null,
            };
        }
    }
}
