using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// The directory entries make up the rows of a table.
    /// Each resource directory entry has the following format.
    /// Whether the entry is a Name or ID entry is indicated by the
    /// resource directory table, which indicates how many Name and
    /// ID entries follow it (remember that all the Name entries
    /// precede all the ID entries for the table). All entries for
    /// the table are sorted in ascending order: the Name entries
    /// by case-sensitive string and the ID entries by numeric value.
    /// Offsets are relative to the address in the IMAGE_DIRECTORY_ENTRY_RESOURCE DataDirectory.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#resource-directory-entries</remarks>
    internal class ResourceDirectoryTableEntry
    {
        #region Name Entry

        /// <summary>
        /// The offset of a string that gives the Type, Name, or Language ID entry, depending on level of table.
        /// </summary>
        public uint NameOffset;

        /// <summary>
        /// A 32-bit integer that identifies the Type, Name, or Language ID entry.
        /// </summary>
        public uint IntegerId => NameOffset;

        /// <summary>
        /// High bit 0. Address of a Resource Data entry (a leaf).
        /// </summary>
        public uint DataEntryOffset;

        /// <summary>
        /// High bit 1. The lower 31 bits are the address of another resource directory table (the next level down).
        /// </summary>
        public uint SubdirectoryOffset => DataEntryOffset;

        #endregion

        /// <summary>
        /// Determine if an entry represents a leaf or another directory table
        /// </summary>
        public bool IsResourceDataEntry() => (DataEntryOffset & (1 << 31)) == 0;

        public static ResourceDirectoryTableEntry Deserialize(Stream stream)
        {
            var idte = new ResourceDirectoryTableEntry();

            idte.NameOffset = stream.ReadUInt32();
            idte.DataEntryOffset = stream.ReadUInt32();

            return idte;
        }

        public static ResourceDirectoryTableEntry Deserialize(byte[] content, int offset)
        {
            var idte = new ResourceDirectoryTableEntry();

            idte.NameOffset = BitConverter.ToUInt32(content, offset); offset += 4;
            idte.DataEntryOffset = BitConverter.ToUInt32(content, offset); offset += 4;

            return idte;
        }
    }
}