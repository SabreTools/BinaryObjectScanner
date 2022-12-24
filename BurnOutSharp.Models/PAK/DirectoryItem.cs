namespace BurnOutSharp.Models.PAK
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/PAKFile.h"/>
    public sealed class DirectoryItem
    {
        /// <summary>
        /// Item Name
        /// </summary>
        public string ItemName;

        /// <summary>
        /// Item Offset
        /// </summary>
        public uint ItemOffset;

        /// <summary>
        /// Item Length
        /// </summary>
        public uint ItemLength;
    }
}
