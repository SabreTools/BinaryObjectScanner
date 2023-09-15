using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class NewExecutable : WrapperBase<SabreTools.Models.NewExecutable.Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "New Executable (NE)";

        #endregion

        #region Pass-Through Properties

        #region MS-DOS Stub

        #region Standard Fields

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Magic"/>
#if NET48
        public string Stub_Magic => this.Model.Stub.Header.Magic;
#else
        public string? Stub_Magic => this.Model.Stub?.Header?.Magic;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.LastPageBytes"/>
#if NET48
        public ushort Stub_LastPageBytes => this.Model.Stub.Header.LastPageBytes;
#else
        public ushort? Stub_LastPageBytes => this.Model.Stub?.Header?.LastPageBytes;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Pages"/>
#if NET48
        public ushort Stub_Pages => this.Model.Stub.Header.Pages;
#else
        public ushort? Stub_Pages => this.Model.Stub?.Header?.Pages;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationItems"/>
#if NET48
        public ushort Stub_RelocationItems => this.Model.Stub.Header.RelocationItems;
#else
        public ushort? Stub_RelocationItems => this.Model.Stub?.Header?.RelocationItems;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.HeaderParagraphSize"/>
#if NET48
        public ushort Stub_HeaderParagraphSize => this.Model.Stub.Header.HeaderParagraphSize;
#else
        public ushort? Stub_HeaderParagraphSize => this.Model.Stub?.Header?.HeaderParagraphSize;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MinimumExtraParagraphs"/>
#if NET48
        public ushort Stub_MinimumExtraParagraphs => this.Model.Stub.Header.MinimumExtraParagraphs;
#else
        public ushort? Stub_MinimumExtraParagraphs => this.Model.Stub?.Header?.MinimumExtraParagraphs;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MaximumExtraParagraphs"/>
#if NET48
        public ushort Stub_MaximumExtraParagraphs => this.Model.Stub.Header.MaximumExtraParagraphs;
#else
        public ushort? Stub_MaximumExtraParagraphs => this.Model.Stub?.Header?.MaximumExtraParagraphs;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSSValue"/>
#if NET48
        public ushort Stub_InitialSSValue => this.Model.Stub.Header.InitialSSValue;
#else
        public ushort? Stub_InitialSSValue => this.Model.Stub?.Header?.InitialSSValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSPValue"/>
#if NET48
        public ushort Stub_InitialSPValue => this.Model.Stub.Header.InitialSPValue;
#else
        public ushort? Stub_InitialSPValue => this.Model.Stub?.Header?.InitialSPValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Checksum"/>
#if NET48
        public ushort Stub_Checksum => this.Model.Stub.Header.Checksum;
#else
        public ushort? Stub_Checksum => this.Model.Stub?.Header?.Checksum;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialIPValue"/>
#if NET48
        public ushort Stub_InitialIPValue => this.Model.Stub.Header.InitialIPValue;
#else
        public ushort? Stub_InitialIPValue => this.Model.Stub?.Header?.InitialIPValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialCSValue"/>
#if NET48
        public ushort Stub_InitialCSValue => this.Model.Stub.Header.InitialCSValue;
#else
        public ushort? Stub_InitialCSValue => this.Model.Stub?.Header?.InitialCSValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationTableAddr"/>
#if NET48
        public ushort Stub_RelocationTableAddr => this.Model.Stub.Header.RelocationTableAddr;
#else
        public ushort? Stub_RelocationTableAddr => this.Model.Stub?.Header?.RelocationTableAddr;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OverlayNumber"/>
#if NET48
        public ushort Stub_OverlayNumber => this.Model.Stub.Header.OverlayNumber;
#else
        public ushort? Stub_OverlayNumber => this.Model.Stub?.Header?.OverlayNumber;
#endif

        #endregion

        #region PE Extensions

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved1"/>
#if NET48
        public ushort[] Stub_Reserved1 => this.Model.Stub.Header.Reserved1;
#else
        public ushort[]? Stub_Reserved1 => this.Model.Stub?.Header?.Reserved1;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMIdentifier"/>
#if NET48
        public ushort Stub_OEMIdentifier => this.Model.Stub.Header.OEMIdentifier;
#else
        public ushort? Stub_OEMIdentifier => this.Model.Stub?.Header?.OEMIdentifier;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMInformation"/>
#if NET48
        public ushort Stub_OEMInformation => this.Model.Stub.Header.OEMInformation;
