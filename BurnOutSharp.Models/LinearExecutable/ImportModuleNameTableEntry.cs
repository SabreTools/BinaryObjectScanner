using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.LinearExecutable
{
    /// <summary>
    /// The import module name table defines the module name strings imported through
    /// dynamic link references. These strings are referenced through the imported
    /// relocation fixups.
    /// 
    /// To determine the length of the import module name table subtract the import
    /// module name table offset from the import procedure name table offset. These
    /// values are located in the linear EXE header. The end of the import module
    /// name table is not terminated by a special character, it is followed directly
    /// by the import procedure name table.
    /// 
    /// The strings are CASE SENSITIVE and NOT NULL TERMINATED.
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public class ImportModuleNameTableEntry
    {
        /// <summary>
        /// String Length.
        /// </summary>
        /// <remarks>
        /// This defines the length of the string in bytes. The length of each
        /// ascii name string is limited to 127 characters.
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
