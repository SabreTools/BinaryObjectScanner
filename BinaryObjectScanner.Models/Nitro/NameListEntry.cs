namespace BinaryObjectScanner.Models.Nitro
{
    /// <summary>
    /// The name list holds the names of the folders, and their contents
    /// in order, with references back to the Folder Allocation Table.
    /// </summary>
    /// <see href="https://github.com/Deijin27/RanseiLink/wiki/NDS-File-System"/>
    public sealed class NameListEntry
    {
        /// <summary>
        /// The least significant 7 bits store the length of the name,
        /// and the most significant bit indicates whether it is a
        /// folder (1 = folder, 0 = file).
        /// </summary>
        public bool Folder;

        /// <summary>
        /// The variable length name (UTF-8)
        /// </summary>
        public string Name;

        /// <summary>
        /// Only there if it is a folder. Refers to the the index of it
        /// within the Folder allocation table, allowing its contents to
        /// be found.
        /// </summary>
        public ushort Index;
    }
}