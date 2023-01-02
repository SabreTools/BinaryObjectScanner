using System.Linq;
using BurnOutSharp.Models.Compression.Quantum;
using static BurnOutSharp.Models.Compression.Quantum.Constants;

namespace BurnOutSharp.Compression
{
    public class Quantum
    {
        // TODO: Implement Quantum decompression

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
        * defined as Uint_BITS.
        *
        * Uint_BITS should be at least 16 bits. Unlike LZX's Huffman decoding,
        * Quantum's arithmetic decoding only needs 1 bit at a time, it doesn't
        * need an assured number. Retrieving larger bitstrings can be done with
        * multiple reads and fills of the bitbuffer. The code should work fine
        * for machines where Uint >= 32 bits.
        *
        * Also note that Quantum reads bytes in normal order; LZX is in
        * little-endian order.
        */

        // #define Q_INIT_BITSTREAM do { bitsleft = 0; bitbuf = 0; } while (0)

        // #define Q_FILL_BUFFER do {                                                  \
        // if (bitsleft <= (CAB_Uint_BITS - 16)) {                                  \
        //     bitbuf |= ((inpos[0]<<8)|inpos[1]) << (CAB_Uint_BITS-16 - bitsleft);   \
        //     bitsleft += 16; inpos += 2;                                             \
        // }                                                                         \
        // } while (0)

        // #define Q_PEEK_BITS(n)   (bitbuf >> (CAB_Uint_BITS - (n)))
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

        // #define Q_MENTRIES(model) (decomp_state.qtm.model).entries)
        // #define Q_MSYM(model,symidx) (decomp_state.qtm.model).syms[(symidx)].sym)
        // #define Q_MSYMFREQ(model,symidx) (decomp_state.qtm.model).syms[(symidx)].cumfreq)

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
        // Quantum.UpdateModel(&(decomp_state.qtm.m)), i);                                         \
        // } while (0)

        /// <summary>
        /// Initialize a Quantum model that decodes symbols from s to (s + n - 1)
        /// </summary>
        /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/fdi.c"/>
        public static void InitModel(Model model, ModelSymbol[] symbols, int entryCount, int initialSymbol)
        {
            // Set the basic values
            model.ShiftsLeft = 4;
            model.Entries = entryCount;
            model.Symbols = symbols;

            // Clear out the look-up table
            model.LookupTable = Enumerable.Repeat<ushort>(0xFF, model.LookupTable.Length).ToArray();

            // Loop through and build the look-up table
            for (ushort i = 0; i < entryCount; i++)
            {
                // Set up a look-up entry for symbol
                model.LookupTable[i + initialSymbol] = i;

                // Actual symbol
                model.Symbols[i].Symbol = (ushort)(i + initialSymbol);

                // Current frequency of that symbol
                model.Symbols[i].CumulativeFrequency = (ushort)(entryCount - i);
            }

            // Set the last symbol frequency to 0
            model.Symbols[entryCount].CumulativeFrequency = 0;
        }

        /// <summary>
        /// Update the quantum model for a particular symbol
        /// </summary>
        /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/fdi.c"/>
        public static void UpdateModel(Model model, int symbol)
        {
            // Update the cumulative frequency for all symbols less than the provided
            for (int i = 0; i < symbol; i++)
            {
                model.Symbols[i].CumulativeFrequency += 8;
            }

            // If the first symbol still has a cumulative frequency under 3800
            if (model.Symbols[0].CumulativeFrequency <= 3800)
                return;

            // If we have more than 1 shift left in the model
            if (--model.ShiftsLeft != 0)
            {
                // Loop through the entries from highest to lowest,
                // performing the shift on the cumulative frequencies
                for (int i = model.Entries - 1; i >= 0; i--)
                {
                    // -1, not -2; the 0 entry saves this
                    model.Symbols[i].CumulativeFrequency >>= 1;
                    if (model.Symbols[i].CumulativeFrequency <= model.Symbols[i + 1].CumulativeFrequency)
                        model.Symbols[i].CumulativeFrequency = (ushort)(model.Symbols[i + 1].CumulativeFrequency + 1);
                }
            }

            // If we have no shifts left in the model
            else
            {
                // Reset the shifts left value to 50
                model.ShiftsLeft = 50;

                // Loop through the entries setting the cumulative frequencies
                for (int i = 0; i < model.Entries; i++)
                {
                    // No -1, want to include the 0 entry
                    // This converts cumfreqs into frequencies, then shifts right
                    model.Symbols[i].CumulativeFrequency -= model.Symbols[i + 1].CumulativeFrequency;
                    model.Symbols[i].CumulativeFrequency++; // Avoid losing things entirely
                    model.Symbols[i].CumulativeFrequency >>= 1;
                }

                // Now sort by frequencies, decreasing order -- this must be an
                // inplace selection sort, or a sort with the same (in)stability
                // characteristics
                for (int i = 0; i < model.Entries - 1; i++)
                {
                    for (int j = i + 1; j < model.Entries; j++)
                    {
                        if (model.Symbols[i].CumulativeFrequency < model.Symbols[j].CumulativeFrequency)
                        {
                            var temp = model.Symbols[i];
                            model.Symbols[i] = model.Symbols[j];
                            model.Symbols[j] = temp;
                        }
                    }
                }

                // Then convert frequencies back to cumfreq
                for (int i = model.Entries - 1; i >= 0; i--)
                {
                    model.Symbols[i].CumulativeFrequency += model.Symbols[i + 1].CumulativeFrequency;
                }

                // Then update the other part of the table
                for (int i = 0; i < model.Entries; i++)
                {
                    model.LookupTable[model.Symbols[i].Symbol] = (ushort)i;
                }
            }
        }
    }
}