namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// The export name pointer table is an array of addresses (RVAs) into the export name table.
    /// The pointers are 32 bits each and are relative to the image base. The pointers are ordered
    /// lexically to allow binary searches.
    /// 
    /// An export name is defined only if the export name pointer table contains a pointer to it.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public sealed class ExportNamePointerTable
    {
        /// <summary>
        /// The pointers are 32 bits each and are relative to the image base.
        /// </summary>
        public uint[] Pointers;
    }
}
