using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static BurnOutSharp.Wrappers.CabinetConstants;
using static BurnOutSharp.Wrappers.FDIConstants;

namespace BurnOutSharp.Wrappers
{
    public partial class MicrosoftCabinet
    {

    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/fdi.h"/>
    #region fdi.h

    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/fdi.h"/>
    internal class ERF
    {
        /// <summary>
        /// FCI/FDI error code - see {FCI,FDI}ERROR_XXX for details.
        /// </summary>
        public int erfOper;

        /// <summary>
        /// Optional error value filled in by FCI/FDI.
        /// </summary>
        public int erfType;

        /// <summary>
        /// TRUE => error present
        /// </summary>
        public bool fError;
    }

    /**********************************************************************/

    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/fdi.h"/>
    internal static class FDIConstants
    {
        public const ushort CB_MAX_CHUNK = 32768;
        public const uint CB_MAX_DISK = 0x7fffffff;
        public const int CB_MAX_FILENAME = 256;
        public const int CB_MAX_CABINET_NAME = 256;
        public const int CB_MAX_CAB_PATH = 256;
        public const int CB_MAX_DISK_NAME = 256;

        /**********************************************************************/

        /// <summary>
        /// Mask for compression type
        /// </summary>
        public const ushort tcompMASK_TYPE = 0x000F;

        /// <summary>
        /// No compression
        /// </summary>
        public const ushort tcompTYPE_NONE = 0x0000;

        /// <summary>
        /// MSZIP
        /// </summary>
        public const ushort tcompTYPE_MSZIP = 0x0001;

        /// <summary>
        /// Quantum
        /// </summary>
        public const ushort tcompTYPE_QUANTUM = 0x0002;

        /// <summary>
        /// LZX
        /// </summary>
        public const ushort tcompTYPE_LZX = 0x0003;

        /// <summary>
        /// Unspecified compression type
        /// </summary>
        public const ushort tcompBAD = 0x000F;

        /// <summary>
        /// Mask for LZX Compression Memory
        /// </summary>
        public const ushort tcompMASK_LZX_WINDOW = 0x1F00;

        /// <summary>
        /// Lowest LZX Memory (15)
        /// </summary>
        public const ushort tcompLZX_WINDOW_LO = 0x0F00;

        /// <summary>
        /// Highest LZX Memory (21)
        /// </summary>
        public const ushort tcompLZX_WINDOW_HI = 0x1500;

        /// <summary>
        /// Amount to shift over to get int
        /// </summary>
        public const ushort tcompSHIFT_LZX_WINDOW = 8;

        /// <summary>
        /// Mask for Quantum Compression Level
        /// </summary>
        public const ushort tcompMASK_QUANTUM_LEVEL = 0x00F0;

        /// <summary>
        /// Lowest Quantum Level (1)
        /// </summary>
        public const ushort tcompQUANTUM_LEVEL_LO = 0x0010;

        /// <summary>
        /// Highest Quantum Level (7)
        /// </summary>
        public const ushort tcompQUANTUM_LEVEL_HI = 0x0070;

        /// <summary>
        /// Amount to shift over to get int
        /// </summary>
        public const ushort tcompSHIFT_QUANTUM_LEVEL = 4;

        /// <summary>
        /// Mask for Quantum Compression Memory
        /// </summary>
        public const ushort tcompMASK_QUANTUM_MEM = 0x1F00;

        /// <summary>
        /// Lowest Quantum Memory (10)
        /// </summary>
        public const ushort tcompQUANTUM_MEM_LO = 0x0A00;

        /// <summary>
        /// Highest Quantum Memory (21)
        /// </summary>
        public const ushort tcompQUANTUM_MEM_HI = 0x1500;

        /// <summary>
        /// Amount to shift over to get int
        /// </summary>
        public const ushort tcompSHIFT_QUANTUM_MEM = 8;

        /// <summary>
        /// Reserved bits (high 3 bits)
        /// </summary>
        public const ushort tcompMASK_RESERVED = 0xE000;

        /**********************************************************************/

        public const byte _A_NAME_IS_UTF = 0x80;

        public const byte _A_EXEC = 0x40;

        /**********************************************************************/

        /// <summary>
        /// FDI does detection
        /// </summary>
        public const int cpuUNKNOWN = -1;

        /// <summary>
        /// '286 opcodes only
        /// </summary>
        public const int cpu80286 = 0;

        /// <summary>
        /// '386 opcodes used
        /// </summary>
        public const int cpu80386 = 1;

        /**********************************************************************/
    }

