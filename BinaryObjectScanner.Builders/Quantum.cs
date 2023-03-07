using System.IO;
using System.Text;
using BinaryObjectScanner.Models.Quantum;
using BinaryObjectScanner.Utilities;
using static BinaryObjectScanner.Models.Quantum.Constants;

namespace BinaryObjectScanner.Builders
{
    public class Quantum
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Quantum archive
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled archive on success, null on error</returns>
        public static Archive ParseArchive(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseArchive(dataStream);
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a Quantum archive
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled archive on success, null on error</returns>
        public static Archive ParseArchive(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new archive to fill
            var archive = new Archive();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the archive header
            archive.Header = header;

            #endregion

            #region File List

            // If we have any files
            if (header.FileCount > 0)
            {
                var fileDescriptors = new FileDescriptor[header.FileCount];

                // Read all entries in turn
                for (int i = 0; i < header.FileCount; i++)
                {
                    var file = ParseFileDescriptor(data, header.MinorVersion);
                    if (file == null)
                        return null;

                    fileDescriptors[i] = file;
                }

                // Set the file list
                archive.FileList = fileDescriptors;
            }

            #endregion

            // Cache the compressed data offset
            archive.CompressedDataOffset = data.Position;

            return archive;
        }

        /// <summary>
        /// Parse a Stream into a header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled header on success, null on error</returns>
        private static Header ParseHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            Header header = new Header();

            byte[] signature = data.ReadBytes(2);
            header.Signature = Encoding.ASCII.GetString(signature);
            if (header.Signature != SignatureString)
                return null;

            header.MajorVersion = data.ReadByteValue();
            header.MinorVersion = data.ReadByteValue();
            header.FileCount = data.ReadUInt16();
            header.TableSize = data.ReadByteValue();
            header.CompressionFlags = data.ReadByteValue();

            return header;
        }

        /// <summary>
        /// Parse a Stream into a file descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="minorVersion">Minor version of the archive</param>
        /// <returns>Filled file descriptor on success, null on error</returns>
        private static FileDescriptor ParseFileDescriptor(Stream data, byte minorVersion)
        {
            // TODO: Use marshalling here instead of building
            FileDescriptor fileDescriptor = new FileDescriptor();

            fileDescriptor.FileNameSize = ReadVariableLength(data);
            if (fileDescriptor.FileNameSize > 0)
            {
                byte[] fileName = data.ReadBytes(fileDescriptor.FileNameSize);
                fileDescriptor.FileName = Encoding.ASCII.GetString(fileName);
            }

            fileDescriptor.CommentFieldSize = ReadVariableLength(data);
            if (fileDescriptor.CommentFieldSize > 0)
            {
                byte[] commentField = data.ReadBytes(fileDescriptor.CommentFieldSize);
                fileDescriptor.CommentField = Encoding.ASCII.GetString(commentField);
            }

            fileDescriptor.ExpandedFileSize = data.ReadUInt32();
            fileDescriptor.FileTime = data.ReadUInt16();
            fileDescriptor.FileDate = data.ReadUInt16();

            // Hack for unknown format data
            if (minorVersion == 22)
                fileDescriptor.Unknown = data.ReadUInt16();

            return fileDescriptor;
        }

        /// <summary>
        /// Parse a Stream into a variable-length size prefix
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Variable-length size prefix</returns>
        /// <remarks>
        /// Strings are prefixed with their length. If the length is less than 128
        /// then it is stored directly in one byte. If it is greater than 127 then
        /// the high bit of the first byte is set to 1 and the remaining fifteen bits
        /// contain the actual length in big-endian format.
        /// </remarks>
        private static int ReadVariableLength(Stream data)
        {
            byte b0 = data.ReadByteValue();
            if (b0 < 0x7F)
                return b0;
            
            b0 &= 0x7F;
            byte b1 = data.ReadByteValue();
            return (b0 << 8) | b1;
        }

        #endregion
    }
}
