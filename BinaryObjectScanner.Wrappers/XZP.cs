using System.IO;
using System.Linq;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class XZP : WrapperBase<SabreTools.Models.XZP.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Xbox Package File (XZP)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public XZP(SabreTools.Models.XZP.File model, byte[] data, int offset)
#else
        public XZP(SabreTools.Models.XZP.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public XZP(SabreTools.Models.XZP.File model, Stream data)
#else
        public XZP(SabreTools.Models.XZP.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a XZP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the XZP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A XZP wrapper on success, null on failure</returns>
#if NET48
        public static XZP Create(byte[] data, int offset)
#else
        public static XZP? Create(byte[]? data, int offset)
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
        /// Create a XZP from a Stream
        /// </summary>
        /// <param name="data">Stream representing the XZP</param>
        /// <returns>A XZP wrapper on success, null on failure</returns>
#if NET48
        public static XZP Create(Stream data)
#else
        public static XZP? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.XZP().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new XZP(file, data);
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
            Printing.XZP.Print(builder, this.Model);
            return builder;
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the XZP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no directory entries
            if (this.Model.DirectoryEntries == null || this.Model.DirectoryEntries.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < this.Model.DirectoryEntries.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the XZP to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no directory entries
            if (this.Model.DirectoryEntries == null || this.Model.DirectoryEntries.Length == 0)
                return false;

            // If we have no directory items
            if (this.Model.DirectoryItems == null || this.Model.DirectoryItems.Length == 0)
                return false;

            // If the directory entry index is invalid
            if (index < 0 || index >= this.Model.DirectoryEntries.Length)
                return false;

            // Get the directory entry
            var directoryEntry = this.Model.DirectoryEntries[index];
            if (directoryEntry == null)
                return false;

            // Get the associated directory item
            var directoryItem = this.Model.DirectoryItems.Where(di => di?.FileNameCRC == directoryEntry.FileNameCRC).FirstOrDefault();
            if (directoryItem == null)
                return false;

            // Load the item data
#if NET48
            byte[] data = ReadFromDataSource((int)directoryEntry.EntryOffset, (int)directoryEntry.EntryLength);
#else
            byte[]? data = ReadFromDataSource((int)directoryEntry.EntryOffset, (int)directoryEntry.EntryLength);
#endif
            if (data == null)
                return false;

            // Create the filename
#if NET48
            string filename = directoryItem.Name;
#else
            string? filename = directoryItem.Name;
#endif

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename ?? $"file{index}");

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