    /**********************************************************************/

    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/fdi.h"/>
    internal enum FDIERROR
    {
        FDIERROR_NONE,
        FDIERROR_CABINET_NOT_FOUND,
        FDIERROR_NOT_A_CABINET,
        FDIERROR_UNKNOWN_CABINET_VERSION,
        FDIERROR_CORRUPT_CABINET,
        FDIERROR_ALLOC_FAIL,
        FDIERROR_BAD_COMPR_TYPE,
        FDIERROR_MDI_FAIL,
        FDIERROR_TARGET_FILE,
        FDIERROR_RESERVE_MISMATCH,
        FDIERROR_WRONG_CABINET,
        FDIERROR_USER_ABORT,
    }

    /**********************************************************************/

    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/fdi.h"/>
    internal class FDICABINETINFO
    {
        /// <summary>
        /// Total length of cabinet file
        /// </summary>
        public long cbCabinet;

        /// <summary>
        /// Count of folders in cabinet
        /// </summary>
        public ushort cFolders;

        /// <summary>
        /// Count of files in cabinet
        /// </summary>
        public ushort cFiles;

        /// <summary>
        /// Cabinet set ID
        /// </summary>
        public ushort setID;

        /// <summary>
        /// Cabinet number in set (0 based)
        /// </summary>
        public ushort iCabinet;

        /// <summary>
        /// TRUE => RESERVE present in cabinet
        /// </summary>
        public bool fReserve;

        /// <summary>
        /// TRUE => Cabinet is chained prev
        /// </summary>
        public bool hasprev;

        /// <summary>
        /// TRUE => Cabinet is chained next
        /// </summary>
        public bool hasnext;
    }

    /**********************************************************************/

    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/fdi.h"/>
    internal enum FDIDECRYPTTYPE
    {
        /// <summary>
        /// New cabinet
        /// </summary>
        fdidtNEW_CABINET,

        /// <summary>
        /// New folder
        /// </summary>
        fdidtNEW_FOLDER,

        /// <summary>
        /// Decrypt a data block
        /// </summary>
        fdidtDECRYPT,
    }

    /**********************************************************************/

    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/fdi.h"/>
    internal class FDIDECRYPT
    {
        /// <summary>
        /// Command type (selects union below)
        /// </summary>
        public FDIDECRYPTTYPE fdidt;

        /// <summary>
        /// Decryption context
        /// </summary>
        public object pvUser;

        #region DUMMYUNIONNAME

        /// <summary>
        /// fdidtNEW_CABINET
        /// </summary>
        public DUMMYUNIONNAMEcabinet cabinet;

        /// <summary>
        /// fdidtNEW_FOLDER
        /// </summary>
        public DUMMYUNIONNAMEfolder folder;

        /// <summary>
        /// fdidtDECRYPT
        /// </summary>
        public DUMMYUNIONNAMEdecrypt decrypt;

        /// <summary>
        /// fdidtNEW_CABINET
        /// </summary>
        public class DUMMYUNIONNAMEcabinet
        {
            /// <summary>
            /// RESERVE section from CFHEADER
            /// </summary>
            public object pHeaderReserve;

            /// <summary>
            /// Size of pHeaderReserve
            /// </summary>
            public ushort cbHeaderReserve;

            /// <summary>
            /// Cabinet set ID
            /// </summary>
            public ushort setID;

            /// <summary>
            /// Cabinet number in set (0 based)
            /// </summary>
            public int iCabinet;
        }

        /// <summary>
        /// fdidtNEW_FOLDER
        /// </summary>
        public class DUMMYUNIONNAMEfolder
        {
            /// <summary>
            /// RESERVE section from CFFOLDER
            /// </summary>
            public object pFolderReserve;

            /// <summary>
            /// Size of pFolderReserve
            /// </summary>
            public ushort cbFolderReserve;

            /// <summary>
            /// Folder number in cabinet (0 based)
            /// </summary>
            public ushort iFolder;
        }

        /// <summary>
        /// fdidtDECRYPT
        /// </summary>
        public class DUMMYUNIONNAMEdecrypt
        {
            /// <summary>
            /// RESERVE section from CFDATA
            /// </summary>
            public object pDataReserve;

            /// <summary>
            /// Size of pDataReserve
            /// </summary>
            public ushort cbDataReserve;

            /// <summary>
            /// Data buffer
            /// </summary>
            public object pbData;

            /// <summary>
            /// Size of data buffer
            /// </summary>
            public ushort cbData;

            /// <summary>
            /// TRUE if this is a split data block
            /// </summary>
            public bool fSplit;

            /// <summary>
            /// 0 if this is not a split block, or the first piece of a split block;
            /// Greater than 0 if this is the second piece of a split block.
            /// </summary>
            public ushort cbPartial;
        }

        #endregion
    }

    /**********************************************************************/

    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/fdi.h"/>
    internal class FDINOTIFICATION
    {
        public long cb;
        public char[] psz1;
        public char[] psz2;

        /// <summary>
        /// Points to a 256 character buffer
        /// </summary>
        public char[] psz3;

        /// <summary>
        /// Value for client
        /// </summary>
        public object pv;

        public IntPtr hf;

        public ushort date;
        public ushort time;
        public ushort attribs;

