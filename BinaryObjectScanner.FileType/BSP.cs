using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Level
    /// </summary>
    public class BSP : IExtractable
    {
        /// <inheritdoc/>
#if NET48
        public string Extract(string file, bool includeDebug)
#else
        public string? Extract(string file, bool includeDebug)
#endif
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(Stream stream, string file, bool includeDebug)
#else
        public string? Extract(Stream stream, string file, bool includeDebug)
#endif
        {
            try
            {
                // Create the wrapper
                var bsp = SabreTools.Serialization.Wrappers.BSP.Create(stream);
                if (bsp == null)
                    return null;

                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Loop through and extract all files
                ExtractAllLumps(bsp, tempPath);
                ExtractAllTextures(bsp, tempPath);

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Extract all lumps from the BSP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all lumps extracted, false otherwise</returns>
        public static bool ExtractAllLumps(SabreTools.Serialization.Wrappers.BSP item, string outputDirectory)
        {
            // If we have no lumps
            if (item.Model.Lumps == null || item.Model.Lumps.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.Lumps.Length; i++)
            {
                allExtracted &= ExtractLump(item, i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a lump from the BSP to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the lump extracted, false otherwise</returns>
        public static bool ExtractLump(SabreTools.Serialization.Wrappers.BSP item, int index, string outputDirectory)
        {
            // If we have no lumps
            if (item.Model.Lumps == null || item.Model.Lumps.Length == 0)
                return false;

            // If the lumps index is invalid
            if (index < 0 || index >= item.Model.Lumps.Length)
                return false;

            // Get the lump
            var lump = item.Model.Lumps[index];
            if (lump == null)
                return false;

            // Read the data
            var data = item.ReadFromDataSource((int)lump.Offset, (int)lump.Length);
            if (data == null)
                return false;

            // Create the filename
            string filename = $"lump_{index}.bin";
            switch (index)
            {
                case SabreTools.Models.BSP.Constants.HL_BSP_LUMP_ENTITIES:
                    filename = "entities.ent";
                    break;
                case SabreTools.Models.BSP.Constants.HL_BSP_LUMP_TEXTUREDATA:
                    filename = "texture_data.bin";
                    break;
            }

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Extract all textures from the BSP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all textures extracted, false otherwise</returns>
        public static bool ExtractAllTextures(SabreTools.Serialization.Wrappers.BSP item, string outputDirectory)
        {
            // If we have no textures
            if (item.Model.TextureHeader?.Offsets == null || item.Model.TextureHeader.Offsets.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.TextureHeader.Offsets.Length; i++)
            {
                allExtracted &= ExtractTexture(item, i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a texture from the BSP to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the texture extracted, false otherwise</returns>
        public static bool ExtractTexture(SabreTools.Serialization.Wrappers.BSP item, int index, string outputDirectory)
        {
            // If we have no textures
            if (item.Model.Textures == null || item.Model.Textures.Length == 0)
                return false;

            // If the texture index is invalid
            if (index < 0 || index >= item.Model.Textures.Length)
                return false;

            // Get the texture
            var texture = item.Model.Textures[index];
            if (texture == null)
                return false;

            // Read the data
            var data = CreateTextureData(texture);
            if (data == null)
                return false;

            // Create the filename
            string filename = $"{texture.Name}.bmp";

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create a bitmap from the texture and palette data
        /// </summary>
        /// <param name="texture">Texture object to format</param>
        /// <returns>Byte array representing the texture as a bitmap</returns>
#if NET48
        private static byte[] CreateTextureData(SabreTools.Models.BSP.Texture texture)
#else
        private static byte[]? CreateTextureData(SabreTools.Models.BSP.Texture texture)
#endif
        {
            // If there's no palette data
            if (texture.PaletteData == null || texture.PaletteData.Length == 0)
                return null;

            // If there's no texture data
            if (texture.TextureData == null || texture.TextureData.Length == 0)
                return null;

            // Create the bitmap file header
            var fileHeader = new SabreTools.Models.BMP.BITMAPFILEHEADER()
            {
                Type = ('M' << 8) | 'B',
                Size = 14 + 40 + (texture.PaletteSize * 4) + (texture.Width * texture.Height),
                OffBits = 14 + 40 + (texture.PaletteSize * 4),
            };

            // Create the bitmap info header
            var infoHeader = new SabreTools.Models.BMP.BITMAPINFOHEADER
            {
                Size = 40,
                Width = (int)texture.Width,
                Height = (int)texture.Height,
                Planes = 1,
                BitCount = 8,
                SizeImage = 0,
                ClrUsed = texture.PaletteSize,
                ClrImportant = texture.PaletteSize,
            };

            // Reformat the palette data
            byte[] paletteData = new byte[texture.PaletteSize * 4];
            for (uint i = 0; i < texture.PaletteSize; i++)
            {
                paletteData[i * 4 + 0] = texture.PaletteData[i * 3 + 2];
                paletteData[i * 4 + 1] = texture.PaletteData[i * 3 + 1];
                paletteData[i * 4 + 2] = texture.PaletteData[i * 3 + 0];
                paletteData[i * 4 + 3] = 0;
            }

            // Reformat the pixel data
            byte[] pixelData = new byte[texture.Width * texture.Height];
            for (uint i = 0; i < texture.Width; i++)
            {
                for (uint j = 0; j < texture.Height; j++)
                {
                    pixelData[i + ((texture.Height - 1 - j) * texture.Width)] = texture.TextureData[i + j * texture.Width];
                }
            }

            // Build the file data
            List<byte> buffer = new List<byte>();

            // Bitmap file header
            buffer.AddRange(BitConverter.GetBytes(fileHeader.Type));
            buffer.AddRange(BitConverter.GetBytes(fileHeader.Size));
            buffer.AddRange(BitConverter.GetBytes(fileHeader.Reserved1));
            buffer.AddRange(BitConverter.GetBytes(fileHeader.Reserved2));
            buffer.AddRange(BitConverter.GetBytes(fileHeader.OffBits));

            // Bitmap info header
            buffer.AddRange(BitConverter.GetBytes(infoHeader.Size));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.Width));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.Height));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.Planes));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.BitCount));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.Compression));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.SizeImage));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.XPelsPerMeter));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.YPelsPerMeter));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.ClrUsed));
            buffer.AddRange(BitConverter.GetBytes(infoHeader.ClrImportant));

            // Palette data
            buffer.AddRange(paletteData);

            // Pixel data
            buffer.AddRange(pixelData);

            return buffer.ToArray();
        }
    }
}
