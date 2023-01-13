namespace BurnOutSharp.Models.DVD
{
    /// <summary>
    /// The VMG_PTL_MAIT is searched by country, and points to
    /// the table for each country.
    /// </summary>
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html"/>
    public sealed class ParentalManagementMasksTable
    {
        /// <summary>
        /// Number of countries
        /// </summary>
        public ushort NumberOfCountries;

        /// <summary>
        /// Number of title sets (NTs)
        /// </summary>
        public ushort NumberOfTitleSets;

        /// <summary>
        /// End address (last byte of last PTL_MAIT)
        /// </summary>
        public uint EndAddress;

        /// <summary>
        /// Entries
        /// </summary>
        public ParentalManagementMasksTableEntry[] Entries;

        /// <summary>
        /// The PTL_MAIT contains the 16-bit masks for the VMG and
        /// all title sets for parental management level 8 followed
        /// by the masks for level 7, and so on to level 1.
        /// </summary>
        public byte[][] BitMasks;
    }
}