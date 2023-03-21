using System;
using System.Runtime.InteropServices;
using BinaryObjectScanner.Models.Compression.MSZIP;
using static BinaryObjectScanner.Models.Compression.MSZIP.Constants;

namespace BinaryObjectScanner.Compression.MSZIP
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/fdi.c"/>
    public unsafe class Decompressor
    {
        /// <summary>
        /// Decompress a byte array using a given State
        /// </summary>
        public static bool Decompress(State state, int inlen, byte[] inbuf, int outlen, byte[] outbuf)
        {
            fixed (byte* inpos = inbuf)
            {
                state.inpos = inpos;
                state.bb = state.bk = state.window_posn = 0;
                if (outlen > ZIPWSIZE)
                    return false;

                //  CK = Chris Kirmse, official Microsoft purloiner
                if (state.inpos[0] != 0x43 || state.inpos[1] != 0x4B)
                    return false;

                state.inpos += 2;

                int lastBlockFlag = 0;
                do
                {
                    if (InflateBlock(&lastBlockFlag, state, inbuf, outbuf) != 0)
                        return false;
                } while (lastBlockFlag == 0);

                // Return success
                return true;
            }

        }

        /// <summary>
        /// Decompress a deflated block
        /// </summary>
        private static uint InflateBlock(int* e, State state, byte[] inbuf, byte[] outbuf)
        {
            // Make local bit buffer
            uint b = state.bb;
            uint k = state.bk;

            // Read the deflate block header
            var header = new DeflateBlockHeader();

            // Read in last block bit
            ZIPNEEDBITS(1, state, ref b, ref k);
            header.BFINAL = (*e = (int)b & 1) != 0;
            ZIPDUMPBITS(1, ref b, ref k);

            // Read in block type
            ZIPNEEDBITS(2, state, ref b, ref k);
            header.BTYPE = (CompressionType)(b & 3);
            ZIPDUMPBITS(2, ref b, ref k);

            // Restore the global bit buffer
            state.bb = b;
            state.bk = k;

            // Inflate that block type
            switch (header.BTYPE)
            {
                case CompressionType.NoCompression:
                    return (uint)DecompressStored(state, inbuf, outbuf);
                case CompressionType.FixedHuffman:
                    return (uint)DecompressFixed(state, inbuf, outbuf);
                case CompressionType.DynamicHuffman:
                    return (uint)DecompressDynamic(state, inbuf, outbuf);

                // Bad block type
                case CompressionType.Reserved:
                default:
                    return 2;
            }
        }

        /// <summary>
        /// "Decompress" a stored block
        /// </summary>
        private static int DecompressStored(State state, byte[] inbuf, byte[] outbuf)
        {
            // Make local copies of globals
            uint b = state.bb;
            uint k = state.bk;
            uint w = state.window_posn;

            // Go to byte boundary
            int n = (int)(k & 7);
            ZIPDUMPBITS(n, ref b, ref k);

            // Read the stored block header
            var header = new NonCompressedBlockHeader();

            // Get the length and its compliment
            ZIPNEEDBITS(16, state, ref b, ref k);
            header.LEN = (ushort)(b & 0xffff);
            ZIPDUMPBITS(16, ref b, ref k);

            ZIPNEEDBITS(16, state, ref b, ref k);
            header.NLEN = (ushort)(b & 0xffff);

            if (header.LEN != (~header.NLEN & 0xffff))
                return 1; // Error in compressed data

            ZIPDUMPBITS(16, ref b, ref k);

            // Read and output the compressed data
            while (n-- > 0)
            {
                ZIPNEEDBITS(8, state, ref b, ref k);
                outbuf[w++] = (byte)b;
                ZIPDUMPBITS(8, ref b, ref k);
            }

            // Restore the globals from the locals
            state.window_posn = w;
            state.bb = b;
            state.bk = k;

            return 0;
        }

        /// <summary>
        /// Decompress a block originally compressed with fixed Huffman codes
        /// </summary>
        private static int DecompressFixed(State state, byte[] inbuf, byte[] outbuf)
        {
            // Create the block header
            FixedHuffmanCompressedBlockHeader header = new FixedHuffmanCompressedBlockHeader();

            fixed (uint* l = state.ll)
            fixed (ushort* Zipcplens = CopyLengths)
            fixed (ushort* Zipcplext = LiteralExtraBits)
            fixed (ushort* Zipcpdist = CopyOffsets)
            fixed (ushort* Zipcpdext = DistanceExtraBits)
            {
                // Assign the literal lengths
                state.ll = header.LiteralLengths;
                HuffmanNode* fixed_tl;
                int fixed_bl = 7;

                // Build the literal length tree
                int i = BuildHuffmanTree(l, 288, 257, Zipcplens, Zipcplext, &fixed_tl, &fixed_bl, state);
                if (i != 0)
                    return i;

                // Assign the distance codes
                state.ll = header.DistanceCodes;
                HuffmanNode* fixed_td;
                int fixed_bd = 5;

                // Build the distance code tree
                i = BuildHuffmanTree(l, 30, 0, Zipcpdist, Zipcpdext, &fixed_td, &fixed_bd, state);
                if (i != 0)
                    return i;

                // Decompress until an end-of-block code
                return InflateCodes(fixed_tl, fixed_td, fixed_bl, fixed_bd, state, inbuf, outbuf);
            }
        }

        /// <summary>
        /// Decompress a block originally compressed with dynamic Huffman codes
        /// </summary>
        private static int DecompressDynamic(State state, byte[] inbuf, byte[] outbuf)
        {
            int i;                  /* temporary variables */
            uint j;
            uint l;                 /* last length */
            uint m;                 /* mask for bit lengths table */
            uint n;                 /* number of lengths to get */
            HuffmanNode* tl;        /* literal/length code table */
            HuffmanNode* td;        /* distance code table */
            int bl;                 /* lookup bits for tl */
            int bd;                 /* lookup bits for td */
            uint nb;                /* number of bit length codes */
            uint nl;                /* number of literal/length codes */
            uint nd;                /* number of distance codes */
            uint b;                 /* bit buffer */
            uint k;                 /* number of bits in bit buffer */

            /* make local bit buffer */
            b = state.bb;
            k = state.bk;

            state.ll = new uint[288 + 32];
            fixed (uint* ll = state.ll)
            {
                /* read in table lengths */
                ZIPNEEDBITS(5, state, ref b, ref k);
                nl = 257 + (b & 0x1f);      /* number of literal/length codes */
                ZIPDUMPBITS(5, ref b, ref k);

                ZIPNEEDBITS(5, state, ref b, ref k);
                nd = 1 + (b & 0x1f);        /* number of distance codes */
                ZIPDUMPBITS(5, ref b, ref k);

                ZIPNEEDBITS(4, state, ref b, ref k);
                nb = 4 + (b & 0xf);         /* number of bit length codes */
                ZIPDUMPBITS(4, ref b, ref k);
                if (nl > 288 || nd > 32)
                    return 1;                   /* bad lengths */

                /* read in bit-length-code lengths */
                for (j = 0; j < nb; j++)
                {
                    ZIPNEEDBITS(3, state, ref b, ref k);
                    state.ll[BitLengthOrder[j]] = b & 7;
                    ZIPDUMPBITS(3, ref b, ref k);
                }
                for (; j < 19; j++)
                    state.ll[BitLengthOrder[j]] = 0;

                /* build decoding table for trees--single level, 7 bit lookup */
                bl = 7;
                if ((i = BuildHuffmanTree(ll, 19, 19, null, null, &tl, &bl, state)) != 0)
                    return i;                   /* incomplete code set */

                /* read in literal and distance code lengths */
                n = nl + nd;
                m = BitMasks[bl];
                i = (int)(l = 0);
                while ((uint)i < n)
                {
                    ZIPNEEDBITS(bl, state, ref b, ref k);
                    j = (td = tl + (b & m))->b;
                    ZIPDUMPBITS((int)j, ref b, ref k);
                    j = td->n;
                    if (j < 16)                 /* length of code in bits (0..15) */
                    {
                        state.ll[i++] = l = j;          /* save last length in l */
                    }
                    else if (j == 16)           /* repeat last length 3 to 6 times */
                    {
                        ZIPNEEDBITS(2, state, ref b, ref k);
                        j = 3 + (b & 3);
                        ZIPDUMPBITS(2, ref b, ref k);
                        if ((uint)i + j > n)
                            return 1;
                        while (j-- > 0)
                        {
                            state.ll[i++] = l;
                        }
                    }
                    else if (j == 17)           /* 3 to 10 zero length codes */
                    {
                        ZIPNEEDBITS(3, state, ref b, ref k);
                        j = 3 + (b & 7);
                        ZIPDUMPBITS(3, ref b, ref k);
                        if ((uint)i + j > n)
                            return 1;
                        while (j-- > 0)
                            state.ll[i++] = 0;
                        l = 0;
                    }
                    else                        /* j == 18: 11 to 138 zero length codes */
                    {
                        ZIPNEEDBITS(7, state, ref b, ref k);
                        j = 11 + (b & 0x7f);
                        ZIPDUMPBITS(7, ref b, ref k);
                        if ((uint)i + j > n)
                            return 1;
                        while (j-- > 0)
                            state.ll[i++] = 0;
                        l = 0;
                    }
                }

                /* restore the global bit buffer */
                state.bb = b;
                state.bk = k;

                fixed (ushort* Zipcplens = CopyLengths)
                fixed (ushort* Zipcplext = LiteralExtraBits)
                fixed (ushort* Zipcpdist = CopyOffsets)
                fixed (ushort* Zipcpdext = DistanceExtraBits)
                {
                    /* build the decoding tables for literal/length and distance codes */
                    bl = ZIPLBITS;
                    if ((i = BuildHuffmanTree(ll, nl, 257, Zipcplens, Zipcplext, &tl, &bl, state)) != 0)
                    {
                        return i;                   /* incomplete code set */
                    }
                    bd = ZIPDBITS;
                    BuildHuffmanTree(ll + nl, nd, 0, Zipcpdist, Zipcpdext, &td, &bd, state);

                    /* decompress until an end-of-block code */
                    if (InflateCodes(tl, td, bl, bd, state, inbuf, outbuf) != 0)
                        return 1;

                    return 0;
                }
            }
        }

        /// <summary>
        /// Build a Huffman tree from a set of lengths
        /// </summary>
        private static int BuildHuffmanTree(uint* b, uint n, uint s, ushort* d, ushort* e, HuffmanNode** t, int* m, State state)
        {
            uint a;                    /* counter for codes of length k */
            uint el;                   /* length of EOB code (value 256) */
            uint f;                    /* i repeats in table every f entries */
            int g;                     /* maximum code length */
            int h;                     /* table level */
            uint i;                    /* counter, current code */
            uint j;                    /* counter */
            int k;                     /* number of bits in current code */
            int* l;                    /* stack of bits per table */
            uint* p;                   /* pointer into state.c[],state.b[],state.v[] */
            HuffmanNode* q;            /* points to current table */
            HuffmanNode r = new HuffmanNode();  /* table entry for structure assignment */
            int w;                     /* bits before this table == (l * h) */
            uint* xp;                  /* pointer into x */
            int y;                     /* number of dummy codes added */
            uint z;                    /* number of entries in current table */

            fixed (int* state_lx_ptr = state.lx)
            {
                l = state_lx_ptr + 1;

                /* Generate counts for each bit length */
                el = n > 256 ? b[256] : ZIPBMAX; /* set length of EOB code, if any */

                for (i = 0; i < ZIPBMAX + 1; ++i)
                    state.c[i] = 0;
                p = b; i = n;
                do
                {
                    state.c[*p]++; p++;               /* assume all entries <= ZIPBMAX */
                } while (--i > 0);

                if (state.c[0] == n)                /* null input--all zero length codes */
                {
                    *t = null;
                    *m = 0;
                    return 0;
                }

                /* Find minimum and maximum length, bound *m by those */
                for (j = 1; j <= ZIPBMAX; j++)
                {
                    if (state.c[j] > 0)
                        break;
                }

                k = (int)j;                        /* minimum code length */
                if ((uint)*m < j)
                    *m = (int)j;

                for (i = ZIPBMAX; i > 0; i--)
                {
                    if (state.c[i] > 0)
                        break;
                }

                g = (int)i;                        /* maximum code length */
                if ((uint)*m > i)
                    *m = (int)i;

                /* Adjust last length count to fill out codes, if needed */
                for (y = 1 << (int)j; j < i; j++, y <<= 1)
                {
                    if ((y -= (int)state.c[j]) < 0)
                        return 2;                 /* bad input: more codes than bits */
                }

                if ((y -= (int)state.c[i]) < 0)
                    return 2;

                state.c[i] += (uint)y;

                /* Generate starting offsets LONGo the value table for each length */
                state.x[1] = j = 0;

                fixed (uint* state_c_ptr = state.c)
                fixed (uint* state_x_ptr = state.x)
                {
                    p = state_c_ptr + 1;
                    xp = state_x_ptr + 2;
                    while (--i > 0)
                    {
                        /* note that i == g from above */
                        *xp++ = (j += *p++);
                    }
                }

                /* Make a table of values in order of bit lengths */
                p = b; i = 0;
                do
                {
                    if ((j = *p++) != 0)
                        state.v[state.x[j]++] = i;
                } while (++i < n);

                /* Generate the Huffman codes and for each, make the table entries */
                state.x[0] = i = 0;                 /* first Huffman code is zero */

                fixed (uint* state_v_ptr = state.v)
                {
                    p = state_v_ptr;                    /* grab values in bit order */
                    h = -1;                             /* no tables yet--level -1 */
                    w = l[-1] = 0;                      /* no bits decoded yet */
                    state.u[0] = default;               /* just to keep compilers happy */
                    q = null;                           /* ditto */
                    z = 0;                              /* ditto */

                    /* go through the bit lengths (k already is bits in shortest code) */
                    for (; k <= g; k++)
                    {
                        a = state.c[k];
                        while (a-- > 0)
                        {
                            /* here i is the Huffman code of length k bits for value *p */
                            /* make tables up to required level */
                            while (k > w + l[h])
                            {
                                w += l[h++];            /* add bits already decoded */

                                /* compute minimum size table less than or equal to *m bits */
                                if ((z = (uint)(g - w)) > (uint)*m)    /* upper limit */
                                    z = (uint)*m;

                                if ((f = (uint)(1 << (int)(j = (uint)(k - w)))) > a + 1)     /* try a k-w bit table */
                                {                       /* too few codes for k-w bit table */
                                    f -= a + 1;           /* deduct codes from patterns left */
                                    fixed (uint* state_c_ptr = state.c)
                                    {
                                        xp = state_c_ptr + k;
                                        while (++j < z)       /* try smaller tables up to z bits */
                                        {
                                            if ((f <<= 1) <= *++xp)
                                                break;            /* enough codes to use up j bits */
                                            f -= *xp;           /* else deduct codes from patterns */
                                        }
                                    }
                                }

                                if ((uint)w + j > el && (uint)w < el)
                                    j = (uint)(el - w);           /* make EOB code end at table */

                                z = (uint)(1 << (int)j);             /* table entries for j-bit table */
                                l[h] = (int)j;               /* set table size in stack */

                                /* allocate and link in new table */
                                q = (HuffmanNode*)Marshal.AllocHGlobal((int)((z + 1) * sizeof(HuffmanNode)));
                                *t = q + 1;             /* link to list for HuffmanNode_free() */
                                *(t = &(*q).t) = null;
                                state.u[h] = ++q;             /* table starts after link */

                                /* connect to last table, if there is one */
                                if (h > 0)
                                {
                                    state.x[h] = i;              /* save pattern for backing up */
                                    r.b = (byte)l[h - 1];        /* bits to dump before this table */
                                    r.e = (byte)(16 + j);        /* bits in this table */
                                    r.t = q;                     /* pointer to this table */
                                    j = (uint)((i & ((1 << w) - 1)) >> (w - l[h - 1]));
                                    state.u[h - 1][j] = r;       /* connect to last table */
                                }
                            }

                            /* set up table entry in r */
                            r.b = (byte)(k - w);

                            fixed (uint* state_v_ptr_comp = state.v)
                            {
                                if (p >= state_v_ptr_comp + n)
                                {
                                    r.e = 99;               /* out of values--invalid code */
                                }
                                else if (*p < s)
                                {
                                    r.e = (byte)(*p < 256 ? 16 : 15);    /* 256 is end-of-block code */
                                    r.n = (ushort)*p++;           /* simple code is just the value */
                                }
                                else
                                {
                                    r.e = (byte)e[*p - s];   /* non-simple--look up in lists */
                                    r.n = d[*p++ - s];
                                }
                            }

                            /* fill code-like entries with r */
                            f = (uint)(1 << (k - w));
                            for (j = i >> w; j < z; j += f)
                            {
                                q[j] = r;
                            }

                            /* backwards increment the k-bit code i */
                            for (j = (uint)(1 << (k - 1)); (i & j) != 0; j >>= 1)
                            {
                                i ^= j;
                            }

                            i ^= j;

                            /* backup over finished tables */
                            while ((i & ((1 << w) - 1)) != state.x[h])
                                w -= l[--h];            /* don't need to update q */
                        }
                    }
                }

                /* return actual size of base table */
                *m = l[0];
            }

            /* Return true (1) if we were given an incomplete table */
            return y != 0 && g != 1 ? 1 : 0;
        }

        /// <summary>
        /// Inflate codes into Huffman trees
        /// </summary>
        private static int InflateCodes(HuffmanNode* tl, HuffmanNode* td, int bl, int bd, State state, byte[] inbuf, byte[] outbuf)
        {
            uint e;              /* table entry flag/number of extra bits */
            uint n, d;           /* length and index for copy */
            uint w;              /* current window position */
            HuffmanNode* t;      /* pointer to table entry */
            uint ml, md;         /* masks for bl and bd bits */
            uint b;              /* bit buffer */
            uint k;              /* number of bits in bit buffer */

            /* make local copies of globals */
            b = state.bb;                       /* initialize bit buffer */
            k = state.bk;
            w = state.window_posn;                       /* initialize window position */

            /* inflate the coded data */
            ml = BitMasks[bl];               /* precompute masks for speed */
            md = BitMasks[bd];

            for (; ; )
            {
                ZIPNEEDBITS(bl, state, ref b, ref k);
                if ((e = (t = tl + (b & ml))->e) > 16)
                {
                    do
                    {
                        if (e == 99)
                            return 1;
                        ZIPDUMPBITS(t->b, ref b, ref k);
                        e -= 16;
                        ZIPNEEDBITS((int)e, state, ref b, ref k);
                    } while ((e = (*(t = t->t + (b & BitMasks[e]))).e) > 16);
                }

                ZIPDUMPBITS(t->b, ref b, ref k);
                if (e == 16)                /* then it's a literal */
                {
                    outbuf[w++] = (byte)t->n;
                }
                else                        /* it's an EOB or a length */
                {
                    /* exit if end of block */
                    if (e == 15)
                        break;

                    /* get length of block to copy */
                    ZIPNEEDBITS((int)e, state, ref b, ref k);
                    n = t->n + (b & BitMasks[e]);
                    ZIPDUMPBITS((int)e, ref b, ref k);

                    /* decode distance of block to copy */
                    ZIPNEEDBITS(bd, state, ref b, ref k);

                    if ((e = (*(t = td + (b & md))).e) > 16)
                        do
                        {
                            if (e == 99)
                                return 1;
                            ZIPDUMPBITS(t->b, ref b, ref k);
                            e -= 16;
                            ZIPNEEDBITS((int)e, state, ref b, ref k);
                        } while ((e = (*(t = t->t + (b & BitMasks[e]))).e) > 16);

                    ZIPDUMPBITS(t->b, ref b, ref k);

                    ZIPNEEDBITS((int)e, state, ref b, ref k);
                    d = w - t->n - (b & BitMasks[e]);
                    ZIPDUMPBITS((int)e, ref b, ref k);

                    do
                    {
                        d &= ZIPWSIZE - 1;
                        e = ZIPWSIZE - Math.Max(d, w);
                        e = Math.Min(e, n);
                        n -= e;
                        do
                        {
                            outbuf[w++] = outbuf[d++];
                        } while (--e > 0);
                    } while (n > 0);
                }
            }

            /* restore the globals from the locals */
            state.window_posn = w;              /* restore global window pointer */
            state.bb = b;                       /* restore global bit buffer */
            state.bk = k;

            /* done */
            return 0;
        }

        #region Macros

        private static void ZIPNEEDBITS(int n, State state, ref uint bitBuffer, ref uint bitCount)
        {
            while (bitCount < n)
            {
                int c = *state.inpos++;
                bitBuffer |= (uint)(c << (int)bitCount);
                bitCount += 8;
            }
        }

        private static void ZIPDUMPBITS(int n, ref uint bitBuffer, ref uint bitCount)
        {
            bitBuffer >>= n;
            bitCount -= (uint)n;
        }

        #endregion
    }
}