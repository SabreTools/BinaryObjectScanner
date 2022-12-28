using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.NewExecutable
{
    /// <summary>
    /// A table of relocation records follows. The following is the format
    /// of each relocation record.
    /// </summary>
    /// <see href="http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class RelocationRecord
    {
        /// <summary>
        /// Source type.
        /// </summary>
        public RelocationRecordSourceType SourceType;

        /// <summary>
        /// Flags byte.
        /// 
        /// The target value has four types that are defined in the flag
        /// byte field.
        /// </summary>
        public RelocationRecordFlag Flags;

        /// <summary>
        /// Offset within this segment of the source chain.
        /// If the ADDITIVE flag is set, then target value is added to
        /// the source contents, instead of replacing the source and
        /// following the chain. The source chain is an 0FFFFh
        /// terminated linked list within this segment of all
        /// references to the target.
        /// </summary>
        public ushort Offset;

        /// <summary>
        /// INTERNALREF
        /// </summary>
        /// <remarks>Must be <c>NULL</c> if <see cref="Flags"/> is not set to <see cref="RelocationRecordFlag.INTERNALREF"/></remarks>
        public InternalRefRelocationRecord InternalRefRelocationRecord;

        /// <summary>
        /// IMPORTNAME
        /// </summary>
        /// <remarks>Must be <c>NULL</c> if <see cref="Flags"/> is not set to <see cref="RelocationRecordFlag.IMPORTNAME"/></remarks>
        public ImportNameRelocationRecord ImportNameRelocationRecord;

        /// <summary>
        /// IMPORTORDINAL
        /// </summary>
        /// <remarks>Must be <c>NULL</c> if <see cref="Flags"/> is not set to <see cref="RelocationRecordFlag.IMPORTORDINAL"/></remarks>
        public ImportOrdinalRelocationRecord ImportOrdinalRelocationRecord;

        /// <summary>
        /// IMPORTORDINAL
        /// </summary>
        /// <remarks>Must be <c>NULL</c> if <see cref="Flags"/> is not set to <see cref="RelocationRecordFlag.OSFIXUP"/></remarks>
        public OSFixupRelocationRecord OSFixupRelocationRecord;
    }
}
