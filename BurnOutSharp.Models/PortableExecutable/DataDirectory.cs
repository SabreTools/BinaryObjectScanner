using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Each data directory gives the address and size of a table or string that Windows uses.
    /// These data directory entries are all loaded into memory so that the system can use them
    /// at run time.
    /// 
    /// Also, do not assume that the RVAs in this table point to the beginning of a section or
    /// that the sections that contain specific tables have specific names.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DataDirectory
    {
        /// <summary>
        /// The first field, VirtualAddress, is actually the RVA of the table. The RVA
        /// is the address of the table relative to the base address of the image when
        /// the table is loaded.
        /// </summary>
        public uint VirtualAddress;

        /// <summary>
        /// The second field gives the size in bytes.
        /// </summary>
        public uint Size;
    }
}
