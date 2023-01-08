namespace BurnOutSharp.Models.Nitro
{
    /// <summary>
    /// The structure of the file allocation table is very simple,
    /// it's just a table of 8 byte entries
    /// </summary>
    /// <see href="https://github.com/Deijin27/RanseiLink/wiki/NDS-File-System"/>
    public sealed class FileAllocationTableEntry
    {
        /// <summary>
        /// Start offset of file
        /// </summary>
        public uint StartOffset;

        /// <summary>
        /// End offset of file (after this is padding)
        /// </summary>
        public uint EndOffset;
    }
}