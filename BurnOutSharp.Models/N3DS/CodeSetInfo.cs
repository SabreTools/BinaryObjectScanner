namespace BurnOutSharp.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header#Code_Set_Info"/>
    public sealed class CodeSetInfo
    {
        /// <summary>
        /// Address
        /// </summary>
        public byte[] Address;

        /// <summary>
        /// Physical region size (in page-multiples)
        /// </summary>
        public uint PhysicalRegionSizeInPages;

        /// <summary>
        /// Size (in bytes)
        /// </summary>
        public uint SizeInBytes;
    }
}
