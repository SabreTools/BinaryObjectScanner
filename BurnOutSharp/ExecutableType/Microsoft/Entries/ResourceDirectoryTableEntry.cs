using System;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.Headers;
using BurnOutSharp.ExecutableType.Microsoft.Tables;
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
    public class ResourceDirectoryTableEntry
    {
        /// <summary>
        /// The offset of a string that gives the Type, Name, or Language ID entry, depending on level of table.
        /// </summary>
        public uint NameOffset => (uint)(IntegerId ^ (1 << 31));

        /// <summary>
        /// The string that gives the Type, Name, or Language ID entry, depending on level of table pointed to by NameOffset
        /// </summary>
        public ResourceDirectoryString Name;

        /// <summary>
        /// A 32-bit integer that identifies the Type, Name, or Language ID entry.
        /// </summary>
        public uint IntegerId;

        /// <summary>
        /// High bit 0. Address of a Resource Data entry (a leaf).
        /// </summary>
        public uint DataEntryOffset;

        /// <summary>
        /// High bit 1. The lower 31 bits are the address of another resource directory table (the next level down).
        /// </summary>
        public uint SubdirectoryOffset => (uint)(DataEntryOffset ^ (1 << 31));

        /// <summary>
        /// Resource Data entry (a leaf).
        /// </summary>
        public ResourceDataEntry DataEntry;

        /// <summary>
        /// Another resource directory table (the next level down).
        /// </summary>
        public ResourceDirectoryTable Subdirectory;

        /// <summary>
        /// Determine if an entry has a name or integer identifier
        /// </summary>
        public bool IsIntegerIDEntry() => (IntegerId & (1 << 31)) == 0;

        /// <summary>
        /// Determine if an entry represents a leaf or another directory table
        /// </summary>
        public bool IsResourceDataEntry() => (DataEntryOffset & (1 << 31)) == 0;

        public static ResourceDirectoryTableEntry Deserialize(Stream stream, long sectionStart, SectionHeader[] sections)
        {
            var rdte = new ResourceDirectoryTableEntry();

            rdte.IntegerId = stream.ReadUInt32();
            if (!rdte.IsIntegerIDEntry())
            {
                int nameAddress = (int)(rdte.NameOffset + sectionStart);
                if (nameAddress >= 0 && nameAddress < stream.Length)
                {
                    long lastPosition = stream.Position;
                    stream.Seek(nameAddress, SeekOrigin.Begin);
                    rdte.Name = ResourceDirectoryString.Deserialize(stream);
                    stream.Seek(lastPosition, SeekOrigin.Begin);
                }
            }

            rdte.DataEntryOffset = stream.ReadUInt32();
            if (rdte.IsResourceDataEntry())
            {
                int dataEntryAddress = (int)(rdte.DataEntryOffset + sectionStart);
                if (dataEntryAddress > 0 && dataEntryAddress < stream.Length)
                {
                    long lastPosition = stream.Position;
                    stream.Seek(dataEntryAddress, SeekOrigin.Begin);
                    rdte.DataEntry = ResourceDataEntry.Deserialize(stream, sections);
                    stream.Seek(lastPosition, SeekOrigin.Begin);
                }
            }
            else
            {
                int subdirectoryAddress = (int)(rdte.SubdirectoryOffset + sectionStart);
                if (subdirectoryAddress > 0 && subdirectoryAddress < stream.Length)
                {
                    long lastPosition = stream.Position;
                    stream.Seek(subdirectoryAddress, SeekOrigin.Begin);
                    rdte.Subdirectory = ResourceDirectoryTable.Deserialize(stream, sectionStart, sections);
                    stream.Seek(lastPosition, SeekOrigin.Begin);
                }
            }

            return rdte;
        }

        public static ResourceDirectoryTableEntry Deserialize(byte[] content, ref int offset, long sectionStart, SectionHeader[] sections)
        {
            var rdte = new ResourceDirectoryTableEntry();

            rdte.IntegerId = content.ReadUInt32(ref offset);
            if (!rdte.IsIntegerIDEntry())
            {
                int nameAddress = (int)(rdte.NameOffset + sectionStart);
                if (nameAddress >= 0 && nameAddress < content.Length)
                    rdte.Name = ResourceDirectoryString.Deserialize(content, ref nameAddress);
            }

            rdte.DataEntryOffset = content.ReadUInt32(ref offset);
            if (rdte.IsResourceDataEntry())
            {
                int dataEntryAddress = (int)(rdte.DataEntryOffset + sectionStart);
                if (dataEntryAddress > 0 && dataEntryAddress < content.Length)
                    rdte.DataEntry = ResourceDataEntry.Deserialize(content, ref dataEntryAddress, sections);
            }
            else
            {
                int subdirectoryAddress = (int)(rdte.SubdirectoryOffset + sectionStart);
                if (subdirectoryAddress > 0 && subdirectoryAddress < content.Length)
                    rdte.Subdirectory = ResourceDirectoryTable.Deserialize(content, ref subdirectoryAddress, sectionStart, sections);
            }

            return rdte;
        }
    }
}