        /// <summary>
        /// Cabinet set ID
        /// </summary>
        public ushort setID;

        /// <summary>
        /// Cabinet number (0-based)
        /// </summary>
        public ushort iCabinet;

        /// <summary>
        /// Folder number (0-based)
        /// </summary>
        public ushort iFolder;

        public FDIERROR fdie;
    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/fdi.h"/>
    internal enum FDINOTIFICATIONTYPE
    {
        /// <summary>
        /// General information about cabinet
        /// </summary>
        fdintCABINET_INFO,

        /// <summary>
        /// First file in cabinet is continuation
        /// </summary>
        fdintPARTIAL_FILE,

        /// <summary>
        /// File to be copied
        /// </summary>
        fdintCOPY_FILE,

        /// <summary>
        /// Close the file, set relevant info
        /// </summary>
        fdintCLOSE_FILE_INFO,

        /// <summary>
        /// File continued to next cabinet
        /// </summary>
        fdintNEXT_CABINET,

        /// <summary>
        /// Enumeration status
        /// </summary>
        fdintENUMERATE,
    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/fdi.h"/>
    internal class FDINOTIFY
    {
        public FDINOTIFICATIONTYPE fdint;

        public FDINOTIFICATION pfdin;
    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/fdi.h"/>
    internal class FDISPILLFILE
    {
        /// <summary>
        /// Set to { '*', '\0' }
        /// </summary>
        public char[] ach = new char[2];

        /// <summary>
        /// Required spill file size
        /// </summary>
        public long cbFile;
    }

