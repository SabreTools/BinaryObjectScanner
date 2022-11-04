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

        // TODO: Object table
        // TODO: Object page map table
        // TODO: Object iterate data map table
        // TODO: Resource table
        // TODO: Resident-names table
        // TODO: Entry table
        // TODO: Module directives table
        // TODO: Fix-up page table
        // TODO: Fix-up record table
        // TODO: Imported modules name table
        // TODO: Imported procedures name table
        // TODO: Per-page checksum table

        // TODO: Code or Data Segment X

        // TODO: Non-resident table
    }
}
