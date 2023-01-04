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
//     internal unsafe class LZXfdi
//     {

// /*******************************************************
//  * LZXfdi_decomp(internal)
//  */
// static int LZXfdi_decomp(int inlen, int outlen, fdi_decomp_state* decomp_state)
// {
//     cab_UBYTE* inpos = CAB(inbuf);
//     const cab_UBYTE* endinp = inpos + inlen;
//     cab_UBYTE* window = LZX(window);
//     cab_UBYTE* runsrc, *rundest;
//     cab_UWORD* hufftbl; /* used in READ_HUFFSYM macro as chosen decoding table */

//     cab_ULONG window_posn = LZX(window_posn);
//     cab_ULONG window_size = LZX(window_size);
//     cab_ULONG R0 = LZX(R0);
//     cab_ULONG R1 = LZX(R1);
//     cab_ULONG R2 = LZX(R2);

//     register cab_ULONG bitbuf;
//     register int bitsleft;
//     cab_ULONG match_offset, i, j, k; /* ijk used in READ_HUFFSYM macro */
//   struct lzx_bits lb; /* used in READ_LENGTHS macro */

// int togo = outlen, this_run, main_element, aligned_bits;
// int match_length, copy_length, length_footer, extra, verbatim_bits;

// TRACE("(inlen == %d, outlen == %d)\n", inlen, outlen);

// INIT_BITSTREAM;

// /* read header if necessary */
// if (!LZX(header_read))
// {
//     i = j = 0;
//     READ_BITS(k, 1); if (k) { READ_BITS(i, 16); READ_BITS(j, 16); }
//     LZX(intel_filesize) = (i << 16) | j; /* or 0 if not encoded */
//     LZX(header_read) = 1;
// }

// /* main decoding loop */
// while (togo > 0)
// {
//     /* last block finished, new block expected */
//     if (LZX(block_remaining) == 0)
//     {
//         if (LZX(block_type) == LZX_BLOCKTYPE_UNCOMPRESSED)
//         {
//             if (LZX(block_length) & 1) inpos++; /* realign bitstream to word */
//             INIT_BITSTREAM;
//         }

//         READ_BITS(LZX(block_type), 3);
//         READ_BITS(i, 16);
//         READ_BITS(j, 8);
//         LZX(block_remaining) = LZX(block_length) = (i << 8) | j;

//         switch (LZX(block_type))
//         {
//             case LZX_BLOCKTYPE_ALIGNED:
//                 for (i = 0; i < 8; i++) { READ_BITS(j, 3); LENTABLE(ALIGNED)[i] = j; }
//                 BUILD_TABLE(ALIGNED);
//             /* rest of aligned header is same as verbatim */

//             case LZX_BLOCKTYPE_VERBATIM:
//                 READ_LENGTHS(MAINTREE, 0, 256, fdi_lzx_read_lens);
//                 READ_LENGTHS(MAINTREE, 256, LZX(main_elements), fdi_lzx_read_lens);
//                 BUILD_TABLE(MAINTREE);
//                 if (LENTABLE(MAINTREE)[0xE8] != 0) LZX(intel_started) = 1;

//                 READ_LENGTHS(LENGTH, 0, LZX_NUM_SECONDARY_LENGTHS, fdi_lzx_read_lens);
//                 BUILD_TABLE(LENGTH);
//                 break;

//             case LZX_BLOCKTYPE_UNCOMPRESSED:
//                 LZX(intel_started) = 1; /* because we can't assume otherwise */
//                 ENSURE_BITS(16); /* get up to 16 pad bits into the buffer */
//                 if (bitsleft > 16) inpos -= 2; /* and align the bitstream! */
//                 R0 = inpos[0] | (inpos[1] << 8) | (inpos[2] << 16) | (inpos[3] << 24); inpos += 4;
//                 R1 = inpos[0] | (inpos[1] << 8) | (inpos[2] << 16) | (inpos[3] << 24); inpos += 4;
//                 R2 = inpos[0] | (inpos[1] << 8) | (inpos[2] << 16) | (inpos[3] << 24); inpos += 4;
//                 break;

//             default:
//                 return DECR_ILLEGALDATA;
//         }
//     }

//     /* buffer exhaustion check */
//     if (inpos > endinp)
//     {
//         /* it's possible to have a file where the next run is less than
//          * 16 bits in size. In this case, the READ_HUFFSYM() macro used
//          * in building the tables will exhaust the buffer, so we should
//          * allow for this, but not allow those accidentally read bits to
//          * be used (so we check that there are at least 16 bits
//          * remaining - in this boundary case they aren't really part of
//          * the compressed data)
//          */
//         if (inpos > (endinp + 2) || bitsleft < 16) return DECR_ILLEGALDATA;
//     }

