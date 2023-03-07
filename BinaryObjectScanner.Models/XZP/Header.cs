namespace BinaryObjectScanner.Models.XZP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/XZPFile.h"/>
    public sealed class Header
    {
        public string Signature;

        public uint Version;

        public uint PreloadDirectoryEntryCount;

        public uint DirectoryEntryCount;

        public uint PreloadBytes;

        public uint HeaderLength;

        public uint DirectoryItemCount;

        public uint DirectoryItemOffset;

        public uint DirectoryItemLength;
    }
}
