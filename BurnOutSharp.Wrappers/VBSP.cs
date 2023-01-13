using System.IO;
using System.Text;
using static BurnOutSharp.Models.VBSP.Constants;

namespace BurnOutSharp.Wrappers
{
    public class VBSP : WrapperBase
    {
        #region Pass-Through Properties

        /// <inheritdoc cref="Models.VBSP.Header.Signature"/>
        public string Signature => _file.Header.Signature;

        /// <inheritdoc cref="Models.VBSP.Header.Version"/>
        public int Version => _file.Header.Version;

        /// <inheritdoc cref="Models.VBSP.File.Lumps"/>
        public Models.VBSP.Lump[] Lumps => _file.Header.Lumps;

        /// <inheritdoc cref="Models.VBSP.Header.MapRevision"/>
        public int MapRevision => _file.Header.MapRevision;

        #endregion

        #region Extension Properties

        // TODO: Figure out what extension oroperties are needed

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the VBSP
        /// </summary>
        private Models.VBSP.File _file;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private VBSP() { }

        /// <summary>
        /// Create a VBSP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the VBSP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A VBSP wrapper on success, null on failure</returns>
        public static VBSP Create(byte[] data, int offset)
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
        public static VBSP Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = Builders.VBSP.ParseFile(data);
            if (file == null)
                return null;

            var wrapper = new VBSP
            {
                _file = file,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
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
                    builder.AppendLine($"    4CC: {string.Join(", ", lump.FourCC)}");
                }
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_file, _jsonSerializerOptions);

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
            byte[] data = ReadFromDataSource((int)lump.Offset, (int)lump.Length);
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
            Directory.CreateDirectory(Path.GetDirectoryName(filename));

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