using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.LinearExecutable
{
    /// <summary>
    /// The Fixup Record Table contains entries for all fixups in the linear EXE module.
    /// The fixup records for a logical page are grouped together and kept in sorted
    /// order by logical page number. The fixups for each page are further sorted such
    /// that all external fixups and internal selector/pointer fixups come before
    /// internal non-selector/non-pointer fixups. This allows the loader to ignore
    /// internal fixups if the loader is able to load all objects at the addresses
    /// specified in the object table.
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    /// <see href="http://www.edm2.com/index.php/LX_-_Linear_eXecutable_Module_Format_Description"/>
    [StructLayout(LayoutKind.Sequential)]
    public class FixupRecordTableEntry
    {
        /// <summary>
        /// Source type.
        /// </summary>
        /// <remarks>
        /// The source type specifies the size and type of the fixup to be performed
        /// on the fixup source.
        /// </remarks>
        public FixupRecordSourceType SourceType;

        /// <summary>
        /// Target Flags.
        /// </summary>
        /// <remarks>
        /// The target flags specify how the target information is interpreted.
        /// </remarks>
        public FixupRecordTargetFlags TargetFlags;

        // TODO: The shape of the data relies on the source and flags from here
        // SRCOFF = DW/CNT = DB Source offset or source offset list count.
    }
}
