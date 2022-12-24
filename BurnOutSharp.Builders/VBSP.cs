using System.IO;
using System.Text;
using BurnOutSharp.Models.VBSP;
using BurnOutSharp.Utilities;

namespace BurnOutSharp.Builders
{
    public static class VBSP
    {
        #region Constants

        /// <summary>
        /// Total number of lumps in the package
        /// </summary>
        public const int HL_VBSP_LUMP_COUNT = 64;

        /// <summary>
        /// Index for the entities lump
        /// </summary>
        public const int HL_VBSP_LUMP_ENTITIES = 0;

        /// <summary>
        /// Idnex for the pakfile lump
        /// </summary>
        public const int HL_VBSP_LUMP_PAKFILE = 40;

        /// <summary>
        /// Zip local file header signature as an integer
        /// </summary>
        public const int HL_VBSP_ZIP_LOCAL_FILE_HEADER_SIGNATURE = 0x04034b50;

        /// <summary>
        /// Zip file header signature as an integer
        /// </summary>
        public const int HL_VBSP_ZIP_FILE_HEADER_SIGNATURE = 0x02014b50;

        /// <summary>
        /// Zip end of central directory record signature as an integer
        /// </summary>
        public const int HL_VBSP_ZIP_END_OF_CENTRAL_DIRECTORY_RECORD_SIGNATURE = 0x06054b50;

        /// <summary>
        /// Length of a ZIP checksum in bytes
        /// </summary>
        public const int HL_VBSP_ZIP_CHECKSUM_LENGTH = 0x00008000;

        #endregion

        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Half-Life 2 Level
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled Half-Life 2 Level on success, null on error</returns>
        public static Models.VBSP.File ParseFile(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseFile(dataStream);
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a Half-Life 2 Level
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life 2 Level on success, null on error</returns>
        public static Models.VBSP.File ParseFile(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            long initialOffset = data.Position;

            // Create a new Half-Life 2 Level to fill
            var file = new Models.VBSP.File();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the package header
            file.Header = header;

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life 2 Level header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life 2 Level header on success, null on error</returns>
        private static Header ParseHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            Header header = new Header();

            byte[] signature = data.ReadBytes(4);
            header.Signature = Encoding.ASCII.GetString(signature);
            if (header.Signature != "VBSP")
                return null;

            header.Version = data.ReadInt32();
            if ((header.Version < 19 || header.Version > 22) && header.Version != 0x00040014)
                return null;

            header.Lumps = new Lump[HL_VBSP_LUMP_COUNT];
            for (int i = 0; i < HL_VBSP_LUMP_COUNT; i++)
            {
                header.Lumps[i] = ParseLump(data, header.Version);
            }

            header.MapRevision = data.ReadInt32();

            return header;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life 2 Level lump
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="version">VBSP version</param>
        /// <returns>Filled Half-Life 2 Level lump on success, null on error</returns>
        private static Lump ParseLump(Stream data, int version)
        {
            // TODO: Use marshalling here instead of building
            Lump lump = new Lump();

            lump.Offset = data.ReadUInt32();
            lump.Length = data.ReadUInt32();
            lump.Version = data.ReadUInt32();
            lump.FourCC = new char[4];
            for (int i = 0; i < 4; i++)
            {
                lump.FourCC[i] = (char)data.ReadByte();
            }

            // This block was commented out because test VBSPs with header
            // version 21 had the values in the "right" order already and
            // were causing decompression issues

            //if (version >= 21 && version != 0x00040014)
            //{
            //    uint temp = lump.Version;
            //    lump.Version = lump.Offset;
            //    lump.Offset = lump.Length;
            //    lump.Length = temp;
            //}

            return lump;
        }

        #endregion
    }
}
