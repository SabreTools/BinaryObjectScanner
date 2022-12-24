namespace BurnOutSharp.Models.VPK
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VPKFile.h"/>
    public sealed class ArchiveHash
    {
        public uint ArchiveIndex;

        public uint ArchiveOffset;

        public uint Length;

        /// <summary>
        /// MD5
        /// </summary>
        public byte[] Hash;
    }
}