    #endregion

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    #region cabinet.h

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal static class CabinetConstants
    {
        public const int CAB_SPLITMAX = (10);

        public const int CAB_SEARCH_SIZE = (32 * 1024);

        /* structure offsets */
        public const byte cfhead_Signature = (0x00);
        public const byte cfhead_CabinetSize = (0x08);
        public const byte cfhead_FileOffset = (0x10);
        public const byte cfhead_MinorVersion = (0x18);
        public const byte cfhead_MajorVersion = (0x19);
        public const byte cfhead_NumFolders = (0x1A);
        public const byte cfhead_NumFiles = (0x1C);
        public const byte cfhead_Flags = (0x1E);
        public const byte cfhead_SetID = (0x20);
        public const byte cfhead_CabinetIndex = (0x22);
        public const byte cfhead_SIZEOF = (0x24);
        public const byte cfheadext_HeaderReserved = (0x00);
        public const byte cfheadext_FolderReserved = (0x02);
        public const byte cfheadext_DataReserved = (0x03);
        public const byte cfheadext_SIZEOF = (0x04);
        public const byte cffold_DataOffset = (0x00);
        public const byte cffold_NumBlocks = (0x04);
        public const byte cffold_CompType = (0x06);
        public const byte cffold_SIZEOF = (0x08);
        public const byte cffile_UncompressedSize = (0x00);
        public const byte cffile_FolderOffset = (0x04);
        public const byte cffile_FolderIndex = (0x08);
        public const byte cffile_Date = (0x0A);
        public const byte cffile_Time = (0x0C);
        public const byte cffile_Attribs = (0x0E);
        public const byte cffile_SIZEOF = (0x10);
        public const byte cfdata_CheckSum = (0x00);
        public const byte cfdata_CompressedSize = (0x04);
        public const byte cfdata_UncompressedSize = (0x06);
        public const byte cfdata_SIZEOF = (0x08);

        /* flags */
        public const ushort cffoldCOMPTYPE_MASK = (0x000f);
        public const ushort cffoldCOMPTYPE_NONE = (0x0000);
        public const ushort cffoldCOMPTYPE_MSZIP = (0x0001);
        public const ushort cffoldCOMPTYPE_QUANTUM = (0x0002);
        public const ushort cffoldCOMPTYPE_LZX = (0x0003);
        public const ushort cfheadPREV_CABINET = (0x0001);
        public const ushort cfheadNEXT_CABINET = (0x0002);
        public const ushort cfheadRESERVE_PRESENT = (0x0004);
        public const ushort cffileCONTINUED_FROM_PREV = (0xFFFD);
        public const ushort cffileCONTINUED_TO_NEXT = (0xFFFE);
        public const ushort cffileCONTINUED_PREV_AND_NEXT = (0xFFFF);
        public const byte cffile_A_RDONLY = (0x01);
        public const byte cffile_A_HIDDEN = (0x02);
        public const byte cffile_A_SYSTEM = (0x04);
        public const byte cffile_A_ARCH = (0x20);
        public const byte cffile_A_EXEC = (0x40);
        public const byte cffile_A_NAME_IS_UTF = (0x80);

        /****************************************************************************/
        /* our archiver information / state */

        /* MSZIP stuff */

        /// <summary>
        /// window size
        /// </summary>
        public const ushort ZIPWSIZE = 0x8000;

        /// <summary>
        /// bits in base literal/length lookup table
        /// </summary>
        public const int ZIPLBITS = 9;

        /// <summary>
        /// bits in base distance lookup table
        /// </summary>
        public const int ZIPDBITS = 6;

        /// <summary>
        /// maximum bit length of any code
        /// </summary>
        public const int ZIPBMAX = 16;

        /// <summary>
        /// maximum number of codes in any set
        /// </summary>
        public const int ZIPN_MAX = 288;

        /* LZX stuff */

        /* some constants defined by the LZX specification */
        public const int LZX_MIN_MATCH = (2);
        public const int LZX_MAX_MATCH = (257);
        public const int LZX_NUM_CHARS = (256);

        /// <summary>
        /// also blocktypes 4-7 invalid
        /// </summary>
        public const int LZX_BLOCKTYPE_INVALID = (0);
        public const int LZX_BLOCKTYPE_VERBATIM = (1);
        public const int LZX_BLOCKTYPE_ALIGNED = (2);
        public const int LZX_BLOCKTYPE_UNCOMPRESSED = (3);
        public const int LZX_PRETREE_NUM_ELEMENTS = (20);

        /// <summary>
        /// aligned offset tree #elements
        /// </summary>
        public const int LZX_ALIGNED_NUM_ELEMENTS = (8);

        /// <summary>
        /// this one missing from spec!
        /// </summary>
        public const int LZX_NUM_PRIMARY_LENGTHS = (7);

        /// <summary>
        /// length tree #elements
        /// </summary>
        public const int LZX_NUM_SECONDARY_LENGTHS = (249);

        /* LZX huffman defines: tweak tablebits as desired */
        public const int LZX_PRETREE_MAXSYMBOLS = (LZX_PRETREE_NUM_ELEMENTS);
        public const int LZX_PRETREE_TABLEBITS = (6);
        public const int LZX_MAINTREE_MAXSYMBOLS = (LZX_NUM_CHARS + 50 * 8);
        public const int LZX_MAINTREE_TABLEBITS = (12);
        public const int LZX_LENGTH_MAXSYMBOLS = (LZX_NUM_SECONDARY_LENGTHS + 1);
        public const int LZX_LENGTH_TABLEBITS = (12);
        public const int LZX_ALIGNED_MAXSYMBOLS = (LZX_ALIGNED_NUM_ELEMENTS);
        public const int LZX_ALIGNED_TABLEBITS = (7);

        public const int LZX_LENTABLE_SAFETY = (64); /* we allow length table decoding overruns */

        /* CAB data blocks are <= 32768 bytes in uncompressed form. Uncompressed
        * blocks have zero growth. MSZIP guarantees that it won't grow above
        * uncompressed size by more than 12 bytes. LZX guarantees it won't grow
        * more than 6144 bytes.
        */
        public const int CAB_BLOCKMAX = (32768);
        public const int CAB_INPUTMAX = (CAB_BLOCKMAX + 6144);

        /****************************************************************************/
        /* Tables for deflate from PKZIP's appnote.txt. */

        //#define THOSE_ZIP_CONSTS

        /* Order of the bit length code lengths */
        public static readonly byte[] Zipborder =
        { 16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15};

        /* Copy lengths for literal codes 257..285 */
        public static readonly ushort[] Zipcplens =
        { 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31, 35, 43, 51,
        59, 67, 83, 99, 115, 131, 163, 195, 227, 258, 0, 0};

        /* Extra bits for literal codes 257..285 */
        public static readonly ushort[] Zipcplext =
        { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4,
        4, 5, 5, 5, 5, 0, 99, 99}; /* 99==invalid */

        /* Copy offsets for distance codes 0..29 */
        public static readonly ushort[] Zipcpdist =
        { 1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385,
        513, 769, 1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577};

        /* Extra bits for distance codes */
        public static readonly ushort[] Zipcpdext =
        { 0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10,
        10, 11, 11, 12, 12, 13, 13};

        /* And'ing with Zipmask[n] masks the lower n bits */
        public static readonly ushort[] Zipmask = new ushort[17]
        { 0x0000, 0x0001, 0x0003, 0x0007, 0x000f, 0x001f, 0x003f, 0x007f, 0x00ff,
        0x01ff, 0x03ff, 0x07ff, 0x0fff, 0x1fff, 0x3fff, 0x7fff, 0xffff };

        /* SESSION Operation */
        public const uint EXTRACT_FILLFILELIST  = 0x00000001;
        public const uint EXTRACT_EXTRACTFILES  = 0x00000002;
    }

