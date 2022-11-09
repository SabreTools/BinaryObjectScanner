namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// An accelerator table is one resource entry in a resource file. The structure definition
    /// provided here is for explanation only; it is not present in any standard header file.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/resource-file-formats"/>
    public class AcceleratorTableResource
    {
        /// <summary>
        /// Each entry consists of a resource header and the data for that resource.
        /// </summary>
        public ResourceHeader ResourceHeader;

        /// <summary>
        /// Accelerator table data
        /// </summary>
        public AcceleratorTableEntry[] AcceleratorTable;
    }
}
