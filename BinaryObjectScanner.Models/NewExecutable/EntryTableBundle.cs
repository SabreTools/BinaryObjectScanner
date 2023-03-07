namespace BinaryObjectScanner.Models.NewExecutable
{
    /// <summary>
    /// The entry table follows the imported-name table. This table contains
    /// bundles of entry-point definitions. Bundling is done to save space in
    /// the entry table. The entry table is accessed by an ordinal value.
    /// Ordinal number one is defined to index the first entry in the entry
    /// table. To find an entry point, the bundles are scanned searching for a
    /// specific entry point using an ordinal number. The ordinal number is
    /// adjusted as each bundle is checked. When the bundle that contains the
    /// entry point is found, the ordinal number is multiplied by the size of
    /// the bundle's entries to index the proper entry.
    /// </summary>
    /// <remarks>
    /// The linker forms bundles in the most dense manner it can, under the
    /// restriction that it cannot reorder entry points to improve bundling.
    /// The reason for this restriction is that other .EXE files may refer to
    /// entry points within this bundle by their ordinal number. The following
    /// describes the format of the entry table bundles.
    /// </remarks>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    public sealed class EntryTableBundle
    {
        /// <summary>
        /// Number of entries in this bundle. All records in one bundle
        /// are either moveable or refer to the same fixed segment. A zero
        /// value in this field indicates the end of the entry table.
        /// </summary>
        public byte EntryCount;

        /// <summary>
        /// Segment indicator for this bundle. This defines the type of
        /// entry table entry data within the bundle. There are three
        /// types of entries that are defined.
        /// - 000h = Unused entries. There is no entry data in an unused
        ///          bundle. The next bundle follows this field. This is
        ///          used by the linker to skip ordinal numbers.
        /// - 001h-0FEh = Segment number for fixed segment entries. A fixed
        ///          segment entry is 3 bytes long.
        /// - 0FFH = Moveable segment entries. The entry data contains the
        ///          segment number for the entry points. A moveable segment
        ///          entry is 6 bytes long.
        /// </summary>
        public byte SegmentIndicator;

        #region Fixed Segment Entry

        /// <summary>
        /// Flag word.
        /// </summary>
        public FixedSegmentEntryFlag FixedFlagWord;

        /// <summary>
        /// Offset within segment to entry point.
        /// </summary>
        public ushort FixedOffset;

        #endregion

        #region Moveable Segment Entry

        /// <summary>
        /// Flag word.
        /// </summary>
        public MoveableSegmentEntryFlag MoveableFlagWord;

        /// <summary>
        /// INT 3FH.
        /// </summary>
        public ushort MoveableReserved;

        /// <summary>
        /// Segment number.
        /// </summary>
        public byte MoveableSegmentNumber;

        /// <summary>
        /// Offset within segment to entry point.
        /// </summary>
        public ushort MoveableOffset;

        #endregion
    }
}
