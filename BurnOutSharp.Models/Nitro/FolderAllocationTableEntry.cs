namespace BurnOutSharp.Models.Nitro
{
    /// <summary>
    /// Each folder in the file system has a 8 byte long entry.
    /// The first one is for the root folder, and acts as an entry
    /// point to the file system.
    /// </summary>
    /// <see href="https://github.com/Deijin27/RanseiLink/wiki/NDS-File-System"/>
    public sealed class FolderAllocationTableEntry
    {
        /// <summary>
        /// Start offset of folder contents within Name List
        /// relative to start of NameTable
        /// </summary>
        public uint StartOffset;

        /// <summary>
        /// Index of first file within folder in File Allocation Table
        /// </summary>
        public ushort FirstFileIndex;

        /// <summary>
        /// Index of parent folder in current table; for root folder
        /// this holds the number of entries in the table
        /// </summary>
        public byte ParentFolderIndex;

        /// <summary>
        /// Unknown, always 0xF0 except for root folder where it is 0x00
        /// </summary>
        public byte Unknown;
    }
}