#else
        public ushort? Stub_OEMInformation => this.Model.Stub?.Header?.OEMInformation;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved2"/>
#if NET48
        public ushort[] Stub_Reserved2 => this.Model.Stub.Header.Reserved2;
#else
        public ushort[]? Stub_Reserved2 => this.Model.Stub?.Header?.Reserved2;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
#if NET48
        public uint Stub_NewExeHeaderAddr => this.Model.Stub.Header.NewExeHeaderAddr;
#else
        public uint? Stub_NewExeHeaderAddr => this.Model.Stub?.Header?.NewExeHeaderAddr;
#endif

        #endregion

        #endregion

        #region Header

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.Magic"/>
#if NET48
        public string Magic => this.Model.Header.Magic;
#else
        public string? Magic => this.Model.Header?.Magic;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.LinkerVersion"/>
#if NET48
        public byte LinkerVersion => this.Model.Header.LinkerVersion;
#else
        public byte? LinkerVersion => this.Model.Header?.LinkerVersion;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.LinkerRevision"/>
#if NET48
        public byte LinkerRevision => this.Model.Header.LinkerRevision;
#else
        public byte? LinkerRevision => this.Model.Header?.LinkerRevision;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.EntryTableOffset"/>
#if NET48
        public ushort EntryTableOffset => this.Model.Header.EntryTableOffset;
#else
        public ushort? EntryTableOffset => this.Model.Header?.EntryTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.EntryTableSize"/>
#if NET48
        public ushort EntryTableSize => this.Model.Header.EntryTableSize;
#else
        public ushort? EntryTableSize => this.Model.Header?.EntryTableSize;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.CrcChecksum"/>
#if NET48
        public uint CrcChecksum => this.Model.Header.CrcChecksum;
#else
        public uint? CrcChecksum => this.Model.Header?.CrcChecksum;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.FlagWord"/>
#if NET48
        public SabreTools.Models.NewExecutable.HeaderFlag FlagWord => this.Model.Header.FlagWord;
#else
        public SabreTools.Models.NewExecutable.HeaderFlag? FlagWord => this.Model.Header?.FlagWord;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.AutomaticDataSegmentNumber"/>
#if NET48
        public ushort AutomaticDataSegmentNumber => this.Model.Header.AutomaticDataSegmentNumber;
#else
        public ushort? AutomaticDataSegmentNumber => this.Model.Header?.AutomaticDataSegmentNumber;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.InitialHeapAlloc"/>
#if NET48
        public ushort InitialHeapAlloc => this.Model.Header.InitialHeapAlloc;
#else
        public ushort? InitialHeapAlloc => this.Model.Header?.InitialHeapAlloc;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.InitialStackAlloc"/>
#if NET48
        public ushort InitialStackAlloc => this.Model.Header.InitialStackAlloc;
#else
        public ushort? InitialStackAlloc => this.Model.Header?.InitialStackAlloc;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.InitialCSIPSetting"/>
#if NET48
        public uint InitialCSIPSetting => this.Model.Header.InitialCSIPSetting;
#else
        public uint? InitialCSIPSetting => this.Model.Header?.InitialCSIPSetting;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.InitialSSSPSetting"/>
#if NET48
        public uint InitialSSSPSetting => this.Model.Header.InitialSSSPSetting;
#else
        public uint? InitialSSSPSetting => this.Model.Header?.InitialSSSPSetting;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.FileSegmentCount"/>
#if NET48
        public ushort FileSegmentCount => this.Model.Header.FileSegmentCount;
#else
        public ushort? FileSegmentCount => this.Model.Header?.FileSegmentCount;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ModuleReferenceTableSize"/>
#if NET48
        public ushort ModuleReferenceTableSize => this.Model.Header.ModuleReferenceTableSize;
#else
        public ushort? ModuleReferenceTableSize => this.Model.Header?.ModuleReferenceTableSize;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.NonResidentNameTableSize"/>
#if NET48
        public ushort NonResidentNameTableSize => this.Model.Header.NonResidentNameTableSize;
#else
        public ushort? NonResidentNameTableSize => this.Model.Header?.NonResidentNameTableSize;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.SegmentTableOffset"/>
#if NET48
        public ushort SegmentTableOffset => this.Model.Header.SegmentTableOffset;
#else
        public ushort? SegmentTableOffset => this.Model.Header?.SegmentTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ResourceTableOffset"/>
