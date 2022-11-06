using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// A leaf's Type, Name, and Language IDs are determined by the path that is
    /// taken through directory tables to reach the leaf. The first table
    /// determines Type ID, the second table (pointed to by the directory entry
    /// in the first table) determines Name ID, and the third table determines
    /// Language ID.
    /// 
    /// The directory entries make up the rows of a table. Each resource directory
    /// entry has the following format. Whether the entry is a Name or ID entry
    /// is indicated by the resource directory table, which indicates how many
    /// Name and ID entries follow it (remember that all the Name entries precede
    /// all the ID entries for the table). All entries for the table are sorted
    /// in ascending order: the Name entries by case-sensitive string and the ID
    /// entries by numeric value. Offsets are relative to the address in the
    /// IMAGE_DIRECTORY_ENTRY_RESOURCE DataDirectory.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    [StructLayout(LayoutKind.Explicit)]
    public class ResourceDirectoryEntry
    {
        /// <summary>
        /// The offset of a string that gives the Type, Name, or Language ID entry,
        /// depending on level of table.
        /// </summary>
        [FieldOffset(0)] public uint NameOffset;

        /// <summary>
        /// A 32-bit integer that identifies the Type, Name, or Language ID entry.
        /// </summary>
        [FieldOffset(0)] public uint IntegerID;

        /// <summary>
        /// High bit 0. Address of a Resource Data entry (a leaf).
        /// </summary>
        [FieldOffset(4)] public uint DataEntryOffset;

        /// <summary>
        /// High bit 1. The lower 31 bits are the address of another resource
        /// directory table (the next level down).
        /// </summary>
        [FieldOffset(4)] public uint SubdirectoryOffset;
    }
}
