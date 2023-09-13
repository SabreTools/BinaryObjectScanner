using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class WAD : WrapperBase<SabreTools.Models.WAD.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Texture Package File (WAD)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.WAD.Header.Signature"/>
#if NET48
        public string Signature => _model.Header.Signature;
#else
        public string? Signature => _model.Header?.Signature;
#endif

        /// <inheritdoc cref="Models.WAD.Header.LumpCount"/>
#if NET48
        public uint LumpCount => _model.Header.LumpCount;
#else
        public uint? LumpCount => _model.Header?.LumpCount;
#endif

        /// <inheritdoc cref="Models.WAD.Header.LumpOffset"/>
#if NET48
        public uint LumpOffset => _model.Header.LumpOffset;
#else
        public uint? LumpOffset => _model.Header?.LumpOffset;
#endif

        #endregion

        #region Lumps

        /// <inheritdoc cref="Models.WAD.File.Lumps"/>
#if NET48
        public SabreTools.Models.WAD.Lump[] Lumps => _model.Lumps;
#else
        public SabreTools.Models.WAD.Lump?[]? Lumps => _model.Lumps;
#endif

        #endregion

        #region Lump Infos

        /// <inheritdoc cref="Models.WAD.File.LumpInfos"/>
#if NET48
        public SabreTools.Models.WAD.LumpInfo[] LumpInfos => _model.LumpInfos;
#else
        public SabreTools.Models.WAD.LumpInfo?[]? LumpInfos => _model.LumpInfos;
#endif

        #endregion

        #endregion

        #region Extension Properties

        // TODO: Figure out what extension oroperties are needed

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public WAD(SabreTools.Models.WAD.File model, byte[] data, int offset)
#else
        public WAD(SabreTools.Models.WAD.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public WAD(SabreTools.Models.WAD.File model, Stream data)
#else
        public WAD(SabreTools.Models.WAD.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a WAD from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the WAD</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A WAD wrapper on success, null on failure</returns>
#if NET48
        public static WAD Create(byte[] data, int offset)
#else
        public static WAD? Create(byte[]? data, int offset)
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
        /// Create a WAD from a Stream
        /// </summary>
        /// <param name="data">Stream representing the WAD</param>
        /// <returns>An WAD wrapper on success, null on failure</returns>
#if NET48
        public static WAD Create(Stream data)
#else
        public static WAD? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.WAD().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new WAD(file, data);
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

            builder.AppendLine("WAD Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintHeader(builder);
            PrintLumps(builder);
            PrintLumpInfos(builder);

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
            builder.AppendLine($"  Signature: {Signature}");
            builder.AppendLine($"  Lump count: {LumpCount} (0x{LumpCount:X})");
            builder.AppendLine($"  Lump offset: {LumpOffset} (0x{LumpOffset:X})");
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
                    builder.AppendLine($"  Lump {i}");
                    if (lump == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine($"    Offset: {lump.Offset} (0x{lump.Offset:X})");
                    builder.AppendLine($"    Disk length: {lump.DiskLength} (0x{lump.DiskLength:X})");
                    builder.AppendLine($"    Length: {lump.Length} (0x{lump.Length:X})");
                    builder.AppendLine($"    Type: {lump.Type} (0x{lump.Type:X})");
                    builder.AppendLine($"    Compression: {lump.Compression} (0x{lump.Compression:X})");
                    builder.AppendLine($"    Padding 0: {lump.Padding0} (0x{lump.Padding0:X})");
                    builder.AppendLine($"    Padding 1: {lump.Padding1} (0x{lump.Padding1:X})");
                    builder.AppendLine($"    Name: {lump.Name ?? "[NULL]"}");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print lump infos information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintLumpInfos(StringBuilder builder)
        {
            builder.AppendLine("  Lump Infos Information:");
            builder.AppendLine("  -------------------------");
            if (LumpInfos == null || LumpInfos.Length == 0)
            {
                builder.AppendLine("  No lump infos");
            }
            else
            {
                for (int i = 0; i < LumpInfos.Length; i++)
                {
                    var lumpInfo = LumpInfos[i];
                    builder.AppendLine($"  Lump Info {i}");
                    if (lumpInfo == null)
                    {
                        builder.AppendLine("    Lump is compressed");
                    }
                    else
                    {
                        builder.AppendLine($"    Name: {lumpInfo.Name ?? "[NULL]"}");
                        builder.AppendLine($"    Width: {lumpInfo.Width} (0x{lumpInfo.Width:X})");
                        builder.AppendLine($"    Height: {lumpInfo.Height} (0x{lumpInfo.Height:X})");
                        builder.AppendLine($"    Pixel offset: {lumpInfo.PixelOffset} (0x{lumpInfo.PixelOffset:X})");
                        // TODO: Print unknown data?
                        // TODO: Print pixel data?
                        builder.AppendLine($"    Palette size: {lumpInfo.PaletteSize} (0x{lumpInfo.PaletteSize:X})");
                        // TODO: Print palette data?
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
        /// Extract all lumps from the WAD to an output directory
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
        /// Extract a lump from the WAD to an output directory by index
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

            // Read the data -- TODO: Handle uncompressed lumps (see BSP.ExtractTexture)
#if NET48
            byte[] data = ReadFromDataSource((int)lump.Offset, (int)lump.Length);
#else
            byte[]? data = ReadFromDataSource((int)lump.Offset, (int)lump.Length);
#endif
            if (data == null)
                return false;

            // Create the filename
            string filename = $"{lump.Name}.lmp";

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

        #endregion
    }
}