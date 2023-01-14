namespace BurnOutSharp.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    public sealed class FileDescriptor
    {
        /// <summary>
        /// Offset to the file descriptor name
        /// </summary>
        public uint NameOffset;

        /// <summary>
        /// File descriptor name
        /// </summary>
        public string Name;

        /// <summary>
        /// Directory index
        /// </summary>
        public uint DirectoryIndex;

        /// <summary>
        /// File flags
        /// </summary>
        public FileFlags Flags;

        /// <summary>
        /// Size of the entry when expanded
        /// </summary>
        public ulong ExpandedSize;

        /// <summary>
        /// Size of the entry when compressed
        /// </summary>
        public ulong CompressedSize;

        /// <summary>
        /// Offset to the entry data
        /// </summary>
        public ulong DataOffset;

        /// <summary>
        /// MD5 of the entry data
        /// </summary>
        public byte[] MD5;

        /// <summary>
        /// Volume number
        /// </summary>
        public ushort Volume;

        /// <summary>
        /// Link previous
        /// </summary>
        public uint LinkPrevious;

        /// <summary>
        /// Link next
        /// </summary>
        public uint LinkNext;

        /// <summary>
        /// Link flags
        /// </summary>
        public LinkFlags LinkFlags;
    }
}