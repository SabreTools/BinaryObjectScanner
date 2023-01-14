namespace BurnOutSharp.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    public sealed class CommonHeader
    {
        /// <summary>
        /// "ISc("
        /// </summary>
        public string Signature;

        /// <summary>
        /// Encoded version
        /// </summary>
        public uint Version;

        /// <summary>
        /// Volume information
        /// </summary>
        public uint VolumeInfo;

        /// <summary>
        /// Offset to cabinet descriptor
        /// </summary>
        public uint DescriptorOffset;

        /// <summary>
        /// Cabinet descriptor size
        /// </summary>
        public uint DescriptorSize;
    }
}