using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE.Entries;
using BurnOutSharp.ExecutableType.Microsoft.PE.Headers;
using BurnOutSharp.ExecutableType.Microsoft.PE.Tables;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.PE.Sections
{
    /// <summary>
    /// The export data section, named .edata, contains information about symbols that other images can access through dynamic linking.
    /// Exported symbols are generally found in DLLs, but DLLs can also import symbols.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#the-edata-section-image-only</remarks>
    public class ExportDataSection
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
        public ExportAddressTableEntry[] ExportAddressTable;

        /// <summary>
        /// An array of pointers to the public export names, sorted in ascending order.
        /// </summary>
        public uint[] ExportNamePointerTable;

        /// <summary>
        /// An array of the ordinals that correspond to members of the name pointer table.
        /// The correspondence is by position; therefore, the name pointer table and the ordinal table must have the same number of members.
        /// Each ordinal is an index into the export address table.
        /// </summary>
        public ExportOrdinalTable OrdinalTable;

        public static ExportDataSection Deserialize(Stream stream, SectionHeader[] sections)
        {
            long originalPosition = stream.Position;
            var eds = new ExportDataSection();

            eds.ExportDirectoryTable = ExportDirectoryTable.Deserialize(stream);

            stream.Seek((int)PortableExecutable.ConvertVirtualAddress(eds.ExportDirectoryTable.ExportAddressTableRVA, sections), SeekOrigin.Begin);
            eds.ExportAddressTable = new ExportAddressTableEntry[(int)eds.ExportDirectoryTable.AddressTableEntries];
            for (int i = 0; i < eds.ExportAddressTable.Length; i++)
            {
                eds.ExportAddressTable[i] = ExportAddressTableEntry.Deserialize(stream, sections);
            }

            stream.Seek((int)PortableExecutable.ConvertVirtualAddress(eds.ExportDirectoryTable.NamePointerRVA, sections), SeekOrigin.Begin);
            eds.ExportNamePointerTable = new uint[(int)eds.ExportDirectoryTable.NumberOfNamePointers];
            for (int i = 0; i < eds.ExportNamePointerTable.Length; i++)
            {
                eds.ExportNamePointerTable[i] = stream.ReadUInt32();
            }

            stream.Seek((int)PortableExecutable.ConvertVirtualAddress(eds.ExportDirectoryTable.OrdinalTableRVA, sections), SeekOrigin.Begin);
            // eds.OrdinalTable = ExportOrdinalTable.Deserialize(stream, count: 0); // TODO: Figure out where this count comes from

            return eds;
        }

        public static ExportDataSection Deserialize(byte[] content, ref int offset, SectionHeader[] sections)
        {
            int originalPosition = offset;
            var eds = new ExportDataSection();

            eds.ExportDirectoryTable = ExportDirectoryTable.Deserialize(content, ref offset);

            offset = (int)PortableExecutable.ConvertVirtualAddress(eds.ExportDirectoryTable.ExportAddressTableRVA, sections);
            eds.ExportAddressTable = new ExportAddressTableEntry[(int)eds.ExportDirectoryTable.AddressTableEntries];
            for (int i = 0; i < eds.ExportAddressTable.Length; i++)
            {
                eds.ExportAddressTable[i] = ExportAddressTableEntry.Deserialize(content, ref offset, sections);
            }

            offset = (int)PortableExecutable.ConvertVirtualAddress(eds.ExportDirectoryTable.NamePointerRVA, sections);
            eds.ExportNamePointerTable = new uint[(int)eds.ExportDirectoryTable.NumberOfNamePointers];
            for (int i = 0; i < eds.ExportNamePointerTable.Length; i++)
            {
                eds.ExportNamePointerTable[i] = content.ReadUInt32(ref offset);
            }

            offset = (int)PortableExecutable.ConvertVirtualAddress(eds.ExportDirectoryTable.OrdinalTableRVA, sections);
            // eds.OrdinalTable = ExportOrdinalTable.Deserialize(content, ref offset, count: 0); // TODO: Figure out where this count comes from

            return eds;
        }
    }
}