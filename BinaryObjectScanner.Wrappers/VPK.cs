using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO;
using static SabreTools.Models.VPK.Constants;

namespace BinaryObjectScanner.Wrappers
{
    public class VPK : WrapperBase<SabreTools.Models.VPK.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Valve Package File (VPK)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.VPK.Header.Signature"/>
#if NET48
        public uint Signature => this.Model.Header.Signature;
#else
        public uint? Signature => this.Model.Header?.Signature;
#endif

        /// <inheritdoc cref="Models.VPK.Header.Version"/>
#if NET48
        public uint Version => this.Model.Header.Version;
#else
        public uint? Version => this.Model.Header?.Version;
#endif

        /// <inheritdoc cref="Models.VPK.Header.DirectoryLength"/>
#if NET48
        public uint DirectoryLength => this.Model.Header.DirectoryLength;
#else
        public uint? DirectoryLength => this.Model.Header?.DirectoryLength;
#endif

        #endregion

        #region Extended Header

        /// <inheritdoc cref="Models.VPK.ExtendedHeader.Dummy0"/>
        public uint? Dummy0 => this.Model.ExtendedHeader?.Dummy0;

        /// <inheritdoc cref="Models.VPK.ExtendedHeader.ArchiveHashLength"/>
        public uint? ArchiveHashLength => this.Model.ExtendedHeader?.ArchiveHashLength;

        /// <inheritdoc cref="Models.VPK.ExtendedHeader.ExtraLength"/>
        public uint? ExtraLength => this.Model.ExtendedHeader?.ExtraLength;

        /// <inheritdoc cref="Models.VPK.ExtendedHeader.Dummy1"/>
        public uint? Dummy1 => this.Model.ExtendedHeader?.Dummy1;

        #endregion

        #region Archive Hashes

        /// <inheritdoc cref="Models.VPK.ArchiveHashes"/>
#if NET48
        public SabreTools.Models.VPK.ArchiveHash[] ArchiveHashes => this.Model.ArchiveHashes;
#else
        public SabreTools.Models.VPK.ArchiveHash?[]? ArchiveHashes => this.Model.ArchiveHashes;
#endif

        #endregion

        #region Directory Items

        /// <inheritdoc cref="Models.VPK.DirectoryItems"/>
#if NET48
        public SabreTools.Models.VPK.DirectoryItem[] DirectoryItems => this.Model.DirectoryItems;
#else
        public SabreTools.Models.VPK.DirectoryItem?[]? DirectoryItems => this.Model.DirectoryItems;
#endif

        #endregion

        #endregion

        #region Extension Properties

        /// <summary>
        /// Array of archive filenames attached to the given VPK
        /// </summary>
#if NET48
        public string[] ArchiveFilenames
#else
        public string[]? ArchiveFilenames
#endif
        {
            get
            {
                // Use the cached value if we have it
                if (_archiveFilenames != null)
                    return _archiveFilenames;

                // If we don't have a source filename
                if (!(_streamData is FileStream fs) || string.IsNullOrWhiteSpace(fs.Name))
                    return null;

                // If the filename is not the right format
                string extension = Path.GetExtension(fs.Name).TrimStart('.');
#if NET48
                string directoryName = Path.GetDirectoryName(fs.Name);
#else
                string? directoryName = Path.GetDirectoryName(fs.Name);
#endif
                string fileName = directoryName == null
                    ? Path.GetFileNameWithoutExtension(fs.Name)
                    : Path.Combine(directoryName, Path.GetFileNameWithoutExtension(fs.Name));

                if (fileName.Length < 3)
                    return null;
                else if (fileName.Substring(fileName.Length - 3) != "dir")
                    return null;

                // Get the archive count
                int archiveCount = DirectoryItems == null
                    ? 0
                    : DirectoryItems
                        .Select(di => di?.DirectoryEntry)
                        .Select(de => de?.ArchiveIndex ?? 0)
                        .Where(ai => ai != HL_VPK_NO_ARCHIVE)
                        .Max();

                // Build the list of archive filenames to populate
                _archiveFilenames = new string[archiveCount];

                // Loop through and create the archive filenames
                for (int i = 0; i < archiveCount; i++)
                {
                    // We need 5 digits to print a short, but we already have 3 for dir.
                    string archiveFileName = $"{fileName.Substring(0, fileName.Length - 3)}{i.ToString().PadLeft(3, '0')}.{extension}";
                    _archiveFilenames[i] = archiveFileName;
                }

                // Return the array
                return _archiveFilenames;
            }
        }

