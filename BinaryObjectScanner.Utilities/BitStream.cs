using System.Collections;
using System.IO;
using SabreTools.IO;

namespace BinaryObjectScanner.Utilities
{
    /// <summary>
    /// Stream that allows reading bits or groups of bits at a time
    /// </summary>
    public class BitStream
    {
        #region Instance Variables

        /// <summary>
        /// Underlying stream to read from
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Bit array representing the current byte in the stream
        /// </summary>
#if NET48
        private BitArray _bitBuffer;
#else
        private BitArray? _bitBuffer;
#endif

        /// <summary>
        /// Next bit position to read from in the buffer
        /// </summary>
        private int _bitPosition;

        #endregion

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

        #region Reading

        /// <summary>
        /// Read an array of bits from the input
        /// </summary>
        /// <param name="bitCount">Number of bits to read</param>
        /// <returns>Array representing the read bits, null on error</returns>
#if NET48
        public BitArray ReadBits(int bitCount)
#else
        public BitArray? ReadBits(int bitCount)
#endif
        {
            // If we have an invalid bit count
            if (bitCount <= 0)
                return null;

            // If we have an invalid bit buffer
            if (_bitBuffer == null || _bitPosition < 0 || _bitPosition > 7)
                RefreshBuffer();

            // Create an array to hold the bits
            var bits = new BitArray(bitCount);

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

        #region Byte-Aligned Reads

        /// <summary>
        /// Read a byte-aligned byte from the input
        /// </summary>
        public byte ReadAlignedByte()
        {
            // Read back a single byte
            if (_stream.Position > 0)
                _stream.Seek(-1, SeekOrigin.Current);

            // Read the value and refreah the buffer
            byte value = _stream.ReadByteValue();
            RefreshBuffer();
            return value;
        }

        /// <summary>
        /// Read an array of bytes from the input
        /// </summary>
        /// <param name="byteCount">Number of bytes to read</param>
        /// <returns>Array representing the read bytes, null on error</returns>
#if NET48
        public byte[] ReadAlignedBytes(int byteCount)
#else
        public byte[]? ReadAlignedBytes(int byteCount)
#endif
        {
            // Read back a single byte
            if (_stream.Position > 0)
                _stream.Seek(-1, SeekOrigin.Current);

            // Read the value and refreah the buffer
            var value = _stream.ReadBytes(byteCount);
            RefreshBuffer();
            return value;
        }

        /// <summary>
        /// Read a byte-aligned sbyte from the input
        /// </summary>
        public sbyte ReadAlignedSByte()
        {
            // Read back a single byte
            if (_stream.Position > 0)
                _stream.Seek(-1, SeekOrigin.Current);

            // Read the value and refreah the buffer
            sbyte value = _stream.ReadSByte();
            RefreshBuffer();
            return value;
        }

        /// <summary>
        /// Read a byte-aligned short from the input
        /// </summary>
        public short ReadAlignedInt16()
        {
            // Read back a single byte
            if (_stream.Position > 0)
                _stream.Seek(-1, SeekOrigin.Current);

            // Read the value and refreah the buffer
            short value = _stream.ReadInt16();
            RefreshBuffer();
            return value;
        }

        /// <summary>
        /// Read a byte-aligned ushort from the input
        /// </summary>
        public ushort ReadAlignedUInt16()
        {
            // Read back a single byte
            if (_stream.Position > 0)
                _stream.Seek(-1, SeekOrigin.Current);

            // Read the value and refreah the buffer
            ushort value = _stream.ReadUInt16();
            RefreshBuffer();
            return value;
        }

        /// <summary>
        /// Read a byte-aligned int from the input
        /// </summary>
        public int ReadAlignedInt32()
        {
            // Read back a single byte
            if (_stream.Position > 0)
                _stream.Seek(-1, SeekOrigin.Current);

            // Read the value and refreah the buffer
            int value = _stream.ReadInt32();
            RefreshBuffer();
            return value;
        }

        /// <summary>
        /// Read a byte-aligned uint from the input
        /// </summary>
        public uint ReadAlignedUInt32()
        {
            // Read back a single byte
            if (_stream.Position > 0)
                _stream.Seek(-1, SeekOrigin.Current);

            // Read the value and refreah the buffer
            uint value = _stream.ReadUInt32();
            RefreshBuffer();
            return value;
        }

        /// <summary>
        /// Read a byte-aligned long from the input
        /// </summary>
        public long ReadAlignedInt64()
        {
            // Read back a single byte
            if (_stream.Position > 0)
                _stream.Seek(-1, SeekOrigin.Current);

            // Read the value and refreah the buffer
            long value = _stream.ReadInt64();
            RefreshBuffer();
            return value;
        }

        /// <summary>
        /// Read a byte-aligned ulong from the input
        /// </summary>
        public ulong ReadAlignedUInt64()
        {
            // Read back a single byte
            if (_stream.Position > 0)
                _stream.Seek(-1, SeekOrigin.Current);

            // Read the value and refreah the buffer
            ulong value = _stream.ReadUInt64();
            RefreshBuffer();
            return value;
        }

        #endregion

        #endregion

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