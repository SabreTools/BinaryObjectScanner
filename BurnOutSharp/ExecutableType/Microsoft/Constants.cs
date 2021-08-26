namespace BurnOutSharp.ExecutableType.Microsoft
{
    /// <summary>
    /// All constant values needed for file header reading
    /// </summary>
    internal static class Constants
    {
        public const ushort IMAGE_DOS_SIGNATURE = 0x5A4D;       // MZ
        public const ushort IMAGE_OS2_SIGNATURE = 0x454E;       // NE
        public const ushort IMAGE_OS2_SIGNATURE_LE = 0x454C;    // LE
        public const uint IMAGE_NT_SIGNATURE = 0x00004550;      // PE00

        #region IMAGE_DOS_HEADER

        public const ushort ENEWEXE = 0x40;   // Value of E_LFARLC for new .EXEs
        public const ushort ENEWHDR = 0x003C; // Offset in old hdr. of ptr. to new
        public const ushort ERESWDS = 0x0010; // No. of reserved words (OLD)
        public const ushort ERES1WDS = 0x0004; // No. of reserved words in e_res
        public const ushort ERES2WDS = 0x000A; // No. of reserved words in e_res2
        public const ushort ECP = 0x0004; // Offset in struct of E_CP 
        public const ushort ECBLP = 0x0002; // Offset in struct of E_CBLP
        public const ushort EMINALLOC = 0x000A; // Offset in struct of E_MINALLOC

        #endregion

        #region IMAGE_OS2_HEADER

        public const ushort NERESWORDS = 3;     // 6 bytes reserved
        public const ushort NECRC = 8;          //Offset into new header of NE_CRC

        #endregion

        #region NewSeg

        public const ushort NSALIGN = 9;        // Segment data aligned on 512 byte boundaries
        public const ushort NSLOADED = 0x0004;  // ns_sector field contains memory addr

        #endregion

        #region RsrcNameInfo

        public const ushort RSORDID = 0x8000;       /* if high bit of ID set then integer id */

        /* otherwise ID is offset of string from
           the beginning of the resource table */
        /* Ideally these are the same as the */
        /* corresponding segment flags */
        public const ushort RNMOVE = 0x0010;	  /* Moveable resource */
        public const ushort RNPURE = 0x0020;	  /* Pure (read-only) resource */
        public const ushort RNPRELOAD = 0x0040;	  /* Preloaded resource */
        public const ushort RNDISCARD = 0xF000;	  /* Discard priority level for resource */

        #endregion

        #region IMAGE_OPTIONAL_HEADER

        public const ushort IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;

        #endregion

        #region IMAGE_SECTION_HEADER

        public const int IMAGE_SIZEOF_SHORT_NAME = 8;

        #endregion

        #region IMAGE_RESOURCE_DATA_ENTRY

        public const uint IMAGE_RESOURCE_DATA_IS_DIRECTORY = 0x80000000;
        public const uint IMAGE_RESOURCE_NAME_IS_STRING	= 0x80000000;

        #endregion
    }
}