        #endregion

        #region Instance Variables

        /// <summary>
        /// Array of archive filenames attached to the given VPK
        /// </summary>
#if NET48
        private string[] _archiveFilenames = null;
#else
        private string[]? _archiveFilenames = null;
#endif

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public VPK(SabreTools.Models.VPK.File model, byte[] data, int offset)
#else
        public VPK(SabreTools.Models.VPK.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public VPK(SabreTools.Models.VPK.File model, Stream data)
#else
        public VPK(SabreTools.Models.VPK.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a VPK from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the VPK</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A VPK wrapper on success, null on failure</returns>
#if NET48
        public static VPK Create(byte[] data, int offset)
#else
        public static VPK? Create(byte[]? data, int offset)
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
        /// Create a VPK from a Stream
        /// </summary>
        /// <param name="data">Stream representing the VPK</param>
        /// <returns>A VPK wrapper on success, null on failure</returns>
#if NET48
        public static VPK Create(Stream data)
#else
        public static VPK? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.VPK().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new VPK(file, data);
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
            Printing.VPK.Print(builder, this.Model);
            return builder;
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the VPK to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no directory items
            if (DirectoryItems == null || DirectoryItems.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < DirectoryItems.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the VPK to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no directory items
            if (DirectoryItems == null || DirectoryItems.Length == 0)
                return false;

            // If the directory item index is invalid
            if (index < 0 || index >= DirectoryItems.Length)
                return false;

            // Get the directory item
            var directoryItem = DirectoryItems[index];
            if (directoryItem?.DirectoryEntry == null)
                return false;

            // If we have an item with no archive
#if NET48
            byte[] data;
#else
            byte[]? data;
#endif
            if (directoryItem.DirectoryEntry.ArchiveIndex == HL_VPK_NO_ARCHIVE)
            {
                if (directoryItem.PreloadData == null)
                    return false;

                data = directoryItem.PreloadData;
            }
            else
            {
                // If we have invalid archives
                if (ArchiveFilenames == null || ArchiveFilenames.Length == 0)
                    return false;

                // If we have an invalid index
                if (directoryItem.DirectoryEntry.ArchiveIndex < 0 || directoryItem.DirectoryEntry.ArchiveIndex >= ArchiveFilenames.Length)
                    return false;

                // Get the archive filename
                string archiveFileName = ArchiveFilenames[directoryItem.DirectoryEntry.ArchiveIndex];
                if (string.IsNullOrWhiteSpace(archiveFileName))
                    return false;

                // If the archive doesn't exist
                if (!File.Exists(archiveFileName))
                    return false;

                // Try to open the archive
#if NET48
                Stream archiveStream = null;
#else
                Stream? archiveStream = null;
#endif
                try
                {
                    // Open the archive
                    archiveStream = File.OpenRead(archiveFileName);

                    // Seek to the data
                    archiveStream.Seek(directoryItem.DirectoryEntry.EntryOffset, SeekOrigin.Begin);

                    // Read the directory item bytes
                    data = archiveStream.ReadBytes((int)directoryItem.DirectoryEntry.EntryLength);
                }
                catch
                {
                    return false;
                }
                finally
                {
                    archiveStream?.Close();
                }

                // If we have preload data, prepend it
                if (data != null && directoryItem.PreloadData != null)
                    data = directoryItem.PreloadData.Concat(data).ToArray();
            }

            // If there is nothing to write out
            if (data == null)
                return false;

            // Create the filename
            string filename = $"{directoryItem.Name}.{directoryItem.Extension}";
            if (!string.IsNullOrWhiteSpace(directoryItem.Path))
                filename = Path.Combine(directoryItem.Path, filename);

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