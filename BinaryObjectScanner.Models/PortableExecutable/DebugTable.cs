namespace BinaryObjectScanner.Models.PortableExecutable
{
    /// <summary>
    /// The .debug section is used in object files to contain compiler-generated debug
    /// information and in image files to contain all of the debug information that is
    /// generated. This section describes the packaging of debug information in object
    /// and image files.
    /// 
    /// The next section describes the format of the debug directory, which can be
    /// anywhere in the image. Subsequent sections describe the "groups" in object
    /// files that contain debug information.
    /// 
    /// The default for the linker is that debug information is not mapped into the
    /// address space of the image. A .debug section exists only when debug information
    /// is mapped in the address space.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public sealed class DebugTable
    {
        /// <summary>
        /// Image files contain an optional debug directory that indicates what form
        /// of debug information is present and where it is. This directory consists
        /// of an array of debug directory entries whose location and size are
        /// indicated in the image optional header.
        /// 
        /// The debug directory can be in a discardable .debug section (if one exists),
        /// or it can be included in any other section in the image file, or not be
        /// in a section at all.
        /// 
        /// Each debug directory entry identifies the location and size of a block of
        /// debug information. The specified RVA can be zero if the debug information
        /// is not covered by a section header (that is, it resides in the image
        /// file and is not mapped into the run-time address space). If it is mapped,
        /// the RVA is its address.
        /// </summary>
        public DebugDirectoryEntry[] DebugDirectoryTable;
    }
}
