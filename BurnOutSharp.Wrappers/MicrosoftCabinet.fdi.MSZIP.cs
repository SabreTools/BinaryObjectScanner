// using static BurnOutSharp.Wrappers.CabinetConstants;
// using static BurnOutSharp.Wrappers.FDIcConstants;
// using static BurnOutSharp.Wrappers.FDIConstants;
// using cab_LONG = System.Int32;
// using cab_off_t = System.UInt32;
// using cab_UBYTE = System.Byte;
// using cab_ULONG = System.UInt32;
// using cab_UWORD = System.UInt16;

// namespace BurnOutSharp.Wrappers
// {
//     internal unsafe class MSZIPfdi
//     {
//         /// <summary>
//         /// Ziphuft_free (internal)
//         /// </summary>
//         static void fdi_Ziphuft_free(FDI_Int fdi, Ziphuft t)
//         {
//             // No-op because of garbage collection
//         }

//         /// <summary>
//         /// fdi_Ziphuft_build (internal)
//         /// </summary>
//         static cab_LONG fdi_Ziphuft_build(cab_ULONG* b, cab_ULONG n, cab_ULONG s, cab_UWORD* d, cab_UWORD* e, ref Ziphuft[] t, cab_LONG* m, fdi_decomp_state decomp_state)
//         {
//             cab_ULONG a;                      /* counter for codes of length k */
//             cab_ULONG el;                   /* length of EOB code (value 256) */
//             cab_ULONG f;                    /* i repeats in table every f entries */
//             cab_LONG g;                     /* maximum code length */
//             cab_LONG h;                     /* table level */
//             cab_ULONG i;           /* counter, current code */
//             cab_ULONG j;           /* counter */
//             cab_LONG k;            /* number of bits in current code */
//             cab_LONG* l;                      /* stack of bits per table */
//             cab_ULONG* p;          /* pointer into decomp_state.zip.c[],decomp_state.zip.b[],decomp_state.zip.v[] */
//             Ziphuft* q;           /* points to current table */
//             Ziphuft r;                     /* table entry for structure assignment */
//             cab_LONG w;                  /* bits before this table == (l * h) */
//             cab_ULONG* xp;                    /* pointer into x */
//             cab_LONG y;                           /* number of dummy codes added */
//             cab_ULONG z;                    /* number of entries in current table */

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

//                         if ((cab_ULONG)w + j > el && (cab_ULONG)w < el)
//                             j = el - w;           /* make EOB code end at table */

//                         z = 1 << j;             /* table entries for j-bit table */
//                         l[h] = j;               /* set table size in stack */

//                         /* allocate and link in new table */
//                         if (!(q = decomp_state.fdi.alloc((z + 1) * sizeof(Ziphuft))))
//                         {
//                             if (h)
//                                 fdi_Ziphuft_free(decomp_state.fdi, decomp_state.zip.u[0]);
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
//             cab_ULONG e;     /* table entry flag/number of extra bits */
//             cab_ULONG n, d;           /* length and index for copy */
//             cab_ULONG w;              /* current window position */
//             Ziphuft t;  /* pointer to table entry */
//             cab_ULONG ml, md;         /* masks for bl and bd bits */
//             cab_ULONG b;     /* bit buffer */
//             cab_ULONG k;     /* number of bits in bit buffer */

//             /* make local copies of globals */
//             b = decomp_state.zip.bb;                       /* initialize bit buffer */
//             k = decomp_state.zip.bk;
//             w = decomp_state.zip.window_posn;                       /* initialize window position */

//             /* inflate the coded data */
//             ml = Zipmask[bl];               /* precompute masks for speed */
//             md = Zipmask[bd];

//             for (; ; )
//             {
//                 ZIPNEEDBITS((cab_ULONG)bl)
//                 if ((e = (t = tl + (b & ml)).e) > 16)
//                     do
//                     {
//                         if (e == 99)
//                             return 1;
//                         ZIPDUMPBITS(t.b)
//                       e -= 16;
//                         ZIPNEEDBITS(e)
//                     } while ((e = (t = t.v.t + (b & Zipmask[e])).e) > 16);
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
//                   n = t.v.n + (b & Zipmask[e]);
//                     ZIPDUMPBITS(e);

