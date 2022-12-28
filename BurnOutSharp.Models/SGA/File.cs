namespace BurnOutSharp.Models.SGA
{
    /// <summary>
    /// SGA game archive
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public sealed class File
    {
        /// <summary>
        ///Header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Directory data
        /// </summary>
        public Directory Directory { get; set; }
    }
}
