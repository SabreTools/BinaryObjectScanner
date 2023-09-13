using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static SabreTools.Models.BSP.Constants;

namespace BinaryObjectScanner.Wrappers
{
    public class BSP : WrapperBase<SabreTools.Models.BSP.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Level (BSP)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.BSP.Header.Version"/>
#if NET48
        public uint Version => _model.Header.Version;
#else
        public uint? Version => _model.Header?.Version;
#endif

        #endregion

        #region Lumps

        /// <inheritdoc cref="Models.BSP.File.Lumps"/>
#if NET48
        public SabreTools.Models.BSP.Lump[] Lumps => _model.Lumps;
#else
        public SabreTools.Models.BSP.Lump?[]? Lumps => _model.Lumps;
#endif

        #endregion

        #region Texture Header

        /// <inheritdoc cref="Models.BSP.TextureHeader.TextureCount"/>
#if NET48
        public uint TextureCount => _model.TextureHeader.TextureCount;
#else
        public uint? TextureCount => _model.TextureHeader?.TextureCount;
#endif

        /// <inheritdoc cref="Models.BSP.TextureHeader.Offsets"/>
#if NET48
        public uint[] Offsets => _model.TextureHeader.Offsets;
#else
        public uint[]? Offsets => _model.TextureHeader?.Offsets;
#endif

        #endregion

        #region Textures

