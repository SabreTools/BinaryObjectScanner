using System.IO;
using System.Text;
using BurnOutSharp.Models.CFB;
using BurnOutSharp.Utilities;
using static BurnOutSharp.Models.CFB.Constants;

namespace BurnOutSharp.Builders
{
    public class CFB
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Compound File Binary
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled Compound File Binary on success, null on error</returns>
        public static Binary ParseBinary(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseBinary(dataStream);
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a Compound File Binary
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Compound File Binary on success, null on error</returns>
        public static Binary ParseBinary(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new binary to fill
            var binary = new Binary();

            #region Header

            // Try to parse the file header
            var fileHeader = ParseFileHeader(data);
            if (fileHeader == null)
                return null;

            // Set the file header
            binary.Header = fileHeader;

            #endregion

            // TODO: Implement FAT sector parsing
            // TODO: Implement Mini FAT sector parsing
            // TODO: Implement DIFAT sector parsing
            // TODO: Implement directory sector parsing

            return binary;
        }

        /// <summary>
        /// Parse a Stream into a file header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled file header on success, null on error</returns>
        private static FileHeader ParseFileHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            FileHeader header = new FileHeader();

            header.Signature = data.ReadBytes(8);
            if (header.Signature != SignatureBytes)
                return null;

            header.CLSID = data.ReadGuid();
            header.MinorVersion = data.ReadUInt16();
            header.MajorVersion = data.ReadUInt16();
            header.ByteOrder = data.ReadUInt16();
            if (header.ByteOrder != 0xFFFE)
                return null;

            header.SectorShift = data.ReadUInt16();
            if (header.MajorVersion == 3 && header.SectorShift != 0x0009)
                return null;
            else if (header.MajorVersion == 4 && header.SectorShift != 0x000C)
                return null;

            header.MiniSectorShift = data.ReadUInt16();
            header.Reserved = data.ReadBytes(6);
            header.NumberOfDirectorySectors = data.ReadUInt32();
            if (header.MajorVersion == 3 && header.NumberOfDirectorySectors != 0)
                return null;

            header.NumberOfFATSectors = data.ReadUInt32();
            header.FirstDirectorySectorLocation = data.ReadUInt32();
            header.TransactionSignatureNumber = data.ReadUInt32();
            header.MiniStreamCutoffSize = data.ReadUInt32();
            if (header.MiniStreamCutoffSize != 0x00001000)
                return null;

            header.FirstMiniFATSectorLocation = data.ReadUInt32();
            header.NumberOfMiniFATSectors = data.ReadUInt32();
            header.FirstDIFATSectorLocation = data.ReadUInt32();
            header.NumberOfDIFATSectors = data.ReadUInt32();
            header.DIFAT = new uint[109];
            for (int i = 0; i < header.DIFAT.Length; i++)
            {
                header.DIFAT[i] = data.ReadUInt32();
            }

            // Skip rest of sector for version 4
            if (header.MajorVersion == 4)
                _ = data.ReadBytes(3584);

            return header;
        }

        #endregion
    }
}
