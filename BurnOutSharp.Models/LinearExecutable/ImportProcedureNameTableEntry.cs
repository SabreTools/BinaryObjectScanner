using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.LinearExecutable
{
    /// <summary>
    /// The import procedure name table defines the procedure name strings imported
    /// by this module through dynamic link references. These strings are referenced
    /// through the imported relocation fixups.
    /// 
    /// To determine the length of the import procedure name table add the fixup
    /// section size to the fixup page table offset, this computes the offset to
    /// the end of the fixup section, then subtract the import procedure name table
    /// offset. These values are located in the linear EXE header. The import
    /// procedure name table is followed by the data pages section. Since the data
    /// pages section is aligned on a 'page size' boundary, padded space may exist
    /// between the last import name string and the first page in the data pages
    /// section. If this padded space exists it will be zero filled.
    /// 
    /// The strings are CASE SENSITIVE and NOT NULL TERMINATED.
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ImportModuleProcedureNameTableEntry
    {
        /// <summary>
        /// String Length.
        /// </summary>
        /// <remarks>
        /// This defines the length of the string in bytes. The length of each
        /// ascii name string is limited to 127 characters.
        /// 
        /// The high bit in the LEN field (bit 7) is defined as an Overload bit.
        /// This bit signifies that additional information is contained in the
        /// linear EXE module and will be used in the future for parameter type
        /// checking.
        /// </remarks>
        public byte Length;

        /// <summary>
        /// ASCII String.
        /// </summary>
        /// <remarks>
        /// This is a variable length string with it's length defined in bytes by
        /// the LEN field. The string is case sensitive and is not null terminated.
        /// </remarks>
        public byte[] Name;
    }
}
