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

        #region Source List Flag

        #region Set

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
        public ushort SourceOffset;

        #endregion

        #region Unset

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
        public byte SourceOffsetListCount;

        #endregion

        #endregion

        #region OBJECT / TRGOFF

        #region 16-bit Object Number/Module Ordinal Flag

        #region Set

        /// <summary>
        /// Target object number.
        /// </summary>
        /// <remarks>
        /// This field is an index into the current module's Object Table to specify
        /// the target Object. It is a Byte value when the '16-bit Object Number/Module
        /// Ordinal Flag' bit in the target flags field is clear and a Word value when
        /// the bit is set.
        /// </remarks>
        public ushort TargetObjectNumberWORD;

        #endregion

        #region Unset

        /// <summary>
        /// Target object number.
        /// </summary>
        /// <remarks>
        /// This field is an index into the current module's Object Table to specify
        /// the target Object. It is a Byte value when the '16-bit Object Number/Module
        /// Ordinal Flag' bit in the target flags field is clear and a Word value when
        /// the bit is set.
        /// </remarks>
        public byte TargetObjectNumberByte;

        #endregion

        #endregion

        #region 32-bit Target Offset Flag

        #region Set

        /// <summary>
        /// Target offset.
        /// </summary>
        /// <remarks>
        /// This field is an offset into the specified target Object. It is not
        /// present when the Source Type specifies a 16-bit Selector fixup. It
        /// is a Word value when the '32-bit Target Offset Flag' bit in the target
        /// flags field is clear and a Dword value when the bit is set.
        /// </remarks>
        public uint TargetOffsetDWORD;

        #endregion

        #region Unset

        /// <summary>
        /// Target offset.
        /// </summary>
        /// <remarks>
        /// This field is an offset into the specified target Object. It is not
        /// present when the Source Type specifies a 16-bit Selector fixup. It
        /// is a Word value when the '32-bit Target Offset Flag' bit in the target
        /// flags field is clear and a Dword value when the bit is set.
        /// </remarks>
        public ushort TargetOffsetWORD;

        #endregion

        #endregion

        #endregion

        #region 16-bit Object Number/Module Ordinal Flag [Incompatible with OBJECT / TRGOFF]

        #region Set

        /// <summary>
        /// Ordinal index into the Import Module Name Table.
        /// </summary>
        /// <remarks>
        /// This value is an ordered index in to the Import Module Name Table for
        /// the module containing the procedure entry point. It is a Byte value
        /// when the '16-bit Object Number/Module Ordinal' Flag bit in the target
        /// flags field is clear and a Word value when the bit is set. The loader
        /// creates a table of pointers with each pointer in the table corresponds
        /// to the modules named in the Import Module Name Table. This value is used
        /// by the loader to index into this table created by the loader to locate
        /// the referenced module.
        /// </remarks>
        public ushort OrdinalIndexImportModuleNameTableWORD;

        #endregion

        #region Unset

        /// <summary>
        /// Ordinal index into the Import Module Name Table.
        /// </summary>
        /// <remarks>
        /// This value is an ordered index in to the Import Module Name Table for
        /// the module containing the procedure entry point. It is a Byte value
        /// when the '16-bit Object Number/Module Ordinal' Flag bit in the target
        /// flags field is clear and a Word value when the bit is set. The loader
        /// creates a table of pointers with each pointer in the table corresponds
        /// to the modules named in the Import Module Name Table. This value is used
        /// by the loader to index into this table created by the loader to locate
        /// the referenced module.
        /// </remarks>
        public byte OrdinalIndexImportModuleNameTableByte;

        #endregion

        #endregion

        #region MOD ORD# / PROCEDURE NAME OFFSET / ADDITIVE

        #region 32-bit Target Offset Flag

        #region Set

        /// <summary>
        /// Offset into the Import Procedure Name Table.
        /// </summary>
        /// <remarks>
        /// This field is an offset into the Import Procedure Name Table. It is
        /// a Word value when the '32-bit Target Offset Flag' bit in the target
        /// flags field is clear and a Dword value when the bit is set. 
        /// </remarks>
        public uint OffsetImportProcedureNameTableDWORD;

        #endregion

        #region Unset

        /// <summary>
        /// Offset into the Import Procedure Name Table.
        /// </summary>
        /// <remarks>
        /// This field is an offset into the Import Procedure Name Table. It is
        /// a Word value when the '32-bit Target Offset Flag' bit in the target
        /// flags field is clear and a Dword value when the bit is set. 
        /// </remarks>
        public ushort OffsetImportProcedureNameTableWORD;

        #endregion

        #endregion

        #endregion

        #region MOD ORD# / IMPORT ORD / ADDITIVE

        /*
        TODO: Implement IMPORT ORD
        IMPORT ORD = D[B|W|D] Imported ordinal number.
            This is the imported procedure's ordinal number. It is a Byte value when the
            '8-bit Ordinal' bit in the target flags field is set. Otherwise it is a Word value
            when the '32-bit Target Offset Flag' bit in the target flags field is clear and a
            Dword value when the bit is set. 
        */

        #endregion

        #region Additive Fixup Flag [Incompatible with OBJECT / TRGOFF]

        #region Set

        #region 32-bit Additive Fixup Flag

        #region Set

        /// <summary>
        /// Additive fixup value.
        /// </summary>
        /// <remarks>
        /// This field exists in the fixup record only when the 'Additive Fixup Flag'
        /// bit in the target flags field is set. When the 'Additive Fixup Flag' is
        /// clear the fixup record does not contain this field and is immediately
        /// followed by the next fixup record (or by the source offset list for this
        /// fixup record).
        /// 
        /// This value is added to the address derived from the target entry point.
        /// This field is a Word value when the '32-bit Additive Flag' bit in the
        /// target flags field is clear and a Dword value when the bit is set.
        /// </remarks>
        public uint AdditiveFixupValueDWORD;

        #endregion

        #region Unset

        /// <summary>
        /// Additive fixup value.
        /// </summary>
        /// <remarks>
        /// This field exists in the fixup record only when the 'Additive Fixup Flag'
        /// bit in the target flags field is set. When the 'Additive Fixup Flag' is
        /// clear the fixup record does not contain this field and is immediately
        /// followed by the next fixup record (or by the source offset list for this
        /// fixup record).
        /// 
        /// This value is added to the address derived from the target entry point.
        /// This field is a Word value when the '32-bit Additive Flag' bit in the
        /// target flags field is clear and a Dword value when the bit is set.
        /// </remarks>
        public ushort AdditiveFixupValueWORD;

        #endregion

        #endregion

        #endregion

        #endregion

        // TODO: Implement SRCOFFn
    }
}
