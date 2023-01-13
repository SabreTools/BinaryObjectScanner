namespace BurnOutSharp.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo.html"/>
    public sealed class CellAddressTable
    {
        /// <summary>
        /// Number of VOB IDs
        /// </summary>
        public ushort NumberOfVOBIDs;

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
        public CellAddressTableEntry[] Entries;
    }
}