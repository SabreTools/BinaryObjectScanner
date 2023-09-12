using System.IO;
using System.Text;
using static SabreTools.Models.VBSP.Constants;

namespace BinaryObjectScanner.Wrappers
{
    public class VBSP : WrapperBase<SabreTools.Models.VBSP.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life 2 Level (VBSP)";

        #endregion

        #region Pass-Through Properties

        /// <inheritdoc cref="Models.VBSP.Header.Signature"/>
#if NET48
        public string Signature => _model.Header.Signature;
#else
        public string? Signature => _model.Header.Signature;
#endif

        /// <inheritdoc cref="Models.VBSP.Header.Version"/>
        public int Version => _model.Header.Version;

        /// <inheritdoc cref="Models.VBSP.File.Lumps"/>
#if NET48
        public SabreTools.Models.VBSP.Lump[] Lumps => _model.Header.Lumps;
#else
        public SabreTools.Models.VBSP.Lump?[]? Lumps => _model.Header.Lumps;
#endif

        /// <inheritdoc cref="Models.VBSP.Header.MapRevision"/>
        public int MapRevision => _model.Header.MapRevision;

        #endregion

        #region Extension Properties

        // TODO: Figure out what extension oroperties are needed

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public VBSP(SabreTools.Models.VBSP.File model, byte[] data, int offset)
#else
        public VBSP(SabreTools.Models.VBSP.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public VBSP(SabreTools.Models.VBSP.File model, Stream data)
#else
        public VBSP(SabreTools.Models.VBSP.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a VBSP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the VBSP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A VBSP wrapper on success, null on failure</returns>
#if NET48
        public static VBSP Create(byte[] data, int offset)
#else
        public static VBSP? Create(byte[]? data, int offset)
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
        /// Create a VBSP from a Stream
        /// </summary>
        /// <param name="data">Stream representing the VBSP</param>
        /// <returns>An VBSP wrapper on success, null on failure</returns>
#if NET48
        public static VBSP Create(Stream data)
#else
        public static VBSP? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.VBSP().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new VBSP(file, data);
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

            builder.AppendLine("VBSP Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintHeader(builder);
            PrintLumps(builder);

            return builder;
        }

        /// <summary>
        /// Print header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        /// <remarks>Slightly out of order due to the lumps</remarks>
        private void PrintHeader(StringBuilder builder)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Signature: {Signature}");
            builder.AppendLine($"  Version: {Version} (0x{Version:X})");
            builder.AppendLine($"  Map revision: {MapRevision} (0x{MapRevision:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print lumps information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        /// <remarks>Technically part of the header</remarks>
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
                        case HL_VBSP_LUMP_ENTITIES:
                            specialLumpName = " (entities)";
                            break;
                        case HL_VBSP_LUMP_PAKFILE:
                            specialLumpName = " (pakfile)";
                            break;
                    }

                    builder.AppendLine($"  Lump {i}{specialLumpName}");
                    builder.AppendLine($"    Offset: {lump.Offset} (0x{lump.Offset:X})");
                    builder.AppendLine($"    Length: {lump.Length} (0x{lump.Length:X})");
                    builder.AppendLine($"    Version: {lump.Version} (0x{lump.Version:X})");
                    builder.AppendLine($"    4CC: {(lump.FourCC == null ? "[NULL]" : string.Join(", ", lump.FourCC))}");
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
        /// Extract all lumps from the VBSP to an output directory
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
        /// Extract a lump from the VBSP to an output directory by index
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
#if NET48
            byte[] data = ReadFromDataSource((int)lump.Offset, (int)lump.Length);
#else
            byte[]? data = ReadFromDataSource((int)lump.Offset, (int)lump.Length);
#endif
            if (data == null)
                return false;

            // Create the filename
            string filename = $"lump_{index}.bin";
            switch (index)
            {
                case HL_VBSP_LUMP_ENTITIES:
                    filename = "entities.ent";
                    break;
                case HL_VBSP_LUMP_PAKFILE:
                    filename = "pakfile.zip";
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

        #endregion
    }
}