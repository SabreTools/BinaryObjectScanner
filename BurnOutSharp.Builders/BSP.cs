using System.IO;
using BurnOutSharp.Models.BSP;
using BurnOutSharp.Utilities;

namespace BurnOutSharp.Builders
{
    public static class BSP
    {
        #region Constants

        /// <summary>
        /// Number of lumps in a BSP
        /// </summary>
        private const int HL_BSP_LUMP_COUNT = 15;

        /// <summary>
        /// Index for the entities lump
        /// </summary>
        public const int HL_BSP_LUMP_ENTITIES = 0;

        /// <summary>
        /// Index for the texture data lump
        /// </summary>
        public const int HL_BSP_LUMP_TEXTUREDATA = 2;

        /// <summary>
        /// Number of valid mipmap levels
        /// </summary>
        public const int HL_BSP_MIPMAP_COUNT = 4;

        #endregion

        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Half-Life Level
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled Half-Life Level on success, null on error</returns>
        public static Models.BSP.File ParseFile(byte[] data, int offset)
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
        /// Parse a Stream into a Half-Life Level
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Level on success, null on error</returns>
        public static Models.BSP.File ParseFile(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new Half-Life Level to fill
            var file = new Models.BSP.File();

            #region Header

            // Try to parse the header
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the level header
            file.Header = header;

            #endregion

            #region Lumps

            // Create the lump array
            file.Lumps = new Lump[HL_BSP_LUMP_COUNT];

            // Try to parse the lumps
            for (int i = 0; i < HL_BSP_LUMP_COUNT; i++)
            {
                var lump = ParseLump(data);
                file.Lumps[i] = lump;
            }

            #endregion

            #region Texture header

            // Try to get the texture header lump
            var textureDataLump = file.Lumps[HL_BSP_LUMP_TEXTUREDATA];
            if (textureDataLump.Offset == 0 || textureDataLump.Length == 0)
                return null;

            // Seek to the texture header
            data.Seek(textureDataLump.Offset, SeekOrigin.Begin);

            // Try to parse the texture header
            var textureHeader = ParseTextureHeader(data);
            if (textureHeader == null)
                return null;

            // Set the texture header
            file.TextureHeader = textureHeader;

            #endregion

            return file;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Level header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Level header on success, null on error</returns>
        private static Header ParseHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            Header header = new Header();

            // Only recognized versions are 29 and 30
            header.Version = data.ReadUInt32();
            if (header.Version != 29 && header.Version != 30)
                return null;

            return header;
        }

        /// <summary>
        /// Parse a Stream into a lump
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled lump on success, null on error</returns>
        private static Lump ParseLump(Stream data)
        {
            // TODO: Use marshalling here instead of building
            Lump lump = new Lump();

            lump.Offset = data.ReadUInt32();
            lump.Length = data.ReadUInt32();

            return lump;
        }

        /// <summary>
        /// Parse a Stream into a Half-Life Level texture header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Half-Life Level texture header on success, null on error</returns>
        private static TextureHeader ParseTextureHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            TextureHeader textureHeader = new TextureHeader();

            textureHeader.TextureCount = data.ReadUInt32();
            
            var offsets = new uint[textureHeader.TextureCount];

            for (int i = 0; i < textureHeader.TextureCount; i++)
            {
                offsets[i] = data.ReadUInt32();
            }

            textureHeader.Offsets = offsets;

            return textureHeader;
        }

        #endregion
    }
}
