using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.NewExecutable
{
    /// <summary>
    /// The resident-name table follows the resource table, and contains this
    /// module's name string and resident exported procedure name strings. The
    /// first string in this table is this module's name. These name strings
    /// are case-sensitive and are not null-terminated. The following
    /// describes the format of the name strings:
    /// </summary>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ResidentNameTableEntry
    {
        /// <summary>
        /// Length of the name string that follows. A zero value indicates
        /// the end of the name table.
        /// </summary>
        public byte Length;

        /// <summary>
        /// ASCII text of the name string.
        /// </summary>
        public byte[] NameString;

        /// <summary>
        /// Ordinal number (index into entry table). This value is ignored
        /// for the module name.
        /// </summary>
        public ushort OrdinalNumber;
    }
}
