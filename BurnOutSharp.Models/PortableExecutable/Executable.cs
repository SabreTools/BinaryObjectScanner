namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// The following list describes the Microsoft PE executable format, with the
    /// base of the image header at the top. The section from the MS-DOS 2.0
    /// Compatible EXE Header through to the unused section just before the PE header
    /// is the MS-DOS 2.0 Section, and is used for MS-DOS compatibility only.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class Executable
    {
        /// <summary>
        /// MS-DOS executable stub
        /// </summary>
        public MSDOS.Executable Stub { get; set; }
    }
}
