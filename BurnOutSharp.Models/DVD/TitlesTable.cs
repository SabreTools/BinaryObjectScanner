namespace BurnOutSharp.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html"/>
    public sealed class TitlesTable
    {
        /// <summary>
        /// Number of titles
        /// </summary>
        public ushort NumberOfTitles;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved;

        /// <summary>
        /// End address (last byte of last entry)
        /// </summary>
        public uint EndAddress;

        /// <summary>
        /// 12-byte entries
        /// </summary>
        public TitlesTableEntry[] Entries;
    }
}