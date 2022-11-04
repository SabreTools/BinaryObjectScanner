using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.LinearExecutable
{
    /// <summary>
    /// The `information block` in the LE header contains the linker version number,
    /// length of various tables that further describe the executable file, the
    /// offsets from the beginning of the header to the beginning of these tables,
    /// the heap and stack sizes, and so on. The following list summarizes the
    /// contents of the header `information block` (the locations are relative to
    /// the beginning of the block):
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public class InformationBlock
    {
        /// <summary>
        /// Specifies the signature word 'LE' (4Ch 45H)
        /// </summary>
        public char[] Signature;

        /// <summary>
        /// Byte order
        /// </summary>
        public ByteOrder ByteOrder;

        /// <summary>
        /// Word order
        /// </summary>
        public WordOrder WordOrder;

        /// <summary>
        /// Executable format level
        /// </summary>
        public uint ExecutableFormatLevel;

        /// <summary>
        /// CPU type
        /// </summary>
        public CPUType CPUType;

        /// <summary>
        /// Target operating system
        /// </summary>
        public OperatingSystem TargetOperatingSystem;

        /// <summary>
        /// Module version
        /// </summary>
        public uint ModuleVersion;

        /// <summary>
        /// Module type flags
        /// </summary>
        public InformationBlockFlag ModuleTypeFlags;

        /// <summary>
        /// Number of memory pages
        /// </summary>
        public uint MemoryPageCount;

        /// <summary>
        /// Initial object CS number
        /// </summary>
        public uint InitialObjectCS;

        /// <summary>
        /// Initial EIP
        /// </summary>
        public uint InitialEIP;

        /// <summary>
        /// Initial object SS number
        /// </summary>
        public uint InitialObjectSS;

        /// <summary>
        /// Initial ESP
        /// </summary>
        public uint InitialESP;

        /// <summary>
        /// Memory page size
        /// </summary>
        public uint MemoryPageSize;

        /// <summary>
        /// Bytes on last page
        /// </summary>
        public uint BytesOnLastPage;

        /// <summary>
        /// Fix-up section size
        /// </summary>
        public uint FixupSectionSize;

        /// <summary>
        /// Fix-up section checksum
        /// </summary>
        public uint FixupSectionChecksum;

        /// <summary>
        /// Loader section size
        /// </summary>
        public uint LoaderSectionSize;

        /// <summary>
        /// Loader section checksum
        /// </summary>
        public uint LoaderSectionChecksum;

        /// <summary>
        /// Offset of object table
        /// </summary>
        public uint ObjectTableOffset;

        /// <summary>
        /// Object table entries
        /// </summary>
        public uint ObjectTableCount;

        /// <summary>
        /// Object page map offset
        /// </summary>
        public uint ObjectPageMapOffset;

        /// <summary>
        /// Object iterate data map offset
        /// </summary>
        public uint ObjectIterateDataMapOffset;

        /// <summary>
        /// Resource table offset
        /// </summary>
        public uint ResourceTableOffset;

        /// <summary>
        /// Resource table entries
        /// </summary>
        public uint ResourceTableCount;

        /// <summary>
        /// Resident names table offset
        /// </summary>
        public uint ResidentNamesTableOffset;

        /// <summary>
        /// Entry table offset
        /// </summary>
        public uint EntryTableOffset;

        /// <summary>
        /// Module directives table offset
        /// </summary>
        public uint ModuleDirectivesTableOffset;

        /// <summary>
        /// Module directives entries
        /// </summary>
        public uint ModuleDirectivesCount;

        /// <summary>
        /// Fix-up page table offset
        /// </summary>
        public uint FixupPageTableOffset;

        /// <summary>
        /// Fix-up record table offset
        /// </summary>
        public uint FixupRecordTableOffset;

        /// <summary>
        /// Imported modules name table offset
        /// </summary>
        public uint ImportedModulesNameTableOffset;

        /// <summary>
        /// Imported modules count
        /// </summary>
        public uint ImportedModulesCount;

        /// <summary>
        /// Imported procedure name table offset
        /// </summary>
        public uint ImportedProcedureNameTableOffset;

        /// <summary>
        /// Per-page checksum table offset
        /// </summary>
        public uint PerPageChecksumTableOffset;

        /// <summary>
        /// Data pages offset from top of file
        /// </summary>
        public uint DataPagesOffset;

        /// <summary>
        /// Preload page count
        /// </summary>
        public uint PreloadPageCount;

        /// <summary>
        /// Non-resident names table offset from top of file
        /// </summary>
        public uint NonresidentNamesTableOffset;

        /// <summary>
        /// Non-resident names table length
        /// </summary>
        public uint NonresidentNamesTableLength;

        /// <summary>
        /// Non-resident names table checksum
        /// </summary>
        public uint NonresidentNamesTableChecksum;

        /// <summary>
        /// Automatic data object
        /// </summary>
        public uint AutomaticDataObject;

        /// <summary>
        /// Debug information offset
        /// </summary>
        public uint DebugInformationOffset;

        /// <summary>
        /// Debug information length
        /// </summary>
        public uint DebugInformationLength;

        /// <summary>
        /// Preload instance pages number
        /// </summary>
        public uint PreloadInstancePagesNumber;

        /// <summary>
        /// Demand instance pages number
        /// </summary>
        public uint DemandInstancePagesNumber;

        /// <summary>
        /// Extra heap allocation
        /// </summary>
        public uint ExtraHeapAllocation;

        /// <summary>
        /// ???
        /// </summary>
        public uint Reserved;
    }
}
