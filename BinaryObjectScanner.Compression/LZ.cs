using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO;
using SabreTools.Models.Compression.LZ;
using static SabreTools.Models.Compression.LZ.Constants;

namespace BinaryObjectScanner.Compression
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/kernel32/lzexpand.c"/>
    public class LZ
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public LZ() { }

        #endregion

        #region Static Methods

        /// <summary>
        /// Decompress LZ-compressed data
        /// </summary>
        /// <param name="compressed">Byte array representing the compressed data</param>
        /// <returns>Decompressed data as a byte array, null on error</returns>
        public static byte[] Decompress(byte[] compressed)
        {
            // If we have and invalid input
            if (compressed == null || compressed.Length == 0)
                return null;

            // Create a memory stream for the input and decompress that
            var compressedStream = new MemoryStream(compressed);
            return Decompress(compressedStream);
        }

        /// <summary>
        /// Decompress LZ-compressed data
        /// </summary>
        /// <param name="compressed">Stream representing the compressed data</param>
        /// <returns>Decompressed data as a byte array, null on error</returns>
        public static byte[] Decompress(Stream compressed)
        {
            // If we have and invalid input
            if (compressed == null || compressed.Length == 0)
                return null;

            // Create a new LZ for decompression
            var lz = new LZ();

            // Open the input data
            var sourceState = lz.Open(compressed, out _);
            if (sourceState?.Window == null)
                return null;

            // Create the output data and open it
            var decompressedStream = new MemoryStream();
            var destState = lz.Open(decompressedStream, out _);
            if (destState == null)
                return null;

            // Decompress the data by copying
            long read = lz.CopyTo(sourceState, destState, out LZERROR error);

            // Copy the data to the buffer
            byte[] decompressed;
            if (read == 0 || (error != LZERROR.LZERROR_OK && error != LZERROR.LZERROR_NOT_LZ))
            {
                decompressed = null;
            }
            else
            {
                int dataEnd = (int)decompressedStream.Position;
                decompressedStream.Seek(0, SeekOrigin.Begin);
                decompressed = decompressedStream.ReadBytes(dataEnd);
            }

            // Close the streams
            lz.Close(sourceState);
            lz.Close(destState);

            return decompressed;
        }

        /// <summary>
        /// Reconstructs the full filename of the compressed file
        /// </summary>
        public static string GetExpandedName(string input, out LZERROR error)
        {
            // Try to open the file as a compressed stream
            var fileStream = File.Open(input, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var state = new LZ().Open(fileStream, out error);
            if (state?.Window == null)
                return null;

            // Get the extension for modification
            string inputExtension = Path.GetExtension(input).TrimStart('.');

            // If we have no extension
            if (string.IsNullOrWhiteSpace(inputExtension))
                return Path.GetFileNameWithoutExtension(input);

            // If we have an extension of length 1
            if (inputExtension.Length == 1)
            {
                if (inputExtension == "_")
                    return $"{Path.GetFileNameWithoutExtension(input)}.{char.ToLower(state.LastChar)}";
                else
                    return Path.GetFileNameWithoutExtension(input);
            }

            // If we have an extension that doesn't end in an underscore
            if (!inputExtension.EndsWith("_"))
                return Path.GetFileNameWithoutExtension(input);

            // Build the new filename
            bool isLowerCase = char.IsUpper(input[0]);
            char replacementChar = isLowerCase ? char.ToLower(state.LastChar) : char.ToUpper(state.LastChar);
            string outputExtension = inputExtension.Substring(0, inputExtension.Length - 1) + replacementChar;
            return $"{Path.GetFileNameWithoutExtension(input)}.{outputExtension}";
        }

        #endregion

        #region State Management

        /// <summary>
        /// Opens a stream and creates a state from it
        /// </summary>
        /// <param name="stream">Source stream to create a state from</stream>
        /// <param name="error">Output representing the last error</param>
        /// <returns>An initialized State, null on error</returns>
        /// <remarks>Uncompressed streams are represented by a State with no buffer</remarks>
        public State Open(Stream stream, out LZERROR error)
        {
            State lzs = Init(stream, out error);
            if (error == LZERROR.LZERROR_OK || error == LZERROR.LZERROR_NOT_LZ)
                return lzs;

            return null;
        }

        /// <summary>
        /// Closes a state by invalidating the source
        /// </summary>
        /// <param name="stream">State object to close</stream>
        public void Close(State state)
        {
            try
            {
                state?.Source?.Close();
            }
            catch { }
        }

        /// <summary>
        /// Initializes internal decompression buffers
        /// </summary>
        /// <param name="source">Input stream to create a state from</param>
        /// <param name="error">Output representing the last error</param>
        /// <returns>An initialized State, null on error</returns>
        /// <remarks>Uncompressed streams are represented by a State with no buffer</remarks>
        public State Init(Stream source, out LZERROR error)
        {
            // If we have an invalid source
            if (source == null)
            {
                error = LZERROR.LZERROR_BADVALUE;
                return null;
            }

            // Attempt to read the header
            var fileHeader = ParseFileHeader(source, out error);

            // If we had a valid but uncompressed stream
            if (error == LZERROR.LZERROR_NOT_LZ)
            {
                source.Seek(0, SeekOrigin.Begin);
                return new State { Source = source };
            }

            // If we had any error
            else if (error != LZERROR.LZERROR_OK)
            {
                source.Seek(0, SeekOrigin.Begin);
                return null;
            }

            // Initialize the table with all spaces
            byte[] table = Enumerable.Repeat((byte)' ', LZ_TABLE_SIZE).ToArray();

            // Build the state
            var state = new State
            {
                Source = source,
                LastChar = fileHeader.LastChar,
                RealLength = fileHeader.RealLength,

                Window = new byte[GETLEN],
                WindowLength = 0,
                WindowCurrent = 0,

                Table = table,
                CurrentTableEntry = 0xff0,
            };

            // Return the state
            return state;
        }

        #endregion

        #region Stream Functionality

        /// <summary>
        /// Attempt to read the specified number of bytes from the State
        /// </summary>
        /// <param name="source">Source State to read from</param>
        /// <param name="buffer">Byte buffer to read into</param>
        /// <param name="offset">Offset within the buffer to read</param>
        /// <param name="count">Number of bytes to read</param>
        /// <param name="error">Output representing the last error</param>
        /// <returns>The number of bytes read, if possible</returns>
        /// <remarks>
        /// If the source data is compressed, this will decompress the data.
        /// If the source data is uncompressed, it is copied directly
        /// </remarks>
        public int Read(State source, byte[] buffer, int offset, int count, out LZERROR error)
        {
            // If we have an uncompressed input
            if (source.Window == null)
            {
                error = LZERROR.LZERROR_NOT_LZ;
                return source.Source.Read(buffer, offset, count);
            }

            // If seeking has occurred, we need to perform the seek
            if (source.RealCurrent != source.RealWanted)
            {
                // If the requested position is before the current, we need to reset
                if (source.RealCurrent > source.RealWanted)
                {
                    // Reset the decompressor state
                    source.Source.Seek(LZ_HEADER_LEN, SeekOrigin.Begin);
                    FlushWindow(source);
                    source.RealCurrent = 0;
                    source.ByteType = 0;
                    source.StringLength = 0;
                    source.Table = Enumerable.Repeat((byte)' ', LZ_TABLE_SIZE).ToArray();
                    source.CurrentTableEntry = 0xFF0;
                }

                // While we are not at the right offset
                while (source.RealCurrent < source.RealWanted)
                {
                    _ = DecompressByte(source, out error);
                    if (error != LZERROR.LZERROR_OK)
                        return 0;
                }
            }

            int bytesRemaining = count;
            while (bytesRemaining > 0)
            {
                byte b = DecompressByte(source, out error);
                if (error != LZERROR.LZERROR_OK)
                    return count - bytesRemaining;

                source.RealWanted++;
                buffer[offset++] = b;
                bytesRemaining--;
            }

            error = LZERROR.LZERROR_OK;
            return count;
        }

        /// <summary>
        /// Perform a seek on the source data
        /// </summary>
        /// <param name="state">State to seek within</param>
        /// <param name="offset">Data offset to seek to</state>
        /// <param name="seekOrigin">SeekOrigin representing how to seek</state>
        /// <param name="error">Output representing the last error</param>
        /// <returns>The position that was seeked to, -1 on error</returns>
        public long Seek(State state, long offset, SeekOrigin seekOrigin, out LZERROR error)
        {
            // If we have an invalid state
            if (state == null)
            {
                error = LZERROR.LZERROR_BADVALUE;
                return -1;
            }

            // If we have an uncompressed input
            if (state.Window == null)
            {
                error = LZERROR.LZERROR_NOT_LZ;
                return state.Source.Seek(offset, seekOrigin);
            }

            // Otherwise, generate the new offset
            long newWanted = state.RealWanted;
            switch (seekOrigin)
            {
                case SeekOrigin.Current:
                    newWanted += offset;
                    break;
                case SeekOrigin.End:
                    newWanted = state.RealLength - offset;
                    break;
                default:
                    newWanted = offset;
                    break;
            }

            // If we have an invalid new offset
            if (newWanted < 0 && newWanted > state.RealLength)
            {
                error = LZERROR.LZERROR_BADVALUE;
                return -1;
            }

            error = LZERROR.LZERROR_OK;
            state.RealWanted = (uint)newWanted;
            return newWanted;
        }

        /// <summary>
        /// Copies all data from the source to the destination
        /// </summary>
        /// <param name="source">Source State to read from</param>
        /// <param name="dest">Destination state to write to</param>
        /// <param name="error">Output representing the last error</param>
        /// <returns>The number of bytes written, -1 on error</returns>
        /// <remarks>
        /// If the source data is compressed, this will decompress the data.
        /// If the source data is uncompressed, it is copied directly
        /// </remarks>
        public long CopyTo(State source, State dest, out LZERROR error)
        {
            error = LZERROR.LZERROR_OK;

            // If we have an uncompressed input
            if (source.Window == null)
            {
                source.Source.CopyTo(dest.Source);
                return source.Source.Length;
            }

            // Loop until we have read everything
            long length = 0;
            while (true)
            {
                // Read at most 1000 bytes
                byte[] buf = new byte[1000];
                int read = Read(source, buf, 0, buf.Length, out error);

                // If we had an error
                if (read == 0)
                {
                    if (error == LZERROR.LZERROR_NOT_LZ)
                    {
                        error = LZERROR.LZERROR_OK;
                        break;
                    }
                    else if (error != LZERROR.LZERROR_OK)
                    {
                        error = LZERROR.LZERROR_READ;
                        return 0;
                    }
                }

                // Otherwise, append the length read and write the data
                length += read;
                dest.Source.Write(buf, 0, read);
            }

            return length;
        }

        /// <summary>
        /// Decompress a single byte of data from the source State
        /// </summary>
        /// <param name="source">Source State to read from</param>
        /// <param name="error">Output representing the last error</param>
        /// <returns>The read byte, if possible</returns>
        private byte DecompressByte(State source, out LZERROR error)
        {
            byte b;

            if (source.StringLength != 0)
            {
                b = source.Table[source.StringPosition];
                source.StringPosition = (source.StringPosition + 1) & 0xFFF;
                source.StringLength--;
            }
            else
            {
                if ((source.ByteType & 0x100) == 0)
                {
                    b = ReadByte(source, out error);
                    if (error != LZERROR.LZERROR_OK)
                        return 0;

                    source.ByteType = (ushort)(b | 0xFF00);
                }
                if ((source.ByteType & 1) != 0)
                {
                    b = ReadByte(source, out error);
                    if (error != LZERROR.LZERROR_OK)
                        return 0;
                }
                else
                {
                    byte b1 = ReadByte(source, out error);
                    if (error != LZERROR.LZERROR_OK)
                        return 0;

                    byte b2 = ReadByte(source, out error);
                    if (error != LZERROR.LZERROR_OK)
                        return 0;

                    // Format:
                    // b1 b2
                    // AB CD
                    // where CAB is the stringoffset in the table
                    // and D+3 is the len of the string
                    source.StringPosition = (uint)(b1 | ((b2 & 0xf0) << 4));
                    source.StringLength = (byte)((b2 & 0xf) + 2);

                    // 3, but we use a byte already below...
                    b = source.Table[source.StringPosition];
                    source.StringPosition = (source.StringPosition + 1) & 0xFFF;
                }

                source.ByteType >>= 1;
            }

            // Store b in table
            source.Table[source.CurrentTableEntry++] = b;
            source.CurrentTableEntry &= 0xFFF;
            source.RealCurrent++;

            error = LZERROR.LZERROR_OK;
            return b;
        }

        /// <summary>
        /// Reads one compressed byte, including buffering
        /// </summary>
        /// <param name="state">State to read using</param>
        /// <param name="error">Output representing the last error</param>
        /// <returns>Byte value that was read, if possible</returns>
        private byte ReadByte(State state, out LZERROR error)
        {
            // If we have enough data in the buffer
            if (state.WindowCurrent < state.WindowLength)
            {
                error = LZERROR.LZERROR_OK;
                return state.Window[state.WindowCurrent++];
            }

            // Otherwise, read from the source
            int ret = state.Source.Read(state.Window, 0, GETLEN);
            if (ret == 0)
            {
                error = LZERROR.LZERROR_NOT_LZ;
                return 0;
            }

            // Reset the window state
            state.WindowLength = (uint)ret;
            state.WindowCurrent = 1;
            error = LZERROR.LZERROR_OK;
            return state.Window[0];
        }

        /// <summary>
        /// Reset the current window position to the length
        /// </summary>
        /// <param name="state">State to flush</param>
        private void FlushWindow(State state)
        {
            state.WindowCurrent = state.WindowLength;
        }

        /// <summary>
        /// Parse a Stream into a file header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="error">Output representing the last error</param>
        /// <returns>Filled file header on success, null on error</returns>
        private FileHeaader ParseFileHeader(Stream data, out LZERROR error)
        {
            error = LZERROR.LZERROR_OK;
            FileHeaader fileHeader = new FileHeaader();

            byte[] magic = data.ReadBytes(LZ_MAGIC_LEN);
            fileHeader.Magic = Encoding.ASCII.GetString(magic);
            if (fileHeader.Magic != MagicString)
            {
                error = LZERROR.LZERROR_NOT_LZ;
                return null;
            }

            fileHeader.CompressionType = data.ReadByteValue();
            if (fileHeader.CompressionType != (byte)'A')
            {
                error = LZERROR.LZERROR_UNKNOWNALG;
                return null;
            }

            fileHeader.LastChar = (char)data.ReadByteValue();
            fileHeader.RealLength = data.ReadUInt32();

            return fileHeader;
        }

        #endregion
    }
}