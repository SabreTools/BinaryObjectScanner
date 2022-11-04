namespace BurnOutSharp.Models.LinearExecutable
{
    /// <summary>
    /// The `LINEAR` executable-file header contains information that the loader requires for
    /// segmented executable files. This information includes the linker version number, data
    /// specified by linker, data specified by resource compiler, tables of segment data, tables
    /// of resource data, and so on. The following illustrations shows the LE file header:
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    public class Executable
    {
        /// <summary>
        /// MS-DOS executable header
        /// </summary>
        public MSDOS.ExecutableHeader Stub { get; set; }

        /// <summary>
        /// Information block
        /// </summary>
        public InformationBlock InformationBlock { get; set; }

        /// <summary>
        /// Object table
        /// </summary>
        public ObjectTableEntry[] ObjectTable { get; set; }

        /// <summary>
        /// Object page table
        /// </summary>
        public ObjectPageTableEntry[] ObjectPageTable { get; set; }

        // TODO: Object iterate data map table [Does this exist?]

        /// <summary>
        /// Resource table
        /// </summary>
        public ResourceTableEntry[] ResourceTable { get; set; }

        /// <summary>
        /// Resident Name table
        /// </summary>
        public ResidentNameTableEntry[] ResidentNameTable { get; set; }

        // TODO: Entry table
        // TODO: Module format directives table
        // TODO: Resident directives data
        // TODO: Per-page checksum table
        // TODO: Fix-up page table
        // TODO: Fix-up record table
        // TODO: Imported modules name table
        // TODO: Imported procedures name table

        // TODO: Preload Pages
        // TODO: Demand Load Pages
        // TODO: Iterated Pages

        /// <summary>
        /// Non-Resident Name table
        /// </summary>
        public NonResidentNameTableEntry[] NonResidentNameTable { get; set; }

        // TODO: Non-resident directives data
        // TODO: Debug Info
    }
}
