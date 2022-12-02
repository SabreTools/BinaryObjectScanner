using System.Collections.Generic;
using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class NewExecutable
    {
        #region Pass-Through Properties

        #region MS-DOS Stub

        #region Standard Fields

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Magic"/>
        public byte[] Stub_Magic => _executable.Stub.Header.Magic;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.LastPageBytes"/>
        public ushort Stub_LastPageBytes => _executable.Stub.Header.LastPageBytes;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Pages"/>
        public ushort Stub_Pages => _executable.Stub.Header.Pages;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationItems"/>
        public ushort Stub_RelocationItems => _executable.Stub.Header.RelocationItems;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.HeaderParagraphSize"/>
        public ushort Stub_HeaderParagraphSize => _executable.Stub.Header.HeaderParagraphSize;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MinimumExtraParagraphs"/>
        public ushort Stub_MinimumExtraParagraphs => _executable.Stub.Header.MinimumExtraParagraphs;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MaximumExtraParagraphs"/>
        public ushort Stub_MaximumExtraParagraphs => _executable.Stub.Header.MaximumExtraParagraphs;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSSValue"/>
        public ushort Stub_InitialSSValue => _executable.Stub.Header.InitialSSValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSPValue"/>
        public ushort Stub_Stub_InitialSPValue => _executable.Stub.Header.InitialSPValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Checksum"/>
        public ushort Stub_Checksum => _executable.Stub.Header.Checksum;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialIPValue"/>
        public ushort Stub_InitialIPValue => _executable.Stub.Header.InitialIPValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialCSValue"/>
        public ushort Stub_InitialCSValue => _executable.Stub.Header.InitialCSValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationTableAddr"/>
        public ushort Stub_RelocationTableAddr => _executable.Stub.Header.RelocationTableAddr;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OverlayNumber"/>
        public ushort Stub_OverlayNumber => _executable.Stub.Header.OverlayNumber;

        #endregion

        #region PE Extensions

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved1"/>
        public ushort[] Stub_Reserved1 => _executable.Stub.Header.Reserved1;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMIdentifier"/>
        public ushort Stub_OEMIdentifier => _executable.Stub.Header.OEMIdentifier;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMInformation"/>
        public ushort Stub_OEMInformation => _executable.Stub.Header.OEMInformation;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved2"/>
        public ushort[] Stub_Reserved2 => _executable.Stub.Header.Reserved2;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
        public uint Stub_NewExeHeaderAddr => _executable.Stub.Header.NewExeHeaderAddr;

        #endregion

        #endregion

        #region Header

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.Magic"/>
        public byte[] Magic => _executable.Header.Magic;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.LinkerVersion"/>
        public byte LinkerVersion => _executable.Header.LinkerVersion;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.LinkerRevision"/>
        public byte LinkerRevision => _executable.Header.LinkerRevision;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.EntryTableOffset"/>
        public ushort EntryTableOffset => _executable.Header.EntryTableOffset;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.EntryTableSize"/>
        public ushort EntryTableSize => _executable.Header.EntryTableSize;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.CrcChecksum"/>
        public uint CrcChecksum => _executable.Header.CrcChecksum;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.FlagWord"/>
        public Models.NewExecutable.HeaderFlag FlagWord => _executable.Header.FlagWord;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.AutomaticDataSegmentNumber"/>
        public ushort AutomaticDataSegmentNumber => _executable.Header.AutomaticDataSegmentNumber;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.InitialHeapAlloc"/>
        public ushort InitialHeapAlloc => _executable.Header.InitialHeapAlloc;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.InitialStackAlloc"/>
        public ushort InitialStackAlloc => _executable.Header.InitialStackAlloc;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.InitialCSIPSetting"/>
        public uint InitialCSIPSetting => _executable.Header.InitialCSIPSetting;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.InitialSSSPSetting"/>
        public uint InitialSSSPSetting => _executable.Header.InitialSSSPSetting;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.FileSegmentCount"/>
        public ushort FileSegmentCount => _executable.Header.FileSegmentCount;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ModuleReferenceTableSize"/>
        public ushort ModuleReferenceTableSize => _executable.Header.ModuleReferenceTableSize;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.NonResidentNameTableSize"/>
        public ushort NonResidentNameTableSize => _executable.Header.NonResidentNameTableSize;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.SegmentTableOffset"/>
        public ushort SegmentTableOffset => _executable.Header.SegmentTableOffset;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ResourceTableOffset"/>
        public ushort ResourceTableOffset => _executable.Header.ResourceTableOffset;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ResidentNameTableOffset"/>
        public ushort ResidentNameTableOffset => _executable.Header.ResidentNameTableOffset;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ModuleReferenceTableOffset"/>
        public ushort ModuleReferenceTableOffset => _executable.Header.ModuleReferenceTableOffset;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ImportedNamesTableOffset"/>
        public ushort ImportedNamesTableOffset => _executable.Header.ImportedNamesTableOffset;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.NonResidentNamesTableOffset"/>
        public uint NonResidentNamesTableOffset => _executable.Header.NonResidentNamesTableOffset;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.MovableEntriesCount"/>
        public ushort MovableEntriesCount => _executable.Header.MovableEntriesCount;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.SegmentAlignmentShiftCount"/>
        public ushort SegmentAlignmentShiftCount => _executable.Header.SegmentAlignmentShiftCount;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ResourceEntriesCount"/>
        public ushort ResourceEntriesCount => _executable.Header.ResourceEntriesCount;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.TargetOperatingSystem"/>
        public Models.NewExecutable.OperatingSystem TargetOperatingSystem => _executable.Header.TargetOperatingSystem;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.AdditionalFlags"/>
        public Models.NewExecutable.OS2Flag AdditionalFlags => _executable.Header.AdditionalFlags;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ReturnThunkOffset"/>
        public ushort ReturnThunkOffset => _executable.Header.ReturnThunkOffset;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.SegmentReferenceThunkOffset"/>
        public ushort SegmentReferenceThunkOffset => _executable.Header.SegmentReferenceThunkOffset;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.MinCodeSwapAreaSize"/>
        public ushort MinCodeSwapAreaSize => _executable.Header.MinCodeSwapAreaSize;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.WindowsSDKRevision"/>
        public byte WindowsSDKRevision => _executable.Header.WindowsSDKRevision;

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.WindowsSDKVersion"/>
        public byte WindowsSDKVersion => _executable.Header.WindowsSDKVersion;

        #endregion

        #region Tables

        /// <inheritdoc cref="Models.NewExecutable.SegmentTable"/>
        public Models.NewExecutable.SegmentTableEntry[] SegmentTable => _executable.SegmentTable;

        /// <inheritdoc cref="Models.NewExecutable.ResourceTable"/>
        public Models.NewExecutable.ResourceTable ResourceTable => _executable.ResourceTable;

        /// <inheritdoc cref="Models.NewExecutable.ResidentNameTable"/>
        public Models.NewExecutable.ResidentNameTableEntry[] ResidentNameTable => _executable.ResidentNameTable;

        /// <inheritdoc cref="Models.NewExecutable.ModuleReferenceTable"/>
        public Models.NewExecutable.ModuleReferenceTableEntry[] ModuleReferenceTable => _executable.ModuleReferenceTable;

        /// <inheritdoc cref="Models.NewExecutable.ImportedNameTable"/>
        public Dictionary<ushort, Models.NewExecutable.ImportedNameTableEntry> ImportedNameTable => _executable.ImportedNameTable;

        /// <inheritdoc cref="Models.NewExecutable.EntryTable"/>
        public Models.NewExecutable.EntryTableBundle[] EntryTable => _executable.EntryTable;

        /// <inheritdoc cref="Models.NewExecutable.NonResidentNameTable"/>
        public Models.NewExecutable.NonResidentNameTableEntry[] NonResidentNameTable => _executable.NonResidentNameTable;

        #endregion

        #endregion

        #region Extension Properties

        // TODO: Determine what extension properties are needed

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the executable
        /// </summary>
        private Models.NewExecutable.Executable _executable;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private NewExecutable() { }

        /// <summary>
        /// Create an NE executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NE executable wrapper on success, null on failure</returns>
        public static NewExecutable Create(byte[] data, int offset)
        {
            var executable = Builder.NewExecutable.ParseExecutable(data, offset);
            if (executable == null)
                return null;

            var wrapper = new NewExecutable { _executable = executable };
            return wrapper;
        }

        /// <summary>
        /// Create an NE executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An NE executable wrapper on success, null on failure</returns>
        public static NewExecutable Create(Stream data)
        {
            var executable = Builder.NewExecutable.ParseExecutable(data);
            if (executable == null)
                return null;

            var wrapper = new NewExecutable { _executable = executable };
            return wrapper;
        }
    }
}