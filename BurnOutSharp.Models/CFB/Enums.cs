namespace BurnOutSharp.Models.CFB
{
    public enum ColorFlag : byte
    {
        Red = 0x00,
        Black = 0x01,
    }
    
    public enum ObjectType : byte
    {
        Unknown = 0x00,
        StorageObject = 0x01,
        StreamObject = 0x02,
        RootStorageObject = 0x05,
    }

    public enum SectorNumber : uint
    {
        /// <summary>
        /// Regular sector number.
        /// </summary>
        REGSECT = 0x00000000, // 0x00000000 - 0xFFFFFFF9

        /// <summary>
        /// Maximum regular sector number.
        /// </summary>
        MAXREGSECT = 0xFFFFFFFA,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        NotApplicable = 0xFFFFFFFB,

        /// <summary>
        /// Specifies a DIFAT sector in the FAT.
        /// </summary>
        DIFSECT = 0xFFFFFFFC,

        /// <summary>
        /// Specifies a FAT sector in the FAT.
        /// </summary>
        FATSECT = 0xFFFFFFFD,

        /// <summary>
        /// End of a linked chain of sectors.
        /// </summary>
        ENDOFCHAIN = 0xFFFFFFFE,

        /// <summary>
        /// Specifies an unallocated sector in the FAT, Mini FAT, or DIFAT.
        /// </summary>
        FREESECT = 0xFFFFFFFF,
    }

    public enum StreamID : uint
    {
        /// <summary>
        /// Regular stream ID to identify the directory entry.
        /// </summary>
        REGSID = 0x00000000, // 0x00000000 - 0xFFFFFFF9

        /// <summary>
        /// Maximum regular stream ID.
        /// </summary>
        MAXREGSID = 0xFFFFFFFA,

        /// <summary>
        /// Terminator or empty pointer.
        /// </summary>
        NOSTREAM = 0xFFFFFFFF,
    }
}