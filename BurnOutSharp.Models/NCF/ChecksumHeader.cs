namespace BurnOutSharp.Models.NCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/NCFFile.h"/>
    public sealed class ChecksumHeader
    {
        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy0 { get; set; }

        /// <summary>
        /// Size of LPNCFCHECKSUMHEADER & LPNCFCHECKSUMMAPHEADER & in bytes.
        /// </summary>
        public uint ChecksumSize { get; set; }
    }
}
