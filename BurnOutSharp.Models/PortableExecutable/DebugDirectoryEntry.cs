using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Image files contain an optional debug directory that indicates what form
    /// of debug information is present and where it is. This directory consists
    /// of an array of debug directory entries whose location and size are indicated
    /// in the image optional header.
    /// 
    /// The debug directory can be in a discardable .debug section (if one exists),
    /// or it can be included in any other section in the image file, or not be in
    /// a section at all.
    /// 
    /// Each debug directory entry identifies the location and size of a block of
    /// debug information. The specified RVA can be zero if the debug information
    /// is not covered by a section header (that is, it resides in the image file
    /// and is not mapped into the run-time address space). If it is mapped, the
    /// RVA is its address.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DebugDirectoryEntry
    {
        /// <summary>
        /// Reserved, must be zero. 
        /// </summary>
        public uint Characteristics;

        /// <summary>
        /// The time and date that the debug data was created.
        /// </summary>
        public uint TimeDateStamp;

        /// <summary>
        /// The major version number of the debug data format.
        /// </summary>
        public ushort MajorVersion;

        /// <summary>
        /// The minor version number of the debug data format.
        /// </summary>
        public ushort MinorVersion;

        /// <summary>
        /// The format of debugging information. This field enables support
        /// of multiple debuggers.
        /// </summary>
        public DebugType DebugType;

        /// <summary>
        /// The size of the debug data (not including the debug directory itself).
        /// </summary>
        public uint SizeOfData;

        /// <summary>
        /// The address of the debug data when loaded, relative to the image base.
        /// </summary>
        public uint AddressOfRawData;

        /// <summary>
        /// The file pointer to the debug data.
        /// </summary>
        public uint PointerToRawData;
    }
}
