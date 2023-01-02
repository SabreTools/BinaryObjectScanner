// using BurnOutSharp.Compression;
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
//     /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
//     internal class QuantumState
//     {
//         /// <summary>
//         /// the actual decoding window
//         /// </summary>
//         public byte[] window;

//         /// <summary>
//         /// window size (1Kb through 2Mb)
//         /// </summary>
//         public uint window_size;

//         /// <summary>
//         /// window size when it was first allocated
//         /// </summary>
//         public uint actual_size;

//         /// <summary>
//         /// current offset within the window
//         /// </summary>
//         public uint window_posn;

//         public Model model7;
//         public ModelSymbol[] m7sym = new ModelSymbol[7 + 1];

//         public Model model4;
//         public Model model5;
//         public Model model6pos;
//         public Model model6len;
//         public ModelSymbol[] m4sym = new ModelSymbol[0x18 + 1];
//         public ModelSymbol[] m5sym = new ModelSymbol[0x24 + 1];
//         public ModelSymbol[] m6psym = new ModelSymbol[0x2a + 1];
//         public ModelSymbol[] m6lsym = new ModelSymbol[0x1b + 1];

//         public Model model00;
//         public Model model40;
//         public Model model80;
//         public Model modelC0;
//         public ModelSymbol[] m00sym = new ModelSymbol[0x40 + 1];
//         public ModelSymbol[] m40sym = new ModelSymbol[0x40 + 1];
//         public ModelSymbol[] m80sym = new ModelSymbol[0x40 + 1];
//         public ModelSymbol[] mC0sym = new ModelSymbol[0x40 + 1];
//     }

//     internal class Quantumfdi
//     {
//         /// <summary>
//         /// QTMfdi_init (internal)
//         /// </summary>
//         internal static int QTMfdi_init(int window, int level, fdi_decomp_state decomp_state)
//         {
//             uint wndsize = (uint)(1 << window);
//             int msz = window * 2, i;
//             uint j;

//             /* QTM supports window sizes of 2^10 (1Kb) through 2^21 (2Mb) */
//             /* if a previously allocated window is big enough, keep it    */
//             if (window < 10 || window > 21) return DECR_DATAFORMAT;
//             if (decomp_state.qtm.actual_size < wndsize)
//             {
//                 if (decomp_state.qtm.window != null) decomp_state.fdi.free(decomp_state.qtm.window);
//                 decomp_state.qtm.window = null;
//             }
//             if (decomp_state.qtm.window == null)
//             {
//                 if ((decomp_state.qtm.window = decomp_state.fdi.alloc((int)wndsize)) == null) return DECR_NOMEMORY;
//                 decomp_state.qtm.actual_size = wndsize;
//             }
//             decomp_state.qtm.window_size = wndsize;
//             decomp_state.qtm.window_posn = 0;

//             /* initialize static slot/extrabits tables */
//             for (i = 0, j = 0; i < 27; i++)
//             {
//                 decomp_state.q_length_extra[i] = (byte)((i == 26) ? 0 : (i < 2 ? 0 : i - 2) >> 2);
//                 decomp_state.q_length_base[i] = (byte)j; j += (uint)(1 << ((i == 26) ? 5 : decomp_state.q_length_extra[i]));
//             }
//             for (i = 0, j = 0; i < 42; i++)
//             {
//                 decomp_state.q_extra_bits[i] = (byte)((i < 2 ? 0 : i - 2) >> 1);
//                 decomp_state.q_position_base[i] = j; j += (uint)(1 << decomp_state.q_extra_bits[i]);
//             }

//             /* initialize arithmetic coding models */

//             Quantum.InitModel(decomp_state.qtm.model7, decomp_state.qtm.m7sym, 7, 0);

//             Quantum.InitModel(decomp_state.qtm.model00, decomp_state.qtm.m00sym, 0x40, 0x00);
//             Quantum.InitModel(decomp_state.qtm.model40, decomp_state.qtm.m40sym, 0x40, 0x40);
//             Quantum.InitModel(decomp_state.qtm.model80, decomp_state.qtm.m80sym, 0x40, 0x80);
//             Quantum.InitModel(decomp_state.qtm.modelC0, decomp_state.qtm.mC0sym, 0x40, 0xC0);

