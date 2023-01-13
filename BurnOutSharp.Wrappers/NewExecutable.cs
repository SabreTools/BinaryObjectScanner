using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static BurnOutSharp.Builders.Extensions;

namespace BurnOutSharp.Wrappers
{
    public class NewExecutable : WrapperBase
    {
        #region Pass-Through Properties

        #region MS-DOS Stub

        #region Standard Fields

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Magic"/>
        public string Stub_Magic => _executable.Stub.Header.Magic;

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
        public ushort Stub_InitialSPValue => _executable.Stub.Header.InitialSPValue;

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
        public string Magic => _executable.Header.Magic;

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

        #region Constructors

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
        public static NewExecutable Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var executable = Builders.NewExecutable.ParseExecutable(data);
            if (executable == null)
                return null;

            var wrapper = new NewExecutable
            {
                _executable = executable,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("New Executable Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Stub
            PrintStubHeader(builder);
            PrintStubExtendedHeader(builder);

            // Header
            PrintHeader(builder);

            // Tables
            PrintSegmentTable(builder);
            PrintResourceTable(builder);
            PrintResidentNameTable(builder);
            PrintModuleReferenceTable(builder);
            PrintImportedNameTable(builder);
            PrintEntryTable(builder);
            PrintNonresidentNameTable(builder);

            return builder;
        }

        /// <summary>
        /// Print stub header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintStubHeader(StringBuilder builder)
        {
            builder.AppendLine("  MS-DOS Stub Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Magic number: {Stub_Magic}");
            builder.AppendLine($"  Last page bytes: {Stub_LastPageBytes} (0x{Stub_LastPageBytes:X})");
            builder.AppendLine($"  Pages: {Stub_Pages} (0x{Stub_Pages:X})");
            builder.AppendLine($"  Relocation items: {Stub_RelocationItems} (0x{Stub_RelocationItems:X})");
            builder.AppendLine($"  Header paragraph size: {Stub_HeaderParagraphSize} (0x{Stub_HeaderParagraphSize:X})");
            builder.AppendLine($"  Minimum extra paragraphs: {Stub_MinimumExtraParagraphs} (0x{Stub_MinimumExtraParagraphs:X})");
            builder.AppendLine($"  Maximum extra paragraphs: {Stub_MaximumExtraParagraphs} (0x{Stub_MaximumExtraParagraphs:X})");
            builder.AppendLine($"  Initial SS value: {Stub_InitialSSValue} (0x{Stub_InitialSSValue:X})");
            builder.AppendLine($"  Initial SP value: {Stub_InitialSPValue} (0x{Stub_InitialSPValue:X})");
            builder.AppendLine($"  Checksum: {Stub_Checksum} (0x{Stub_Checksum:X})");
            builder.AppendLine($"  Initial IP value: {Stub_InitialIPValue} (0x{Stub_InitialIPValue:X})");
            builder.AppendLine($"  Initial CS value: {Stub_InitialCSValue} (0x{Stub_InitialCSValue:X})");
            builder.AppendLine($"  Relocation table address: {Stub_RelocationTableAddr} (0x{Stub_RelocationTableAddr:X})");
            builder.AppendLine($"  Overlay number: {Stub_OverlayNumber} (0x{Stub_OverlayNumber:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print stub extended header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintStubExtendedHeader(StringBuilder builder)
        {
            builder.AppendLine("  MS-DOS Stub Extended Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Reserved words: {string.Join(", ", Stub_Reserved1)}");
            builder.AppendLine($"  OEM identifier: {Stub_OEMIdentifier} (0x{Stub_OEMIdentifier:X})");
            builder.AppendLine($"  OEM information: {Stub_OEMInformation} (0x{Stub_OEMInformation:X})");
            builder.AppendLine($"  Reserved words: {string.Join(", ", Stub_Reserved2)}");
            builder.AppendLine($"  New EXE header address: {Stub_NewExeHeaderAddr} (0x{Stub_NewExeHeaderAddr:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintHeader(StringBuilder builder)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Magic number: {Magic}");
            builder.AppendLine($"  Linker version: {LinkerVersion} (0x{LinkerVersion:X})");
            builder.AppendLine($"  Linker revision: {LinkerRevision} (0x{LinkerRevision:X})");
            builder.AppendLine($"  Entry table offset: {EntryTableOffset} (0x{EntryTableOffset:X})");
            builder.AppendLine($"  Entry table size: {EntryTableSize} (0x{EntryTableSize:X})");
            builder.AppendLine($"  CRC checksum: {CrcChecksum} (0x{CrcChecksum:X})");
            builder.AppendLine($"  Flag word: {FlagWord} (0x{FlagWord:X})");
            builder.AppendLine($"  Automatic data segment number: {AutomaticDataSegmentNumber} (0x{AutomaticDataSegmentNumber:X})");
            builder.AppendLine($"  Initial heap allocation: {InitialHeapAlloc} (0x{InitialHeapAlloc:X})");
            builder.AppendLine($"  Initial stack allocation: {InitialStackAlloc} (0x{InitialStackAlloc:X})");
            builder.AppendLine($"  Initial CS:IP setting: {InitialCSIPSetting} (0x{InitialCSIPSetting:X})");
            builder.AppendLine($"  Initial SS:SP setting: {InitialSSSPSetting} (0x{InitialSSSPSetting:X})");
            builder.AppendLine($"  File segment count: {FileSegmentCount} (0x{FileSegmentCount:X})");
            builder.AppendLine($"  Module reference table size: {ModuleReferenceTableSize} (0x{ModuleReferenceTableSize:X})");
            builder.AppendLine($"  Non-resident name table size: {NonResidentNameTableSize} (0x{NonResidentNameTableSize:X})");
            builder.AppendLine($"  Segment table offset: {SegmentTableOffset} (0x{SegmentTableOffset:X})");
            builder.AppendLine($"  Resource table offset: {ResourceTableOffset} (0x{ResourceTableOffset:X})");
            builder.AppendLine($"  Resident name table offset: {ResidentNameTableOffset} (0x{ResidentNameTableOffset:X})");
            builder.AppendLine($"  Module reference table offset: {ModuleReferenceTableOffset} (0x{ModuleReferenceTableOffset:X})");
            builder.AppendLine($"  Imported names table offset: {ImportedNamesTableOffset} (0x{ImportedNamesTableOffset:X})");
            builder.AppendLine($"  Non-resident name table offset: {NonResidentNamesTableOffset} (0x{NonResidentNamesTableOffset:X})");
            builder.AppendLine($"  Moveable entries count: {MovableEntriesCount} (0x{MovableEntriesCount:X})");
            builder.AppendLine($"  Segment alignment shift count: {SegmentAlignmentShiftCount} (0x{SegmentAlignmentShiftCount:X})");
            builder.AppendLine($"  Resource entries count: {ResourceEntriesCount} (0x{ResourceEntriesCount:X})");
            builder.AppendLine($"  Target operating system: {TargetOperatingSystem} (0x{TargetOperatingSystem:X})");
            builder.AppendLine($"  Additional flags: {AdditionalFlags} (0x{AdditionalFlags:X})");
            builder.AppendLine($"  Return thunk offset: {ReturnThunkOffset} (0x{ReturnThunkOffset:X})");
            builder.AppendLine($"  Segment reference thunk offset: {SegmentReferenceThunkOffset} (0x{SegmentReferenceThunkOffset:X})");
            builder.AppendLine($"  Minimum code swap area size: {MinCodeSwapAreaSize} (0x{MinCodeSwapAreaSize:X})");
            builder.AppendLine($"  Windows SDK revision: {WindowsSDKRevision} (0x{WindowsSDKRevision:X})");
            builder.AppendLine($"  Windows SDK version: {WindowsSDKVersion} (0x{WindowsSDKVersion:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print segment table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintSegmentTable(StringBuilder builder)
        {
            builder.AppendLine("  Segment Table Information:");
            builder.AppendLine("  -------------------------");
            if (FileSegmentCount == 0 || SegmentTable.Length == 0)
            {
                builder.AppendLine("  No segment table items");
            }
            else
            {
                for (int i = 0; i < SegmentTable.Length; i++)
                {
                    var entry = SegmentTable[i];
                    builder.AppendLine($"  Segment Table Entry {i}");
                    builder.AppendLine($"    Offset: {entry.Offset} (0x{entry.Offset:X})");
                    builder.AppendLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                    builder.AppendLine($"    Flag word: {entry.FlagWord} (0x{entry.FlagWord:X})");
                    builder.AppendLine($"    Minimum allocation size: {entry.MinimumAllocationSize} (0x{entry.MinimumAllocationSize:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print resource table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintResourceTable(StringBuilder builder)
        {
            builder.AppendLine("  Resource Table Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Alignment shift count: {ResourceTable.AlignmentShiftCount} (0x{ResourceTable.AlignmentShiftCount:X})");
            if (ResourceEntriesCount == 0 || ResourceTable.ResourceTypes.Length == 0)
            {
                builder.AppendLine("  No resource table items");
            }
            else
            {
                for (int i = 0; i < ResourceTable.ResourceTypes.Length; i++)
                {
                    // TODO: If not integer type, print out name
                    var entry = ResourceTable.ResourceTypes[i];
                    builder.AppendLine($"  Resource Table Entry {i}");
                    builder.AppendLine($"    Type ID: {entry.TypeID} (0x{entry.TypeID:X}) (Is Integer Type: {entry.IsIntegerType()})");
                    builder.AppendLine($"    Resource count: {entry.ResourceCount} (0x{entry.ResourceCount:X})");
                    builder.AppendLine($"    Reserved: {entry.Reserved} (0x{entry.Reserved:X})");
                    builder.AppendLine($"    Resources = ");
                    if (entry.ResourceCount == 0 || entry.Resources.Length == 0)
                    {
                        builder.AppendLine("      No resource items");
                    }
                    else
                    {
                        for (int j = 0; j < entry.Resources.Length; j++)
                        {
                            // TODO: If not integer type, print out name
                            var resource = entry.Resources[j];
                            builder.AppendLine($"      Resource Entry {i}");
                            builder.AppendLine($"        Offset: {resource.Offset} (0x{resource.Offset:X})");
                            builder.AppendLine($"        Length: {resource.Length} (0x{resource.Length:X})");
                            builder.AppendLine($"        Flag word: {resource.FlagWord} (0x{resource.FlagWord:X})");
                            builder.AppendLine($"        Resource ID: {resource.ResourceID} (0x{resource.ResourceID:X}) (Is Integer Type: {resource.IsIntegerType()})");
                            builder.AppendLine($"        Reserved: {resource.Reserved} (0x{resource.Reserved:X})");
                        }
                    }
                }
            }

            if (ResourceTable.TypeAndNameStrings.Count == 0)
            {
                builder.AppendLine("  No resource table type/name strings");
            }
            else
            {
                foreach (var typeAndNameString in ResourceTable.TypeAndNameStrings)
                {
                    builder.AppendLine($"  Resource Type/Name Offset {typeAndNameString.Key}");
                    builder.AppendLine($"    Length: {typeAndNameString.Value.Length} (0x{typeAndNameString.Value.Length:X})");
                    builder.AppendLine($"    Text: {(typeAndNameString.Value.Text != null ? Encoding.ASCII.GetString(typeAndNameString.Value.Text).TrimEnd('\0') : "[EMPTY]")}");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print resident-name table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintResidentNameTable(StringBuilder builder)
        {
            builder.AppendLine("  Resident-Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (ResidentNameTableOffset == 0 || ResidentNameTable.Length == 0)
            {
                builder.AppendLine("  No resident-name table items");
            }
            else
            {
                for (int i = 0; i < ResidentNameTable.Length; i++)
                {
                    var entry = ResidentNameTable[i];
                    builder.AppendLine($"  Resident-Name Table Entry {i}");
                    builder.AppendLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                    builder.AppendLine($"    Name string: {(entry.NameString != null ? Encoding.ASCII.GetString(entry.NameString).TrimEnd('\0') : "[EMPTY]")}");
                    builder.AppendLine($"    Ordinal number: {entry.OrdinalNumber} (0x{entry.OrdinalNumber:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print module-reference table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintModuleReferenceTable(StringBuilder builder)
        {
            builder.AppendLine("  Module-Reference Table Information:");
            builder.AppendLine("  -------------------------");
            if (ModuleReferenceTableSize == 0 || ModuleReferenceTable.Length == 0)
            {
                builder.AppendLine("  No module-reference table items");
            }
            else
            {
                for (int i = 0; i < ModuleReferenceTable.Length; i++)
                {
                    // TODO: Read the imported names table and print value here
                    var entry = ModuleReferenceTable[i];
                    builder.AppendLine($"  Module-Reference Table Entry {i}");
                    builder.AppendLine($"    Offset: {entry.Offset} (adjusted to be {entry.Offset + Stub_NewExeHeaderAddr + ImportedNamesTableOffset})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print imported-name table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintImportedNameTable(StringBuilder builder)
        {
            builder.AppendLine("  Imported-Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (ImportedNamesTableOffset == 0 || ImportedNameTable.Count == 0)
            {
                builder.AppendLine("  No imported-name table items");
            }
            else
            {
                foreach (var entry in ImportedNameTable)
                {
                    builder.AppendLine($"  Imported-Name Table at Offset {entry.Key}");
                    builder.AppendLine($"    Length: {entry.Value.Length} (0x{entry.Value.Length:X})");
                    builder.AppendLine($"    Name string: {(entry.Value.NameString != null ? Encoding.ASCII.GetString(entry.Value.NameString) : "[EMPTY]")}");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print entry table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintEntryTable(StringBuilder builder)
        {
            builder.AppendLine("  Entry Table Information:");
            builder.AppendLine("  -------------------------");
            if (EntryTableSize == 0 || EntryTable.Length == 0)
            {
                builder.AppendLine("  No entry table items");
            }
            else
            {
                for (int i = 0; i < EntryTable.Length; i++)
                {
                    var entry = EntryTable[i];
                    builder.AppendLine($"  Entry Table Entry {i}");
                    builder.AppendLine($"    Entry count: {entry.EntryCount} (0x{entry.EntryCount:X})");
                    builder.AppendLine($"    Segment indicator: {entry.SegmentIndicator} (0x{entry.SegmentIndicator:X}) ({entry.GetEntryType()})");
                    switch (entry.GetEntryType())
                    {
                        case BurnOutSharp.Models.NewExecutable.SegmentEntryType.FixedSegment:
                            builder.AppendLine($"    Flag word: {entry.FixedFlagWord} (0x{entry.FixedFlagWord:X})");
                            builder.AppendLine($"    Offset: {entry.FixedOffset} (0x{entry.FixedOffset:X})");
                            break;
                        case BurnOutSharp.Models.NewExecutable.SegmentEntryType.MoveableSegment:
                            builder.AppendLine($"    Flag word: {entry.MoveableFlagWord} (0x{entry.MoveableFlagWord:X})");
                            builder.AppendLine($"    Reserved: {entry.MoveableReserved} (0x{entry.MoveableReserved:X})");
                            builder.AppendLine($"    Segment number: {entry.MoveableSegmentNumber} (0x{entry.MoveableSegmentNumber:X})");
                            builder.AppendLine($"    Offset: {entry.MoveableOffset} (0x{entry.MoveableOffset:X})");
                            break;
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print nonresident-name table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintNonresidentNameTable(StringBuilder builder)
        {
            builder.AppendLine("  Nonresident-Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (NonResidentNameTableSize == 0 || NonResidentNameTable.Length == 0)
            {
                builder.AppendLine("  No nonresident-name table items");
            }
            else
            {
                for (int i = 0; i < NonResidentNameTable.Length; i++)
                {
                    var entry = NonResidentNameTable[i];
                    builder.AppendLine($"  Nonresident-Name Table Entry {i}");
                    builder.AppendLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                    builder.AppendLine($"    Name string: {(entry.NameString != null ? Encoding.ASCII.GetString(entry.NameString) : "[EMPTY]")}");
                    builder.AppendLine($"    Ordinal number: {entry.OrdinalNumber} (0x{entry.OrdinalNumber:X})");
                }
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_executable, _jsonSerializerOptions);

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
        public byte[] ReadArbitraryRange(int rangeStart = -1, int length = -1)
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
                        length = _byteArrayData.Length - _byteArrayOffset;
                        break;

                    case DataSource.Stream:
                        length = (int)_streamData.Length;
                        break;
                }
            }

            return ReadFromDataSource(rangeStart, length);
        }

        #endregion
    }
}