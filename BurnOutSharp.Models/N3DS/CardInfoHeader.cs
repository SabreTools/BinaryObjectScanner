namespace BurnOutSharp.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCSD#Card_Info_Header"/>
    public sealed class CardInfoHeader
    {
        /// <summary>
        /// CARD2: Writable Address In Media Units (For 'On-Chip' Savedata). CARD1: Always 0xFFFFFFFF.
        /// </summary>
        public uint WritableAddressMediaUnits;

        /// <summary>
        /// Card Info Bitmask
        /// </summary>
        public uint CardInfoBitmask;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved1;

        /// <summary>
        /// Filled size of cartridge
        /// </summary>
        public uint FilledSize;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved2;

        /// <summary>
        /// Title version
        /// </summary>
        public ushort TitleVersion;

        /// <summary>
        /// Card revision
        /// </summary>
        public ushort CardRevision;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved3;

        /// <summary>
        /// Title ID of CVer in included update partition
        /// </summary>
        public byte[] CVerTitleID;

        /// <summary>
        /// Version number of CVer in included update partition
        /// </summary>
        public ushort CVerVersionNumber;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved4;
    }
}
