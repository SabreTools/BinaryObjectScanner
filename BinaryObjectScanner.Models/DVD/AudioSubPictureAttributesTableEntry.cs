namespace BinaryObjectScanner.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo.html"/>
    public sealed class AudioSubPictureAttributesTableEntry
    {
        /// <summary>
        /// End address (EA)
        /// </summary>
        public uint EndAddress;

        /// <summary>
        /// VTS_CAT (copy of offset 022-025 of the VTS IFO file)
        /// 0=unspecified, 1=Karaoke
        /// </summary>
        public uint Category;

        /// <summary>
        /// Copy of VTS attributes (offset 100 and on from the VTS IFO
        /// file, usually 0x300 bytes long)
        /// </summary>
        public byte[] AttributesCopy;
    }
}