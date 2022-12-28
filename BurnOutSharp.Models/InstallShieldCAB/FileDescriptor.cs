namespace BurnOutSharp.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    public sealed class FileDescriptor
    {
        public uint NameOffset;

        public string Name;

        public uint DirectoryIndex;

        public FileFlags Flags;

        public ulong ExpandedSize;

        public ulong CompressedSize;

        public ulong DataOffset;

        public byte[] MD5;

        public ushort Volume;

        public uint LinkPrevious;

        public uint LinkNext;

        public LinkFlags LinkFlags;
    }
}