        /// <inheritdoc cref="Models.BSP.File.Textures"/>
#if NET48
        public SabreTools.Models.BSP.Texture[] Textures => _model.Textures;
#else
        public SabreTools.Models.BSP.Texture?[]? Textures => _model.Textures;
#endif

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public BSP(SabreTools.Models.BSP.File model, byte[] data, int offset)
#else
        public BSP(SabreTools.Models.BSP.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public BSP(SabreTools.Models.BSP.File model, Stream data)
#else
        public BSP(SabreTools.Models.BSP.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a BSP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the BSP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BSP wrapper on success, null on failure</returns>
#if NET48
        public static BSP Create(byte[] data, int offset)
#else
        public static BSP? Create(byte[]? data, int offset)
#endif
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a BSP from a Stream
        /// </summary>
        /// <param name="data">Stream representing the BSP</param>
        /// <returns>An BSP wrapper on success, null on failure</returns>
#if NET48
        public static BSP Create(Stream data)
#else
        public static BSP? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.BSP().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new BSP(file, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("BSP Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintHeader(builder);
            PrintLumps(builder);
            PrintTextureHeader(builder);
            PrintTextures(builder);

            return builder;
        }

        /// <summary>
        /// Print header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintHeader(StringBuilder builder)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Version: {Version} (0x{Version:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print lumps information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintLumps(StringBuilder builder)
        {
            builder.AppendLine("  Lumps Information:");
            builder.AppendLine("  -------------------------");
            if (Lumps == null || Lumps.Length == 0)
            {
                builder.AppendLine("  No lumps");
            }
            else
            {
                for (int i = 0; i < Lumps.Length; i++)
                {
                    var lump = Lumps[i];
                    string specialLumpName = string.Empty;
                    switch (i)
                    {
                        case HL_BSP_LUMP_ENTITIES:
                            specialLumpName = " (entities)";
                            break;
                        case HL_BSP_LUMP_TEXTUREDATA:
                            specialLumpName = " (texture data)";
                            break;
                    }

                    builder.AppendLine($"  Lump {i}{specialLumpName}");
                    if (lump == null)
                    {
                        builder.AppendLine("    [NULL]");
                    }
                    else
                    {
                        builder.AppendLine($"    Offset: {lump.Offset} (0x{lump.Offset:X})");
                        builder.AppendLine($"    Length: {lump.Length} (0x{lump.Length:X})");
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print texture header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintTextureHeader(StringBuilder builder)
        {
            builder.AppendLine("  Texture Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Texture count: {TextureCount}");
            builder.AppendLine($"  Offsets:");
            if (Offsets == null || Offsets.Length == 0)
            {
                builder.AppendLine("  No offsets");
            }
            else
            {
                for (int i = 0; i < Offsets.Length; i++)
                {
                    builder.AppendLine($"    Offset {i}: {Offsets[i]} (0x{Offsets[i]:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print textures information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintTextures(StringBuilder builder)
        {
            builder.AppendLine("  Textures Information:");
            builder.AppendLine("  -------------------------");
            if (Textures == null || Textures.Length == 0)
            {
                builder.AppendLine("  No textures");
            }
            else
            {
                for (int i = 0; i < Textures.Length; i++)
                {
                    var texture = Textures[i];
                    builder.AppendLine($"  Texture {i}");
                    if (texture == null)
                    {
                        builder.AppendLine("    [NULL]");
                    }
                    else
                    {
                        builder.AppendLine($"    Name: {texture.Name}");
                        builder.AppendLine($"    Width: {texture.Width} (0x{texture.Width:X})");
                        builder.AppendLine($"    Height: {texture.Height} (0x{texture.Height:X})");
                        builder.AppendLine($"    Offsets:");
                        if (texture.Offsets == null || texture.Offsets.Length == 0)
                        {
                            builder.AppendLine($"    No offsets");
                        }
                        else
                        {
                            for (int j = 0; j < texture.Offsets.Length; j++)
                            {
                                builder.AppendLine($"      Offset {j}: {texture.Offsets[i]} (0x{texture.Offsets[j]:X})");
                            }
                        }
                        // Skip texture data
                        builder.AppendLine($"    Palette size: {texture.PaletteSize} (0x{texture.PaletteSize:X})");
                        // Skip palette data
                    }
                }
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all lumps from the BSP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all lumps extracted, false otherwise</returns>
        public bool ExtractAllLumps(string outputDirectory)
        {
            // If we have no lumps
            if (Lumps == null || Lumps.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < Lumps.Length; i++)
            {
                allExtracted &= ExtractLump(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a lump from the BSP to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the lump extracted, false otherwise</returns>
        public bool ExtractLump(int index, string outputDirectory)
        {
            // If we have no lumps
            if (Lumps == null || Lumps.Length == 0)
                return false;

            // If the lumps index is invalid
            if (index < 0 || index >= Lumps.Length)
                return false;

            // Get the lump
            var lump = Lumps[index];
            if (lump == null)
                return false;

            // Read the data
            byte[] data = ReadFromDataSource((int)lump.Offset, (int)lump.Length);
            if (data == null)
                return false;

            // Create the filename
            string filename = $"lump_{index}.bin";
            switch (index)
            {
                case HL_BSP_LUMP_ENTITIES:
                    filename = "entities.ent";
                    break;
                case HL_BSP_LUMP_TEXTUREDATA:
                    filename = "texture_data.bin";
                    break;
            }

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
#if NET48
            string directoryName = Path.GetDirectoryName(filename);
#else
            string? directoryName = Path.GetDirectoryName(filename);
#endif
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
        public bool ExtractAllTextures(string outputDirectory)
        {
            // If we have no textures
            if (Offsets == null || Offsets.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < Offsets.Length; i++)
            {
                allExtracted &= ExtractTexture(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a texture from the BSP to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the texture extracted, false otherwise</returns>
        public bool ExtractTexture(int index, string outputDirectory)
        {
            // If we have no textures
            if (Textures == null || Textures.Length == 0)
                return false;

            // If the texture index is invalid
            if (index < 0 || index >= Textures.Length)
                return false;

            // Get the texture
            var texture = Textures[index];
            if (texture == null)
                return false;

            // Read the data
            byte[] data = CreateTextureData(texture);
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
#if NET48
            string directoryName = Path.GetDirectoryName(filename);
#else
            string? directoryName = Path.GetDirectoryName(filename);
#endif
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

        #endregion
    }
}