    /* MSZIP stuff */

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class Ziphuft
    {
        /// <summary>
        /// number of extra bits or operation
        /// </summary>
        public byte e;

        /// <summary>
        /// number of bits in this code or subcode
        /// </summary>
        public byte b;

        #region v

        /// <summary>
        /// literal, length base, or distance base
        /// </summary>
        public ushort n;

        /// <summary>
        /// pointer to next level of table
        /// </summary>
        public Ziphuft t;

        #endregion
    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class ZIPstate
    {
        /// <summary>
        /// current offset within the window
        /// </summary>
        public uint window_posn;

        /// <summary>
        /// bit buffer
        /// </summary>
        public uint bb;

        /// <summary>
        /// bits in bit buffer
        /// </summary>
        public uint bk;

        /// <summary>
        /// literal/length and distance code lengths
        /// </summary>
        public uint[] ll = new uint[288 + 32];

        /// <summary>
        /// bit length count table
        /// </summary>
        public uint[] c = new uint[ZIPBMAX + 1];

        /// <summary>
        /// memory for l[-1..ZIPBMAX-1]
        /// </summary>
        public uint[] lx = new uint[ZIPBMAX + 1];

        /// <summary>
        /// table stack
        /// </summary>
        public Ziphuft[] u = new Ziphuft[ZIPBMAX];

        /// <summary>
        /// values in order of bit length
        /// </summary>
        public uint[] v = new uint[ZIPN_MAX];

        /// <summary>
        /// bit offsets, then code stack
        /// </summary>
        public uint[] x = new uint[ZIPBMAX + 1];

        public int inpos; // byte*
    }

    /* Quantum stuff */

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class QTMmodelsym
    {
        public ushort sym;

        public ushort cumfreq;
    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class QTMmodel
    {
        public int shiftsleft;

        public int entries;

        public QTMmodelsym[] syms;

        public ushort[] tabloc = new ushort[256];
    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class QTMstate
    {
        /// <summary>
        /// the actual decoding window
        /// </summary>
        public byte[] window;

        /// <summary>
        /// window size (1Kb through 2Mb)
        /// </summary>
        public uint window_size;

        /// <summary>
        /// window size when it was first allocated
        /// </summary>
        public uint actual_size;

        /// <summary>
        /// current offset within the window
        /// </summary>
        public uint window_posn;

        public QTMmodel model7;
        public QTMmodelsym[] m7sym = new QTMmodelsym[7 + 1];

        public QTMmodel model4;
        public QTMmodel model5;
        public QTMmodel model6pos;
        public QTMmodel model6len;
        public QTMmodelsym[] m4sym = new QTMmodelsym[0x18 + 1];
        public QTMmodelsym[] m5sym = new QTMmodelsym[0x24 + 1];
        public QTMmodelsym[] m6psym = new QTMmodelsym[0x2a + 1];
        public QTMmodelsym[] m6lsym = new QTMmodelsym[0x1b + 1];

        public QTMmodel model00;
        public QTMmodel model40;
        public QTMmodel model80;
        public QTMmodel modelC0;
        public QTMmodelsym[] m00sym = new QTMmodelsym[0x40 + 1];
        public QTMmodelsym[] m40sym = new QTMmodelsym[0x40 + 1];
        public QTMmodelsym[] m80sym = new QTMmodelsym[0x40 + 1];
        public QTMmodelsym[] mC0sym = new QTMmodelsym[0x40 + 1];
    }

    /* LZX stuff */

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class LZXstate
    {
        /// <summary>
        /// the actual decoding window
        /// </summary>
        public byte[] window;

        /// <summary>
        /// window size (32Kb through 2Mb)
        /// </summary>
        public uint window_size;

        /// <summary>
        /// window size when it was first allocated
        /// </summary>
        public uint actual_size;

        /// <summary>
        /// current offset within the window
        /// </summary>
        public uint window_posn;

        /// <summary>
        /// for the LRU offset system
        /// </summary>
        public uint R0, R1, R2;

        /// <summary>
        /// number of main tree elements
        /// </summary>
        public ushort main_elements;

        /// <summary>
        /// have we started decoding at all yet?
        /// </summary>
        public int header_read;

        /// <summary>
        /// type of this block
        /// </summary>
        public ushort block_type;

        /// <summary>
        /// uncompressed length of this block
        /// </summary>
        public uint block_length;

        /// <summary>
        /// uncompressed bytes still left to decode
        /// </summary>
        public uint block_remaining;

        /// <summary>
        /// the number of CFDATA blocks processed
        /// </summary>
        public uint frames_read;

        /// <summary>
        /// magic header value used for transform
        /// </summary>
        public int intel_filesize;

        /// <summary>
        /// current offset in transform space
        /// </summary>
        public int intel_curpos;

        /// <summary>
        /// have we seen any translatable data yet?
        /// </summary>
        public int intel_started;

        public ushort[] tblPRETREE_table = new ushort[(1 << LZX_PRETREE_TABLEBITS) + (LZX_PRETREE_MAXSYMBOLS << 1)];
        public ushort[] tblMAINTREE_table = new ushort[(1 << LZX_MAINTREE_TABLEBITS) + (LZX_MAINTREE_MAXSYMBOLS << 1)];
        public ushort[] tblLENGTH_table = new ushort[(1 << LZX_LENGTH_TABLEBITS) + (LZX_LENGTH_MAXSYMBOLS << 1)];
        public ushort[] tblALIGNED_table = new ushort[(1 << LZX_ALIGNED_TABLEBITS) + (LZX_ALIGNED_MAXSYMBOLS << 1)];
    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class lzx_bits
    {
        public uint bb;

        public int bl;

        public byte[] ip; // byte*
    }

    /****************************************************************************/

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class cab_file
    {
        /// <summary>
        /// next file in sequence
        /// </summary>
        public cab_file next;

        /// <summary>
        /// folder that contains this file
        /// </summary>
        public cab_folder folder;

        /// <summary>
        /// output name of file
        /// </summary>
        public string filename;

        /// <summary>
        /// open file handle or NULL
        /// </summary>
        public Stream fh;

        /// <summary>
        /// uncompressed length of file
        /// </summary>
        public uint length;

        /// <summary>
        /// uncompressed offset in folder
        /// </summary>
        public uint offset;

        /// <summary>
        /// magic index number of folder
        /// </summary>
        public ushort index;

        /// <summary>
        /// MS-DOS time/date/attributes
        /// </summary>
        public ushort time, date, attribs;
    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class cab_folder
    {
        /// <summary>
        /// next folder in sequence
        /// </summary>
        public cab_folder next;

        /// <summary>
        /// cabinet(s) this folder spans
        /// </summary>
        public cabinet[] cab = new cabinet[CAB_SPLITMAX];

        /// <summary>
        /// offset to data blocks
        /// </summary>
        public uint[] offset = new uint[CAB_SPLITMAX];

        /// <summary>
        /// compression format/window size
        /// </summary>
        public ushort comp_type;

        /// <summary>
        /// compressed size of folder
        /// </summary>
        public uint comp_size;

        /// <summary>
        /// number of split blocks + 1
        /// </summary>
        public byte num_splits;

        /// <summary>
        /// total number of blocks
        /// </summary>
        public ushort num_blocks;

        /// <summary>
        /// the first split file
        /// </summary>
        public cab_file contfile;
    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class cabinet
    {
        /// <summary>
        /// next cabinet in sequence
        /// </summary>
        public cabinet next;

        /// <summary>
        /// input name of cabinet
        /// </summary>
        public string filename;

        /// <summary>
        /// open file handle or NULL
        /// </summary>
        public Stream fh;

        /// <summary>
        /// length of cabinet file
        /// </summary>
        public uint filelen;

        /// <summary>
        /// offset to data blocks in file
        /// </summary>
        public uint blocks_off;

        /// <summary>
        /// multipart cabinet chains
        /// </summary>
        public cabinet prevcab, nextcab;

        /// <summary>
        /// and their filenames
        /// </summary>
        public string prevname, nextname;

        /// <summary>
        /// and their visible names
        /// </summary>
        public string previnfo, nextinfo;

        /// <summary>
        /// first folder in this cabinet
        /// </summary>
        public cab_folder folders;

        /// <summary>
        /// first file in this cabinet
        /// </summary>
        public cab_file files;

        /// <summary>
        /// reserved space in datablocks
        /// </summary>
        public byte block_resv;

        /// <summary>
        /// header flags
        /// </summary>
        public byte flags;
    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class cds_forward
    {
        /// <summary>
        /// current folder we're extracting from
        /// </summary>
        public cab_folder current;

        /// <summary>
        /// uncompressed offset within folder
        /// </summary>
        public uint offset;

        /// <summary>
        /// (high level) start of data to use up
        /// </summary>
        public byte outpos; // byte*

        /// <summary>
        /// (high level) amount of data to use up
        /// </summary>
        public ushort outlen;

        /// <summary>
        /// at which split in current folder?
        /// </summary>
        public ushort split;

        /// <summary>
        /// chosen compress fn
        /// </summary>
        public Func<int, int, cds_forward, int> decompress;

        /// <summary>
        /// +2 for lzx bitbuffer overflows!
        /// </summary>
        public byte[] inbuf = new byte[CAB_INPUTMAX + 2];

        public byte[] outbuf = new byte[CAB_INPUTMAX];

        public byte[] q_length_base = new byte[27], q_length_extra = new byte[27], q_extra_bits = new byte[42];

        public uint[] q_position_base = new uint[42];

        public uint[] lzx_position_base = new uint[51];

        public uint[] extra_bits = new uint[51];

        #region methods

        public ZIPstate zip;
        public QTMstate qtm;
        public LZXstate lzx;

        #endregion
    }

    /*
    * the rest of these are somewhat kludgy macros which are shared between fdi.c
    * and cabextract.c.
    */

    /* Bitstream reading macros (Quantum / normal byte order)
    *
    * Q_INIT_BITSTREAM    should be used first to set up the system
    * Q_READ_BITS(var,n)  takes N bits from the buffer and puts them in var.
    *                     unlike LZX, this can loop several times to get the
    *                     requisite number of bits.
    * Q_FILL_BUFFER       adds more data to the bit buffer, if there is room
    *                     for another 16 bits.
    * Q_PEEK_BITS(n)      extracts (without removing) N bits from the bit
    *                     buffer
    * Q_REMOVE_BITS(n)    removes N bits from the bit buffer
    *
    * These bit access routines work by using the area beyond the MSB and the
    * LSB as a free source of zeroes. This avoids having to mask any bits.
    * So we have to know the bit width of the bitbuffer variable. This is
    * defined as ULONG_BITS.
    *
    * ULONG_BITS should be at least 16 bits. Unlike LZX's Huffman decoding,
    * Quantum's arithmetic decoding only needs 1 bit at a time, it doesn't
    * need an assured number. Retrieving larger bitstrings can be done with
    * multiple reads and fills of the bitbuffer. The code should work fine
    * for machines where ULONG >= 32 bits.
    *
    * Also note that Quantum reads bytes in normal order; LZX is in
    * little-endian order.
    */

    // #define Q_INIT_BITSTREAM do { bitsleft = 0; bitbuf = 0; } while (0)

    // #define Q_FILL_BUFFER do {                                                  \
    // if (bitsleft <= (CAB_ULONG_BITS - 16)) {                                  \
    //     bitbuf |= ((inpos[0]<<8)|inpos[1]) << (CAB_ULONG_BITS-16 - bitsleft);   \
    //     bitsleft += 16; inpos += 2;                                             \
    // }                                                                         \
    // } while (0)

    // #define Q_PEEK_BITS(n)   (bitbuf >> (CAB_ULONG_BITS - (n)))
    // #define Q_REMOVE_BITS(n) ((bitbuf <<= (n)), (bitsleft -= (n)))

    // #define Q_READ_BITS(v,n) do {                                           \
    // (v) = 0;                                                              \
    // for (bitsneed = (n); bitsneed; bitsneed -= bitrun) {                  \
    //     Q_FILL_BUFFER;                                                      \
    //     bitrun = (bitsneed > bitsleft) ? bitsleft : bitsneed;               \
    //     (v) = ((v) << bitrun) | Q_PEEK_BITS(bitrun);                        \
    //     Q_REMOVE_BITS(bitrun);                                              \
    // }                                                                     \
    // } while (0)

    // #define Q_MENTRIES(model) (QTM(model).entries)
    // #define Q_MSYM(model,symidx) (QTM(model).syms[(symidx)].sym)
    // #define Q_MSYMFREQ(model,symidx) (QTM(model).syms[(symidx)].cumfreq)

    /* GET_SYMBOL(model, var) fetches the next symbol from the stated model
    * and puts it in var. it may need to read the bitstream to do this.
    */
    // #define GET_SYMBOL(m, var) do {                                         \
    // range =  ((H - L) & 0xFFFF) + 1;                                      \
    // symf = ((((C - L + 1) * Q_MSYMFREQ(m,0)) - 1) / range) & 0xFFFF;      \
    //                                                                         \
    // for (i=1; i < Q_MENTRIES(m); i++) {                                   \
    //     if (Q_MSYMFREQ(m,i) <= symf) break;                                 \
    // }                                                                     \
    // (var) = Q_MSYM(m,i-1);                                                \
    //                                                                         \
    // range = (H - L) + 1;                                                  \
    // H = L + ((Q_MSYMFREQ(m,i-1) * range) / Q_MSYMFREQ(m,0)) - 1;          \
    // L = L + ((Q_MSYMFREQ(m,i)   * range) / Q_MSYMFREQ(m,0));              \
    // while (1) {                                                           \
    //     if ((L & 0x8000) != (H & 0x8000)) {                                 \
    //     if ((L & 0x4000) && !(H & 0x4000)) {                              \
    //         /* underflow case */                                            \
    //         C ^= 0x4000; L &= 0x3FFF; H |= 0x4000;                          \
    //     }                                                                 \
    //     else break;                                                       \
    //     }                                                                   \
    //     L <<= 1; H = (H << 1) | 1;                                          \
    //     Q_FILL_BUFFER;                                                      \
    //     C  = (C << 1) | Q_PEEK_BITS(1);                                     \
    //     Q_REMOVE_BITS(1);                                                   \
    // }                                                                     \
    //                                                                         \
    // QTMupdatemodel(&(QTM(m)), i);                                         \
    // } while (0)

    /* Bitstream reading macros (LZX / intel little-endian byte order)
    *
    * INIT_BITSTREAM    should be used first to set up the system
    * READ_BITS(var,n)  takes N bits from the buffer and puts them in var
    *
    * ENSURE_BITS(n)    ensures there are at least N bits in the bit buffer.
    *                   it can guarantee up to 17 bits (i.e. it can read in
    *                   16 new bits when there is down to 1 bit in the buffer,
    *                   and it can read 32 bits when there are 0 bits in the
    *                   buffer).
    * PEEK_BITS(n)      extracts (without removing) N bits from the bit buffer
    * REMOVE_BITS(n)    removes N bits from the bit buffer
    *
    * These bit access routines work by using the area beyond the MSB and the
    * LSB as a free source of zeroes. This avoids having to mask any bits.
    * So we have to know the bit width of the bitbuffer variable.
    */

    // #define INIT_BITSTREAM do { bitsleft = 0; bitbuf = 0; } while (0)

    /* Quantum reads bytes in normal order; LZX is little-endian order */
    // #define ENSURE_BITS(n)                                                    \
    // while (bitsleft < (n)) {                                                \
    //     bitbuf |= ((inpos[1]<<8)|inpos[0]) << (CAB_ULONG_BITS-16 - bitsleft); \
    //     bitsleft += 16; inpos+=2;                                             \
    // }

    // #define PEEK_BITS(n)   (bitbuf >> (CAB_ULONG_BITS - (n)))
    // #define REMOVE_BITS(n) ((bitbuf <<= (n)), (bitsleft -= (n)))

    // #define READ_BITS(v,n) do {                                             \
    // if (n) {                                                              \
    //     ENSURE_BITS(n);                                                     \
    //     (v) = PEEK_BITS(n);                                                 \
    //     REMOVE_BITS(n);                                                     \
    // }                                                                     \
    // else {                                                                \
    //     (v) = 0;                                                            \
    // }                                                                     \
    // } while (0)

    /* Huffman macros */

    // #define TABLEBITS(tbl)   (LZX_##tbl##_TABLEBITS)
    // #define MAXSYMBOLS(tbl)  (LZX_##tbl##_MAXSYMBOLS)
    // #define SYMTABLE(tbl)    (LZX(tbl##_table))
    // #define LENTABLE(tbl)    (LZX(tbl##_len))

    /* BUILD_TABLE(tablename) builds a huffman lookup table from code lengths.
    * In reality, it just calls make_decode_table() with the appropriate
    * values - they're all fixed by some #defines anyway, so there's no point
    * writing each call out in full by hand.
    */
    // #define BUILD_TABLE(tbl)                                                \
    // if (make_decode_table(                                                \
    //     MAXSYMBOLS(tbl), TABLEBITS(tbl), LENTABLE(tbl), SYMTABLE(tbl)       \
    // )) { return DECR_ILLEGALDATA; }

    /* READ_HUFFSYM(tablename, var) decodes one huffman symbol from the
    * bitstream using the stated table and puts it in var.
    */
    // #define READ_HUFFSYM(tbl,var) do {                                      \
    // ENSURE_BITS(16);                                                      \
    // hufftbl = SYMTABLE(tbl);                                              \
    // if ((i = hufftbl[PEEK_BITS(TABLEBITS(tbl))]) >= MAXSYMBOLS(tbl)) {    \
    //     j = 1 << (CAB_ULONG_BITS - TABLEBITS(tbl));                         \
    //     do {                                                                \
    //     j >>= 1; i <<= 1; i |= (bitbuf & j) ? 1 : 0;                      \
    //     if (!j) { return DECR_ILLEGALDATA; }                              \
    //     } while ((i = hufftbl[i]) >= MAXSYMBOLS(tbl));                      \
    // }                                                                     \
    // j = LENTABLE(tbl)[(var) = i];                                         \
    // REMOVE_BITS(j);                                                       \
    // } while (0)

    /* READ_LENGTHS(tablename, first, last) reads in code lengths for symbols
    * first to last in the given table. The code lengths are stored in their
    * own special LZX way.
    */
    // #define READ_LENGTHS(tbl,first,last,fn) do { \
    // lb.bb = bitbuf; lb.bl = bitsleft; lb.ip = inpos; \
    // if (fn(LENTABLE(tbl),(first),(last),&lb,decomp_state)) { \
    //     return DECR_ILLEGALDATA; \
    // } \
    // bitbuf = lb.bb; bitsleft = lb.bl; inpos = lb.ip; \
    // } while (0)

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class FILELIST
    {
        public string FileName;

        public FILELIST next;

        public bool DoExtract;
    }

    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    internal class SESSION
    {
        public int FileSize;

        public ERF Error;

        public FILELIST FileList;

        public int FileCount;

        public int Operation;

        public char[] Destination = new char[CB_MAX_CAB_PATH];

        public char[] CurrentFile = new char[CB_MAX_CAB_PATH];

        public char[] Reserved = new char[CB_MAX_CAB_PATH];

        public FILELIST FilterList;
    }

    #endregion
}