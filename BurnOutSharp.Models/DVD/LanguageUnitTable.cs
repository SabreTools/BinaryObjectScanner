namespace BurnOutSharp.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html"/>
    public sealed class LanguageUnitTable
    {
        /// <summary>
        /// Number of Language Units
        /// </summary>
        public ushort NumberOfLanguageUnits;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved;

        /// <summary>
        /// End address (last byte of last PGC in last LU)
        /// relative to VMGM_PGCI_UT
        /// </summary>
        public uint EndAddress;

        /// <summary>
        /// Language Units
        /// </summary>
        public LanguageUnitTableEntry[] Entries;

        /// <summary>
        /// Program Chains
        /// </summary>
        public ProgramChainTable[] ProgramChains;
    }
}