namespace BurnOutSharp.Models.VPK
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VPKFile.h"/>
    public sealed class Header
    {
        /// <summary>
        /// Always 0x55aa1234.
        /// </summary>
        public uint Signature;

        public uint Version;

        public uint DirectoryLength;
    }
}
