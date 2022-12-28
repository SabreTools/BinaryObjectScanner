namespace BurnOutSharp.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    public sealed class OffsetList
    {
        public uint NameOffset;

        public string Name;

        public uint DescriptorOffset;

        public uint NextOffset;
    }
}