using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.LinearExecutable
{
    /// <summary>
    /// The Per-Page Checksum table provides space for a cryptographic checksum for
    /// each physical page in the EXE file.
    /// 
    /// The checksum table is arranged such that the first entry in the table corresponds
    /// to the first logical page of code/data in the EXE file (usually a preload page)
    /// and the last entry corresponds to the last logical page in the EXE file (usually
    /// a iterated data page).
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public class PerPageChecksumTableEntry
    {
        /// <summary>
        /// Cryptographic checksum.
        /// </summary>
        public uint Checksum;
    }
}
