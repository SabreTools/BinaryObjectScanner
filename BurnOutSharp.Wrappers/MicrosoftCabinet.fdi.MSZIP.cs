// using static BurnOutSharp.Wrappers.CabinetConstants;
// using static BurnOutSharp.Wrappers.FDIcConstants;
// using static BurnOutSharp.Wrappers.FDIConstants;
// using static BurnOutSharp.Models.Compression.MSZIP.Constants;
// using cab_LONG = System.Int32;
// using cab_off_t = System.UInt32;
// using cab_UBYTE = System.Byte;
// using uint = System.UInt32;
// using cab_UWORD = System.UInt16;

// namespace BurnOutSharp.Wrappers
// {
//     internal unsafe class MSZIPfdi
//     {
//         /// <summary>
//         /// BuildHuffmanTree (internal)
//         /// </summary>
//         static cab_LONG BuildHuffmanTree(uint* b, uint n, uint s, cab_UWORD* d, cab_UWORD* e, ref Ziphuft[] t, cab_LONG* m, fdi_decomp_state decomp_state)
//         {
//             uint a;                      /* counter for codes of length k */
//             uint el;                   /* length of EOB code (value 256) */
//             uint f;                    /* i repeats in table every f entries */
//             cab_LONG g;                     /* maximum code length */
//             cab_LONG h;                     /* table level */
//             uint i;           /* counter, current code */
//             uint j;           /* counter */
//             cab_LONG k;            /* number of bits in current code */
//             cab_LONG* l;                      /* stack of bits per table */
//             uint* p;          /* pointer into decomp_state.zip.c[],decomp_state.zip.b[],decomp_state.zip.v[] */
//             Ziphuft* q;           /* points to current table */
//             Ziphuft r;                     /* table entry for structure assignment */
//             cab_LONG w;                  /* bits before this table == (l * h) */
//             uint* xp;                    /* pointer into x */
//             cab_LONG y;                           /* number of dummy codes added */
//             uint z;                    /* number of entries in current table */

//             l = decomp_state.zip.lx + 1;

//             // Generate counts for each bit length
//             // set length of EOB code, if any
//             el = n > 256 ? b[256] : ZIPBMAX;

//             for (i = 0; i < ZIPBMAX + 1; ++i)
//             {
//                 decomp_state.zip.c[i] = 0;
//             }

//             p = b; i = n;

//             // assume all entries <= ZIPBMAX
//             do
//             {
//                 decomp_state.zip.c[*p]++; p++;
//             } while (--i != 0);

//             // null input--all zero length codes
//             if (decomp_state.zip.c[0] == n)
//             {
//                 t = null;
//                 *m = 0;
//                 return 0;
//             }

//             // Find minimum and maximum length, bound *m by those
//             for (j = 1; j <= ZIPBMAX; j++)
//             {
//                 if (decomp_state.zip.c[j] != 0)
//                     break;
//             }

//             // minimum code length
//             k = (int)j;
//             if (*m < j)
//                 *m = (int)j;

//             for (i = ZIPBMAX; i != 0; i--)
//             {
//                 if (decomp_state.zip.c[i] != 0)
//                     break;
//             }

//             // maximum code length
//             g = (int)i;
//             if (*m > i)
//                 *m = (int)i;

//             // Adjust last length count to fill out codes, if needed */
//             for (y = 1 << (int)j; j < i; j++, y <<= 1)
//             {
//                 // bad input: more codes than bits
//                 if ((y -= (int)decomp_state.zip.c[j]) < 0)
//                     return 2;
//             }

//             if ((y -= (int)decomp_state.zip.c[i]) < 0)
//                 return 2;

//             decomp_state.zip.c[i] += (uint)y;

//             // Generate starting offsets LONGo the value table for each length
//             decomp_state.zip.x[1] = j = 0;
//             p = decomp_state.zip.c + 1; xp = decomp_state.zip.x + 2;
//             while (--i != 0)
//             {
//                 // note that i == g from above
//                 *xp++ = (j += *p++);
//             }

//             /* Make a table of values in order of bit lengths */
//             p = b; i = 0;
//             do
//             {
//                 if ((j = *p++) != 0)
//                     decomp_state.zip.v[decomp_state.zip.x[j]++] = i;
//             } while (++i < n);

