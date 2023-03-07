using System;

namespace BinaryObjectScanner.Models.TAR
{
    [Flags]
    public enum Mode : ushort
    {
        /// <summary>
        /// Execute/search by other
        /// </summary>
        TOEXEC = 0x0001,

        /// <summary>
        /// Write by other
        /// </summary>
        TOWRITE = 0x0002,

        /// <summary>
        /// Read by other
        /// </summary>
        TOREAD = 0x0004,

        /// <summary>
        /// Execute/search by group
        /// </summary>
        TGEXEC = 0x0008,

        /// <summary>
        /// Write by group
        /// </summary>
        TGWRITE = 0x0010,

        /// <summary>
        /// Read by group
        /// </summary>
        TGREAD = 0x0020,

        /// <summary>
        /// Execute/search by owner
        /// </summary>
        TUEXEC = 0x0040,

        /// <summary>
        /// Write by owner
        /// </summary>
        TUWRITE = 0x0080,

        /// <summary>
        /// Read by owner
        /// </summary>
        TUREAD = 0x0100,

        /// <summary>
        /// Reserved
        /// </summary>
        TSVTX = 0x0200,

        /// <summary>
        /// Set GID on execution
        /// </summary>
        TSGID = 0x0400,

        /// <summary>
        /// Set UID on execution
        /// </summary>
        TSUID = 0x0800,
    }

    public enum TypeFlag : byte
    {
        /// <summary>
        /// Regular file
        /// </summary>
        REGTYPE = (byte)'0',

        /// <summary>
        /// Regular file
        /// </summary>
        AREGTYPE = 0,

        /// <summary>
        /// Hard link
        /// </summary>
        LNKTYPE = (byte)'1',

        /// <summary>
        /// Symbolic link
        /// </summary>
        SYMTYPE = (byte)'2',

        /// <summary>
        /// Character special
        /// </summary>
        CHRTYPE = (byte)'3',

        /// <summary>
        /// Block special
        /// </summary>
        BLKTYPE = (byte)'4',

        /// <summary>
        /// Directory
        /// </summary>
        DIRTYPE = (byte)'5',

        /// <summary>
        /// FIFO
        /// </summary>
        FIFOTYPE = (byte)'6',

        /// <summary>
        /// Contiguous file
        /// </summary>
        CONTTYPE = (byte)'7',

        /// <summary>
        /// Global extended header with meta data (POSIX.1-2001)
        /// </summary>
        XHDTYPE = (byte)'g',

        /// <summary>
        /// Extended header with metadata for the next file in the archive (POSIX.1-2001)
        /// </summary>
        XGLTYPE = (byte)'x',

        #region Vendor-Specific Extensions (POSIX.1-1988)

        VendorSpecificA = (byte)'A',
        VendorSpecificB = (byte)'B',
        VendorSpecificC = (byte)'C',
        VendorSpecificD = (byte)'D',
        VendorSpecificE = (byte)'E',
        VendorSpecificF = (byte)'F',
        VendorSpecificG = (byte)'G',
        VendorSpecificH = (byte)'H',
        VendorSpecificI = (byte)'I',
        VendorSpecificJ = (byte)'J',
        VendorSpecificK = (byte)'K',
        VendorSpecificL = (byte)'L',
        VendorSpecificM = (byte)'M',
        VendorSpecificN = (byte)'N',
        VendorSpecificO = (byte)'O',
        VendorSpecificP = (byte)'P',
        VendorSpecificQ = (byte)'Q',
        VendorSpecificR = (byte)'R',
        VendorSpecificS = (byte)'S',
        VendorSpecificT = (byte)'T',
        VendorSpecificU = (byte)'U',
        VendorSpecificV = (byte)'V',
        VendorSpecificW = (byte)'W',
        VendorSpecificX = (byte)'X',
        VendorSpecificY = (byte)'Y',
        VendorSpecificZ = (byte)'Z',

        #endregion
    }
}