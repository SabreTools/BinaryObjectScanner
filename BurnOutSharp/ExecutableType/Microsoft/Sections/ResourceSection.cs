using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.ExecutableType.Microsoft.Headers;
using BurnOutSharp.ExecutableType.Microsoft.Tables;

namespace BurnOutSharp.ExecutableType.Microsoft.Sections
{
    /// <summary>
    /// A series of resource directory tables relates all of the levels in the following way:
    // Each directory table is followed by a series of directory entries that give the name or
    // identifier (ID) for that level (Type, Name, or Language level) and an address of either
    // a data description or another directory table. If the address points to a data description,
    // then the data is a leaf in the tree. If the address points to another directory table,
    // then that table lists directory entries at the next level down
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#the-rsrc-section</remarks>
    internal class ResourceSection
    {
        /// <summary>
        /// A table with just one row (unlike the debug directory).
        /// This table indicates the locations and sizes of the other export tables.
        /// </summary>
        public ResourceDirectoryTable ResourceDirectoryTable;

        public static ResourceSection Deserialize(Stream stream, SectionHeader[] sections)
        {
            var rs = new ResourceSection();

            rs.ResourceDirectoryTable = ResourceDirectoryTable.Deserialize(stream, sections);

            return rs;
        }

        public static ResourceSection Deserialize(byte[] content, int offset, SectionHeader[] sections)
        {
            var rs = new ResourceSection();

            unsafe
            {
                rs.ResourceDirectoryTable = ResourceDirectoryTable.Deserialize(content, offset, sections); offset += Marshal.SizeOf(rs.ResourceDirectoryTable);
            }

            return rs;
        }
    }
}