// using BurnOutSharp.Compression.Quantum;
// using BurnOutSharp.Models.Compression.Quantum;
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
//     internal unsafe class Quantumfdi
//     {
//         /// <summary>
//         /// QTMfdi_decomp(internal)
//         /// </summary>
//         internal static int QTMfdi_decomp(int inlen, int outlen, fdi_decomp_state decomp_state)
//         {
//             cab_UBYTE* inpos = decomp_state.inbuf;
//             cab_UBYTE* window = decomp_state.Window;
//             cab_UBYTE* runsrc, rundest;
//             cab_ULONG window_posn = decomp_state.WindowPosition;
//             cab_ULONG window_size = decomp_state.WindowSize;

//             /* used by bitstream macros */
//             int bitsleft, bitrun, bitsneed;
//             cab_ULONG bitbuf;

//             /* used by GET_SYMBOL */
//             cab_ULONG range;
//             cab_UWORD symf;
//             int i;

//             int extra = 0, togo = outlen, match_length = 0, copy_length;
//             cab_UBYTE selector, sym;
//             cab_ULONG match_offset = 0;

//             cab_UWORD H = 0xFFFF, L = 0, C;

//             System.Diagnostics.Debug.WriteLine("(inlen == %d, outlen == %d)\n", inlen, outlen);

//             /* read initial value of C */
//             Q_INIT_BITSTREAM(out bitsleft, out bitbuf);
//             C = Q_READ_BITS_UINT16(16, ref inpos, ref bitsleft, ref bitbuf);

//             /* apply 2^x-1 mask */
//             window_posn &= window_size - 1;
//             /* runs can't straddle the window wraparound */
//             if ((window_posn + togo) > window_size)
//             {
//                 System.Diagnostics.Debug.WriteLine("straddled run\n");
//                 return DECR_DATAFORMAT;
//             }

//             while (togo > 0)
//             {
//                 selector = GET_SYMBOL(state.Model7, ref H, ref L, ref C, ref inpos, ref bitsleft, ref bitbuf);
//                 switch (selector)
//                 {
//                     case 0:
//                         sym = GET_SYMBOL(state.Model7Submodel00, ref H, ref L, ref C, ref inpos, ref bitsleft, ref bitbuf);
//                         window[window_posn++] = sym;
//                         togo--;
//                         break;
//                     case 1:
//                         sym = GET_SYMBOL(state.Model7Submodel40, ref H, ref L, ref C, ref inpos, ref bitsleft, ref bitbuf);
//                         window[window_posn++] = sym;
//                         togo--;
//                         break;
//                     case 2:
//                         sym = GET_SYMBOL(state.Model7Submodel80, ref H, ref L, ref C, ref inpos, ref bitsleft, ref bitbuf);
//                         window[window_posn++] = sym;
//                         togo--;
//                         break;
//                     case 3:
//                         sym = GET_SYMBOL(state.Model7SubmodelC0, ref H, ref L, ref C, ref inpos, ref bitsleft, ref bitbuf);
//                         window[window_posn++] = sym;
//                         togo--;
//                         break;

//                     // Selector 4 = fixed length of 3
//                     case 4:
//                         sym = GET_SYMBOL(state.Model4, ref H, ref L, ref C, ref inpos, ref bitsleft, ref bitbuf);
//                         extra = Q_READ_BITS_INT32(state.q_extra_bits[sym], ref inpos, ref bitsleft, ref bitbuf);
//                         match_offset = decomp_state.q_position_base[sym] + extra + 1;
//                         match_length = 3;
//                         break;

//                     // Selector 5 = fixed length of 4
//                     case 5:
//                         sym = GET_SYMBOL(state.Model5, ref H, ref L, ref C, ref inpos, ref bitsleft, ref bitbuf);
//                         extra = Q_READ_BITS_INT32(state.q_extra_bits[sym], ref inpos, ref bitsleft, ref bitbuf);
//                         match_offset = decomp_state.q_position_base[sym] + extra + 1;
//                         match_length = 4;
//                         break;

//                     // Selector 6 = variable length
//                     case 6:
//                         sym = GET_SYMBOL(state.Model6Length, ref H, ref L, ref C, ref inpos, ref bitsleft, ref bitbuf);
//                         extra = Q_READ_BITS_INT32(state.q_length_extra[sym], ref inpos, ref bitsleft, ref bitbuf);
//                         match_length = decomp_state.q_length_base[sym] + extra + 5;
//                         sym = GET_SYMBOL(state.Model6Position, ref H, ref L, ref C, ref inpos, ref bitsleft, ref bitbuf);
//                         extra = Q_READ_BITS_INT32(state.q_extra_bits[sym], ref inpos, ref bitsleft, ref bitbuf);
//                         match_offset = decomp_state.q_position_base[sym] + extra + 1;
//                         break;

//                     default:
//                         System.Diagnostics.Debug.WriteLine("Selector is bogus\n");
//                         return DECR_ILLEGALDATA;
//                 }

//                 /* if this is a match */
//                 if (selector >= 4)
//                 {
//                     rundest = window + window_posn;
//                     togo -= match_length;

//                     /* copy any wrapped around source data */
//                     if (window_posn >= match_offset)
//                     {
//                         /* no wrap */
//                         runsrc = rundest - match_offset;
//                     }
//                     else
//                     {
//                         runsrc = rundest + (window_size - match_offset);
//                         copy_length = match_offset - window_posn;
//                         if (copy_length < match_length)
//                         {
//                             match_length -= copy_length;
//                             window_posn += copy_length;
//                             while (copy_length-- > 0) *rundest++ = *runsrc++;
//                             runsrc = window;
//                         }
//                     }
//                     window_posn += match_length;

