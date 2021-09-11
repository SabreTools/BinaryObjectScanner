using System.IO;
using System.Text;
using BurnOutSharp.Tools;
using BurnOutSharp.ExecutableType.Microsoft.Headers;

namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// Each entry in the export address table is a field that uses one of two formats in the following table.
    /// If the address specified is not within the export section (as defined by the address and length that are indicated in the optional header), the field is an export RVA, which is an actual address in code or data.
    /// Otherwise, the field is a forwarder RVA, which names a symbol in another DLL.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#export-address-table</remarks>
    public class ExportAddressTableEntry
    {
        /// <summary>
        /// The address of the exported symbol when loaded into memory, relative to the image base.
        /// For example, the address of an exported function.
        /// </summary>
        public uint ExportRVA;

        /// <summary>
        /// The pointer to a null-terminated ASCII string in the export section.
        /// This string must be within the range that is given by the export table data directory entry.
        /// This string gives the DLL name and the name of the export (for example, "MYDLL.expfunc") or the DLL name and the ordinal number of the export (for example, "MYDLL.#27").
        /// </summary>
        public uint ForwarderRVA; // TODO: Read this into a separate field

        /// <summary>
        /// A null-terminated ASCII string in the export section.
        /// This string must be within the range that is given by the export table data directory entry.
        /// This string gives the DLL name and the name of the export (for example, "MYDLL.expfunc") or the DLL name and the ordinal number of the export (for example, "MYDLL.#27").
        /// </summary>
        public string Forwarder;

        public static ExportAddressTableEntry Deserialize(Stream stream, SectionHeader[] sections)
        {
            var eate = new ExportAddressTableEntry();

            eate.ExportRVA = stream.ReadUInt32();
            eate.ForwarderRVA = eate.ExportRVA;

            int forwarderAddress = (int)PortableExecutable.ConvertVirtualAddress(eate.ForwarderRVA, sections);
            if (forwarderAddress > -1 && forwarderAddress < stream.Length)
            {
                long originalPosition = stream.Position;
                stream.Seek(forwarderAddress, SeekOrigin.Begin);
                eate.Forwarder = stream.ReadString(Encoding.ASCII);
                stream.Seek(originalPosition, SeekOrigin.Begin);
            }

            return eate;
        }

        public static ExportAddressTableEntry Deserialize(byte[] content, ref int offset, SectionHeader[] sections)
        {
            var eate = new ExportAddressTableEntry();

            eate.ExportRVA = content.ReadUInt32(ref offset);
            eate.ForwarderRVA = eate.ExportRVA;

            int forwarderAddress = (int)PortableExecutable.ConvertVirtualAddress(eate.ForwarderRVA, sections);
            if (forwarderAddress > -1 && forwarderAddress < content.Length)
                eate.Forwarder = content.ReadString(ref forwarderAddress, Encoding.ASCII);

            return eate;
        }
    }
}