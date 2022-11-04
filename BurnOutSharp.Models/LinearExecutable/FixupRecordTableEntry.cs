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
    [StructLayout(LayoutKind.Explicit)]
    public class FixupRecordTableEntry
    {
        /// <summary>
        /// Source type.
        /// </summary>
        /// <remarks>
        /// The source type specifies the size and type of the fixup to be performed
        /// on the fixup source.
        /// </remarks>
        [FieldOffset(0)] public FixupRecordSourceType SourceType;

        /// <summary>
        /// Target Flags.
        /// </summary>
        /// <remarks>
        /// The target flags specify how the target information is interpreted.
        /// </remarks>
        [FieldOffset(1)] public FixupRecordTargetFlags TargetFlags;

        #region Source List Flag Set

        /// <summary>
        /// Source offset.
        /// </summary>
        /// <remarks>
        /// This field contains either an offset or a count depending on the Source
        /// List Flag. If the Source List Flag is set, a list of source offsets
        /// follows the additive field and this field contains the count of the
        /// entries in the source offset list. Otherwise, this is the single source
        /// offset for the fixup. Source offsets are relative to the beginning of
        /// the page where the fixup is to be made.
        /// 
        /// Note that for fixups that cross page boundaries, a separate fixup record
        /// is specified for each page. An offset is still used for the 2nd page but
        /// it now becomes a negative offset since the fixup originated on the
        /// preceding page. (For example, if only the last one byte of a 32-bit
        /// address is on the page to be fixed up, then the offset would have a value
        /// of -3.)
        /// </remarks>
        [FieldOffset(2)] public ushort SourceOffset;

        // TODO: Field offsets branch out from here based on other flags

        #endregion

        #region Source List Flag Unset

        /// <summary>
        /// Source offset list count.
        /// </summary>
        /// <remarks>
        /// This field contains either an offset or a count depending on the Source
        /// List Flag. If the Source List Flag is set, a list of source offsets
        /// follows the additive field and this field contains the count of the
        /// entries in the source offset list. Otherwise, this is the single source
        /// offset for the fixup. Source offsets are relative to the beginning of
        /// the page where the fixup is to be made.
        /// 
        /// Note that for fixups that cross page boundaries, a separate fixup record
        /// is specified for each page. An offset is still used for the 2nd page but
        /// it now becomes a negative offset since the fixup originated on the
        /// preceding page. (For example, if only the last one byte of a 32-bit
        /// address is on the page to be fixed up, then the offset would have a value
        /// of -3.)
        /// </remarks>
        [FieldOffset(2)] public byte SourceOffsetListCount;

        // TODO: Field offsets branch out from here based on other flags

        #endregion
    }
}
