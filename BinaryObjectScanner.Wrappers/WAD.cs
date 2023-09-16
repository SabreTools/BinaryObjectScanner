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
            Printing.WAD.Print(builder, this.Model);
            return builder;
        }

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
            if (this.Model.Lumps == null || this.Model.Lumps.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < this.Model.Lumps.Length; i++)
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
            if (this.Model.Lumps == null || this.Model.Lumps.Length == 0)
                return false;

            // If the lumps index is invalid
            if (index < 0 || index >= this.Model.Lumps.Length)
                return false;

            // Get the lump
            var lump = this.Model.Lumps[index];
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