namespace BurnOutSharp.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class DirectoryHeader<T>
    {
        public uint SectionOffset;

        public T SectionCount;

        public uint FolderOffset;

        public T FolderCount;

        public uint FileOffset;

        public T FileCount;

        public uint StringTableOffset;

        public T StringTableCount;
    }
}
