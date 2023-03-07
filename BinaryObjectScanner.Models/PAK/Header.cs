namespace BinaryObjectScanner.Models.PAK
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/PAKFile.h"/>
    public sealed class Header
    {
        /// <summary>
        /// Signature
        /// </summary>
        public string Signature;

        /// <summary>
        /// Directory Offset
        /// </summary>
        public uint DirectoryOffset;

        /// <summary>
        /// Directory Length
        /// </summary>
        public uint DirectoryLength;
    }
}
