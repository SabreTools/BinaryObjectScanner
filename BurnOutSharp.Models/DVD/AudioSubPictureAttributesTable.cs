namespace BurnOutSharp.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo.html"/>
    public sealed class AudioSubPictureAttributesTable
    {
        /// <summary>
        /// Number of title sets
        /// </summary>
        public ushort NumberOfTitleSets;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved;

        /// <summary>
        /// End address (last byte of last VTS_ATRT)
        /// </summary>
        public uint EndAddress;

        /// <summary>
        /// Offset to VTS_ATRT n
        /// </summary>
        public uint[] Offsets;

        /// <summary>
        /// Entries
        /// </summary>
        public AudioSubPictureAttributesTableEntry[] Entries;
    }
}