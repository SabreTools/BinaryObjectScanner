using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// The symbol table in this section is inherited from the traditional
    /// COFF format. It is distinct from Microsoft Visual C++ debug information.
    /// A file can contain both a COFF symbol table and Visual C++ debug
    /// information, and the two are kept separate. Some Microsoft tools use
    /// the symbol table for limited but important purposes, such as
    /// communicating COMDAT information to the linker. Section names and file
    /// names, as well as code and data symbols, are listed in the symbol table.
    /// 
    /// The location of the symbol table is indicated in the COFF header.
    /// 
    /// The symbol table is an array of records, each 18 bytes long. Each record
    /// is either a standard or auxiliary symbol-table record. A standard record
    /// defines a symbol or name.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    [StructLayout(LayoutKind.Explicit)]
    public class COFFSymbolTableEntry
    {
        /// <summary>
        /// An array of 8 bytes. This array is padded with nulls on the right if
        /// the name is less than 8 bytes long.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        [FieldOffset(0)] public byte[] ShortName;

        /// <summary>
        /// A field that is set to all zeros if the name is longer than 8 bytes.
        /// </summary>
        [FieldOffset(0)] public uint Zeroes;

        /// <summary>
        /// An offset into the string table.
        /// </summary>
        [FieldOffset(4)] public uint Offset;

        /// <summary>
        /// The value that is associated with the symbol. The interpretation of this
        /// field depends on SectionNumber and StorageClass. A typical meaning is the
        /// relocatable address.
        /// </summary>
        [FieldOffset(8)] public uint Value;

        /// <summary>
        /// The signed integer that identifies the section, using a one-based index
        /// into the section table. Some values have special meaning.
        /// </summary>
        [FieldOffset(12)] public ushort SectionNumber;

        /// <summary>
        /// A number that represents type. Microsoft tools set this field to 0x20
        /// (function) or 0x0 (not a function).
        /// </summary>
        [FieldOffset(14)] public SymbolType SymbolType;

        /// <summary>
        /// An enumerated value that represents storage class.
        /// </summary>
        [FieldOffset(16)] public StorageClass StorageClass;

        /// <summary>
        /// The number of auxiliary symbol table entries that follow this record.
        /// </summary>
        [FieldOffset(17)] public byte NumberOfAuxSymbols;
    }
}
