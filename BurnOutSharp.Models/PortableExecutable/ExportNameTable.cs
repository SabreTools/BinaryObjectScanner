namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// The export name table contains the actual string data that was pointed to by the export
    /// name pointer table. The strings in this table are public names that other images can use
    /// to import the symbols. These public export names are not necessarily the same as the
    /// private symbol names that the symbols have in their own image file and source code,
    /// although they can be.
    /// 
    /// Every exported symbol has an ordinal value, which is just the index into the export
    /// address table. Use of export names, however, is optional. Some, all, or none of the
    /// exported symbols can have export names. For exported symbols that do have export names,
    /// corresponding entries in the export name pointer table and export ordinal table work
    /// together to associate each name with an ordinal.
    /// 
    /// The structure of the export name table is a series of null-terminated ASCII strings
    /// of variable length.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public class ExportNameTable
    {
        /// <summary>
        /// A series of null-terminated ASCII strings of variable length.
        /// </summary>
        public string[] Indexes;
    }
}
