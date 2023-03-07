using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.PortableExecutable
{
    /// <summary>
    /// COFF line numbers are no longer produced and, in the future, will
    /// not be consumed.
    /// 
    /// COFF line numbers indicate the relationship between code and line
    /// numbers in source files. The Microsoft format for COFF line numbers
    /// is similar to standard COFF, but it has been extended to allow a
    /// single section to relate to line numbers in multiple source files.
    /// 
    /// COFF line numbers consist of an array of fixed-length records.
    /// The location (file offset) and size of the array are specified in
    /// the section header.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    [StructLayout(LayoutKind.Explicit)]
    public sealed class COFFLineNumber
    {
        /// <summary>
        /// Used when Linenumber is zero: index to symbol table entry for a function.
        /// This format is used to indicate the function to which a group of
        /// line-number records refers.
        /// </summary>
        [FieldOffset(0)] public uint SymbolTableIndex;

        /// <summary>
        /// Used when Linenumber is non-zero: the RVA of the executable code that
        /// corresponds to the source line indicated. In an object file, this
        /// contains the VA within the section.
        /// </summary>
        [FieldOffset(0)] public uint VirtualAddress;

        /// <summary>
        /// When nonzero, this field specifies a one-based line number. When zero,
        /// the Type field is interpreted as a symbol table index for a function.
        /// </summary>
        [FieldOffset(4)] public ushort Linenumber;
    }
}