//     while ((this_run = LZX(block_remaining)) > 0 && togo > 0)
//     {
//         if (this_run > togo) this_run = togo;
//         togo -= this_run;
//         LZX(block_remaining) -= this_run;

//         /* apply 2^x-1 mask */
//         window_posn &= window_size - 1;
//         /* runs can't straddle the window wraparound */
//         if ((window_posn + this_run) > window_size)
//             return DECR_DATAFORMAT;

//         switch (LZX(block_type))
//         {

//             case LZX_BLOCKTYPE_VERBATIM:
//                 while (this_run > 0)
//                 {
//                     READ_HUFFSYM(MAINTREE, main_element);

//                     if (main_element < LZX_NUM_CHARS)
//                     {
//                         /* literal: 0 to LZX_NUM_CHARS-1 */
//                         window[window_posn++] = main_element;
//                         this_run--;
//                     }
//                     else
//                     {
//                         /* match: LZX_NUM_CHARS + ((slot<<3) | length_header (3 bits)) */
//                         main_element -= LZX_NUM_CHARS;

//                         match_length = main_element & LZX_NUM_PRIMARY_LENGTHS;
//                         if (match_length == LZX_NUM_PRIMARY_LENGTHS)
//                         {
//                             READ_HUFFSYM(LENGTH, length_footer);
//                             match_length += length_footer;
//                         }
//                         match_length += LZX_MIN_MATCH;

//                         match_offset = main_element >> 3;

//                         if (match_offset > 2)
//                         {
//                             /* not repeated offset */
//                             if (match_offset != 3)
//                             {
//                                 extra = CAB(extra_bits)[match_offset];
//                                 READ_BITS(verbatim_bits, extra);
//                                 match_offset = CAB(lzx_position_base)[match_offset]
//                                                - 2 + verbatim_bits;
//                             }
//                             else
//                             {
//                                 match_offset = 1;
//                             }

//                             /* update repeated offset LRU queue */
//                             R2 = R1; R1 = R0; R0 = match_offset;
//                         }
//                         else if (match_offset == 0)
//                         {
//                             match_offset = R0;
//                         }
//                         else if (match_offset == 1)
//                         {
//                             match_offset = R1;
//                             R1 = R0; R0 = match_offset;
//                         }
//                         else /* match_offset == 2 */
//                         {
//                             match_offset = R2;
//                             R2 = R0; R0 = match_offset;
//                         }

//                         rundest = window + window_posn;
//                         this_run -= match_length;

//                         /* copy any wrapped around source data */
//                         if (window_posn >= match_offset)
//                         {
//                             /* no wrap */
//                             runsrc = rundest - match_offset;
//                         }
//                         else
//                         {
//                             runsrc = rundest + (window_size - match_offset);
//                             copy_length = match_offset - window_posn;
//                             if (copy_length < match_length)
//                             {
//                                 match_length -= copy_length;
//                                 window_posn += copy_length;
//                                 while (copy_length-- > 0) *rundest++ = *runsrc++;
//                                 runsrc = window;
//                             }
//                         }
//                         window_posn += match_length;

//                         /* copy match data - no worries about destination wraps */
//                         while (match_length-- > 0) *rundest++ = *runsrc++;
//                     }
//                 }
//                 break;

//             case LZX_BLOCKTYPE_ALIGNED:
//                 while (this_run > 0)
//                 {
//                     READ_HUFFSYM(MAINTREE, main_element);

//                     if (main_element < LZX_NUM_CHARS)
//                     {
//                         /* literal: 0 to LZX_NUM_CHARS-1 */
//                         window[window_posn++] = main_element;
//                         this_run--;
//                     }
//                     else
//                     {
//                         /* match: LZX_NUM_CHARS + ((slot<<3) | length_header (3 bits)) */
//                         main_element -= LZX_NUM_CHARS;

//                         match_length = main_element & LZX_NUM_PRIMARY_LENGTHS;
//                         if (match_length == LZX_NUM_PRIMARY_LENGTHS)
//                         {
//                             READ_HUFFSYM(LENGTH, length_footer);
//                             match_length += length_footer;
//                         }
//                         match_length += LZX_MIN_MATCH;

//                         match_offset = main_element >> 3;

