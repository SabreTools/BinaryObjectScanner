using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.LinearExecutable
{
    /// <summary>
    /// The object table contains information that describes each segment in
    /// an executable file. This information includes segment length, segment
    /// type, and segment-relocation data. The following list summarizes the
    /// values found in in the segment table (the locations are relative to
    /// the beginning of each entry):
    /// </summary>
    /// <remarks>
    /// Entries in the Object Table are numbered starting from one.
    /// </remarks>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ObjectTableEntry
    {
        /// <summary>
        /// Virtual memory size.
        /// </summary>
        /// <remarks>
        /// This is the size of the object that will be allocated when the object
        /// is loaded. The object's virtual size (rounded up to the page size value)
        /// must be greater than or equal to the total size of the pages in the EXE
        /// file for the object. This memory size must also be large enough to
        /// contain all of the iterated data and uninitialized data in the EXE file. 
        /// </remarks>
        public uint VirtualSegmentSize;

        /// <summary>
        /// Relocation Base Address.
        /// </summary>
        /// <remarks>
        /// The relocation base address the object is currently relocated to. If the
        /// internal relocation fixups for the module have been removed, this is the
        /// address the object will be allocated at by the loader.
        /// </remarks>
        public uint RelocationBaseAddress;

        /// <summary>
        /// Flag bits for the object.
        /// </summary>
        public ObjectFlags ObjectFlags;

        /// <summary>
        /// Object Page Table Index.
        /// </summary>
        /// <remarks>
        /// This specifies the number of the first object page table entry for this object.
        /// The object page table specifies where in the EXE file a page can be found for
        /// a given object and specifies per-page attributes.
        /// 
        /// The object table entries are ordered by logical page in the object table. In
        /// other words the object table entries are sorted based on the object page table
        /// index value.
        /// </remarks>
        public uint PageTableIndex;

        /// <summary>
        /// # of object page table entries for this object.
        /// </summary>
        /// <remarks>
        /// Any logical pages at the end of an object that do not have an entry in the object
        /// page table associated with them are handled as zero filled or invalid pages by
        /// the loader.
        /// 
        /// When the last logical pages of an object are not specified with an object page
        /// table entry, they are treated as either zero filled pages or invalid pages based
        /// on the last entry in the object page table for that object. If the last entry
        /// was neither a zero filled or invalid page, then the additional pages are treated
        /// as zero filled pages.
        /// </remarks>
        public uint PageTableEntries;

        /// <summary>
        /// Reserved for future use. Must be set to zero.
        /// </summary>
        public uint Reserved;
    }
}
