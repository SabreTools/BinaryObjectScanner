namespace BinaryObjectScanner.Models.N3DS
{
    /// <summary>
    /// There are two known specialisations of the NCSD container format:
    /// - The CTR Cart Image (CCI) format, the 3DS' raw NAND format
    /// - CCI is the format of game ROM images.
    /// 
    /// CTR System Update (CSU) is a variant of CCI, where the only difference
    /// is in the file extension.
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/NCSD"/>
    public sealed class NCSDHeader
    {
        #region Common to all NCSD files

        /// <summary>
        /// RSA-2048 SHA-256 signature of the NCSD header
        /// </summary>
        public byte[] RSA2048Signature;

        /// <summary>
        /// Magic Number 'NCSD'
        /// </summary>
        public string MagicNumber;

        /// <summary>
        /// Size of the NCSD image, in media units (1 media unit = 0x200 bytes)
        /// </summary>
        public uint ImageSizeInMediaUnits;

        /// <summary>
        /// Media ID
        /// </summary>
        public byte[] MediaId;

        /// <summary>
        /// Partitions FS type (0=None, 1=Normal, 3=FIRM, 4=AGB_FIRM save)
        /// </summary>
        public FilesystemType PartitionsFSType;

        /// <summary>
        /// Partitions crypt type (each byte corresponds to a partition in the partition table)
        /// </summary>
        public byte[] PartitionsCryptType;

        /// <summary>
        /// Offset & Length partition table, in media units
        /// </summary>
        public PartitionTableEntry[] PartitionsTable;

        #endregion

        #region CTR Cart Image (CCI) Specific

        /// <summary>
        /// Exheader SHA-256 hash
        /// </summary>
        public byte[] ExheaderHash;

        /// <summary>
        /// Additional header size
        /// </summary>
        public uint AdditionalHeaderSize;

        /// <summary>
        /// Sector zero offset
        /// </summary>
        public uint SectorZeroOffset;

        /// <summary>
        /// Partition Flags
        /// </summary>
        public byte[] PartitionFlags;

        /// <summary>
        /// Partition ID table
        /// </summary>
        public ulong[] PartitionIdTable;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved1;

        /// <summary>
        /// Reserved?
        /// </summary>
        public byte[] Reserved2;

        /// <summary>
        /// Support for this was implemented with 9.6.0-X FIRM. Bit0=1 enables using bits 1-2, it's unknown
        /// what these two bits are actually used for(the value of these two bits get compared with some other
        /// value during NCSD verification/loading). This appears to enable a new, likely hardware-based,
        /// antipiracy check on cartridges.
        /// </summary>
        public byte FirmUpdateByte1;

        /// <summary>
        /// Support for this was implemented with 9.6.0-X FIRM, see below regarding save crypto.
        /// </summary>
        public byte FirmUpdateByte2;

        #endregion

        #region Raw NAND Format Specific

        /// <summary>
        /// Unknown
        /// </summary>
        public byte[] Unknown;

        /// <summary>
        /// Encrypted MBR partition-table, for the TWL partitions(key-data used for this keyslot is console-unique).
        /// </summary>
        public byte[] EncryptedMBR;

        #endregion
    }
}
