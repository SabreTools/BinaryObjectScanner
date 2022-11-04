using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.NewExecutable
{
    /// <summary>
    /// Segment number for fixed segment entries. A fixed
    /// segment entry is 3 bytes long and has the following
    /// format.
    /// </summary>
    /// <remarks>001h-0FEh</remarks>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public class FixedSegmentEntry
    {
        /// <summary>
        /// Flag word.
        /// </summary>
        public FixedSegmentEntryFlag FlagWord;

        /// <summary>
        /// Offset within segment to entry point.
        /// </summary>
        public ushort Offset;
    }
}
