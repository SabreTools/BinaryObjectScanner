using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.LinearExecutable
{
    /// <summary>
    /// The Object page table provides information about a logical page in an object.
    /// A logical page may be an enumerated page, a pseudo page or an iterated page.
    /// The structure of the object page table in conjunction with the structure of
    /// the object table allows for efficient access of a page when a page fault occurs,
    /// while still allowing the physical page data to be located in the preload page,
    /// demand load page or iterated data page sections in the linear EXE module. The
    /// logical page entries in the Object Page Table are numbered starting from one.
    /// The Object Page Table is parallel to the Fixup Page Table as they are both
    /// indexed by the logical page number. 
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ObjectPageTableEntry
    {
        /// <summary>
        /// Offset to the page data in the EXE file.
        /// </summary>
        /// <remarks>
        /// This field, when bit shifted left by the PAGE OFFSET SHIFT from the module
        /// header, specifies the offset from the beginning of the Preload Page section
        /// of the physical page data in the EXE file that corresponds to this logical
        /// page entry. The page data may reside in the Preload Pages, Demand Load Pages
        /// or the Iterated Data Pages sections.
        /// 
        /// If the FLAGS field specifies that this is a zero-Filled page then the PAGE
        /// DATA OFFSET field will contain a 0.
        /// 
        /// If the logical page is specified as an iterated data page, as indicated by
        /// the FLAGS field, then this field specifies the offset into the Iterated Data
        /// Pages section.
        /// 
        /// The logical page number (Object Page Table index), is used to index the Fixup
        /// Page Table to find any fixups associated with the logical page. 
        /// </remarks>
        public uint PageDataOffset;

        /// <summary>
        /// Number of bytes of data for this page.
        /// </summary>
        /// <remarks>
        /// This field specifies the actual number of bytes that represent the page in the
        /// file. If the PAGE SIZE field from the module header is greater than the value
        /// of this field and the FLAGS field indicates a Legal Physical Page, the remaining
        /// bytes are to be filled with zeros. If the FLAGS field indicates an Iterated Data
        /// Page, the iterated data records will completely fill out the remainder. 
        /// </remarks>
        public ushort DataSize;

        /// <summary>
        /// Attributes specifying characteristics of this logical page.
        /// </summary>
        public ObjectPageFlags Flags;
    }
}
