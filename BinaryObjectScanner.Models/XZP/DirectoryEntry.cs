namespace BinaryObjectScanner.Models.XZP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/XZPFile.h"/>
    public sealed class DirectoryEntry
    {
        public uint FileNameCRC;

        public uint EntryLength;

        public uint EntryOffset;
    }
}
