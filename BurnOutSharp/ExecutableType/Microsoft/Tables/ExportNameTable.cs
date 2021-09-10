namespace BurnOutSharp.ExecutableType.Microsoft.Tables
{
    /// <summary>
    /// The export name table contains the actual string data that was pointed to by the export name pointer table.
    /// The strings in this table are public names that other images can use to import the symbols.
    /// These public export names are not necessarily the same as the private symbol names that the symbols have in their own image file and source code, although they can be.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#export-ordinal-table</remarks>
    public class ExportNameTable
    {
        /// <remarks>Number of entries is defined externally</remarks>
        public string[] Entries;
    }
}