namespace BurnOutSharp.Models.NewExecutable
{
    /// <summary>
    /// The segmented EXE header contains general information about the EXE
    /// file and contains information on the location and size of the other
    /// sections. The Windows loader copies this section, along with other
    /// data, into the module table in the system data. The module table is
    /// internal data used by the loader to manage the loaded executable
    /// modules in the system and to support dynamic linking.
    /// </summary>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    public class Executable
    {
        /// <summary>
        /// MS-DOS executable header
        /// </summary>
        public MSDOS.ExecutableHeader Stub { get; set; }

        /// <summary>
        /// New Executable header
        /// </summary>
        public ExecutableHeader Header { get; set; }

        /// <summary>
        /// Segment table
        /// </summary>
        public SegmentTableEntry[] SegmentTable { get; set; }

        // TODO: Resource Table
        // TODO: Resident-Name Table
        // TODO: Module-Reference Table
        // TODO: Imported-Name Table
        // TODO: Entry Table
        // TODO: Nonresident-Name Table
        // TODO: Per Segment Data
    }
}
