using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.LinearExecutable
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
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class InformationBlock
    {
        /// <summary>
        /// Specifies the signature word
        /// 'LE' (4Ch 45H)
        /// 'LX' (4Ch 58H)
        /// </summary>
        /// <remarks>
        /// The signature word is used by the loader to identify the EXE
        /// file as a valid 32-bit Linear Executable Module Format.
        /// </remarks>
        public string Signature;

        /// <summary>
        /// Byte Ordering. 
        /// </summary>
        /// <remarks>
        /// This byte specifies the byte ordering for the linear EXE format.
        /// </remarks>
        public ByteOrder ByteOrder;

        /// <summary>
        /// Word Ordering.
        /// </summary>
        /// <remarks>
        /// This byte specifies the Word ordering for the linear EXE format.
        /// </remarks>
        public WordOrder WordOrder;

        /// <summary>
        /// Linear EXE Format Level.
        /// </summary>
        /// <remarks>
        /// The Linear EXE Format Level is set to 0 for the initial version of the
        /// 32-bit linear EXE format. Each incompatible change to the linear EXE
        /// format must increment this value. This allows the system to recognized
        /// future EXE file versions so that an appropriate error message may be
        /// displayed if an attempt is made to load them. 
        /// </remarks>
        public uint ExecutableFormatLevel;

        /// <summary>
        /// Module CPU Type.
        /// </summary>
        /// <remarks>
        /// This field specifies the type of CPU required by this module to run.
        /// </remarks>
        public CPUType CPUType;

        /// <summary>
        /// Module OS Type.
        /// </summary>
        /// <remarks>
        /// This field specifies the type of Operating system required to run this module.
        /// </remarks>
        public OperatingSystem ModuleOS;

        /// <summary>
        /// Module version
        /// </summary>
        /// <remarks>
        /// This is useful for differentiating between revisions of dynamic linked modules.
        /// This value is specified at link time by the user. 
        /// </remarks>
        public uint ModuleVersion;

        /// <summary>
        /// Module type flags
        /// </summary>
        public ModuleFlags ModuleTypeFlags;

        /// <summary>
        /// Number of pages in module.
        /// </summary>
        /// <remarks>
        /// This field specifies the number of pages physically contained in this module.
        /// In other words, pages containing either enumerated or iterated data, or
        /// zero-fill pages that have relocations, not invalid or zero-fill pages implied
        /// by the Virtual Size in the Object Table being larger than the number of pages
        /// actually in the linear EXE file. These pages are contained in the 'preload
        /// pages', 'demand load pages' and 'iterated data pages' sections of the linear
        /// EXE module. This is used to determine the size of the page information tables
        /// in the linear EXE module. 
        /// </remarks>
        public uint ModuleNumberPages;

        /// <summary>
        /// The Object number to which the Entry Address is relative.
        /// </summary>
        /// <remarks>
        /// This specifies the object to which the Entry Address is relative. This must be
        /// a nonzero value for a program module to be correctly loaded. A zero value for
        /// a library module indicates that no library entry routine exists. If this value
        /// is zero, then both the Per-process Library Initialization bit and the Per-process
        /// Library Termination bit must be clear in the module flags, or else the loader
        /// will fail to load the module. Further, if the Per-process Library Termination bit
        /// is set, then the object to which this field refers must be a 32-bit object (i.e.,
        /// the Big/Default bit must be set in the object flags; see below).
        /// </remarks>
        public uint InitialObjectCS;

        /// <summary>
        /// Entry Address of module.
        /// </summary>
        /// <remarks>
        /// The Entry Address is the starting address for program modules and the library
        /// initialization and Library termination address for library modules. 
        /// </remarks>
        public uint InitialEIP;

        /// <summary>
        /// The Object number to which the ESP is relative.
        /// </summary>
        /// <remarks>
        /// This specifies the object to which the starting ESP is relative. This must be a
        /// nonzero value for a program module to be correctly loaded. This field is ignored
        /// for a library module. 
        /// </remarks>
        public uint InitialObjectSS;

        /// <summary>
        /// Starting stack address of module.
        /// </summary>
        /// <remarks>
        /// The ESP defines the starting stack pointer address for program modules. A zero
        /// value in this field indicates that the stack pointer is to be initialized to the
        /// highest address/offset in the object. This field is ignored for a library module. 
        /// </remarks>
        public uint InitialESP;

        /// <summary>
        /// The size of one page for this system.
        /// </summary>
        /// <remarks>
        /// This field specifies the page size used by the linear EXE format and the system.
        /// For the initial version of this linear EXE format the page size is 4Kbytes.
        /// (The 4K page size is specified by a value of 4096 in this field.)
        /// </remarks>
        public uint MemoryPageSize;

        /// <summary>
        /// The shift left bits for page offsets.
        /// </summary>
        /// <remarks>
        /// This field gives the number of bit positions to shift left when interpreting
        /// the Object Page Table entries' page offset field. This determines the alignment
        /// of the page information in the file. For example, a value of 4 in this field
        /// would align all pages in the Data Pages and Iterated Pages sections on 16 byte
        /// (paragraph) boundaries. A Page Offset Shift of 9 would align all pages on a
        /// 512 byte (disk sector) basis. The default value for this field is 12 (decimal),
        /// which give a 4096 byte alignment. All other offsets are byte aligned.
        /// </remarks>
        public uint BytesOnLastPage;

        /// <summary>
        /// Total size of the fixup information in bytes.
        /// </summary>
        /// <remarks>
        /// This includes the following 4 tables:
        /// - Fixup Page Table
        /// - Fixup Record Table
        /// - Import Module name Table
        /// - Import Procedure Name Table
        /// </remarks>
        public uint FixupSectionSize;

        /// <summary>
        /// Checksum for fixup information.
        /// </summary>
        /// <remarks>
        /// This is a cryptographic checksum covering all of the fixup information. The
        /// checksum for the fixup information is kept separate because the fixup data is
        /// not always loaded into main memory with the 'loader section'. If the checksum
        /// feature is not implemented, then the linker will set these fields to zero.
        /// </remarks>
        public uint FixupSectionChecksum;

        /// <summary>
        /// Size of memory resident tables.
        /// </summary>
        /// <remarks>
        /// This is the total size in bytes of the tables required to be memory resident
        /// for the module, while the module is in use. This total size includes all
        /// tables from the Object Table down to and including the Per-Page Checksum Table.
        /// </remarks>
        public uint LoaderSectionSize;

        /// <summary>
        /// Checksum for loader section.
        /// </summary>
        /// <remarks>
        /// This is a cryptographic checksum covering all of the loader section information.
        /// If the checksum feature is not implemented, then the linker will set these fields
        /// to zero.
        /// </remarks>
        public uint LoaderSectionChecksum;

        /// <summary>
        /// Object Table offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint ObjectTableOffset;

        /// <summary>
        /// Object Table Count.
        /// </summary>
        /// <remarks>
        /// This defines the number of entries in Object Table.
        /// </remarks>
        public uint ObjectTableCount;

        /// <summary>
        /// Object Page Table offset
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint ObjectPageMapOffset;

        /// <summary>
        /// Object Iterated Pages offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint ObjectIterateDataMapOffset;

        /// <summary>
        /// Resource Table offset
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint ResourceTableOffset;

        /// <summary>
        /// Number of entries in Resource Table.
        /// </summary>
        public uint ResourceTableCount;

        /// <summary>
        /// Resident Name Table offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint ResidentNamesTableOffset;

        /// <summary>
        /// Entry Table offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint EntryTableOffset;

        /// <summary>
        /// Module Format Directives Table offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint ModuleDirectivesTableOffset;

        /// <summary>
        /// Number of Module Format Directives in the Table.
        /// </summary>
        /// <remarks>
        /// This field specifies the number of entries in the
        /// Module Format Directives Table.
        /// </remarks>
        public uint ModuleDirectivesCount;

        /// <summary>
        /// Fixup Page Table offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint FixupPageTableOffset;

        /// <summary>
        /// Fixup Record Table offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint FixupRecordTableOffset;

        /// <summary>
        /// Import Module Name Table offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint ImportedModulesNameTableOffset;

        /// <summary>
        /// The number of entries in the Import Module Name Table.
        /// </summary>
        public uint ImportedModulesCount;

        /// <summary>
        /// Import Procedure Name Table offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint ImportProcedureNameTableOffset;

        /// <summary>
        /// Per-page Checksum Table offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint PerPageChecksumTableOffset;

        /// <summary>
        /// Data Pages Offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the EXE file.
        /// </remarks>
        public uint DataPagesOffset;

        /// <summary>
        /// Number of Preload pages for this module.
        /// </summary>
        /// <remarks>
        /// Note that OS/2 2.0 does not respect the preload of pages as specified
        /// in the executable file for performance reasons.
        /// </remarks>
        public uint PreloadPageCount;

        /// <summary>
        /// Non-Resident Name Table offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the EXE file.
        /// </remarks>
        public uint NonResidentNamesTableOffset;

        /// <summary>
        /// Number of bytes in the Non-resident name table.
        /// </summary>
        public uint NonResidentNamesTableLength;

        /// <summary>
        /// Non-Resident Name Table Checksum.
        /// </summary>
        /// <remarks>
        /// This is a cryptographic checksum of the Non-Resident Name Table.
        /// </remarks>
        public uint NonResidentNamesTableChecksum;

        /// <summary>
        /// The Auto Data Segment Object number.
        /// </summary>
        /// <remarks>
        /// This is the object number for the Auto Data Segment used by 16-bit modules.
        /// This field is supported for 16-bit compatibility only and is not used by
        /// 32-bit modules.
        /// </remarks>
        public uint AutomaticDataObject;

        /// <summary>
        /// Debug Information offset.
        /// </summary>
        /// <remarks>
        /// This offset is relative to the beginning of the linear EXE header.
        /// </remarks>
        public uint DebugInformationOffset;

        /// <summary>
        /// Debug Information length.
        /// </summary>
        /// <remarks>
        /// The length of the debug information in bytes.
        /// </remarks>
        public uint DebugInformationLength;

        /// <summary>
        /// Instance pages in preload section.
        /// </summary>
        /// <remarks>
        /// The number of instance data pages found in the preload section.
        /// </remarks>
        public uint PreloadInstancePagesNumber;

        /// <summary>
        /// Instance pages in demand section.
        /// </summary>
        /// <remarks>
        /// The number of instance data pages found in the demand section.
        /// </remarks>
        public uint DemandInstancePagesNumber;

        /// <summary>
        /// Heap size added to the Auto DS Object.
        /// </summary>
        /// <remarks>
        /// The heap size is the  number of bytes added to the Auto Data Segment
        /// by the loader. This field is supported for 16-bit compatibility only and
        /// is not used by 32-bit modules.
        /// </remarks>
        public uint ExtraHeapAllocation;
    }
}
