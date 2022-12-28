namespace BurnOutSharp.Models.Compression.Quantum
{
    /// <remarks>
    /// Strings are prefixed with their length. If the length is less than
    /// 128 then it is stored directly in one byte. If it is greater than 127
    /// then the high bit of the first byte is set to 1 and the remaining
    /// fifteen bits contain the actual length in big-endian format. 
    /// </remarks>
    public class FileDescriptor
    {
        /// <summary>
        /// File name, variable length string, not zero-terminated
        /// </summary>
        public string FileName;

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