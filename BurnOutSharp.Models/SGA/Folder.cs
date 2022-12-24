namespace BurnOutSharp.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public class Folder<T>
    {
        public uint NameOffset;

        public T FolderStartIndex;

        public T FolderEndIndex;

        public T FileStartIndex;

        public T FileEndIndex;
    }
}
