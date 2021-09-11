using System;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.Entries;
using BurnOutSharp.ExecutableType.Microsoft.Headers;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Tables
{
    /// <summary>
    /// Each resource directory table has the following format.
    /// This data structure should be considered the heading of a table
    /// because the table actually consists of directory entries and this structure
    /// </summary>
    public class ResourceDirectoryTable
    {
        /// <summary>
        /// Resource flags.
        /// This field is reserved for future use.
        /// It is currently set to zero.
        /// </summary>
        public uint Characteristics;

        /// <summary>
        /// The time that the resource data was created by the resource compiler.
        /// </summary>
        public uint TimeDateStamp;

        /// <summary>
        /// The major version number, set by the user.
        /// </summary>
        public ushort MajorVersion;
        
        /// <summary>
        /// The minor version number, set by the user.
        /// </summary>
        public ushort MinorVersion;
        
        /// <summary>
        /// The number of directory entries immediately following
        /// the table that use strings to identify Type, Name, or
        /// Language entries (depending on the level of the table).
        /// </summary>
        public ushort NumberOfNamedEntries;
        
        /// <summary>
        /// The number of directory entries immediately following
        /// the Name entries that use numeric IDs for Type, Name,
        /// or Language entries.
        /// </summary>
        public ushort NumberOfIdEntries;

        /// <summary>
        /// The directory entries immediately following
        /// the table that use strings to identify Type, Name, or
        /// Language entries (depending on the level of the table).
        /// </summary>
        public ResourceDirectoryTableEntry[] NamedEntries;

        /// <summary>
        /// The directory entries immediately following
        /// the Name entries that use numeric IDs for Type, Name,
        /// or Language entries.
        /// </summary>
        public ResourceDirectoryTableEntry[] IdEntries;

        public static ResourceDirectoryTable Deserialize(Stream stream, long sectionStart, SectionHeader[] sections)
        {
            var rdt = new ResourceDirectoryTable();

            rdt.Characteristics = stream.ReadUInt32();
            rdt.TimeDateStamp = stream.ReadUInt32();
            rdt.MajorVersion = stream.ReadUInt16();
            rdt.MinorVersion = stream.ReadUInt16();
            rdt.NumberOfNamedEntries = stream.ReadUInt16();
            rdt.NumberOfIdEntries = stream.ReadUInt16();

            rdt.NamedEntries = new ResourceDirectoryTableEntry[rdt.NumberOfNamedEntries];
            for (int i = 0; i < rdt.NumberOfNamedEntries; i++)
            {
                rdt.NamedEntries[i] = ResourceDirectoryTableEntry.Deserialize(stream, sectionStart, sections);
            }

            rdt.IdEntries = new ResourceDirectoryTableEntry[rdt.NumberOfIdEntries];
            for (int i = 0; i < rdt.NumberOfIdEntries; i++)
            {
                rdt.IdEntries[i] = ResourceDirectoryTableEntry.Deserialize(stream, sectionStart, sections);
            }

            return rdt;
        }

        public static ResourceDirectoryTable Deserialize(byte[] content, ref int offset, long sectionStart, SectionHeader[] sections)
        {
            var rdt = new ResourceDirectoryTable();

            rdt.Characteristics = BitConverter.ToUInt32(content, offset); offset += 4;
            rdt.TimeDateStamp = BitConverter.ToUInt32(content, offset); offset += 4;
            rdt.MajorVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            rdt.MinorVersion = BitConverter.ToUInt16(content, offset); offset += 2;
            rdt.NumberOfNamedEntries = BitConverter.ToUInt16(content, offset); offset += 2;
            rdt.NumberOfIdEntries = BitConverter.ToUInt16(content, offset); offset += 2;

            rdt.NamedEntries = new ResourceDirectoryTableEntry[rdt.NumberOfNamedEntries];
            for (int i = 0; i < rdt.NumberOfNamedEntries; i++)
            {
                rdt.NamedEntries[i] = ResourceDirectoryTableEntry.Deserialize(content, ref offset, sectionStart, sections);
            }

            rdt.IdEntries = new ResourceDirectoryTableEntry[rdt.NumberOfIdEntries];
            for (int i = 0; i < rdt.NumberOfIdEntries; i++)
            {
                rdt.IdEntries[i] = ResourceDirectoryTableEntry.Deserialize(content, ref offset, sectionStart, sections);
            }

            return rdt;
        }
    }
}