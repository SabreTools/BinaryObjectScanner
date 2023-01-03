namespace BurnOutSharp.Models.Quantum
{
    /// <summary>
    /// Quantum archive file descriptor
    /// </summary>
    /// <see href="https://handwiki.org/wiki/Software:Quantum_compression"/>
    public class FileDescriptor
    {
        /// <summary>
        /// Length of file name
        /// </summary>
        public int FileNameSize;
        
        /// <summary>
        /// File name, variable length string, not zero-terminated
        /// </summary>
        public string FileName;

        /// <summary>
        /// Length of comment field
        /// </summary>
        public int CommentFieldSize;

        /// <summary>
        /// Comment field, variable length string, not zero-terminated
        /// </summary>
        public string CommentField;

        /// <summary>
        /// Fully expanded file size in bytes
        /// </summary>
        public uint ExpandedFileSize;

        /// <summary>
        /// File time (DOS format) 
        /// </summary>
        public ushort FileTime;

        /// <summary>
        /// File date (DOS format) 
        /// </summary>
        public ushort FileDate;
    }
}