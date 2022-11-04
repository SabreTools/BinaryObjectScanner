using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.LinearExecutable
{
    /// <summary>
    /// The resident and non-resident name tables define the ASCII names and ordinal
    /// numbers for exported entries in the module. In addition the first entry in
    /// the resident name table contains the module name. These tables are used to
    /// translate a procedure name string into an ordinal number by searching for a
    /// matching name string. The ordinal number is used to locate the entry point
    /// information in the entry table.
    /// 
    /// The resident name table is kept resident in system memory while the module is
    /// loaded.It is intended to contain the exported entry point names that are
    /// frequently dynamically linked to by name.Non-resident names are not kept in
    /// memory and are read from the EXE file when a dynamic link reference is made.
    /// Exported entry point names that are infrequently dynamically linked to by name
    /// or are commonly referenced by ordinal number should be placed in the
    /// non-resident name table.The trade off made for references by name is performance
    /// vs memory usage.
    /// 
    /// Import references by name require these tables to be searched to obtain the entry
    /// point ordinal number.Import references by ordinal number provide the fastest
    /// lookup since the search of these tables is not required.
    /// 
    /// The strings are CASE SENSITIVE and are NOT NULL TERMINATED. 
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public class NonResidentNameTableEntry
    {
        /// <summary>
        /// String Length.
        /// </summary>
        /// <remarks>
        /// This defines the length of the string in bytes. A zero length indicates there are
        /// no more entries in table. The length of each ascii name string is limited to 127
        /// characters.
        /// 
        /// The high bit in the LEN field (bit 7) is defined as an Overload bit. This bit
        /// signifies that additional information is contained in the linear EXE module and
        /// will be used in the future for parameter type checking.
        /// </remarks>
        public byte Length;

        /// <summary>
        /// ASCII String.
        /// </summary>
        /// <remarks>
        /// This is a variable length string with it's length defined in bytes by the LEN field.
        /// The string is case case sensitive and is not null terminated.
        /// </remarks>
        public byte[] Name;

        /// <summary>
        /// Ordinal number.
        /// </summary>
        /// <remarks>
        /// The ordinal number in an ordered index into the entry table for this entry point.
        /// </remarks>
        public ushort OrdinalNumber;
    }
}
