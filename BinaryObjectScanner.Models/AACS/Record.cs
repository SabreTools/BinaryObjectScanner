namespace BinaryObjectScanner.Models.AACS
{
    /// <summary>
    /// Each Record begins with a one-byte Record Type field, followed by a
    /// three-byte Record Length field. 
    /// 
    /// The following subsections describe the currently defined Record types,
    /// and how a device processes each. All multi-byte integers, including
    /// the length field, are “Big Endian”; in other words, the most significant
    /// byte comes first in the record.
    /// </summary>
    /// <see href="https://aacsla.com/wp-content/uploads/2019/02/AACS_Spec_Common_Final_0953.pdf"/>
    public abstract class Record
    {
        /// <summary>
        /// The Record Type field value indicates the type of the Record.
        /// </summary>
        public RecordType RecordType;

        /// <summary>
        /// The Record Length field value indicates the number of bytes in
        /// the Record, including the Record Type and the Record Length
        /// fields themselves. Record lengths are always multiples of 4 bytes.
        /// </summary>
        // <remarks>UInt24 not UInt32</remarks>
        public uint RecordLength;
    }
}