//                         if (match_offset > 2)
//                         {
//                             /* not repeated offset */
//                             extra = CAB(extra_bits)[match_offset];
//                             match_offset = CAB(lzx_position_base)[match_offset] - 2;
//                             if (extra > 3)
//                             {
//                                 /* verbatim and aligned bits */
//                                 extra -= 3;
//                                 READ_BITS(verbatim_bits, extra);
//                                 match_offset += (verbatim_bits << 3);
//                                 READ_HUFFSYM(ALIGNED, aligned_bits);
//                                 match_offset += aligned_bits;
//                             }
//                             else if (extra == 3)
//                             {
//                                 /* aligned bits only */
//                                 READ_HUFFSYM(ALIGNED, aligned_bits);
//                                 match_offset += aligned_bits;
//                             }
//                             else if (extra > 0)
//                             { /* extra==1, extra==2 */
//                                 /* verbatim bits only */
//                                 READ_BITS(verbatim_bits, extra);
//                                 match_offset += verbatim_bits;
//                             }
//                             else /* extra == 0 */
//                             {
//                                 /* ??? */
//                                 match_offset = 1;
//                             }

//                             /* update repeated offset LRU queue */
//                             R2 = R1; R1 = R0; R0 = match_offset;
//                         }
//                         else if (match_offset == 0)
//                         {
//                             match_offset = R0;
//                         }
//                         else if (match_offset == 1)
//                         {
//                             match_offset = R1;
//                             R1 = R0; R0 = match_offset;
//                         }
//                         else /* match_offset == 2 */
//                         {
//                             match_offset = R2;
//                             R2 = R0; R0 = match_offset;
//                         }

//                         rundest = window + window_posn;
//                         this_run -= match_length;

//                         /* copy any wrapped around source data */
//                         if (window_posn >= match_offset)
//                         {
//                             /* no wrap */
//                             runsrc = rundest - match_offset;
//                         }
//                         else
//                         {
//                             runsrc = rundest + (window_size - match_offset);
//                             copy_length = match_offset - window_posn;
//                             if (copy_length < match_length)
//                             {
//                                 match_length -= copy_length;
//                                 window_posn += copy_length;
//                                 while (copy_length-- > 0) *rundest++ = *runsrc++;
//                                 runsrc = window;
//                             }
//                         }
//                         window_posn += match_length;

//                         /* copy match data - no worries about destination wraps */
//                         while (match_length-- > 0) *rundest++ = *runsrc++;
//                     }
//                 }
//                 break;

//             case LZX_BLOCKTYPE_UNCOMPRESSED:
//                 if ((inpos + this_run) > endinp) return DECR_ILLEGALDATA;
//                 memcpy(window + window_posn, inpos, (size_t)this_run);
//                 inpos += this_run; window_posn += this_run;
//                 break;

//             default:
//                 return DECR_ILLEGALDATA; /* might as well */
//         }

//     }
// }

// if (togo != 0) return DECR_ILLEGALDATA;
// memcpy(CAB(outbuf), window + ((!window_posn) ? window_size : window_posn) -
//   outlen, (size_t)outlen);

// LZX(window_posn) = window_posn;
// LZX(R0) = R0;
// LZX(R1) = R1;
// LZX(R2) = R2;

// /* intel E8 decoding */
// if ((LZX(frames_read)++ < 32768) && LZX(intel_filesize) != 0)
// {
//     if (outlen <= 6 || !LZX(intel_started))
//     {
//         LZX(intel_curpos) += outlen;
//     }
//     else
//     {
//         cab_UBYTE* data = CAB(outbuf);
//         cab_UBYTE* dataend = data + outlen - 10;
//         cab_LONG curpos = LZX(intel_curpos);
//         cab_LONG filesize = LZX(intel_filesize);
//         cab_LONG abs_off, rel_off;

//         LZX(intel_curpos) = curpos + outlen;

//         while (data < dataend)
//         {
//             if (*data++ != 0xE8) { curpos++; continue; }
//             abs_off = data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24);
//             if ((abs_off >= -curpos) && (abs_off < filesize))
//             {
//                 rel_off = (abs_off >= 0) ? abs_off - curpos : abs_off + filesize;
//                 data[0] = (cab_UBYTE)rel_off;
//                 data[1] = (cab_UBYTE)(rel_off >> 8);
//                 data[2] = (cab_UBYTE)(rel_off >> 16);
//                 data[3] = (cab_UBYTE)(rel_off >> 24);
//             }
//             data += 4;
//             curpos += 5;
//         }
//     }
// }
// return DECR_OK;
// }
//     }
// }