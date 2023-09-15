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
        public string Stub_Magic => _model.Stub.Header.Magic;
#else
        public string? Stub_Magic => _model.Stub?.Header?.Magic;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.LastPageBytes"/>
#if NET48
        public ushort Stub_LastPageBytes => _model.Stub.Header.LastPageBytes;
#else
        public ushort? Stub_LastPageBytes => _model.Stub?.Header?.LastPageBytes;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Pages"/>
#if NET48
        public ushort Stub_Pages => _model.Stub.Header.Pages;
#else
        public ushort? Stub_Pages => _model.Stub?.Header?.Pages;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationItems"/>
#if NET48
        public ushort Stub_RelocationItems => _model.Stub.Header.RelocationItems;
#else
        public ushort? Stub_RelocationItems => _model.Stub?.Header?.RelocationItems;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.HeaderParagraphSize"/>
#if NET48
        public ushort Stub_HeaderParagraphSize => _model.Stub.Header.HeaderParagraphSize;
#else
        public ushort? Stub_HeaderParagraphSize => _model.Stub?.Header?.HeaderParagraphSize;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MinimumExtraParagraphs"/>
#if NET48
        public ushort Stub_MinimumExtraParagraphs => _model.Stub.Header.MinimumExtraParagraphs;
#else
        public ushort? Stub_MinimumExtraParagraphs => _model.Stub?.Header?.MinimumExtraParagraphs;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MaximumExtraParagraphs"/>
#if NET48
        public ushort Stub_MaximumExtraParagraphs => _model.Stub.Header.MaximumExtraParagraphs;
#else
        public ushort? Stub_MaximumExtraParagraphs => _model.Stub?.Header?.MaximumExtraParagraphs;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSSValue"/>
#if NET48
        public ushort Stub_InitialSSValue => _model.Stub.Header.InitialSSValue;
#else
        public ushort? Stub_InitialSSValue => _model.Stub?.Header?.InitialSSValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSPValue"/>
#if NET48
        public ushort Stub_InitialSPValue => _model.Stub.Header.InitialSPValue;
#else
        public ushort? Stub_InitialSPValue => _model.Stub?.Header?.InitialSPValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Checksum"/>
#if NET48
        public ushort Stub_Checksum => _model.Stub.Header.Checksum;
#else
        public ushort? Stub_Checksum => _model.Stub?.Header?.Checksum;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialIPValue"/>
#if NET48
        public ushort Stub_InitialIPValue => _model.Stub.Header.InitialIPValue;
#else
        public ushort? Stub_InitialIPValue => _model.Stub?.Header?.InitialIPValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialCSValue"/>
#if NET48
        public ushort Stub_InitialCSValue => _model.Stub.Header.InitialCSValue;
#else
        public ushort? Stub_InitialCSValue => _model.Stub?.Header?.InitialCSValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationTableAddr"/>
#if NET48
        public ushort Stub_RelocationTableAddr => _model.Stub.Header.RelocationTableAddr;
#else
        public ushort? Stub_RelocationTableAddr => _model.Stub?.Header?.RelocationTableAddr;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OverlayNumber"/>
#if NET48
        public ushort Stub_OverlayNumber => _model.Stub.Header.OverlayNumber;
#else
        public ushort? Stub_OverlayNumber => _model.Stub?.Header?.OverlayNumber;
#endif

        #endregion

        #region PE Extensions

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved1"/>
#if NET48
        public ushort[] Stub_Reserved1 => _model.Stub.Header.Reserved1;
#else
        public ushort[]? Stub_Reserved1 => _model.Stub?.Header?.Reserved1;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMIdentifier"/>
#if NET48
        public ushort Stub_OEMIdentifier => _model.Stub.Header.OEMIdentifier;
#else
        public ushort? Stub_OEMIdentifier => _model.Stub?.Header?.OEMIdentifier;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMInformation"/>
#if NET48
        public ushort Stub_OEMInformation => _model.Stub.Header.OEMInformation;
#else
        public ushort? Stub_OEMInformation => _model.Stub?.Header?.OEMInformation;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved2"/>
#if NET48
        public ushort[] Stub_Reserved2 => _model.Stub.Header.Reserved2;
#else
        public ushort[]? Stub_Reserved2 => _model.Stub?.Header?.Reserved2;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
#if NET48
        public uint Stub_NewExeHeaderAddr => _model.Stub.Header.NewExeHeaderAddr;
#else
        public uint? Stub_NewExeHeaderAddr => _model.Stub?.Header?.NewExeHeaderAddr;
#endif

        #endregion

        #endregion

        #region Header

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.Magic"/>
#if NET48
        public string Magic => _model.Header.Magic;
#else
        public string? Magic => _model.Header?.Magic;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.LinkerVersion"/>
#if NET48
        public byte LinkerVersion => _model.Header.LinkerVersion;