//             // Generate the Huffman codes and for each, make the table entries
//             decomp_state.zip.x[0] = i = 0;                 /* first Huffman code is zero */
//             p = decomp_state.zip.v;                        /* grab values in bit order */
//             h = -1;                       /* no tables yet--level -1 */
//             w = l[-1] = 0;                /* no bits decoded yet */
//             decomp_state.zip.u[0] = null;             /* just to keep compilers happy */
//             q = null;                     /* ditto */
//             z = 0;                        /* ditto */

//             /* go through the bit lengths (k already is bits in shortest code) */
//             for (; k <= g; k++)
//             {
//                 a = decomp_state.zip.c[k];
//                 while (a-- != 0)
//                 {
//                     // here i is the Huffman code of length k bits for value *p
//                     // make tables up to required level
//                     while (k > w + l[h])
//                     {
//                         // add bits already decoded
//                         w += l[h++];

//                         // compute minimum size table less than or equal to *m bits
//                         if ((z = (uint)(g - w)) > *m)    /* upper limit */
//                             z = (uint)*m;

//                         // try a k-w bit table
//                         if ((f = (uint)(1 << (int)(j = (uint)(k - w)))) > a + 1)
//                         {                       /* too few codes for k-w bit table */
//                             f -= a + 1;           /* deduct codes from patterns left */
//                             xp = decomp_state.zip.c + k;
//                             while (++j < z)       /* try smaller tables up to z bits */
//                             {
//                                 if ((f <<= 1) <= *++xp)
//                                     break;            /* enough codes to use up j bits */
//                                 f -= *xp;           /* else deduct codes from patterns */
//                             }
//                         }

//                         if ((uint)w + j > el && (uint)w < el)
//                             j = el - w;           /* make EOB code end at table */

//                         z = 1 << j;             /* table entries for j-bit table */
//                         l[h] = j;               /* set table size in stack */

//                         /* allocate and link in new table */
//                         if (!(q = decomp_state.fdi.alloc((z + 1) * sizeof(Ziphuft))))
//                         {
//                             if (h)
//                                 Free(decomp_state.fdi, decomp_state.zip.u[0]);
//                             return 3;             /* not enough memory */
//                         }

//                         *t = q + 1;             /* link to list for Ziphuft_free() */
//                         *(t = &(q.v.t)) = null;
//                         decomp_state.zip.u[h] = ++q;             /* table starts after link */

//                         // connect to last table, if there is one
//                         if (h != 0)
//                         {
//                             decomp_state.zip.x[h] = i;              /* save pattern for backing up */
//                             r.b = (cab_UBYTE)l[h - 1];    /* bits to dump before this table */
//                             r.e = (cab_UBYTE)(16 + j);  /* bits in this table */
//                             r.t = q;                  /* pointer to this table */
//                             j = (i & ((1 << w) - 1)) >> (w - l[h - 1]);
//                             decomp_state.zip.u[h - 1][j] = r;        /* connect to last table */
//                         }
//                     }

//                     /* set up table entry in r */
//                     r.b = (cab_UBYTE)(k - w);
//                     if (p >= decomp_state.zip.v + n)
//                     {
//                         r.e = 99;               /* out of values--invalid code */
//                     }
//                     else if (*p < s)
//                     {
//                         r.e = (cab_UBYTE)(*p < 256 ? 16 : 15);    /* 256 is end-of-block code */
//                         r.n = *p++;           /* simple code is just the value */
//                     }
//                     else
//                     {
//                         r.e = (cab_UBYTE)e[*p - s];   /* non-simple--look up in lists */
//                         r.n = d[*p++ - s];
//                     }

//                     /* fill code-like entries with r */
//                     f = 1 << (k - w);
//                     for (j = i >> w; j < z; j += f)
//                         q[j] = r;

//                     /* backwards increment the k-bit code i */
//                     for (j = 1 << (k - 1); i & j; j >>= 1)
//                         i ^= j;
//                     i ^= j;

//                     /* backup over finished tables */
//                     while ((i & ((1 << w) - 1)) != decomp_state.zip.x[h])
//                         w -= l[--h];            /* don't need to update q */
//                 }
//             }

//             /* return actual size of base table */
//             *m = l[0];

//             /* Return true (1) if we were given an incomplete table */
//             return y != 0 && g != 1;
//         }

