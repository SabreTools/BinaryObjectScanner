using System;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.Headers;
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
        /// <summary>
        /// The offset of a string that gives the Type, Name, or Language ID entry, depending on level of table.
        /// </summary>
        public uint NameOffset => (uint)(IntegerId & (1 << 32));

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
        public uint SubdirectoryOffset => (uint)(DataEntryOffset & (1 << 32));

        /// <summary>
        /// Resource Data entry (a leaf).
        /// </summary>
        public ResourceDataEntry DataEntry;

        private bool NameFieldIsIntegerId = false;

        /// <summary>
        /// Determine if an entry has a name or integer identifier
        /// </summary>
        public bool IsIntegerIDEntry() => (NameOffset & (1 << 32)) == 0;

        /// <summary>
        /// Determine if an entry represents a leaf or another directory table
        /// </summary>
        public bool IsResourceDataEntry() => (DataEntryOffset & (1 << 32)) == 0;

        public static ResourceDirectoryTableEntry Deserialize(Stream stream, long sectionStart)
        {
            var rdte = new ResourceDirectoryTableEntry();

            rdte.IntegerId = stream.ReadUInt32();
            if (!rdte.IsIntegerIDEntry())
            {
                long lastPosition = stream.Position;
                int nameAddress = (int)(rdte.NameOffset + sectionStart);
                if (nameAddress >= 0 && nameAddress < stream.Length)
                {
                    try
                    {
                        stream.Seek(nameAddress, SeekOrigin.Begin);
                        rdte.Name = ResourceDirectoryString.Deserialize(stream);
                    }
                    catch { }
                    finally
                    {
                        stream.Seek(lastPosition, SeekOrigin.Begin);
                    }
                }
            }

            rdte.DataEntryOffset = stream.ReadUInt32();
            if (rdte.IsResourceDataEntry())
            {
                long lastPosition = stream.Position;
                int dataEntryAddress = (int)(rdte.DataEntryOffset + sectionStart);
                if (dataEntryAddress > 0 && dataEntryAddress < stream.Length)
                {
                    try
                    {
                        stream.Seek(dataEntryAddress, SeekOrigin.Begin);
                        rdte.DataEntry = ResourceDataEntry.Deserialize(stream);
                    }
                    catch { }
                    finally
                    {
                        stream.Seek(lastPosition, SeekOrigin.Begin);
                    }
                }
            }

            // TODO: Add parsing for further directory table entries in the tree

            return rdte;
        }

        public static ResourceDirectoryTableEntry Deserialize(byte[] content, int offset, long sectionStart)
        {
            var rdte = new ResourceDirectoryTableEntry();

            rdte.IntegerId = BitConverter.ToUInt32(content, offset); offset += 4;
            if (!rdte.IsIntegerIDEntry())
            {
                int nameAddress = (int)(rdte.NameOffset + sectionStart);
                if (nameAddress >= 0 && nameAddress < content.Length)
                {
                    try
                    {
                        rdte.Name = ResourceDirectoryString.Deserialize(content, nameAddress);
                    }
                    catch { }
                }
            }

            rdte.DataEntryOffset = BitConverter.ToUInt32(content, offset); offset += 4;
            if (rdte.IsResourceDataEntry())
            {
                int dataEntryAddress = (int)(rdte.DataEntryOffset + sectionStart);
                if (dataEntryAddress > 0 && dataEntryAddress < content.Length)
                {
                    try
                    {
                        rdte.DataEntry = ResourceDataEntry.Deserialize(content, dataEntryAddress);
                    }
                    catch { }
                }
            }

            // TODO: Add parsing for further directory table entries in the tree

            return rdte;
        }
    }
}