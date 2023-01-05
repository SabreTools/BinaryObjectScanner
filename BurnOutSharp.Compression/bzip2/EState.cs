using static BurnOutSharp.Compression.bzip2.Constants;

namespace BurnOutSharp.Compression.bzip2
{
    /// <summary>
    /// Structure holding all the compression-side stuff.
    /// </summary>
    /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/bzip2/bzlib_private.h"/>
    internal unsafe class EState
    {
        /* pointer back to the struct bz_stream */
        public bz_stream* strm;

        /* mode this stream is in, and whether inputting */
        /* or outputting data */
        public int mode;
        public int state;

        /* remembers avail_in when flush/finish requested */
        public uint avail_in_expect;

        /* for doing the block sorting */
        public uint* arr1;
        public uint* arr2;
        public uint* ftab;
        public int origPtr;

        /* aliases for arr1 and arr2 */
        public uint* ptr;
        public byte* block;
        public ushort* mtfv;
        public byte* zbits;

        /* for deciding when to use the fallback sorting algorithm */
        public int workFactor;

        /* run-length-encoding of the input */
        public uint state_in_ch;
        public int state_in_len;
        public int rNToGo;
        public int rTPos;

        /* input and output limits and current posns */
        public int nblock;
        public int nblockMAX;
        public int numZ;
        public int state_out_pos;

        /* map of bytes used in block */
        public int nInUse;
        public bool[] inUse = new bool[256];
        public byte[] unseqToSeq = new byte[256];

        /* the buffer for bit stream creation */
        public uint bsBuff;
        public int bsLive;

        /* block and combined CRCs */
        public uint blockCRC;
        public uint combinedCRC;

        /* misc administratium */
        public int verbosity;
        public int blockNo;
        public int blockSize100k;

        /* stuff for coding the MTF values */
        public int nMTF;
        public int[] mtfFreq = new int[BZ_MAX_ALPHA_SIZE];
        public byte[] selector = new byte[BZ_MAX_SELECTORS];
        public byte[] selectorMtf = new byte[BZ_MAX_SELECTORS];

        public byte[,] len = new byte[BZ_N_GROUPS, BZ_MAX_ALPHA_SIZE];
        public int[,] code = new int[BZ_N_GROUPS, BZ_MAX_ALPHA_SIZE];
        public int[,] rfreq = new int[BZ_N_GROUPS, BZ_MAX_ALPHA_SIZE];

        /* second dimension: only 3 needed; 4 makes index calculations faster */
        public uint[,] len_pack = new uint[BZ_MAX_ALPHA_SIZE, 4];
    }
}