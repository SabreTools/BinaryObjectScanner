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

        /// <summary>
        /// Resource Data entry (a leaf).
        /// </summary>
        public ResourceDataEntry DataEntry;

        /// <summary>
        /// Determine if an entry represents a leaf or another directory table
        /// </summary>
        public bool IsResourceDataEntry() => (DataEntryOffset & (1 << 31)) == 0;

        public static ResourceDirectoryTableEntry Deserialize(Stream stream, SectionHeader[] sections)
        {
            var rdte = new ResourceDirectoryTableEntry();

            rdte.NameOffset = stream.ReadUInt32();
            rdte.DataEntryOffset = stream.ReadUInt32();

            // Read in the data if we have a leaf
            if (rdte.IsResourceDataEntry())
            {
                long lastPosition = stream.Position;
                try
                {
                    int dataEntryAddress = (int)ConvertVirtualAddress(rdte.DataEntryOffset, sections);
                    if (dataEntryAddress > 0)
                    {
                        stream.Seek(dataEntryAddress, SeekOrigin.Begin);
                        rdte.DataEntry = ResourceDataEntry.Deserialize(stream);
                    }
                }
                catch { }
                finally
                {
                    stream.Seek(lastPosition, SeekOrigin.Begin);
                }
            }

            // TODO: Add parsing for further directory table entries in the tree

            return rdte;
        }

        public static ResourceDirectoryTableEntry Deserialize(byte[] content, int offset, SectionHeader[] sections)
        {
            var rdte = new ResourceDirectoryTableEntry();

            rdte.NameOffset = BitConverter.ToUInt32(content, offset); offset += 4;
            rdte.DataEntryOffset = BitConverter.ToUInt32(content, offset); offset += 4;

            // Read in the data if we have a leaf
            if (rdte.IsResourceDataEntry())
            {
                try
                {
                    int dataEntryAddress = (int)ConvertVirtualAddress(rdte.DataEntryOffset, sections);
                    if (dataEntryAddress > 0)
                        rdte.DataEntry = ResourceDataEntry.Deserialize(content, dataEntryAddress);
                }
                catch { }
            }

            // TODO: Add parsing for further directory table entries in the tree

            return rdte;
        }

        /// <summary>
        /// Convert a virtual address to a physical one
        /// </summary>
        /// <param name="virtualAddress">Virtual address to convert</param>
        /// <param name="sections">Array of sections to check against</param>
        /// <returns>Physical address, 0 on error</returns>
        private static uint ConvertVirtualAddress(uint virtualAddress, SectionHeader[] sections)
        {
            // Loop through all of the sections
            for (int i = 0; i < sections.Length; i++)
            {
                // If the section is invalid, just skip it
                if (sections[i] == null)
                    continue;

                // Attempt to derive the physical address from the current section
                var section = sections[i];
                if (virtualAddress >= section.VirtualAddress && virtualAddress <= section.VirtualAddress + section.VirtualSize)
                    return section.PointerToRawData + virtualAddress - section.VirtualAddress;
            }

            return 0;
        }
    }
}