//                     /* copy match data - no worries about destination wraps */
//                     while (match_length-- > 0) *rundest++ = *runsrc++;
//                 }
//             } /* while (togo > 0) */

//             if (togo != 0)
//             {
//                 System.Diagnostics.Debug.WriteLine("Frame overflow, this_run = %d\n", togo);
//                 return DECR_ILLEGALDATA;
//             }

//             memcpy(decomp_state.outbuf, window + ((!window_posn) ? window_size : window_posn) - outlen, outlen);

//             decomp_state.WindowPosition = window_posn;
//             return DECR_OK;
//         }

//         /// <summary>
//         /// Should be used first to set up the system
//         /// </summary>
//         private static void Q_INIT_BITSTREAM(out int bitsleft, out uint bitbuf)
//         {
//             bitsleft = 0; bitbuf = 0;
//         }

//         /// <summary>
//         /// Adds more data to the bit buffer, if there is room for another 16 bits.
//         /// </summary>
//         private static void Q_FILL_BUFFER(ref byte* inpos, ref int bitsleft, ref uint bitbuf)
//         {
//             if (bitsleft <= 16)
//             {
//                 bitbuf |= (uint)((inpos[0] << 8) | inpos[1]) << (16 - bitsleft);
//                 bitsleft += 16; inpos += 2;
//             }
//         }

//         /// <summary>
//         /// Extracts (without removing) N bits from the bit buffer
//         /// </summary>
//         private static uint Q_PEEK_BITS(int n, uint bitbuf)
//         {
//             return bitbuf >> (32 - n);
//         }

//         /// <summary>
//         /// Removes N bits from the bit buffer
//         /// </summary>
//         private static void Q_REMOVE_BITS(int n, ref int bitsleft, ref uint bitbuf)
//         {
//             bitbuf <<= n;
//             bitsleft -= n;
//         }

//         /// <summary>
//         /// Takes N bits from the buffer and puts them in v. Unlike LZX, this can loop
//         /// several times to get the requisite number of bits.
//         /// </summary>
//         private static ushort Q_READ_BITS_UINT16(int n, ref byte* inpos, ref int bitsleft, ref uint bitbuf)
//         {
//             ushort v = 0; int bitrun;
//             for (int bitsneed = n; bitsneed != 0; bitsneed -= bitrun)
//             {
//                 Q_FILL_BUFFER(ref inpos, ref bitsleft, ref bitbuf);

//                 bitrun = (bitsneed > bitsleft) ? bitsleft : bitsneed;
//                 v = (ushort)((v << bitrun) | Q_PEEK_BITS(bitrun, bitbuf));

//                 Q_REMOVE_BITS(bitrun, ref bitsleft, ref bitbuf);
//             }

//             return v;
//         }

//         /// <summary>
//         /// Takes N bits from the buffer and puts them in v. Unlike LZX, this can loop
//         /// several times to get the requisite number of bits.
//         /// </summary>
//         private static int Q_READ_BITS_INT32(int n, ref byte* inpos, ref int bitsleft, ref uint bitbuf)
//         {
//             int v = 0; int bitrun;
//             for (int bitsneed = n; bitsneed != 0; bitsneed -= bitrun)
//             {
//                 Q_FILL_BUFFER(ref inpos, ref bitsleft, ref bitbuf);

//                 bitrun = (bitsneed > bitsleft) ? bitsleft : bitsneed;
//                 v = (int)((v << bitrun) | Q_PEEK_BITS(bitrun, bitbuf));

//                 Q_REMOVE_BITS(bitrun, ref bitsleft, ref bitbuf);
//             }

//             return v;
//         }

//         /// <summary>
//         /// Fetches the next symbol from the stated model and puts it in v.
//         /// It may need to read the bitstream to do this.
//         /// </summary>
//         private static int GET_SYMBOL(Model model, ref ushort H, ref ushort L, ref ushort C, ref byte* inpos, ref int bitsleft, ref uint bitbuf)
//         {
//             uint range = (uint)(((H - L) & 0xFFFF) + 1);
//             ushort symf = (ushort)(((((C - L + 1) * model.Symbols[0].CumulativeFrequency) - 1) / range) & 0xFFFF);

//             int i;
//             for (i = 1; i < model.Entries; i++)
//             {
//                 if (model.Symbols[i].CumulativeFrequency <= symf)
//                     break;
//             }

//             int v = model.Symbols[i - 1].Symbol;
//             range = (uint)(H - L + 1);
//             H = (ushort)(L + ((model.Symbols[i - 1].CumulativeFrequency * range) / model.Symbols[0].CumulativeFrequency) - 1);
//             L = (ushort)(L + ((model.Symbols[i].CumulativeFrequency * range) / model.Symbols[0].CumulativeFrequency));

//             while (true)
//             {
//                 if ((L & 0x8000) != (H & 0x8000))
//                 {
//                     if ((L & 0x4000) != 0 && (H & 0x4000) == 0)
//                     {
//                         // Underflow case
//                         C ^= 0x4000; L &= 0x3FFF; H |= 0x4000;
//                     }
//                     else
//                     {
//                         break;
//                     }
//                 }

//                 L <<= 1; H = (ushort)((H << 1) | 1);
//                 Q_FILL_BUFFER(ref inpos, ref bitsleft, ref bitbuf);
//                 C = (ushort)((C << 1) | Q_PEEK_BITS(1, bitbuf));
//                 Q_REMOVE_BITS(1, ref bitsleft, ref bitbuf);
//             }

//             Decompressor.UpdateModel(model, i);
//             return v;
//         }
//     }
// }