using System;

namespace BurnOutSharp.Models.CFB
{
    /// <see href="https://winprotocoldoc.blob.core.windows.net/productionwindowsarchives/MS-CFB/%5bMS-CFB%5d.pdf"/>
    public sealed class FileHeader
    {
        /// <summary>
        /// IOdentification signature for the compound file structure, and MUST be
        /// set to the value 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1.
        /// </summary>
        public byte[] Signature;

        /// <summary>
        /// Reserved and unused class ID that MUST be set to all zeroes (CLSID_NULL)
        /// </summary>
        public Guid CLSID;

        /// <summary>
        /// Version number for nonbreaking changes. This field SHOULD be set to
        /// 0x003E if the major version field is either 0x0003 or 0x0004.
        /// </summary>
        public ushort MinorVersion;

        /// <summary>
        /// Version number for breaking changes. This field MUST be set to either
        /// 0x0003 (version 3) or 0x0004 (version 4).
        /// </summary>
        public ushort MajorVersion;

        /// <summary>
        /// This field MUST be set to 0xFFFE. This field is a byte order mark for
        /// all integer fields, specifying little-endian byte order.
        /// </summary>
        public ushort ByteOrder;

        /// <summary>
        /// This field MUST be set to 0x0009, or 0x000c, depending on the Major
        /// Version field. This field specifies the sector size of the compound file
        /// as a power of 2.
        /// 
        /// If Major Version is 3, the Sector Shift MUST be 0x0009, specifying a
        /// sector size of 512 bytes.
        /// 
        /// If Major Version is 4, the Sector Shift MUST be 0x000C, specifying a
        /// sector size of 4096 bytes.
        /// </summary>
        public ushort SectorShift;

        /// <summary>
        /// This field MUST be set to 0x0006. This field specifies the sector size
        /// of the Mini Stream as a power of 2. The sector size of the Mini Stream
        /// MUST be 64 bytes.
        /// </summary>
        public ushort MiniSectorShift;

        /// <summary>
        /// This field MUST be set to all zeroes.
        /// </summary>
        public byte[] Reserved;

        /// <summary>
        /// This integer field contains the count of the number of directory sectors
        /// in the compound file.
        /// 
        /// If Major Version is 3, the Number of Directory Sectors MUST be zero. This
        /// field is not supported for version 3 compound files.
        /// </summary>
        public uint NumberOfDirectorySectors;

        /// <summary>
        /// This integer field contains the count of the number of FAT sectors in the
        /// compound file.
        /// </summary>
        public uint NumberOfFATSectors;

        /// <summary>
        /// This integer field contains the starting sector number for the directory stream.
        /// </summary>
        public uint FirstDirectorySectorLocation;

        /// <summary>
        /// This integer field MAY contain a sequence number that is incremented every time
        /// the compound file is saved by an implementation that supports file transactions.
        /// This is the field that MUST be set to all zeroes if file transactions are not
        /// implemented.
        /// </summary>
        public uint TransactionSignatureNumber;

        /// <summary>
        /// This integer field MUST be set to 0x00001000. This field specifies the maximum
        /// size of a user-defined data stream that is allocated from the mini FAT and mini
        /// stream, and that cutoff is 4,096 bytes. Any user-defined data stream that is
        /// greater than or equal to this cutoff size must be allocated as normal sectors from
        /// the FAT.
        /// </summary>
        public uint MiniStreamCutoffSize;

        /// <summary>
        /// This integer field contains the starting sector number for the mini FAT.
        /// </summary>
        public uint FirstMiniFATSectorLocation;

        /// <summary>
        /// This integer field contains the count of the number of mini FAT sectors in the
        /// compound file.
        /// </summary>
        public uint NumberOfMiniFATSectors;

        /// <summary>
        /// This integer field contains the starting sector number for the DIFAT.
        /// </summary>
        public uint FirstDIFATSectorLocation;

        /// <summary>
        /// This integer field contains the count of the number of DIFAT sectors in the
        /// compound file.
        /// </summary>
        public uint NumberOfDIFATSectors;

        /// <summary>
        /// This array of 32-bit integer fields contains the first 109 FAT sector
        /// locations of the compound file
        /// </summary>
        public uint[] DIFAT;
    }
}