//                     /* decode distance of block to copy */
//                     ZIPNEEDBITS((cab_ULONG)bd)
//                   if ((e = (t = td + (b & md)).e) > 16)
//                         do
//                         {
//                             if (e == 99)
//                                 return 1;
//                             ZIPDUMPBITS(t.b)
//                           e -= 16;
//                             ZIPNEEDBITS(e)
//                         } while ((e = (t = t.v.t + (b & Zipmask[e])).e) > 16);
//                     ZIPDUMPBITS(t.b)
//                   ZIPNEEDBITS(e)
//                   d = w - t.v.n - (b & Zipmask[e]);
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
//         /// Zipinflate_stored (internal)
//         /// 
//         /// "decompress" an inflated type 0 (stored) block.
//         /// </summary>
//         static cab_LONG fdi_Zipinflate_stored(fdi_decomp_state decomp_state)
//         {
//             cab_ULONG n;           /* number of bytes in block */
//             cab_ULONG w;           /* current window position */
//             cab_ULONG b;  /* bit buffer */
//             cab_ULONG k;  /* number of bits in bit buffer */

//             /* make local copies of globals */
//             b = decomp_state.zip.bb;                       /* initialize bit buffer */
//             k = decomp_state.zip.bk;
//             w = decomp_state.zip.window_posn;              /* initialize window position */

//             /* go to byte boundary */
//             n = k & 7;
//             ZIPDUMPBITS(n);

//             /* get the length and its complement */
//             ZIPNEEDBITS(16)
//           n = (b & 0xffff);
//             ZIPDUMPBITS(16)
//           ZIPNEEDBITS(16)
//           if (n != ((~b) & 0xffff))
//                 return 1;                   /* error in compressed data */
//             ZIPDUMPBITS(16)

//   /* read and output the compressed data */
//   while (n--)
//             {
//                 ZIPNEEDBITS(8)
//     decomp_state.outbuf[w++] = (cab_UBYTE)b;
//                 ZIPDUMPBITS(8)
//   }

//             /* restore the globals from the locals */
//             decomp_state.zip.window_posn = w;              /* restore global window pointer */
//             decomp_state.zip.bb = b;                       /* restore global bit buffer */
//             decomp_state.zip.bk = k;
//             return 0;
//         }

//         /// <summary>
//         /// fdi_Zipinflate_fixed (internal)
//         /// </summary>
//         static cab_LONG fdi_Zipinflate_fixed(fdi_decomp_state decomp_state)
//         {
//             Ziphuft fixed_tl;
//             Ziphuft fixed_td;
//             cab_LONG fixed_bl, fixed_bd;
//             cab_LONG i;                /* temporary variable */
//             cab_ULONG* l;

//             l = decomp_state.zip.ll;

//             /* literal table */
//             for (i = 0; i < 144; i++)
//                 l[i] = 8;
//             for (; i < 256; i++)
//                 l[i] = 9;
//             for (; i < 280; i++)
//                 l[i] = 7;
//             for (; i < 288; i++)          /* make a complete, but wrong code set */
//                 l[i] = 8;
//             fixed_bl = 7;
//             if ((i = fdi_Ziphuft_build(l, 288, 257, Zipcplens, Zipcplext, &fixed_tl, &fixed_bl, decomp_state)))
//                 return i;

//             /* distance table */
//             for (i = 0; i < 30; i++)      /* make an incomplete code set */
//                 l[i] = 5;
//             fixed_bd = 5;
//             if ((i = fdi_Ziphuft_build(l, 30, 0, Zipcpdist, Zipcpdext, &fixed_td, &fixed_bd, decomp_state)) > 1)
//             {
//                 fdi_Ziphuft_free(decomp_state.fdi, fixed_tl);
//                 return i;
//             }

//             /* decompress until an end-of-block code */
//             i = fdi_Zipinflate_codes(fixed_tl, fixed_td, fixed_bl, fixed_bd, decomp_state);

