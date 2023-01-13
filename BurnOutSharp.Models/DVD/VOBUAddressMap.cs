namespace BurnOutSharp.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo.html"/>
    public sealed class VOBUAddressMap
    {
        /// <summary>
        /// End address (last byte of last entry)
        /// </summary>
        public uint EndAddress;

        /// <summary>
        /// Starting sector within VOB of nth VOBU
        /// </summary>
        public uint[] StartingSectors;
    }
}