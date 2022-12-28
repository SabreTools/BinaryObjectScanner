using System.Collections.Generic;

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
    public sealed class Executable
    {
        /// <summary>
        /// MS-DOS executable stub
        /// </summary>
        public MSDOS.Executable Stub { get; set; }

        /// <summary>
        /// New Executable header
        /// </summary>
        public ExecutableHeader Header { get; set; }

        /// <summary>
        /// Segment table
        /// </summary>
        public SegmentTableEntry[] SegmentTable { get; set; }

        /// <summary>
        /// Resource table
        /// </summary>
        public ResourceTable ResourceTable { get; set; }

        /// <summary>
        /// Resident-Name table
        /// </summary>
        public ResidentNameTableEntry[] ResidentNameTable { get; set; }

        /// <summary>
        /// Module-Reference table
        /// </summary>
        public ModuleReferenceTableEntry[] ModuleReferenceTable { get; set; }

        /// <summary>
        /// Imported-Name table
        /// </summary>
        public Dictionary<ushort, ImportedNameTableEntry> ImportedNameTable { get; set; }

        /// <summary>
        /// Entry table
        /// </summary>
        public EntryTableBundle[] EntryTable { get; set; }

        /// <summary>
        /// Nonresident-Name table
        /// </summary>
        public NonResidentNameTableEntry[] NonResidentNameTable { get; set; }
    }
}