//             fdi_Ziphuft_free(decomp_state.fdi, fixed_td);
//             fdi_Ziphuft_free(decomp_state.fdi, fixed_tl);
//             return i;
//         }

//         /// <summary>
//         /// fdi_Zipinflate_dynamic (internal)
//         /// 
//         /// decompress an inflated type 2 (dynamic Huffman codes) block.
//         /// </summary>
//         static cab_LONG fdi_Zipinflate_dynamic(fdi_decomp_state decomp_state)
//         {
//             cab_LONG i;             /* temporary variables */
//             cab_ULONG j;
//             cab_ULONG* ll;
//             cab_ULONG l;            /* last length */
//             cab_ULONG m;            /* mask for bit lengths table */
//             cab_ULONG n;            /* number of lengths to get */
//             Ziphuft tl;           /* literal/length code table */
//             Ziphuft td;           /* distance code table */
//             cab_LONG bl;                  /* lookup bits for tl */
//             cab_LONG bd;                  /* lookup bits for td */
//             cab_ULONG nb;           /* number of bit length codes */
//             cab_ULONG nl;           /* number of literal/length codes */
//             cab_ULONG nd;           /* number of distance codes */
//             cab_ULONG b;         /* bit buffer */
//             cab_ULONG k;           /* number of bits in bit buffer */

//             /* make local bit buffer */
//             b = decomp_state.zip.bb;
//             k = decomp_state.zip.bk;
//             ll = decomp_state.zip.ll;

//             /* read in table lengths */
//             ZIPNEEDBITS(5)
//   nl = 257 + (b & 0x1f);      /* number of literal/length codes */
//             ZIPDUMPBITS(5)
//   ZIPNEEDBITS(5)
//   nd = 1 + (b & 0x1f);        /* number of distance codes */
//             ZIPDUMPBITS(5)
//   ZIPNEEDBITS(4)
//   nb = 4 + (b & 0xf);         /* number of bit length codes */
//             ZIPDUMPBITS(4)
//   if (nl > 288 || nd > 32)
//                 return 1;                   /* bad lengths */

//             /* read in bit-length-code lengths */
//             for (j = 0; j < nb; j++)
//             {
//                 ZIPNEEDBITS(3)
//                 ll[Zipborder[j]] = b & 7;
//                 ZIPDUMPBITS(3)
//               }
//             for (; j < 19; j++)
//                 ll[Zipborder[j]] = 0;

//             /* build decoding table for trees--single level, 7 bit lookup */
//             bl = 7;
//             if ((i = fdi_Ziphuft_build(ll, 19, 19, null, null, &tl, &bl, decomp_state)) != 0)
//             {
//                 if (i == 1)
//                     fdi_Ziphuft_free(decomp_state.fdi, tl);
//                 return i;                   /* incomplete code set */
//             }

//             /* read in literal and distance code lengths */
//             n = nl + nd;
//             m = Zipmask[bl];
//             i = l = 0;
//             while ((cab_ULONG)i < n)
//             {
//                 ZIPNEEDBITS((cab_ULONG)bl)
//                 j = (td = tl + (b & m)).b;
//                 ZIPDUMPBITS(j)
//                 j = td.v.n;
//                 if (j < 16)                 /* length of code in bits (0..15) */
//                     ll[i++] = l = j;          /* save last length in l */
//                 else if (j == 16)           /* repeat last length 3 to 6 times */
//                 {
//                     ZIPNEEDBITS(2)
//                   j = 3 + (b & 3);
//                     ZIPDUMPBITS(2)
//                   if ((cab_ULONG)i + j > n)
//                         return 1;
//                     while (j--)
//                         ll[i++] = l;
//                 }
//                 else if (j == 17)           /* 3 to 10 zero length codes */
//                 {
//                     ZIPNEEDBITS(3)
//                   j = 3 + (b & 7);
//                     ZIPDUMPBITS(3)
//                   if ((cab_ULONG)i + j > n)
//                         return 1;
//                     while (j--)
//                         ll[i++] = 0;
//                     l = 0;
//                 }
//                 else                        /* j == 18: 11 to 138 zero length codes */
//                 {
//                     ZIPNEEDBITS(7)
//                   j = 11 + (b & 0x7f);
//                     ZIPDUMPBITS(7)
//                   if ((cab_ULONG)i + j > n)
//                         return 1;
//                     while (j--)
//                         ll[i++] = 0;
//                     l = 0;
//                 }
//             }