#if NET48
        public ushort ResourceTableOffset => this.Model.Header.ResourceTableOffset;
#else
        public ushort? ResourceTableOffset => this.Model.Header?.ResourceTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ResidentNameTableOffset"/>
#if NET48
        public ushort ResidentNameTableOffset => this.Model.Header.ResidentNameTableOffset;
#else
        public ushort? ResidentNameTableOffset => this.Model.Header?.ResidentNameTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ModuleReferenceTableOffset"/>
#if NET48
        public ushort ModuleReferenceTableOffset => this.Model.Header.ModuleReferenceTableOffset;
#else
        public ushort? ModuleReferenceTableOffset => this.Model.Header?.ModuleReferenceTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ImportedNamesTableOffset"/>
#if NET48
        public ushort ImportedNamesTableOffset => this.Model.Header.ImportedNamesTableOffset;
#else
        public ushort? ImportedNamesTableOffset => this.Model.Header?.ImportedNamesTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.NonResidentNamesTableOffset"/>
#if NET48
        public uint NonResidentNamesTableOffset => this.Model.Header.NonResidentNamesTableOffset;
#else
        public uint? NonResidentNamesTableOffset => this.Model.Header?.NonResidentNamesTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.MovableEntriesCount"/>
#if NET48
        public ushort MovableEntriesCount => this.Model.Header.MovableEntriesCount;
#else
        public ushort? MovableEntriesCount => this.Model.Header?.MovableEntriesCount;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.SegmentAlignmentShiftCount"/>
#if NET48
        public ushort SegmentAlignmentShiftCount => this.Model.Header.SegmentAlignmentShiftCount;
#else
        public ushort? SegmentAlignmentShiftCount => this.Model.Header?.SegmentAlignmentShiftCount;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ResourceEntriesCount"/>
#if NET48
        public ushort ResourceEntriesCount => this.Model.Header.ResourceEntriesCount;
#else
        public ushort? ResourceEntriesCount => this.Model.Header?.ResourceEntriesCount;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.TargetOperatingSystem"/>
#if NET48
        public SabreTools.Models.NewExecutable.OperatingSystem TargetOperatingSystem => this.Model.Header.TargetOperatingSystem;
#else
        public SabreTools.Models.NewExecutable.OperatingSystem? TargetOperatingSystem => this.Model.Header?.TargetOperatingSystem;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.AdditionalFlags"/>
#if NET48
        public SabreTools.Models.NewExecutable.OS2Flag AdditionalFlags => this.Model.Header.AdditionalFlags;
#else
        public SabreTools.Models.NewExecutable.OS2Flag? AdditionalFlags => this.Model.Header?.AdditionalFlags;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ReturnThunkOffset"/>
#if NET48
        public ushort ReturnThunkOffset => this.Model.Header.ReturnThunkOffset;
#else
        public ushort? ReturnThunkOffset => this.Model.Header?.ReturnThunkOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.SegmentReferenceThunkOffset"/>
#if NET48
        public ushort SegmentReferenceThunkOffset => this.Model.Header.SegmentReferenceThunkOffset;
#else
        public ushort? SegmentReferenceThunkOffset => this.Model.Header?.SegmentReferenceThunkOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.MinCodeSwapAreaSize"/>
#if NET48
        public ushort MinCodeSwapAreaSize => this.Model.Header.MinCodeSwapAreaSize;
#else
        public ushort? MinCodeSwapAreaSize => this.Model.Header?.MinCodeSwapAreaSize;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.WindowsSDKRevision"/>
#if NET48
        public byte WindowsSDKRevision => this.Model.Header.WindowsSDKRevision;
#else
        public byte? WindowsSDKRevision => this.Model.Header?.WindowsSDKRevision;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.WindowsSDKVersion"/>
#if NET48
        public byte WindowsSDKVersion => this.Model.Header.WindowsSDKVersion;
#else
        public byte? WindowsSDKVersion => this.Model.Header?.WindowsSDKVersion;
#endif

        #endregion

        #region Tables

        /// <inheritdoc cref="Models.NewExecutable.SegmentTable"/>
#if NET48
        public SabreTools.Models.NewExecutable.SegmentTableEntry[] SegmentTable => this.Model.SegmentTable;
#else
        public SabreTools.Models.NewExecutable.SegmentTableEntry?[]? SegmentTable => this.Model.SegmentTable;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ResourceTable"/>
#if NET48
        public SabreTools.Models.NewExecutable.ResourceTable ResourceTable => this.Model.ResourceTable;
