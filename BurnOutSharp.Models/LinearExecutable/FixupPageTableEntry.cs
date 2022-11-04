using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.LinearExecutable
{
    /// <summary>
    /// The Fixup Page Table provides a simple mapping of a logical page number
    /// to an offset into the Fixup Record Table for that page.
    /// 
    /// This table is parallel to the Object Page Table, except that there is
    /// one additional entry in this table to indicate the end of the Fixup
    /// Record Table.
    /// 
    /// The fixup records are kept in order by logical page in the fixup record
    /// table. This allows the end of each page's fixup records is defined by the
    /// offset for the next logical page's fixup records. This last entry provides
    /// support of this mechanism for the last page in the fixup page table. 
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public class FixupPageTableEntry
    {
        /// <summary>
        /// Offset for fixup record for this page. (1 to n)
        /// Offset to the end of the fixup records. (n + 1)
        /// </summary>
        /// <remarks>
        /// This field specifies the offset, from the beginning of the fixup record
        /// table, to the first fixup record for this page. (1 to n)
        /// 
        /// This field specifies the offset following the last fixup record in the
        /// fixup record table. This is the last entry in the fixup page table. (n + 1)
        /// </remarks>
        public uint Offset;
    }
}
