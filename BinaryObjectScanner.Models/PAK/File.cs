namespace BinaryObjectScanner.Models.PAK
{
    /// <summary>
    /// Half-Life Package File
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/PAKFile.h"/>
    public sealed class File
    {
        /// <summary>
        /// Deserialized directory header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Deserialized directory items data
        /// </summary>
        public DirectoryItem[] DirectoryItems { get; set; }
    }
}
