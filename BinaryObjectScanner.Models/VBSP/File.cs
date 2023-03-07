namespace BinaryObjectScanner.Models.VBSP
{
    /// <summary>
    /// Half-Life 2 Level
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VBSPFile.h"/>
    public sealed class File
    {
        /// <summary>
        /// Directory header data
        /// </summary>
        public Header Header { get; set; }
    }
}