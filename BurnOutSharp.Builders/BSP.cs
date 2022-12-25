using System.IO;
using System.Linq;
using System.Text;
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

            #region Textures

            // Create the texture array
            file.Textures = new Texture[textureHeader.TextureCount];

            // Try to parse the textures
            for (int i = 0; i < textureHeader.TextureCount; i++)
            {
                // Get the texture offset
                int offset = (int)(textureHeader.Offsets[i] + file.Lumps[HL_BSP_LUMP_TEXTUREDATA].Offset);
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the texture
                data.Seek(offset, SeekOrigin.Begin);

                var texture = ParseTexture(data);
                file.Textures[i] = texture;
            }

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

        /// <summary>
        /// Parse a Stream into a texture
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="mipmap">Mipmap level</param>
        /// <returns>Filled texture on success, null on error</returns>
        private static Texture ParseTexture(Stream data, uint mipmap = 0)
        {
            // TODO: Use marshalling here instead of building
            Texture texture = new Texture();

            byte[] name = data.ReadBytes(16).TakeWhile(c => c != '\0').ToArray();
            texture.Name = Encoding.ASCII.GetString(name);
            texture.Width = data.ReadUInt32();
            texture.Height = data.ReadUInt32();
            texture.Offsets = new uint[4];
            for (int i = 0; i < 4; i++)
            {
                texture.Offsets[i] = data.ReadUInt32();
            }

            // Get the size of the pixel data
            uint pixelSize = 0;
            for (int i = 0; i < HL_BSP_MIPMAP_COUNT; i++)
            {
                if (texture.Offsets[i] != 0)
                {
                    pixelSize += (texture.Width >> i) * (texture.Height >> i);
                }
            }

            // If we have no pixel data
            if (pixelSize == 0)
                return texture;

            texture.TextureData = data.ReadBytes((int)pixelSize);
            texture.PaletteSize = data.ReadUInt16();
            texture.PaletteData = data.ReadBytes((int)(texture.PaletteSize * 3));

            // Adjust the dimensions based on mipmap level
            switch (mipmap)
            {
                case 1:
                    texture.Width /= 2;
                    texture.Height /= 2;
                    break;
                case 2:
                    texture.Width /= 4;
                    texture.Height /= 4;
                    break;
                case 3:
                    texture.Width /= 8;
                    texture.Height /= 8;
                    break;
            }

            return texture;
        }

        #endregion
    }
}