//         /// <summary>
//         /// fdi_Zipinflate_codes (internal)
//         /// </summary>
//         static cab_LONG fdi_Zipinflate_codes(in Ziphuft tl, in Ziphuft td, cab_LONG bl, cab_LONG bd, fdi_decomp_state decomp_state)
//         {
//             uint e;     /* table entry flag/number of extra bits */
//             uint n, d;           /* length and index for copy */
//             uint w;              /* current window position */
//             Ziphuft t;  /* pointer to table entry */
//             uint ml, md;         /* masks for bl and bd bits */
//             uint b;     /* bit buffer */
//             uint k;     /* number of bits in bit buffer */

//             /* make local copies of globals */
//             b = decomp_state.zip.bb;                       /* initialize bit buffer */
//             k = decomp_state.zip.bk;
//             w = decomp_state.zip.window_posn;                       /* initialize window position */

//             /* inflate the coded data */
//             ml = BitMasks[bl];               /* precompute masks for speed */
//             md = BitMasks[bd];

//             for (; ; )
//             {
//                 ZIPNEEDBITS((uint)bl)
//                 if ((e = (t = tl + (b & ml)).e) > 16)
//                     do
//                     {
//                         if (e == 99)
//                             return 1;
//                         ZIPDUMPBITS(t.b)
//                       e -= 16;
//                         ZIPNEEDBITS(e)
//                     } while ((e = (t = t.v.t + (b & BitMasks[e])).e) > 16);
//                 ZIPDUMPBITS(t.b)
//                 if (e == 16)                /* then it's a literal */
//                     decomp_state.outbuf[w++] = (cab_UBYTE)t.v.n;
//                 else                        /* it's an EOB or a length */
//                 {
//                     /* exit if end of block */
//                     if (e == 15)
//                         break;

//                     /* get length of block to copy */
//                     ZIPNEEDBITS(e)
//                   n = t.v.n + (b & BitMasks[e]);
//                     ZIPDUMPBITS(e);

//                     /* decode distance of block to copy */
//                     ZIPNEEDBITS((uint)bd)
//                   if ((e = (t = td + (b & md)).e) > 16)
//                         do
//                         {
//                             if (e == 99)
//                                 return 1;
//                             ZIPDUMPBITS(t.b)
//                           e -= 16;
//                             ZIPNEEDBITS(e)
//                         } while ((e = (t = t.v.t + (b & BitMasks[e])).e) > 16);
//                     ZIPDUMPBITS(t.b)
//                   ZIPNEEDBITS(e)
//                   d = w - t.v.n - (b & BitMasks[e]);
//                     ZIPDUMPBITS(e)
//                   do
//                     {
//                         d &= ZIPWSIZE - 1;
//                         e = ZIPWSIZE - max(d, w);
//                         e = min(e, n);
//                         n -= e;
//                         do
//                         {
//                             decomp_state.outbuf[w++] = decomp_state.outbuf[d++];
//                         } while (--e);
//                     } while (n);
//                 }
//             }

//             /* restore the globals from the locals */
//             decomp_state.zip.window_posn = w;              /* restore global window pointer */
//             decomp_state.zip.bb = b;                       /* restore global bit buffer */
//             decomp_state.zip.bk = k;

//             /* done */
//             return 0;
//         }

//         /// <summary>
//         /// fdi_Zipinflate_dynamic (internal)
//         /// 
//         /// decompress an inflated type 2 (dynamic Huffman codes) block.
//         /// </summary>
//         static cab_LONG fdi_Zipinflate_dynamic(fdi_decomp_state decomp_state)
//         {
//             cab_LONG i;             /* temporary variables */
//             uint j;
//             uint l;            /* last length */
//             uint m;            /* mask for bit lengths table */
//             uint n;            /* number of lengths to get */
//             Ziphuft tl;           /* literal/length code table */
//             Ziphuft td;           /* distance code table */
//             cab_LONG bl;                  /* lookup bits for tl */
//             cab_LONG bd;                  /* lookup bits for td */
//             uint nb;           /* number of bit length codes */
//             uint nl;           /* number of literal/length codes */
//             uint nd;           /* number of distance codes */
//             uint bitBuffer;         /* bit buffer */
//             uint bitCount;           /* number of bits in bit buffer */

