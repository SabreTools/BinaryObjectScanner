namespace BurnOutSharp.Models.GCF
{
    /// <remarks>
    /// Added in version 4 or version 5.
    /// </remarks>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    public sealed class DirectoryMapHeader
    {
        /// <summary>
        /// Always 0x00000001
        /// </summary>
        public uint Dummy0;

        /// <summary>
        /// Always 0x00000000
        /// </summary>
        public uint Dummy1;
    }
}