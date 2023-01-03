using System;
using System.Linq;
using BurnOutSharp.Models.Compression.Quantum;
using BurnOutSharp.Models.MicrosoftCabinet;

namespace BurnOutSharp.Compression.Quantum
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/cabinet.h"/>
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/fdi.c"/>
    /// <see href="https://github.com/wine-mirror/wine/blob/master/include/fdi.h"/>
    /// <see href="http://www.russotto.net/quantumcomp.html"/>
    public static class Decompressor
    {
        /// <summary>
        /// Decompress a byte array using a given State
        /// </summary>
        public static bool Decompress(State state, int inlen, byte[] inbuf, int outlen, byte[] outbuf)
        {
            int inpos = 0; // inbuf[0]
            int window = 0; // state.Window[0]
            int runsrc, rundest;
            uint windowPosition = state.WindowPosition;
            uint windowSize = state.WindowSize;

            int extra, togo = outlen, matchLength = 0, copyLength;
            byte selector, sym;
            uint matchOffset = 0;

            ushort H = 0xFFFF, L = 0;

            // Read initial value of C
            Q_INIT_BITSTREAM(out int bitsleft, out uint bitbuf);
            ushort C = (ushort)Q_READ_BITS(16, inbuf, ref inpos, ref bitsleft, ref bitbuf);

            // Apply 2^x-1 mask
            windowPosition &= windowSize - 1;

            // Runs can't straddle the window wraparound
            if ((windowPosition + togo) > windowSize)
                return false;

            while (togo > 0)
            {
                selector = (byte)GET_SYMBOL(state.SelectorModel, ref H, ref L, ref C, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                switch (selector)
                {
                    // Selector 0 = literal model, 64 entries, 0x00-0x3F
                    case 0:
                        sym = (byte)GET_SYMBOL(state.Model0, ref H, ref L, ref C, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                        state.Window[window + windowPosition++] = sym;
                        togo--;
                        break;

                    // Selector 1 = literal model, 64 entries, 0x40-0x7F
                    case 1:
                        sym = (byte)GET_SYMBOL(state.Model1, ref H, ref L, ref C, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                        state.Window[window + windowPosition++] = sym;
                        togo--;
                        break;

                    // Selector 2 = literal model, 64 entries, 0x80-0xBF
                    case 2:
                        sym = (byte)GET_SYMBOL(state.Model2, ref H, ref L, ref C, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                        state.Window[window + windowPosition++] = sym;
                        togo--;
                        break;

                    // Selector 3 = literal model, 64 entries, 0xC0-0xFF
                    case 3:
                        sym = (byte)GET_SYMBOL(state.Model3, ref H, ref L, ref C, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                        state.Window[window + windowPosition++] = sym;
                        togo--;
                        break;

                    // Selector 4 = fixed length of 3
                    case 4:
                        sym = (byte)GET_SYMBOL(state.Model4, ref H, ref L, ref C, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                        extra = (int)Q_READ_BITS(state.ExtraBits[sym], inbuf, ref inpos, ref bitsleft, ref bitbuf);
                        matchOffset = (uint)(state.PositionSlotBases[sym] + extra + 1);
                        matchLength = 3;
                        break;

                    // Selector 5 = fixed length of 4
                    case 5:
                        sym = (byte)GET_SYMBOL(state.Model5, ref H, ref L, ref C, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                        extra = (int)Q_READ_BITS(state.ExtraBits[sym], inbuf, ref inpos, ref bitsleft, ref bitbuf);
                        matchOffset = (uint)(state.PositionSlotBases[sym] + extra + 1);
                        matchLength = 4;
                        break;

                    // Selector 6 = variable length
                    case 6:
                        sym = (byte)GET_SYMBOL(state.Model6Length, ref H, ref L, ref C, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                        extra = (int)Q_READ_BITS(state.LengthExtraBits[sym], inbuf, ref inpos, ref bitsleft, ref bitbuf);
                        matchLength = state.LengthBases[sym] + extra + 5;

                        sym = (byte)GET_SYMBOL(state.Model6Position, ref H, ref L, ref C, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                        extra = (int)Q_READ_BITS(state.ExtraBits[sym], inbuf, ref inpos, ref bitsleft, ref bitbuf);
                        matchOffset = (uint)(state.PositionSlotBases[sym] + extra + 1);
                        break;

                    default:
                        return false;
                }

                // If this is a match
                if (selector >= 4)
                {
                    rundest = (int)(window + windowPosition);
                    togo -= matchLength;

                    // Copy any wrapped around source data
                    if (windowPosition >= matchOffset)
                    {
                        // No wrap
                        runsrc = (int)(rundest - matchOffset);
                    }
                    else
                    {
                        runsrc = (int)(rundest + (windowSize - matchOffset));
                        copyLength = (int)(matchOffset - windowPosition);
                        if (copyLength < matchLength)
                        {
                            matchLength -= copyLength;
                            windowPosition += (uint)copyLength;
                            while (copyLength-- > 0)
                            {
                                state.Window[rundest++] = state.Window[rundest++];
                            }

                            runsrc = window;
                        }
                    }

                    windowPosition += (uint)matchLength;

                    // Copy match data - no worries about destination wraps
                    while (matchLength-- > 0)
                    {
                        state.Window[rundest++] = state.Window[runsrc++];
                    }
                }
            }

            if (togo > 0)
                return false;

            Array.Copy(state.Window, (windowPosition == 0 ? windowSize : windowPosition) - outlen, outbuf, 0, outlen);

            state.WindowPosition = windowPosition;
            return true;
        }

        /// <summary>
        /// Initialize a Quantum decompressor state
        /// </summary>
        public static bool InitState(State state, CFFOLDER folder)
        {
            int window = ((ushort)folder.CompressionType >> 8) & 0x1f;
            int level = ((ushort)folder.CompressionType >> 4) & 0xF;
            return InitState(state, window, level);
        }

        /// <summary>
        /// Initialize a Quantum decompressor state
        /// </summary>
        public static bool InitState(State state, int window, int level)
        {
            uint windowSize = (uint)(1 << window);
            int maxSize = window * 2;

            // QTM supports window sizes of 2^10 (1Kb) through 2^21 (2Mb)
            // If a previously allocated window is big enough, keep it
            if (window < 10 || window > 21)
                return false;

            // If we don't have the proper window size
            if (state.ActualSize < windowSize)
                state.Window = null;

            // If we have no window
            if (state.Window == null)
            {
                state.Window = new byte[windowSize];
                state.ActualSize = windowSize;
            }

            // Set the window size and position
            state.WindowSize = windowSize;
            state.WindowPosition = 0;

            // Initialize arithmetic coding models
            state.SelectorModel = CreateModel(state.SelectorModelSymbols, 7, 0);

            state.Model0 = CreateModel(state.Model0Symbols, 0x40, 0x00);
            state.Model1 = CreateModel(state.Model1Symbols, 0x40, 0x40);
            state.Model2 = CreateModel(state.Model2Symbols, 0x40, 0x80);
            state.Model3 = CreateModel(state.Model3Symbols, 0x40, 0xC0);

            // Model 4 depends on table size, ranges from 20 to 24
            state.Model4 = CreateModel(state.Model4Symbols, (maxSize < 24) ? maxSize : 24, 0);

            // Model 5 depends on table size, ranges from 20 to 36
            state.Model5 = CreateModel(symbols: state.Model5Symbols, (maxSize < 36) ? maxSize : 36, 0);

            // Model 6 Position depends on table size, ranges from 20 to 42
            state.Model6Position = CreateModel(state.Model6PositionSymbols, (maxSize < 42) ? maxSize : 42, 0);
            state.Model6Length = CreateModel(state.Model6LengthSymbols, 27, 0);

            return true;
        }

        /// <summary>
        /// Initialize a Quantum model that decodes symbols from s to (s + n - 1)
        /// </summary>
        private static Model CreateModel(ModelSymbol[] symbols, int entryCount, int initialSymbol)
        {
            // Set the basic values
            Model model = new Model
            {
                TimeToReorder = 4,
                Entries = entryCount,
                Symbols = symbols,
            };

            // Clear out the look-up table
            model.LookupTable = Enumerable.Repeat<ushort>(0xFFFF, model.LookupTable.Length).ToArray();

            // Loop through and build the look-up table
            for (ushort i = 0; i < entryCount; i++)
            {
                // Set up a look-up entry for symbol
                model.LookupTable[i + initialSymbol] = i;

                // Create the symbol in the table
                model.Symbols[i] = new ModelSymbol
                {
                    Symbol = (ushort)(i + initialSymbol),
                    CumulativeFrequency = (ushort)(entryCount - i),
                };
            }

            // Set the last symbol frequency to 0
            model.Symbols[entryCount] = new ModelSymbol { CumulativeFrequency = 0 };
            return model;
        }

        /// <summary>
        /// Update the Quantum model for a particular symbol
        /// </summary>
        private static void UpdateModel(Model model, int symbol)
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
            if (--model.TimeToReorder != 0)
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
                model.TimeToReorder = 50;

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
                for (ushort i = 0; i < model.Entries; i++)
                {
                    model.LookupTable[model.Symbols[i].Symbol] = i;
                }
            }
        }

        // Bitstream reading macros (Quantum / normal byte order)
        #region Macros

        /* 
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

        /// <summary>
        /// Should be used first to set up the system
        /// </summary>
        private static void Q_INIT_BITSTREAM(out int bitsleft, out uint bitbuf)
        {
            bitsleft = 0;
            bitbuf = 0;
        }

        /// <summary>
        /// Adds more data to the bit buffer, if there is room for another 16 bits.
        /// </summary>
        private static void Q_FILL_BUFFER(byte[] inbuf, ref int inpos, ref int bitsleft, ref uint bitbuf)
        {
            if (bitsleft > 16)
                return;

            byte b0 = inpos + 0 < inbuf.Length ? inbuf[inpos + 0] : (byte)0;
            byte b1 = inpos + 1 < inbuf.Length ? inbuf[inpos + 1] : (byte)0;

            bitbuf |= (uint)(((b0 << 8) | b1) << (16 - bitsleft));
            bitsleft += 16;
            inpos += 2;
        }

        /// <summary>
        /// Extracts (without removing) N bits from the bit buffer
        /// </summary>
        private static uint Q_PEEK_BITS(int n, uint bitbuf)
        {
            return bitbuf >> (32 - n);
        }

        /// <summary>
        /// Removes N bits from the bit buffer
        /// </summary>
        private static void Q_REMOVE_BITS(int n, ref int bitsleft, ref uint bitbuf)
        {
            bitbuf <<= n;
            bitsleft -= n;
        }

        /// <summary>
        /// Takes N bits from the buffer and puts them in v. Unlike LZX, this can loop
        /// several times to get the requisite number of bits.
        /// </summary>
        private static uint Q_READ_BITS(int n, byte[] inbuf, ref int inpos, ref int bitsleft, ref uint bitbuf)
        {
            uint v = 0; int bitrun;
            for (int bitsneed = n; bitsneed != 0; bitsneed -= bitrun)
            {
                Q_FILL_BUFFER(inbuf, ref inpos, ref bitsleft, ref bitbuf);
                bitrun = (bitsneed > bitsleft) ? bitsleft : bitsneed;
                v = (v << bitrun) | Q_PEEK_BITS(bitrun, bitbuf);
                Q_REMOVE_BITS(bitrun, ref bitsleft, ref bitbuf);
            }

            return v;
        }

        /// <summary>
        /// Fetches the next symbol from the stated model and puts it in symbol.
        /// It may need to read the bitstream to do this.
        /// </summary>
        private static ushort GET_SYMBOL(Model model, ref ushort H, ref ushort L, ref ushort C, byte[] inbuf, ref int inpos, ref int bitsleft, ref uint bitbuf)
        {
            ushort symf = GetFrequency(model.Symbols[0].CumulativeFrequency, H, L, C);

            int i;
            for (i = 1; i < model.Entries; i++)
            {
                if (model.Symbols[i].CumulativeFrequency <= symf)
                    break;
            }

            ushort symbol = model.Symbols[i - 1].Symbol;
            GetCode(model.Symbols[i - 1].CumulativeFrequency,
                model.Symbols[i].CumulativeFrequency,
                model.Symbols[0].CumulativeFrequency,
                ref H, ref L, ref C,
                inbuf, ref inpos, ref bitsleft, ref bitbuf);

            UpdateModel(model, i);
            return symbol;
        }

        /// <summary>
        /// Get the frequency for a given range and total frequency
        /// </summary>
        private static ushort GetFrequency(ushort totalFrequency, ushort H, ushort L, ushort C)
        {
            uint range = (uint)(((H - L) & 0xFFFF) + 1);
            uint freq = (uint)(((C - L + 1) * totalFrequency - 1) / range);
            return (ushort)(freq & 0xFFFF);
        }

        /// <summary>
        /// The decoder renormalization loop
        /// </summary>
        private static void GetCode(int previousFrequency,
            int cumulativeFrequency,
            int totalFrequency,
            ref ushort H,
            ref ushort L,
            ref ushort C,
            byte[] inbuf,
            ref int inpos,
            ref int bitsleft,
            ref uint bitbuf)
        {
            uint range = (uint)((H - L) + 1);
            H = (ushort)(L + ((previousFrequency * range) / totalFrequency) - 1);
            L = (ushort)(L + (cumulativeFrequency * range) / totalFrequency);

            while (true)
            {
                if ((L & 0x8000) != (H & 0x8000))
                {
                    if ((L & 0x4000) == 0 || (H & 0x4000) != 0)
                        break;

                    // Underflow case
                    C ^= 0x4000;
                    L &= 0x3FFF;
                    H |= 0x4000;
                }

                L <<= 1;
                H = (ushort)((H << 1) | 1);
                C = (ushort)((C << 1) | Q_READ_BITS(1, inbuf, ref inpos, ref bitsleft, ref bitbuf));
            }
        }

        #endregion
    }
}