namespace BurnOutSharp.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    public sealed class CommonHeader
    {
        public uint Signature;

        public uint Version;

        public uint VolumeInfo;

        public uint CabDescriptorOffset;

        public uint CabDescriptorSize;
    }
}