//             /* make local bit buffer */
//             bitBuffer = decomp_state.zip.bb;
//             bitCount = decomp_state.zip.bk;

//             /* read in table lengths */
//             ZIPNEEDBITS(5)
//              nl = 257 + (bitBuffer & 0x1f);      /* number of literal/length codes */
//             ZIPDUMPBITS(5)
//              ZIPNEEDBITS(5)
//              nd = 1 + (bitBuffer & 0x1f);        /* number of distance codes */
//             ZIPDUMPBITS(5)
//              ZIPNEEDBITS(4)
//              nb = 4 + (bitBuffer & 0xf);         /* number of bit length codes */
//             ZIPDUMPBITS(4)
//               if (nl > 288 || nd > 32)
//                 return 1;                   /* bad lengths */

//             /* read in bit-length-code lengths */
//             for (j = 0; j < nb; j++)
//             {
//                 ZIPNEEDBITS(3)
//                 state.Lengths[BitLengthOrder[j]] = bitBuffer & 7;
//                 ZIPDUMPBITS(3)
//               }
//             for (; j < 19; j++)
//                 state.Lengths[BitLengthOrder[j]] = 0;

//             /* build decoding table for trees--single level, 7 bit lookup */
//             bl = 7;
//             if ((i = BuildHuffmanTree(state.Lengths, 19, 19, null, null, &tl, &bl, decomp_state)) != 0)
//             {
//                 if (i == 1)
//                     Free(decomp_state.fdi, tl);
//                 return i;                   /* incomplete code set */
//             }

//             /* read in literal and distance code lengths */
//             n = nl + nd;
//             m = BitMasks[bl];
//             i = l = 0;
//             while ((uint)i < n)
//             {
//                 ZIPNEEDBITS((uint)bl)
//                 j = (td = tl + (bitBuffer & m)).b;
//                 ZIPDUMPBITS(j)
//                 j = td.v.n;
//                 if (j < 16)                 /* length of code in bits (0..15) */
//                     state.Lengths[i++] = l = j;          /* save last length in l */
//                 else if (j == 16)           /* repeat last length 3 to 6 times */
//                 {
//                     ZIPNEEDBITS(2)
//                   j = 3 + (bitBuffer & 3);
//                     ZIPDUMPBITS(2)
//                   if ((uint)i + j > n)
//                         return 1;
//                     while (j--)
//                         state.Lengths[i++] = l;
//                 }
//                 else if (j == 17)           /* 3 to 10 zero length codes */
//                 {
//                     ZIPNEEDBITS(3)
//                   j = 3 + (bitBuffer & 7);
//                     ZIPDUMPBITS(3)
//                   if ((uint)i + j > n)
//                         return 1;
//                     while (j--)
//                         state.Lengths[i++] = 0;
//                     l = 0;
//                 }
//                 else                        /* j == 18: 11 to 138 zero length codes */
//                 {
//                     ZIPNEEDBITS(7)
//                   j = 11 + (bitBuffer & 0x7f);
//                     ZIPDUMPBITS(7)
//                   if ((uint)i + j > n)
//                         return 1;
//                     while (j--)
//                         state.Lengths[i++] = 0;
//                     l = 0;
//                 }
//             }

//             /* free decoding table for trees */
//             Free(decomp_state.fdi, tl);

//             /* restore the global bit buffer */
//             decomp_state.zip.bb = bitBuffer;
//             decomp_state.zip.bk = bitCount;

//             /* build the decoding tables for literal/length and distance codes */
//             bl = ZIPLBITS;
//             if ((i = BuildHuffmanTree(state.Lengths, nl, 257, CopyLengths, LiteralExtraBits, &tl, &bl, decomp_state)) != 0)
//             {
//                 if (i == 1)
//                     Free(decomp_state.fdi, tl);
//                 return i;                   /* incomplete code set */
//             }
//             bd = ZIPDBITS;
//             BuildHuffmanTree(state.Lengths + nl, nd, 0, CopyOffsets, DistanceExtraBits, &td, &bd, decomp_state);

//             /* decompress until an end-of-block code */
//             if (fdi_Zipinflate_codes(tl, td, bl, bd, decomp_state))
//                 return 1;

//             /* free the decoding tables, return */
//             Free(decomp_state.fdi, tl);
//             Free(decomp_state.fdi, td);
//             return 0;
//         }
//     }
// }