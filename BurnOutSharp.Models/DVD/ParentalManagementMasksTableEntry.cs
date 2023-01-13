namespace BurnOutSharp.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html"/>
    public sealed class ParentalManagementMasksTableEntry
    {
        /// <summary>
        /// Country code
        /// </summary>
        public ushort CountryCode;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved;

        /// <summary>
        /// Offset to PTL_MAIT
        /// </summary>
        public uint Offset;
    }
}