//             /* model 4 depends on table size, ranges from 20 to 24  */
//             Quantum.InitModel(decomp_state.qtm.model4, decomp_state.qtm.m4sym, (msz < 24) ? msz : 24, 0);
//             /* model 5 depends on table size, ranges from 20 to 36  */
//             Quantum.InitModel(decomp_state.qtm.model5, decomp_state.qtm.m5sym, (msz < 36) ? msz : 36, 0);
//             /* model 6pos depends on table size, ranges from 20 to 42 */
//             Quantum.InitModel(decomp_state.qtm.model6pos, decomp_state.qtm.m6psym, msz, 0);
//             Quantum.InitModel(decomp_state.qtm.model6len, decomp_state.qtm.m6lsym, 27, 0);

//             return DECR_OK;
//         }

//         /// <summary>
//         /// QTMfdi_decomp(internal)
//         /// </summary>
//         internal static int QTMfdi_decomp(int inlen, int outlen, fdi_decomp_state decomp_state)
//         {
//             cab_UBYTE* inpos = decomp_state.inbuf;
//             cab_UBYTE* window = decomp_state.qtm.window;
//             cab_UBYTE* runsrc, rundest;
//             cab_ULONG window_posn = decomp_state.qtm.window_posn;
//             cab_ULONG window_size = decomp_state.qtm.window_size;

//             /* used by bitstream macros */
//             int bitsleft, bitrun, bitsneed;
//             cab_ULONG bitbuf;

//             /* used by GET_SYMBOL */
//             cab_ULONG range;
//             cab_UWORD symf;
//             int i;

//             int extra, togo = outlen, match_length = 0, copy_length;
//             cab_UBYTE selector, sym;
//             cab_ULONG match_offset = 0;

//             cab_UWORD H = 0xFFFF, L = 0, C;

//             System.Diagnostics.Debug.WriteLine("(inlen == %d, outlen == %d)\n", inlen, outlen);

//             /* read initial value of C */
//             Q_INIT_BITSTREAM;
//             Q_READ_BITS(C, 16);

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
//                 GET_SYMBOL(model7, selector);
//                 switch (selector)
//                 {
//                     case 0:
//                         GET_SYMBOL(model00, sym); window[window_posn++] = sym; togo--;
//                         break;
//                     case 1:
//                         GET_SYMBOL(model40, sym); window[window_posn++] = sym; togo--;
//                         break;
//                     case 2:
//                         GET_SYMBOL(model80, sym); window[window_posn++] = sym; togo--;
//                         break;
//                     case 3:
//                         GET_SYMBOL(modelC0, sym); window[window_posn++] = sym; togo--;
//                         break;

//                     case 4:
//                         /* selector 4 = fixed length of 3 */
//                         GET_SYMBOL(model4, sym);
//                         Q_READ_BITS(extra, decomp_state.q_extra_bits[sym]);
//                         match_offset = decomp_state.q_position_base[sym] + extra + 1;
//                         match_length = 3;
//                         break;

//                     case 5:
//                         /* selector 5 = fixed length of 4 */
//                         GET_SYMBOL(model5, sym);
//                         Q_READ_BITS(extra, decomp_state.q_extra_bits[sym]);
//                         match_offset = decomp_state.q_position_base[sym] + extra + 1;
//                         match_length = 4;
//                         break;

//                     case 6:
//                         /* selector 6 = variable length */
//                         GET_SYMBOL(model6len, sym);
//                         Q_READ_BITS(extra, decomp_state.q_length_extra[sym]);
//                         match_length = decomp_state.q_length_base[sym] + extra + 5;
//                         GET_SYMBOL(model6pos, sym);
//                         Q_READ_BITS(extra, decomp_state.q_extra_bits[sym]);
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

//             decomp_state.qtm.window_posn = window_posn;
//             return DECR_OK;
//         }
//     }
// }