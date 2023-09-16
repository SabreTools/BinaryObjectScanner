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
            Printing.VBSP.Print(builder, this.Model);
            return builder;
        }

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
            if (this.Model.Header?.Lumps == null || this.Model.Header.Lumps.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < this.Model.Header.Lumps.Length; i++)
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
            if (this.Model.Header?.Lumps == null || this.Model.Header.Lumps.Length == 0)
                return false;

            // If the lumps index is invalid
            if (index < 0 || index >= this.Model.Header.Lumps.Length)
                return false;

            // Get the lump
            var lump = this.Model.Header.Lumps[index];
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