#else
        public byte? LinkerVersion => _model.Header?.LinkerVersion;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.LinkerRevision"/>
#if NET48
        public byte LinkerRevision => _model.Header.LinkerRevision;
#else
        public byte? LinkerRevision => _model.Header?.LinkerRevision;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.EntryTableOffset"/>
#if NET48
        public ushort EntryTableOffset => _model.Header.EntryTableOffset;
#else
        public ushort? EntryTableOffset => _model.Header?.EntryTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.EntryTableSize"/>
#if NET48
        public ushort EntryTableSize => _model.Header.EntryTableSize;
#else
        public ushort? EntryTableSize => _model.Header?.EntryTableSize;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.CrcChecksum"/>
#if NET48
        public uint CrcChecksum => _model.Header.CrcChecksum;
#else
        public uint? CrcChecksum => _model.Header?.CrcChecksum;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.FlagWord"/>
#if NET48
        public SabreTools.Models.NewExecutable.HeaderFlag FlagWord => _model.Header.FlagWord;
#else
        public SabreTools.Models.NewExecutable.HeaderFlag? FlagWord => _model.Header?.FlagWord;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.AutomaticDataSegmentNumber"/>
#if NET48
        public ushort AutomaticDataSegmentNumber => _model.Header.AutomaticDataSegmentNumber;
#else
        public ushort? AutomaticDataSegmentNumber => _model.Header?.AutomaticDataSegmentNumber;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.InitialHeapAlloc"/>
#if NET48
        public ushort InitialHeapAlloc => _model.Header.InitialHeapAlloc;
#else
        public ushort? InitialHeapAlloc => _model.Header?.InitialHeapAlloc;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.InitialStackAlloc"/>
#if NET48
        public ushort InitialStackAlloc => _model.Header.InitialStackAlloc;
#else
        public ushort? InitialStackAlloc => _model.Header?.InitialStackAlloc;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.InitialCSIPSetting"/>
#if NET48
        public uint InitialCSIPSetting => _model.Header.InitialCSIPSetting;
#else
        public uint? InitialCSIPSetting => _model.Header?.InitialCSIPSetting;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.InitialSSSPSetting"/>
#if NET48
        public uint InitialSSSPSetting => _model.Header.InitialSSSPSetting;
#else
        public uint? InitialSSSPSetting => _model.Header?.InitialSSSPSetting;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.FileSegmentCount"/>
#if NET48
        public ushort FileSegmentCount => _model.Header.FileSegmentCount;
#else
        public ushort? FileSegmentCount => _model.Header?.FileSegmentCount;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ModuleReferenceTableSize"/>
#if NET48
        public ushort ModuleReferenceTableSize => _model.Header.ModuleReferenceTableSize;
#else
        public ushort? ModuleReferenceTableSize => _model.Header?.ModuleReferenceTableSize;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.NonResidentNameTableSize"/>
#if NET48
        public ushort NonResidentNameTableSize => _model.Header.NonResidentNameTableSize;
#else
        public ushort? NonResidentNameTableSize => _model.Header?.NonResidentNameTableSize;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.SegmentTableOffset"/>
#if NET48
        public ushort SegmentTableOffset => _model.Header.SegmentTableOffset;
#else
        public ushort? SegmentTableOffset => _model.Header?.SegmentTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ResourceTableOffset"/>
#if NET48
        public ushort ResourceTableOffset => _model.Header.ResourceTableOffset;
#else
        public ushort? ResourceTableOffset => _model.Header?.ResourceTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ResidentNameTableOffset"/>
#if NET48
        public ushort ResidentNameTableOffset => _model.Header.ResidentNameTableOffset;
#else
        public ushort? ResidentNameTableOffset => _model.Header?.ResidentNameTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ModuleReferenceTableOffset"/>
#if NET48
        public ushort ModuleReferenceTableOffset => _model.Header.ModuleReferenceTableOffset;
#else
        public ushort? ModuleReferenceTableOffset => _model.Header?.ModuleReferenceTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ImportedNamesTableOffset"/>
#if NET48
        public ushort ImportedNamesTableOffset => _model.Header.ImportedNamesTableOffset;
#else
        public ushort? ImportedNamesTableOffset => _model.Header?.ImportedNamesTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.NonResidentNamesTableOffset"/>
#if NET48
        public uint NonResidentNamesTableOffset => _model.Header.NonResidentNamesTableOffset;
#else
        public uint? NonResidentNamesTableOffset => _model.Header?.NonResidentNamesTableOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.MovableEntriesCount"/>
#if NET48
        public ushort MovableEntriesCount => _model.Header.MovableEntriesCount;
#else
        public ushort? MovableEntriesCount => _model.Header?.MovableEntriesCount;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.SegmentAlignmentShiftCount"/>
#if NET48
        public ushort SegmentAlignmentShiftCount => _model.Header.SegmentAlignmentShiftCount;
