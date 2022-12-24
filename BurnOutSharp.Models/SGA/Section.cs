namespace BurnOutSharp.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public class Section<T>
    {
        public string Alias;

        public string Name;

        public T FolderStartIndex;

        public T FolderEndIndex;

        public T FileStartIndex;

        public T FileEndIndex;

        public T FolderRootIndex;
    }
}
