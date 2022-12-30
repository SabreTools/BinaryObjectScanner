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

    // TODO: Left off at `struct cds_forward `

    #endregion
}