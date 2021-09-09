using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.ExecutableType.Microsoft.Tables;

namespace BurnOutSharp.ExecutableType.Microsoft.Sections
{
    /// <summary>
    /// The export data section, named .edata, contains information about symbols that other images can access through dynamic linking.
    /// Exported symbols are generally found in DLLs, but DLLs can also import symbols.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#the-edata-section-image-only</remarks>
    internal class ExportDataSection
    {
        /// <summary>
        /// A table with just one row (unlike the debug directory).
        /// This table indicates the locations and sizes of the other export tables.
        /// </summary>
        public ExportDirectoryTable ExportDirectoryTable;

        /// <summary>
        /// An array of RVAs of exported symbols.
        /// These are the actual addresses of the exported functions and data within the executable code and data sections.
        /// Other image files can import a symbol by using an index to this table (an ordinal) or, optionally, by using the public name that corresponds to the ordinal if a public name is defined.
        /// </summary>
        public ExportAddressTable ExportAddressTable;

        /// <summary>
        /// An array of pointers to the public export names, sorted in ascending order.
        /// </summary>
        public ExportNamePointerTable NamePointerTable;

        /// <summary>
        /// An array of the ordinals that correspond to members of the name pointer table.
        /// The correspondence is by position; therefore, the name pointer table and the ordinal table must have the same number of members.
        /// Each ordinal is an index into the export address table.
        /// </summary>
        public ExportOrdinalTable OrdinalTable;

        /// <summary>
        /// A series of null-terminated ASCII strings.
        /// Members of the name pointer table point into this area.
        /// These names are the public names through which the symbols are imported and exported; they are not necessarily the same as the private names that are used within the image file.
        /// </summary>
        public ExportNameTable ExportNameTable;

        public static ExportDataSection Deserialize(Stream stream)
        {
            var eds = new ExportDataSection();

            eds.ExportDirectoryTable = ExportDirectoryTable.Deserialize(stream);
            // eds.ExportAddressTable = ExportAddressTable.Deserialize(stream, count: 0); // TODO: Figure out where this count comes from
            // eds.NamePointerTable = ExportNamePointerTable.Deserialize(stream, count: 0); // TODO: Figure out where this count comes from
            // eds.OrdinalTable = ExportOrdinalTable.Deserialize(stream, count: 0); // TODO: Figure out where this count comes from
            // eds.ExportNameTable = ExportNameTable.Deserialize(stream); // TODO: set this table based on the NamePointerTable value

            return eds;
        }

        public static ExportDataSection Deserialize(byte[] content, int offset)
        {
            var eds = new ExportDataSection();

            unsafe
            {
                eds.ExportDirectoryTable = ExportDirectoryTable.Deserialize(content, offset); offset += Marshal.SizeOf(eds.ExportDirectoryTable);
                // eds.ExportAddressTable = ExportAddressTable.Deserialize(content, offset, count: 0); offset += Marshal.SizeOf(eds.ExportAddressTable); // TODO: Figure out where this count comes from
                // eds.NamePointerTable = ExportNamePointerTable.Deserialize(content, offset, count: 0); offset += Marshal.SizeOf(eds.NamePointerTable); // TODO: Figure out where this count comes from
                // eds.OrdinalTable = ExportOrdinalTable.Deserialize(content, offset, count: 0); offset += Marshal.SizeOf(eds.OrdinalTable); // TODO: Figure out where this count comes from
                // eds.ExportNameTable = ExportNameTable.Deserialize(stream); offset += Marshal.SizeOf(eds.ExportAddressTable); // TODO: set this table based on the NamePointerTable value
            }

            return eds;
        }
    }
}