namespace BinaryObjectScanner.Models.VPK
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VPKFile.h"/>
    public sealed class DirectoryItem
    {
        public string Extension;

        public string Path;

        public string Name;

        public DirectoryEntry DirectoryEntry;

        public byte[] PreloadData;
    }
}
