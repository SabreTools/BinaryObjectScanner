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

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.XZP.Header.Signature"/>
#if NET48
        public string Signature => this.Model.Header.Signature;
#else
        public string? Signature => this.Model.Header?.Signature;
#endif

        /// <inheritdoc cref="Models.XZP.Header.Version"/>
#if NET48
        public uint Version => this.Model.Header.Version;
#else
        public uint? Version => this.Model.Header?.Version;
#endif

        /// <inheritdoc cref="Models.XZP.Header.PreloadDirectoryEntryCount"/>
#if NET48
        public uint PreloadDirectoryEntryCount => this.Model.Header.PreloadDirectoryEntryCount;
#else
        public uint? PreloadDirectoryEntryCount => this.Model.Header?.PreloadDirectoryEntryCount;
#endif

        /// <inheritdoc cref="Models.XZP.Header.DirectoryEntryCount"/>
#if NET48
        public uint DirectoryEntryCount => this.Model.Header.DirectoryEntryCount;
#else
        public uint? DirectoryEntryCount => this.Model.Header?.DirectoryEntryCount;
#endif

        /// <inheritdoc cref="Models.XZP.Header.PreloadBytes"/>
#if NET48
        public uint PreloadBytes => this.Model.Header.PreloadBytes;
#else
        public uint? PreloadBytes => this.Model.Header?.PreloadBytes;
#endif

        /// <inheritdoc cref="Models.XZP.Header.HeaderLength"/>
#if NET48
        public uint HeaderLength => this.Model.Header.HeaderLength;
#else
        public uint? HeaderLength => this.Model.Header?.HeaderLength;
#endif

        /// <inheritdoc cref="Models.XZP.Header.DirectoryItemCount"/>
#if NET48
        public uint DirectoryItemCount => this.Model.Header.DirectoryItemCount;
#else
        public uint? DirectoryItemCount => this.Model.Header?.DirectoryItemCount;
#endif

        /// <inheritdoc cref="Models.XZP.Header.DirectoryItemOffset"/>
#if NET48
        public uint DirectoryItemOffset => this.Model.Header.DirectoryItemOffset;
#else
        public uint? DirectoryItemOffset => this.Model.Header?.DirectoryItemOffset;
#endif

        /// <inheritdoc cref="Models.XZP.Header.DirectoryItemLength"/>
#if NET48
        public uint DirectoryItemLength => this.Model.Header.DirectoryItemLength;
#else
        public uint? DirectoryItemLength => this.Model.Header?.DirectoryItemLength;
#endif

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.XZP.DirectoryEntries"/>
#if NET48
        public SabreTools.Models.XZP.DirectoryEntry[] DirectoryEntries => this.Model.DirectoryEntries;
#else
        public SabreTools.Models.XZP.DirectoryEntry?[]? DirectoryEntries => this.Model.DirectoryEntries;
#endif

        #endregion

        #region Preload Directory Entries

        /// <inheritdoc cref="Models.XZP.PreloadDirectoryEntries"/>
#if NET48
        public SabreTools.Models.XZP.DirectoryEntry[] PreloadDirectoryEntries => this.Model.PreloadDirectoryEntries;
#else
        public SabreTools.Models.XZP.DirectoryEntry?[]? PreloadDirectoryEntries => this.Model.PreloadDirectoryEntries;
#endif

        #endregion

        #region Preload Directory Entries

        /// <inheritdoc cref="Models.XZP.PreloadDirectoryMappings"/>
#if NET48
        public SabreTools.Models.XZP.DirectoryMapping[] PreloadDirectoryMappings => this.Model.PreloadDirectoryMappings;
#else
        public SabreTools.Models.XZP.DirectoryMapping?[]? PreloadDirectoryMappings => this.Model.PreloadDirectoryMappings;
#endif

        #endregion

        #region Directory Items

        /// <inheritdoc cref="Models.XZP.DirectoryItems"/>
#if NET48
        public SabreTools.Models.XZP.DirectoryItem[] DirectoryItems => this.Model.DirectoryItems;
#else
        public SabreTools.Models.XZP.DirectoryItem?[]? DirectoryItems => this.Model.DirectoryItems;
#endif

        #endregion

        #region Footer

        /// <inheritdoc cref="Models.XZP.Footer.FileLength"/>
#if NET48
        public uint F_FileLength => this.Model.Footer.FileLength;
#else
        public uint? F_FileLength => this.Model.Footer?.FileLength;
#endif

        /// <inheritdoc cref="Models.XZP.Footer.Signature"/>
#if NET48
        public string F_Signature => this.Model.Footer.Signature;
#else
        public string? F_Signature => this.Model.Footer?.Signature;
#endif

        #endregion

        #endregion

        #region Extension Properties

        // TODO: Figure out what extensions are needed

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
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < DirectoryEntries.Length; i++)
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
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
                return false;

            // If we have no directory items
            if (DirectoryItems == null || DirectoryItems.Length == 0)
                return false;

            // If the directory entry index is invalid
            if (index < 0 || index >= DirectoryEntries.Length)
                return false;

            // Get the directory entry
            var directoryEntry = DirectoryEntries[index];
            if (directoryEntry == null)
                return false;

            // Get the associated directory item
            var directoryItem = DirectoryItems.Where(di => di?.FileNameCRC == directoryEntry.FileNameCRC).FirstOrDefault();
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