using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.NewExecutable
{
    /// <summary>
    /// The module-reference table follows the resident-name table. Each entry
    /// contains an offset for the module-name string within the imported-
    /// names table; each entry is 2 bytes long.
    /// </summary>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ModuleReferenceTableEntry
    {
        /// <summary>
        /// Offset within Imported Names Table to referenced module name string.
        /// </summary>
        public ushort Offset;
    }
}
