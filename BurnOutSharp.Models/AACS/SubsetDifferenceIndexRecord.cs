namespace BurnOutSharp.Models.AACS
{
    /// <summary>
    /// This is a speed-up record which can be ignored by devices not wishing to
    /// take advantage of it. It is a lookup table which allows devices to quickly
    /// find their subset-difference in the Explicit Subset-Difference record,
    /// without processing the entire record. This Subset-Difference Index record
    /// is always present, and always precedes the Explicit Subset-Difference record
    /// in the MKB, although it does not necessarily immediately precede it.
    /// </summary>
    /// <see href="http://web.archive.org/web/20180718234519/https://aacsla.com/jp/marketplace/evaluating/aacs_technical_overview_040721.pdf"/>
    public sealed class SubsetDifferenceIndexRecord : Record
    {
        /// <summary>
        /// The number of devices per index offset.
        /// </summary>
        public uint Span;

        /// <summary>
        /// These offsets refer to the offset within the following Explicit
        /// Subset-Difference record, with 0 being the start of the record.
        /// </summary>
        // <remarks>UInt24 not UInt32</remarks>
        public uint[] Offsets;
    }
}