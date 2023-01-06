namespace BurnOutSharp.Models.N3DS
{
    /// <summary>
    /// There are 64 of these records, usually only the first is used.
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/Title_metadata#Content_Info_Records"/>
    public sealed class ContentInfoRecord
    {
        /// <summary>
        /// Content index offset
        /// </summary>
        public ushort ContentIndexOffset;

        /// <summary>
        /// Content command count [k]
        /// </summary>
        public ushort ContentCommandCount;

        /// <summary>
        /// SHA-256 hash of the next k content records that have not been hashed yet
        /// </summary>
        public byte[] UnhashedContentRecordsSHA256Hash;
    }
}