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
    }
}
