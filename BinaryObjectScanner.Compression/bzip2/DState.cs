using static BinaryObjectScanner.Compression.bzip2.Constants;

namespace BinaryObjectScanner.Compression.bzip2
{
    /// <summary>
    /// Structure holding all the decompression-side stuff.
    /// </summary>
    /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/bzip2/bzlib_private.h"/>
    internal unsafe class DState
    {
        /* pointer back to the struct bz_stream */
        public bz_stream strm;

        /* state indicator for this stream */
        public int state;

        /* for doing the final run-length decoding */
        public byte state_out_ch;
        public int state_out_len;
        public bool blockRandomised;
        public int rNToGo;
        public int rTPos;

        /* the buffer for bit stream reading */
        public uint bsBuff;
        public int bsLive;

        /* misc administratium */
        public int blockSize100k;
        public bool smallDecompress;
        public int currBlockNo;
        public int verbosity;

        /* for undoing the Burrows-Wheeler transform */
        public int origPtr;
        public uint tPos;
        public int k0;
        public int[] unzftab = new int[256];
        public int nblock_used;
        public int[] cftab = new int[257];
        public int[] cftabCopy = new int[257];

        /* for undoing the Burrows-Wheeler transform (FAST) */
        public uint* tt;

        /* for undoing the Burrows-Wheeler transform (SMALL) */
        public ushort* ll16;
        public byte* ll4;

        /* stored and calculated CRCs */
        public uint storedBlockCRC;
        public uint storedCombinedCRC;
        public uint calculatedBlockCRC;
        public uint calculatedCombinedCRC;

        /* map of bytes used in block */
        public int nInUse;
        public bool[] inUse = new bool[256];
        public bool[] inUse16 = new bool[16];
        public byte[] seqToUnseq = new byte[256];

        /* for decoding the MTF values */
        public byte[] mtfa = new byte[MTFA_SIZE];
        public int[] mtfbase = new int[256 / MTFL_SIZE];
        public byte[] selector = new byte[BZ_MAX_SELECTORS];
        public byte[] selectorMtf = new byte[BZ_MAX_SELECTORS];
        public byte[,] len = new byte[BZ_N_GROUPS, BZ_MAX_ALPHA_SIZE];

        public int[,] limit = new int[BZ_N_GROUPS, BZ_MAX_ALPHA_SIZE];
        public int[,] @base = new int[BZ_N_GROUPS, BZ_MAX_ALPHA_SIZE];
        public int[,] perm = new int[BZ_N_GROUPS, BZ_MAX_ALPHA_SIZE];
        public int[] minLens = new int[BZ_N_GROUPS];

        /* save area for scalars in the main decompress code */
        public int save_i;
        public int save_j;
        public int save_t;
        public int save_alphaSize;
        public int save_nGroups;
        public int save_nSelectors;
        public int save_EOB;
        public int save_groupNo;
        public int save_groupPos;
        public int save_nextSym;
        public int save_nblockMAX;
        public int save_nblock;
        public int save_es;
        public int save_N;
        public int save_curr;
        public int save_zt;
        public int save_zn;
        public int save_zvec;
        public int save_zj;
        public int save_gSel;
        public int save_gMinlen;
        public int* save_gLimit;
        public int* save_gBase;
        public int* save_gPerm;
    }
}