using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.NewExecutable
{
    /// <summary>
    /// Moveable segment entries. The entry data contains the
    /// segment number for the entry points. A moveable segment
    /// entry is 6 bytes long and has the following format.
    /// </summary>
    /// <remarks>0FFH</remarks>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public class MoveableSegmentEntry
    {
        /// <summary>
        /// Flag word.
        /// </summary>
        public MoveableSegmentEntryFlag FlagWord;

        /// <summary>
        /// INT 3FH.
        /// </summary>
        public ushort Reserved;

        /// <summary>
        /// Segment number.
        /// </summary>
        public byte SegmentNumber;

        /// <summary>
        /// Offset within segment to entry point.
        /// </summary>
        public ushort Offset;
    }
}
