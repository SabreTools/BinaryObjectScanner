namespace BurnOutSharp.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/libunshield.h"/>
    public sealed class Component
    {
        public uint NameOffset;

        public string Name;

        public ushort FileGroupCount;

        public uint FileGroupTableOffset;

        public string[] FileGroupNames;
    }
}