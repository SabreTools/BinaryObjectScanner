namespace BurnOutSharp.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    public sealed class CabDescriptor
    {
        public byte[] Reserved0;

        public uint FileTableOffset;

        public byte[] Reserved1;

        public uint FileTableSize;

        public uint FileTableSize2;

        public uint DirectoryCount;

        public byte[] Reserved2;

        public uint FileCount;

        public uint FileTableOffset2;

        public byte[] Reserved3;

        public uint[] FileGroupOffsets;

        public uint[] ComponentOffsets;
    }
}