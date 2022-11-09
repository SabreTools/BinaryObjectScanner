using System.Diagnostics;
using System;
using System.Drawing;
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
        #region Standard COFF Symbol Table Entry

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

        #endregion

        #region Auxiliary Symbol Records

        // Auxiliary symbol table records always follow, and apply to, some standard
        // symbol table record. An auxiliary record can have any format that the tools
        // can recognize, but 18 bytes must be allocated for them so that symbol table
        // is maintained as an array of regular size. Currently, Microsoft tools
        // recognize auxiliary formats for the following kinds of records: function
        // definitions, function begin and end symbols (.bf and .ef), weak externals,
        // file names, and section definitions.
        // 
        // The traditional COFF design also includes auxiliary-record formats for arrays
        // and structures.Microsoft tools do not use these, but instead place that
        // symbolic information in Visual C++ debug format in the debug sections.

        #region Auxiliary Format 1: Function Definitions

        // A symbol table record marks the beginning of a function definition if it
        // has all of the following: a storage class of EXTERNAL (2), a Type value
        // that indicates it is a function (0x20), and a section number that is
        // greater than zero. Note that a symbol table record that has a section
        // number of UNDEFINED (0) does not define the function and does not have
        // an auxiliary record. Function-definition symbol records are followed by
        // an auxiliary record in the format described below:

        /// <summary>
        /// The symbol-table index of the corresponding .bf (begin function)
        /// symbol record.
        /// </summary>
        [FieldOffset(0)] public uint AuxFormat1TagIndex;

        /// <summary>
        /// The size of the executable code for the function itself. If the function
        /// is in its own section, the SizeOfRawData in the section header is greater
        /// or equal to this field, depending on alignment considerations.
        /// </summary>
        [FieldOffset(4)] public uint AuxFormat1TotalSize;

        /// <summary>
        /// The file offset of the first COFF line-number entry for the function, or
        /// zero if none exists.
        /// </summary>
        [FieldOffset(8)] public uint AuxFormat1PointerToLinenumber;

        /// <summary>
        /// The symbol-table index of the record for the next function. If the function
        /// is the last in the symbol table, this field is set to zero.
        /// </summary>
        [FieldOffset(12)] public uint AuxFormat1PointerToNextFunction;

        /// <summary>
        /// Unused
        /// </summary>
        [FieldOffset(16)] public ushort AuxFormat1Unused;

        #endregion

        #region Auxiliary Format 2: .bf and .ef Symbols

        // For each function definition in the symbol table, three items describe
        // the beginning, ending, and number of lines. Each of these symbols has
        // storage class FUNCTION (101):
        // 
        // A symbol record named .bf (begin function). The Value field is unused.
        // 
        // A symbol record named .lf (lines in function). The Value field gives the
        // number of lines in the function.
        //
        // A symbol record named .ef (end of function). The Value field has the same
        // number as the Total Size field in the function-definition symbol record.
        //
        // The .bf and .ef symbol records (but not .lf records) are followed by an
        // auxiliary record with the following format:

        /// <summary>
        /// Unused
        /// </summary>
        [FieldOffset(0)] public uint AuxFormat2Unused1;

        /// <summary>
        /// The actual ordinal line number (1, 2, 3, and so on) within the source file,
        /// corresponding to the .bf or .ef record.
        /// </summary>
        [FieldOffset(4)] public ushort AuxFormat2Linenumber;

        /// <summary>
        /// Unused
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        [FieldOffset(6)] public byte[] AuxFormat2Unused2;

        /// <summary>
        /// The symbol-table index of the next .bf symbol record. If the function is the
        /// last in the symbol table, this field is set to zero. It is not used for
        /// .ef records.
        /// </summary>
        [FieldOffset(12)] public uint AuxFormat2PointerToNextFunction;

        /// <summary>
        /// Unused
        /// </summary>
        [FieldOffset(12)] public ushort AuxFormat2Unused3;

        #endregion

        #region Auxiliary Format 3: Weak Externals

        // "Weak externals" are a mechanism for object files that allows flexibility at
        // link time. A module can contain an unresolved external symbol (sym1), but it
        // can also include an auxiliary record that indicates that if sym1 is not
        // present at link time, another external symbol (sym2) is used to resolve
        // references instead.
        // 
        // If a definition of sym1 is linked, then an external reference to the symbol
        // is resolved normally. If a definition of sym1 is not linked, then all references
        // to the weak external for sym1 refer to sym2 instead. The external symbol, sym2,
        // must always be linked; typically, it is defined in the module that contains
        // the weak reference to sym1.
        //
        // Weak externals are represented by a symbol table record with EXTERNAL storage
        // class, UNDEF section number, and a value of zero. The weak-external symbol
        // record is followed by an auxiliary record with the following format:

        /// <summary>
        /// The symbol-table index of sym2, the symbol to be linked if sym1 is not found. 
        /// </summary>
        [FieldOffset(0)] public uint AuxFormat3TagIndex;

        /// <summary>
        /// A value of IMAGE_WEAK_EXTERN_SEARCH_NOLIBRARY indicates that no library search
        /// for sym1 should be performed.
        /// A value of IMAGE_WEAK_EXTERN_SEARCH_LIBRARY indicates that a library search for
        /// sym1 should be performed.
        /// A value of IMAGE_WEAK_EXTERN_SEARCH_ALIAS indicates that sym1 is an alias for sym2.
        /// </summary>
        [FieldOffset(4)] public uint AuxFormat3Characteristics;

        /// <summary>
        /// Unused
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        [FieldOffset(8)] public byte[] AuxFormat3Unused;

        #endregion

        #region Auxiliary Format 4: Files

        // This format follows a symbol-table record with storage class FILE (103).
        // The symbol name itself should be .file, and the auxiliary record that
        // follows it gives the name of a source-code file.

        /// <summary>
        /// An ANSI string that gives the name of the source file. This is padded
        /// with nulls if it is less than the maximum length.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
        [FieldOffset(0)] public byte[] AuxFormat4FileName;

        #endregion

        #region Auxiliary Format 5: Section Definitions

        // This format follows a symbol-table record that defines a section. Such a
        // record has a symbol name that is the name of a section (such as .text or
        // .drectve) and has storage class STATIC (3). The auxiliary record provides
        // information about the section to which it refers. Thus, it duplicates some
        // of the information in the section header.

        /// <summary>
        /// The size of section data; the same as SizeOfRawData in the section header.
        /// </summary>
        [FieldOffset(0)] public uint AuxFormat5Length;

        /// <summary>
        /// The number of relocation entries for the section.
        /// </summary>
        [FieldOffset(4)] public ushort AuxFormat5NumberOfRelocations;

        /// <summary>
        /// The number of line-number entries for the section.
        /// </summary>
        [FieldOffset(6)] public ushort AuxFormat5NumberOfLinenumbers;

        /// <summary>
        /// The checksum for communal data. It is applicable if the IMAGE_SCN_LNK_COMDAT
        /// flag is set in the section header.
        /// </summary>
        [FieldOffset(8)] public uint AuxFormat5CheckSum;

        /// <summary>
        /// One-based index into the section table for the associated section. This is
        /// used when the COMDAT selection setting is 5.
        /// </summary>
        [FieldOffset(12)] public ushort AuxFormat5Number;

        /// <summary>
        /// The COMDAT selection number. This is applicable if the section is a
        /// COMDAT section.
        /// </summary>
        [FieldOffset(14)] public byte AuxFormat5Selection;

        /// <summary>
        /// Unused
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        [FieldOffset(15)] public byte[] AuxFormat5Unused;

        #endregion

        #region Auxiliary Format 6: CLR Token Definition (Object Only)

        // This auxiliary symbol generally follows the IMAGE_SYM_CLASS_CLR_TOKEN. It is
        // used to associate a token with the COFF symbol table's namespace.

        /// <summary>
        /// Must be IMAGE_AUX_SYMBOL_TYPE_TOKEN_DEF (1).
        /// </summary>
        [FieldOffset(0)] public byte AuxFormat6AuxType;

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        [FieldOffset(1)] public byte AuxFormat6Reserved1;

        /// <summary>
        /// The symbol index of the COFF symbol to which this CLR token definition refers.
        /// </summary>
        [FieldOffset(2)] public uint AuxFormat6SymbolTableIndex;

        /// <summary>
        /// Reserved, must be zero.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        [FieldOffset(6)] public byte[] AuxFormat6Reserved2;

        #endregion

        #endregion
    }
}
