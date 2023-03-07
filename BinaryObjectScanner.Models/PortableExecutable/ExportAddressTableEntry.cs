using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.PortableExecutable
{
    /// <summary>
    /// The export address table contains the address of exported entry points
    /// and exported data and absolutes. An ordinal number is used as an index
    /// into the export address table.
    /// 
    /// Each entry in the export address table is a field that uses one of two
    /// formats in the following table. If the address specified is not within
    /// the export section (as defined by the address and length that are
    /// indicated in the optional header), the field is an export RVA, which is
    /// an actual address in code or data. Otherwise, the field is a forwarder RVA,
    /// which names a symbol in another DLL.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    [StructLayout(LayoutKind.Explicit)]
    public sealed class ExportAddressTableEntry
    {
        /// <summary>
        /// The address of the exported symbol when loaded into memory, relative to
        /// the image base. For example, the address of an exported function.
        /// </summary>
        [FieldOffset(0)] public uint ExportRVA;

        /// <summary>
        /// The pointer to a null-terminated ASCII string in the export section. This
        /// string must be within the range that is given by the export table data
        /// directory entry. See Optional Header Data Directories (Image Only). This
        /// string gives the DLL name and the name of the export (for example,
        /// "MYDLL.expfunc") or the DLL name and the ordinal number of the export
        /// (for example, "MYDLL.#27").
        /// </summary>
        [FieldOffset(0)] public uint ForwarderRVA;
    }
}
