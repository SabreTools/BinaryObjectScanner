using System.IO;
using System.Text;
using BurnOutSharp.Models.PFF;
using BurnOutSharp.Utilities;
using static BurnOutSharp.Models.PFF.Constants;

namespace BurnOutSharp.Builders
{
    public class PFF
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a PFF archive
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
        /// Parse a Stream into a PFF archive
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

            #region Segments

            // Get the segments
            long offset = header.FileListOffset;
            if (offset < 0 || offset >= data.Length)
                return null;

            // Seek to the segments
            data.Seek(offset, SeekOrigin.Begin);

            // Create the segments array
            archive.Segments = new Segment[header.NumberOfFiles];

            // Read all segments in turn
            for (int i = 0; i < header.NumberOfFiles; i++)
            {
                var file = ParseSegment(data, header.FileSegmentSize);
                if (file == null)
                    return null;

                archive.Segments[i] = file;
            }

            #endregion

            #region Footer

            // Get the footer offset
            offset = header.FileListOffset + (header.FileSegmentSize * header.NumberOfFiles);
            if (offset < 0 || offset >= data.Length)
                return null;

            // Seek to the footer
            data.Seek(offset, SeekOrigin.Begin);

            // Try to parse the footer
            var footer = ParseFooter(data);
            if (footer == null)
                return null;

            // Set the archive footer
            archive.Footer = footer;

            #endregion

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

            header.HeaderSize = data.ReadUInt32();
            byte[] signature = data.ReadBytes(4);
            header.Signature = Encoding.ASCII.GetString(signature);
            header.NumberOfFiles = data.ReadUInt32();
            header.FileSegmentSize = data.ReadUInt32();
            switch (header.Signature)
            {
                case Version0SignatureString:
                    if (header.FileSegmentSize != Version0HSegmentSize)
                        return null;
                    break;

                case Version2SignatureString:
                    if (header.FileSegmentSize != Version2SegmentSize)
                        return null;
                    break;

                // Version 3 can sometimes have Version 2 segment sizes
                case Version3SignatureString:
                    if (header.FileSegmentSize != Version2SegmentSize && header.FileSegmentSize != Version3SegmentSize)
                        return null;
                    break;

                case Version4SignatureString:
                    if (header.FileSegmentSize != Version4SegmentSize)
                        return null;
                    break;

                default:
                    return null;
            }

            header.FileListOffset = data.ReadUInt32();

            return header;
        }

        /// <summary>
        /// Parse a Stream into a footer
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled footer on success, null on error</returns>
        private static Footer ParseFooter(Stream data)
        {
            // TODO: Use marshalling here instead of building
            Footer footer = new Footer();

            footer.SystemIP = data.ReadUInt32();
            footer.Reserved = data.ReadUInt32();
            byte[] kingTag = data.ReadBytes(4);
            footer.KingTag = Encoding.ASCII.GetString(kingTag);

            return footer;
        }

        /// <summary>
        /// Parse a Stream into a file entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="segmentSize">PFF segment size</param>
        /// <returns>Filled file entry on success, null on error</returns>
        private static Segment ParseSegment(Stream data, uint segmentSize)
        {
            // TODO: Use marshalling here instead of building
            Segment segment = new Segment();

            segment.Deleted = data.ReadUInt32();
            segment.FileLocation = data.ReadUInt32();
            segment.FileSize = data.ReadUInt32();
            segment.PackedDate = data.ReadUInt32();
            byte[] fileName = data.ReadBytes(0x10);
            segment.FileName = Encoding.ASCII.GetString(fileName).TrimEnd('\0');
            if (segmentSize > Version2SegmentSize)
                segment.ModifiedDate = data.ReadUInt32();
            if (segmentSize > Version3SegmentSize)
                segment.CompressionLevel = data.ReadUInt32();

            return segment;
        }

        #endregion
    }
}