//             /* free decoding table for trees */
//             fdi_Ziphuft_free(decomp_state.fdi, tl);

//             /* restore the global bit buffer */
//             decomp_state.zip.bb = b;
//             decomp_state.zip.bk = k;

//             /* build the decoding tables for literal/length and distance codes */
//             bl = ZIPLBITS;
//             if ((i = fdi_Ziphuft_build(ll, nl, 257, Zipcplens, Zipcplext, &tl, &bl, decomp_state)) != 0)
//             {
//                 if (i == 1)
//                     fdi_Ziphuft_free(decomp_state.fdi, tl);
//                 return i;                   /* incomplete code set */
//             }
//             bd = ZIPDBITS;
//             fdi_Ziphuft_build(ll + nl, nd, 0, Zipcpdist, Zipcpdext, &td, &bd, decomp_state);

//             /* decompress until an end-of-block code */
//             if (fdi_Zipinflate_codes(tl, td, bl, bd, decomp_state))
//                 return 1;

//             /* free the decoding tables, return */
//             fdi_Ziphuft_free(decomp_state.fdi, tl);
//             fdi_Ziphuft_free(decomp_state.fdi, td);
//             return 0;
//         }

//         /// <summary>
//         /// fdi_Zipinflate_block (internal)
//         /// 
//         /// decompress an inflated block
//         /// </summary>
//         static cab_LONG fdi_Zipinflate_block(cab_LONG* e, fdi_decomp_state decomp_state) /* e == last block flag */
//         {
//             cab_ULONG t;            /* block type */
//             cab_ULONG b;     /* bit buffer */
//             cab_ULONG k;     /* number of bits in bit buffer */

//             /* make local bit buffer */
//             b = decomp_state.zip.bb;
//             k = decomp_state.zip.bk;

//             /* read in last block bit */
//             ZIPNEEDBITS(1)
//             * e = (cab_LONG)b & 1;
//             ZIPDUMPBITS(1)

//   /* read in block type */
//   ZIPNEEDBITS(2)
//           t = b & 3;
//             ZIPDUMPBITS(2)

//   /* restore the global bit buffer */
//   decomp_state.zip.bb = b;
//             decomp_state.zip.bk = k;

//             /* inflate that block type */
//             if (t == 2)
//                 return fdi_Zipinflate_dynamic(decomp_state);
//             if (t == 0)
//                 return fdi_Zipinflate_stored(decomp_state);
//             if (t == 1)
//                 return fdi_Zipinflate_fixed(decomp_state);
//             /* bad block type */
//             return 2;
//         }

//         /// <summary>
//         /// ZIPfdi_decomp(internal)
//         /// </summary>
//         static int ZIPfdi_decomp(int inlen, int outlen, fdi_decomp_state decomp_state)
//         {
//             cab_LONG e;               /* last block flag */

//             TRACE("(inlen == %d, outlen == %d)\n", inlen, outlen);

//             decomp_state.zip.inpos = decomp_state.inbuf;
//             decomp_state.zip.bb = decomp_state.zip.bk = decomp_state.zip.window_posn = 0;
//             if (outlen > ZIPWSIZE)
//                 return DECR_DATAFORMAT;

//             /* CK = Chris Kirmse, official Microsoft purloiner */
//             if (decomp_state.zip.inpos[0] != 0x43 || decomp_state.zip.inpos[1] != 0x4B)
//                 return DECR_ILLEGALDATA;
//             decomp_state.zip.inpos += 2;

//             do
//             {
//                 if (fdi_Zipinflate_block(&e, decomp_state))
//                     return DECR_ILLEGALDATA;
//             } while (!e);

//             /* return success */
//             return DECR_OK;
//         }
//     }
// }