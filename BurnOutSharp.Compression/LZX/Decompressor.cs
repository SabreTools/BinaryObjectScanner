using BurnOutSharp.Models.Compression.LZX;
using static BurnOutSharp.Models.Compression.LZX.Constants;

namespace BurnOutSharp.Compression.LZX
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/fdi.c"/>
    public class Decompressor
    {
        /// <summary>
        /// Decompress a byte array using a given State
        /// </summary>
        public static bool Decompress(State state, int inlen, byte[] inbuf, int outlen, byte[] outbuf)
        {

            // TODO: Finish implementation
            return false;
        }

        /// <summary>
        /// Read and build the Huffman tree from the lengths
        /// </summary>
        /// INCORRECT IMPLEMENTATION TO SATISFY COMPILER FOR NOW
        private static int ReadLengths(byte[] lengths, uint first, uint last, Bits lb, State state, byte[] inbuf, ref int inpos, ref int bitsleft, ref uint bitbuf)
        {

            // TODO: Finish implementation
            return 0;
        }

        // Bitstream reading macros (LZX / intel little-endian byte order)
        #region Bitstream Reading Macros

        /*
        * These bit access routines work by using the area beyond the MSB and the
        * LSB as a free source of zeroes. This avoids having to mask any bits.
        * So we have to know the bit width of the bitbuffer variable.
        */

        /// <summary>
        /// Should be used first to set up the system
        /// </summary>
        private static void INIT_BITSTREAM(out int bitsleft, out uint bitbuf)
        {
            bitsleft = 0;
            bitbuf = 0;
        }

        /// <summary>
        /// Ensures there are at least N bits in the bit buffer. It can guarantee
        // up to 17 bits (i.e. it can read in 16 new bits when there is down to
        /// 1 bit in the buffer, and it can read 32 bits when there are 0 bits in
        /// the buffer).
        /// </summary>
        /// <remarks>Quantum reads bytes in normal order; LZX is little-endian order</remarks>
        private static void ENSURE_BITS(int n, byte[] inbuf, ref int inpos, ref int bitsleft, ref uint bitbuf)
        {
            while (bitsleft < n)
            {
                bitbuf |= (uint)(((inbuf[inpos + 1] << 8) | inbuf[inpos + 0]) << (16 - bitsleft));
                bitsleft += 16;
                inpos += 2;
            }
        }

        /// <summary>
        /// Extracts (without removing) N bits from the bit buffer
        /// </summary>
        private static uint PEEK_BITS(int n, uint bitbuf)
        {
            return bitbuf >> (32 - n);
        }

        /// <summary>
        /// Removes N bits from the bit buffer
        /// </summary>
        private static void REMOVE_BITS(int n, ref int bitsleft, ref uint bitbuf)
        {
            bitbuf <<= n;
            bitsleft -= n;
        }

        /// <summary>
        /// Takes N bits from the buffer and puts them in v.
        /// </summary>
        private static uint READ_BITS(int n, byte[] inbuf, ref int inpos, ref int bitsleft, ref uint bitbuf)
        {
            uint v = 0;
            if (n > 0)
            {
                ENSURE_BITS(n, inbuf, ref inpos, ref bitsleft, ref bitbuf);
                v = PEEK_BITS(n, bitbuf);
                REMOVE_BITS(n, ref bitsleft, ref bitbuf);
            }

            return v;
        }

        #endregion

        // Huffman macros
        #region Huffman Macros

        /// <summary>
        /// Decodes one huffman symbol from the bitstream using the stated table and
        /// puts it in v.
        /// </summary>
        private static int? READ_HUFFSYM(ushort[] hufftbl, byte[] lentable, int tablebits, int maxsymbols, byte[] inbuf, ref int inpos, ref int bitsleft, ref uint bitbuf)
        {
            int v = 0, i, j = 0;
            ENSURE_BITS(16, inbuf, ref inpos, ref bitsleft, ref bitbuf);
            if ((i = hufftbl[PEEK_BITS(tablebits, bitbuf)]) >= maxsymbols)
            {
                j = 1 << (32 - tablebits);
                do
                {
                    j >>= 1;
                    i <<= 1;
                    i |= (bitbuf & j) != 0 ? 1 : 0;
                    if (j == 0)
                        return null;
                } while ((i = hufftbl[i]) >= maxsymbols);
            }

            j = lentable[v = i];
            REMOVE_BITS(j, ref bitsleft, ref bitbuf);
            return v;
        }

        /// <summary>
        /// Reads in code lengths for symbols first to last in the given table. The
        /// code lengths are stored in their own special LZX way.
        /// </summary>
        private static bool READ_LENGTHS(byte[] lentable, uint first, uint last, Bits lb, State state, byte[] inbuf, ref int inpos, ref int bitsleft, ref uint bitbuf)
        {
            lb.BitBuffer = bitbuf;
            lb.BitsLeft = bitsleft;
            lb.InitialPosition = inpos;

            if (ReadLengths(lentable, first, last, lb, state, inbuf, ref inpos, ref bitsleft, ref bitbuf) != 0)
                return false;

            bitbuf = lb.BitBuffer;
            bitsleft = lb.BitsLeft;
            inpos = lb.InitialPosition;
            return true;
        }

        #endregion
    }
}