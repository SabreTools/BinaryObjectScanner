namespace BurnOutSharp.Models.NewExecutable
{
    /// <summary>
    /// The location and size of the per-segment data is defined in the
    /// segment table entry for the segment. If the segment has relocation
    /// fixups, as defined in the segment table entry flags, they directly
    /// follow the segment data in the file. The relocation fixup information
    /// is defined as follows:
    /// </summary>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    public class PerSegmentData
    {
        /// <summary>
        /// Number of relocation records that follow.
        /// </summary>
        public ushort RelocationRecordCount;

        /// <summary>
        /// A table of relocation records follows.
        /// </summary>
        public RelocationRecord[] RelocationRecords;
    }
}
