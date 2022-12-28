using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.MSDOS
{
    /// <summary>
    /// Each pointer in the relocation table looks as such
    /// </summary>
    /// <see href="https://wiki.osdev.org/MZ"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class RelocationEntry
    {
        /// <summary>
        /// Offset of the relocation within provided segment.
        /// </summary>
        public ushort Offset;

        /// <summary>
        /// Segment of the relocation, relative to the load segment address.
        /// </summary>
        public ushort Segment;
    }
}