#else
        public SabreTools.Models.NewExecutable.ResourceTable? ResourceTable => this.Model.ResourceTable;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ResidentNameTable"/>
#if NET48
        public SabreTools.Models.NewExecutable.ResidentNameTableEntry[] ResidentNameTable => this.Model.ResidentNameTable;
#else
        public SabreTools.Models.NewExecutable.ResidentNameTableEntry?[]? ResidentNameTable => this.Model.ResidentNameTable;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ModuleReferenceTable"/>
#if NET48
        public SabreTools.Models.NewExecutable.ModuleReferenceTableEntry[] ModuleReferenceTable => this.Model.ModuleReferenceTable;
#else
        public SabreTools.Models.NewExecutable.ModuleReferenceTableEntry?[]? ModuleReferenceTable => this.Model.ModuleReferenceTable;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ImportedNameTable"/>
#if NET48
        public Dictionary<ushort, SabreTools.Models.NewExecutable.ImportedNameTableEntry> ImportedNameTable => this.Model.ImportedNameTable;
#else
        public Dictionary<ushort, SabreTools.Models.NewExecutable.ImportedNameTableEntry?>? ImportedNameTable => this.Model.ImportedNameTable;
#endif

        /// <inheritdoc cref="Models.NewExecutable.EntryTable"/>
#if NET48
        public SabreTools.Models.NewExecutable.EntryTableBundle[] EntryTable => this.Model.EntryTable;
#else
        public SabreTools.Models.NewExecutable.EntryTableBundle?[]? EntryTable => this.Model.EntryTable;
#endif

        /// <inheritdoc cref="Models.NewExecutable.NonResidentNameTable"/>
#if NET48
        public SabreTools.Models.NewExecutable.NonResidentNameTableEntry[] NonResidentNameTable => this.Model.NonResidentNameTable;
#else
        public SabreTools.Models.NewExecutable.NonResidentNameTableEntry?[]? NonResidentNameTable => this.Model.NonResidentNameTable;
#endif

        #endregion

        #endregion

        #region Extension Properties

        // TODO: Determine what extension properties are needed

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public NewExecutable(SabreTools.Models.NewExecutable.Executable model, byte[] data, int offset)
#else
        public NewExecutable(SabreTools.Models.NewExecutable.Executable? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public NewExecutable(SabreTools.Models.NewExecutable.Executable model, Stream data)
#else
        public NewExecutable(SabreTools.Models.NewExecutable.Executable? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an NE executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NE executable wrapper on success, null on failure</returns>
#if NET48
        public static NewExecutable Create(byte[] data, int offset)
#else
        public static NewExecutable? Create(byte[]? data, int offset)
#endif
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create an NE executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An NE executable wrapper on success, null on failure</returns>
#if NET48
        public static NewExecutable Create(Stream data)
#else
        public static NewExecutable? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var executable = new SabreTools.Serialization.Streams.NewExecutable().Deserialize(data);
            if (executable == null)
                return null;

            try
            {
                return new NewExecutable(executable, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();
            Printing.NewExecutable.Print(builder, this.Model);
            return builder;
        }

        #endregion

        #region REMOVE -- DO NOT USE

        /// <summary>
        /// Read an arbitrary range from the source
        /// </summary>
        /// <param name="rangeStart">The start of where to read data from, -1 means start of source</param>
        /// <param name="length">How many bytes to read, -1 means read until end</param>
        /// <returns>Byte array representing the range, null on error</returns>
        [Obsolete]
#if NET48
        public byte[] ReadArbitraryRange(int rangeStart = -1, int length = -1)
#else
        public byte[]? ReadArbitraryRange(int rangeStart = -1, int length = -1)
#endif
        {
            // If we have an unset range start, read from the start of the source
            if (rangeStart == -1)
                rangeStart = 0;

            // If we have an unset length, read the whole source
            if (length == -1)
            {
                switch (_dataSource)
                {
                    case DataSource.ByteArray:
#if NET48
                        length = _byteArrayData.Length - _byteArrayOffset;
#else
                        length = _byteArrayData!.Length - _byteArrayOffset;
#endif
                        break;

                    case DataSource.Stream:
#if NET48
                        length = (int)_streamData.Length;
#else
                        length = (int)_streamData!.Length;
#endif
                        break;
                }
            }

            return ReadFromDataSource(rangeStart, length);
        }

        #endregion
    }
}