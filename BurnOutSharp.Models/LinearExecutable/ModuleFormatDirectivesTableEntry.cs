using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.LinearExecutable
{
    /// <summary>
    /// The Module Format Directives Table is an optional table that allows additional
    /// options to be specified. It also allows for the extension of the linear EXE
    /// format by allowing additional tables of information to be added to the linear
    /// EXE module without affecting the format of the linear EXE header. Likewise,
    /// module format directives provide a place in the linear EXE module for
    /// 'temporary tables' of information, such as incremental linking information
    /// and statistic information gathered on the module. When there are no module
    /// format directives for a linear EXE module, the fields in the linear EXE header
    /// referencing the module format directives table are zero.
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ModuleFormatDirectivesTableEntry
    {
        /// <summary>
        /// Directive number.
        /// </summary>
        /// <remarks>
        /// The directive number specifies the type of directive defined. This can be
        /// used to determine the format of the information in the directive data.
        /// </remarks>
        public DirectiveNumber DirectiveNumber;

        /// <summary>
        /// Directive data length.
        /// </summary>
        /// <remarks>
        /// This specifies the length in byte of the directive data for this directive number.
        /// </remarks>
        public ushort DirectiveDataLength;

        /// <summary>
        /// Directive data offset.
        /// </summary>
        /// <remarks>
        /// This is the offset to the directive data for this directive number. It is relative
        /// to beginning of linear EXE header for a resident table, and relative to the
        /// beginning of the EXE file for non-resident tables.
        /// </remarks>
        public uint DirectiveDataOffset;
    }
}
