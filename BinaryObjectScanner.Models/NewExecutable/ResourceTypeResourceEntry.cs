using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.NewExecutable
{
    /// <summary>
    /// A table of resources for this type follows. The following is
    /// the format of each resource (8 bytes each):
    /// </summary>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ResourceTypeResourceEntry
    {
        /// <summary>
        /// File offset to the contents of the resource data,
        /// relative to beginning of file. The offset is in terms
        /// of the alignment shift count value specified at
        /// beginning of the resource table.
        /// </summary>
        public ushort Offset;

        /// <summary>
        /// Length of the resource in the file (in bytes).
        /// </summary>
        public ushort Length;

        /// <summary>
        /// Flag word.
        /// </summary>
        public ResourceTypeResourceFlag FlagWord;

        /// <summary>
        /// Resource ID. This is an integer type if the high-order
        /// bit is set (8000h), otherwise it is the offset to the
        /// resource string, the offset is relative to the
        /// beginning of the resource table.
        /// </summary>
        public ushort ResourceID;

        /// <summary>
        /// Reserved.
        /// </summary>
        public uint Reserved;
    }
}
