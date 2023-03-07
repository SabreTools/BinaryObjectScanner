namespace BinaryObjectScanner.Models.N3DS
{
    /// <summary>
    /// There is one of these for each content contained in this title.
    /// (Determined by "Content Count" in the TMD Header).
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/Title_metadata#Content_chunk_records"/>
    public sealed class ContentChunkRecord
    {
        /// <summary>
        /// Content id
        /// </summary>
        public uint ContentId;

        /// <summary>
        /// Content index
        /// </summary>
        /// <remarks>
        /// This does not apply to DLC.
        /// </remarks>
        public ContentIndex ContentIndex;

        /// <summary>
        /// Content type
        /// </summary>
        public TMDContentType ContentType;

        /// <summary>
        /// Content size
        /// </summary>
        public ulong ContentSize;

        /// <summary>
        /// SHA-256 hash
        /// </summary>
        public byte[] SHA256Hash;
    }
}