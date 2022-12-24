namespace BurnOutSharp.Models.GCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    public sealed class DirectoryMapEntry
    {
        /// <summary>
        /// Index of the first data block. (N/A if == BlockCount.)
        /// </summary>
        public uint FirstBlockIndex;
    }
}