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
    public sealed class Executable
    {
        /// <summary>
        /// MS-DOS executable stub
        /// </summary>
        public MSDOS.Executable Stub { get; set; }

        /// <summary>
        /// Information block
        /// </summary>
        public InformationBlock InformationBlock { get; set; }

        /// <summary>
        /// Object table
        /// </summary>
        public ObjectTableEntry[] ObjectTable { get; set; }

        /// <summary>
        /// Object page map
        /// </summary>
        public ObjectPageMapEntry[] ObjectPageMap { get; set; }

        // TODO: Object iterate data map table (Undefined)

        /// <summary>
        /// Resource table
        /// </summary>
        public ResourceTableEntry[] ResourceTable { get; set; }

        /// <summary>
        /// Resident Name table
        /// </summary>
        public ResidentNamesTableEntry[] ResidentNamesTable { get; set; }

        /// <summary>
        /// Entry table
        /// </summary>
        public EntryTableBundle[] EntryTable { get; set; }

        /// <summary>
        /// Module format directives table (optional)
        /// </summary>
        public ModuleFormatDirectivesTableEntry[] ModuleFormatDirectivesTable { get; set; }

        /// <summary>
        /// Verify record directive table (optional)
        /// </summary>
        public VerifyRecordDirectiveTableEntry[] VerifyRecordDirectiveTable { get; set; }

        /// <summary>
        /// Fix-up page table
        /// </summary>
        public FixupPageTableEntry[] FixupPageTable { get; set; }

        /// <summary>
        /// Fix-up record table
        /// </summary>
        public FixupRecordTableEntry[] FixupRecordTable { get; set; }

        /// <summary>
        /// Import module name table
        /// </summary>
        public ImportModuleNameTableEntry[] ImportModuleNameTable { get; set; }

        /// <summary>
        /// Import procedure name table
        /// </summary>
        public ImportModuleProcedureNameTableEntry[] ImportModuleProcedureNameTable { get; set; }

        /// <summary>
        /// Per-Page checksum table
        /// </summary>
        public PerPageChecksumTableEntry[] PerPageChecksumTable { get; set; }

        /// <summary>
        /// Non-Resident Name table
        /// </summary>
        public NonResidentNamesTableEntry[] NonResidentNamesTable { get; set; }

        // TODO: Non-resident directives data (Undefined)

        /// <summary>
        /// Debug information
        /// </summary>
        public DebugInformation DebugInformation { get; set; }
    }
}
