using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.NewExecutable
{
    /// <summary>
    /// The segment table contains an entry for each segment in the executable
    /// file. The number of segment table entries are defined in the segmented
    /// EXE header. The first entry in the segment table is segment number 1.
    /// The following is the structure of a segment table entry.
    /// </summary>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public class SegmentTableEntry
    {
        /// <summary>
        /// Logical-sector offset (n byte) to the contents of the segment
        /// data, relative to the beginning of the file. Zero means no
        /// file data.
        /// </summary>
        public ushort Offset;

        /// <summary>
        /// Length of the segment in the file, in bytes. Zero means 64K.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// Flag word.
        /// </summary>
        public SegmentTableEntryFlag FlagWord;

        /// <summary>
        /// Minimum allocation size of the segment, in bytes. Total size
        /// of the segment. Zero means 64K.
        /// </summary>
        public ushort MinimumAllocationSize;
    }
}