#else
        public ushort? SegmentAlignmentShiftCount => _model.Header?.SegmentAlignmentShiftCount;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ResourceEntriesCount"/>
#if NET48
        public ushort ResourceEntriesCount => _model.Header.ResourceEntriesCount;
#else
        public ushort? ResourceEntriesCount => _model.Header?.ResourceEntriesCount;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.TargetOperatingSystem"/>
#if NET48
        public SabreTools.Models.NewExecutable.OperatingSystem TargetOperatingSystem => _model.Header.TargetOperatingSystem;
#else
        public SabreTools.Models.NewExecutable.OperatingSystem? TargetOperatingSystem => _model.Header?.TargetOperatingSystem;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.AdditionalFlags"/>
#if NET48
        public SabreTools.Models.NewExecutable.OS2Flag AdditionalFlags => _model.Header.AdditionalFlags;
#else
        public SabreTools.Models.NewExecutable.OS2Flag? AdditionalFlags => _model.Header?.AdditionalFlags;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.ReturnThunkOffset"/>
#if NET48
        public ushort ReturnThunkOffset => _model.Header.ReturnThunkOffset;
#else
        public ushort? ReturnThunkOffset => _model.Header?.ReturnThunkOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.SegmentReferenceThunkOffset"/>
#if NET48
        public ushort SegmentReferenceThunkOffset => _model.Header.SegmentReferenceThunkOffset;
#else
        public ushort? SegmentReferenceThunkOffset => _model.Header?.SegmentReferenceThunkOffset;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.MinCodeSwapAreaSize"/>
#if NET48
        public ushort MinCodeSwapAreaSize => _model.Header.MinCodeSwapAreaSize;
#else
        public ushort? MinCodeSwapAreaSize => _model.Header?.MinCodeSwapAreaSize;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.WindowsSDKRevision"/>
#if NET48
        public byte WindowsSDKRevision => _model.Header.WindowsSDKRevision;
#else
        public byte? WindowsSDKRevision => _model.Header?.WindowsSDKRevision;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ExecutableHeader.WindowsSDKVersion"/>
#if NET48
        public byte WindowsSDKVersion => _model.Header.WindowsSDKVersion;
#else
        public byte? WindowsSDKVersion => _model.Header?.WindowsSDKVersion;
#endif

        #endregion

        #region Tables

        /// <inheritdoc cref="Models.NewExecutable.SegmentTable"/>
#if NET48
        public SabreTools.Models.NewExecutable.SegmentTableEntry[] SegmentTable => _model.SegmentTable;
#else
        public SabreTools.Models.NewExecutable.SegmentTableEntry?[]? SegmentTable => _model.SegmentTable;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ResourceTable"/>
#if NET48
        public SabreTools.Models.NewExecutable.ResourceTable ResourceTable => _model.ResourceTable;
#else
        public SabreTools.Models.NewExecutable.ResourceTable? ResourceTable => _model.ResourceTable;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ResidentNameTable"/>
#if NET48
        public SabreTools.Models.NewExecutable.ResidentNameTableEntry[] ResidentNameTable => _model.ResidentNameTable;
#else
        public SabreTools.Models.NewExecutable.ResidentNameTableEntry?[]? ResidentNameTable => _model.ResidentNameTable;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ModuleReferenceTable"/>
#if NET48
        public SabreTools.Models.NewExecutable.ModuleReferenceTableEntry[] ModuleReferenceTable => _model.ModuleReferenceTable;
#else
        public SabreTools.Models.NewExecutable.ModuleReferenceTableEntry?[]? ModuleReferenceTable => _model.ModuleReferenceTable;
#endif

        /// <inheritdoc cref="Models.NewExecutable.ImportedNameTable"/>
#if NET48
        public Dictionary<ushort, SabreTools.Models.NewExecutable.ImportedNameTableEntry> ImportedNameTable => _model.ImportedNameTable;
#else
        public Dictionary<ushort, SabreTools.Models.NewExecutable.ImportedNameTableEntry?>? ImportedNameTable => _model.ImportedNameTable;
#endif

        /// <inheritdoc cref="Models.NewExecutable.EntryTable"/>
#if NET48
        public SabreTools.Models.NewExecutable.EntryTableBundle[] EntryTable => _model.EntryTable;
#else
        public SabreTools.Models.NewExecutable.EntryTableBundle?[]? EntryTable => _model.EntryTable;
#endif

        /// <inheritdoc cref="Models.NewExecutable.NonResidentNameTable"/>
#if NET48
        public SabreTools.Models.NewExecutable.NonResidentNameTableEntry[] NonResidentNameTable => _model.NonResidentNameTable;
#else
        public SabreTools.Models.NewExecutable.NonResidentNameTableEntry?[]? NonResidentNameTable => _model.NonResidentNameTable;
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
            Printing.NewExecutable.Print(builder, _model);
            return builder;
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

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