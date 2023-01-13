namespace BurnOutSharp.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html"/>
    public sealed class ProgramChainTable
    {
        /// <summary>
        /// Number of Program Chains
        /// </summary>
        public ushort NumberOfProgramChains;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved;

        /// <summary>
        /// End address (last byte of last PGC in this LU)
        /// relative to VMGM_LU
        /// </summary>
        public uint EndAddress;

        /// <summary>
        /// Program Chains
        /// </summary>
        public ProgramChainTableEntry[] Entries;
    }
}