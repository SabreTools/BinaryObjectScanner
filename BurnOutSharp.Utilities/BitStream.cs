using System.Collections;
using System.IO;

namespace BurnOutSharp.Utilities
{
    /// <summary>
    /// Stream that allows reading bits or groups of bits at a time
    /// </summary>
    public class BitStream
    {
        /// <summary>
        /// Underlying stream to read from
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Bit array representing the current byte in the stream
        /// </summary>
        private BitArray _bitBuffer;

        /// <summary>
        /// Next bit position to read from in the buffer
        /// </summary>
        private int _bitPosition;

        public BitStream(byte[] data)
        {
            _stream = new MemoryStream(data);
            _bitBuffer = null;
            _bitPosition = -1;
        }

        public BitStream(Stream data)
        {
            _stream = data;
            _bitBuffer = null;
            _bitPosition = -1;
        }

        /// <summary>
        /// Read an array of bits from the input
        /// </summary>
        /// <param name="bitCount">Number of bits to read</param>
        /// <returns>Array representing the read bits, null on error</returns>
        public BitArray ReadBits(int bitCount)
        {
            // If we have an invalid bit count
            if (bitCount <= 0)
                return null;

            // If we have an invalid bit buffer
            if (_bitBuffer == null || _bitPosition < 0 || _bitPosition >= 8)
                RefreshBuffer();

            // Create an array to hold the bits
            BitArray bits = new BitArray(bitCount);

            // Loop through and populate the bits
            for (int i = 0; i < bitCount; i++)
            {
                // Read the next bit from the buffer
                bits[i] = _bitBuffer[_bitPosition++];

                // Attempt to refresh the buffer if we need to
                if (_bitPosition < 0 || _bitPosition > 7)
                {
                    // If the refresh failed
                    if (!RefreshBuffer())
                        break;
                }
            }

            // Return the bits
            return bits;
        }

        /// <summary>
        /// Read an array of bytes from the input
        /// </summary>
        /// <param name="byteCount">Number of bytes to read</param>
        /// <returns>Array representing the read bytes, null on error</returns>
        public byte[] ReadBytes(int byteCount)
        {
            // If we have an invalid byte count
            if (byteCount <= 0)
                return null;

            // Get the corresponding bit array
            BitArray bits = ReadBits(byteCount * 8);
            if (bits == null)
                return null;

            // Create the byte array
            byte[] bytes = new byte[byteCount];

            // Initialize the loop variables
            byte byt = 0; int bitNumber = 0, byteNumber = 0;

            // Loop and build the byte array
            for (int i = 0; i < bits.Length; i++)
            {
                // Add the new bit to the byte
                byt <<= 1;
                byt |= (byte)(bits[i] ? 1 : 0);
                bitNumber++;

                // If we are at the end of a byte
                if (bitNumber == 8)
                {
                     bytes[byteNumber++] = byt;
                     bitNumber = 0;
                }
            }

            // Add the last byte
            bytes[byteNumber] = byt;

            return bytes;
        }

        /// <summary>
        /// Discard the remaining bits in the buffer
        /// </summary>
        public void DiscardBuffer() => RefreshBuffer();

        /// <summary>
        /// Refresh the bit buffer from the data source
        /// </summary>
        /// <returns>True if the buffer refreshed, false otherwise</returns>
        private bool RefreshBuffer()
        {
            // If we ran out of bytes
            if (_stream.Position >= _stream.Length)
            {
                _bitBuffer = null;
                _bitPosition = -1;
                return false;
            }

            // Read the next byte and reset
            byte next = _stream.ReadByteValue();
            _bitBuffer = new BitArray(new byte[] { next });
            _bitPosition = 0;
            return true;
        }
    }
}