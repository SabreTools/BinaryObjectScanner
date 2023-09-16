using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public partial class MicrosoftCabinet : WrapperBase<SabreTools.Models.MicrosoftCabinet.Cabinet>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Microsoft Cabinet";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public MicrosoftCabinet(SabreTools.Models.MicrosoftCabinet.Cabinet model, byte[] data, int offset)
#else
        public MicrosoftCabinet(SabreTools.Models.MicrosoftCabinet.Cabinet? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public MicrosoftCabinet(SabreTools.Models.MicrosoftCabinet.Cabinet model, Stream data)
#else
        public MicrosoftCabinet(SabreTools.Models.MicrosoftCabinet.Cabinet? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }/// <summary>
         /// Create a Microsoft Cabinet from a byte array and offset
         /// </summary>
         /// <param name="data">Byte array representing the cabinet</param>
         /// <param name="offset">Offset within the array to parse</param>
         /// <returns>A cabinet wrapper on success, null on failure</returns>
#if NET48
        public static MicrosoftCabinet Create(byte[] data, int offset)
#else
        public static MicrosoftCabinet? Create(byte[]? data, int offset)
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
        /// Create a Microsoft Cabinet from a Stream
        /// </summary>
        /// <param name="data">Stream representing the cabinet</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
#if NET48
        public static MicrosoftCabinet Create(Stream data)
#else
        public static MicrosoftCabinet? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var cabinet = new SabreTools.Serialization.Streams.MicrosoftCabinet().Deserialize(data);
            if (cabinet == null)
                return null;

            try
            {
                return new MicrosoftCabinet(cabinet, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Checksumming

        /// <summary>
        /// The computation and verification of checksums found in CFDATA structure entries cabinet files is
        /// done by using a function described by the following mathematical notation. When checksums are
        /// not supplied by the cabinet file creating application, the checksum field is set to 0 (zero). Cabinet
        /// extracting applications do not compute or verify the checksum if the field is set to 0 (zero).
        /// </summary>
        private static uint ChecksumData(byte[] data)
        {
            uint[] C = new uint[4]
            {
                S(data, 1, data.Length),
                S(data, 2, data.Length),
                S(data, 3, data.Length),
                S(data, 4, data.Length),
            };

            return C[0] ^ C[1] ^ C[2] ^ C[3];
        }

        /// <summary>
        /// Individual algorithmic step
        /// </summary>
        private static uint S(byte[] a, int b, int x)
        {
            int n = a.Length;

            if (x < 4 && b > n % 4)
                return 0;
            else if (x < 4 && b <= n % 4)
                return a[n - b + 1];
            else // if (x >= 4)
                return a[n - x + b] ^ S(a, b, x - 4);
        }

        #endregion

        #region Folders

        /// <summary>
        /// Get the uncompressed data associated with a folder
        /// </summary>
        /// <param name="folderIndex">Folder index to check</param>
        /// <returns>Byte array representing the data, null on error</returns>
        /// <remarks>All but uncompressed are unimplemented</remarks>
#if NET48
        public byte[] GetUncompressedData(int folderIndex)
#else
        public byte[]? GetUncompressedData(int folderIndex)
#endif
        {
            // If we have an invalid folder index
            if (folderIndex < 0 || this.Model.Folders == null || folderIndex >= this.Model.Folders.Length)
                return null;

            // Get the folder header
            var folder = this.Model.Folders[folderIndex];
            if (folder == null)
                return null;

            // If we have invalid data blocks
            if (folder.DataBlocks == null || folder.DataBlocks.Length == 0)
                return null;

            // Setup LZX decompression
            var lzx = new Compression.LZX.State();
            Compression.LZX.Decompressor.Init(((ushort)folder.CompressionType >> 8) & 0x1f, lzx);

            // Setup MS-ZIP decompression
            Compression.MSZIP.State mszip = new Compression.MSZIP.State();

            // Setup Quantum decompression
            var qtm = new Compression.Quantum.State();
            Compression.Quantum.Decompressor.InitState(qtm, folder);

            List<byte> data = new List<byte>();
            foreach (var dataBlock in folder.DataBlocks)
            {
                if (dataBlock == null)
                    continue;

#if NET48
                byte[] decompressed = new byte[dataBlock.UncompressedSize];
#else
                byte[]? decompressed = new byte[dataBlock.UncompressedSize];
#endif
                switch (folder.CompressionType & SabreTools.Models.MicrosoftCabinet.CompressionType.MASK_TYPE)
                {
                    case SabreTools.Models.MicrosoftCabinet.CompressionType.TYPE_NONE:
                        decompressed = dataBlock.CompressedData;
                        break;
                    case SabreTools.Models.MicrosoftCabinet.CompressionType.TYPE_MSZIP:
                        decompressed = new byte[SabreTools.Models.Compression.MSZIP.Constants.ZIPWSIZE];
                        Compression.MSZIP.Decompressor.Decompress(mszip, dataBlock.CompressedSize, dataBlock.CompressedData, dataBlock.UncompressedSize, decompressed);
                        Array.Resize(ref decompressed, dataBlock.UncompressedSize);
                        break;
                    case SabreTools.Models.MicrosoftCabinet.CompressionType.TYPE_QUANTUM:
                        Compression.Quantum.Decompressor.Decompress(qtm, dataBlock.CompressedSize, dataBlock.CompressedData, dataBlock.UncompressedSize, decompressed);
                        break;
                    case SabreTools.Models.MicrosoftCabinet.CompressionType.TYPE_LZX:
                        Compression.LZX.Decompressor.Decompress(state: lzx, dataBlock.CompressedSize, dataBlock.CompressedData, dataBlock.UncompressedSize, decompressed);
                        break;
                    default:
                        return null;
                }

                if (decompressed != null)
                    data.AddRange(decompressed);
            }

            return data.ToArray();
        }

        #endregion

        #region Files

        /// <summary>
        /// Extract all files from the MS-CAB to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all filez extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no files
            if (this.Model.Files == null || this.Model.Files.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < this.Model.Files.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the MS-CAB to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have an invalid file index
            if (index < 0 || this.Model.Files == null || index >= this.Model.Files.Length)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Ensure the directory exists
            Directory.CreateDirectory(outputDirectory);

            // Get the file header
            var file = this.Model.Files[index];
            if (file == null || file.FileSize == 0)
                return false;

            // Create the output filename
            string fileName = Path.Combine(outputDirectory, file.Name ?? $"file{index}");

            // Get the file data, if possible
#if NET48
            byte[] fileData = GetFileData(index);
#else
            byte[]? fileData = GetFileData(index);
#endif
            if (fileData == null)
                return false;

            // Write the file data
            using (FileStream fs = File.OpenWrite(fileName))
            {
                fs.Write(fileData, 0, fileData.Length);
            }

            return true;
        }

        /// <summary>
        /// Get the DateTime for a particular file index
        /// </summary>
        /// <param name="fileIndex">File index to check</param>
        /// <returns>DateTime representing the file time, null on error</returns>
        public DateTime? GetDateTime(int fileIndex)
        {
            // If we have an invalid file index
            if (fileIndex < 0 || this.Model.Files == null || fileIndex >= this.Model.Files.Length)
                return null;

            // Get the file header
            var file = this.Model.Files[fileIndex];
            if (file == null)
                return null;

            // If we have an invalid DateTime
            if (file.Date == 0 && file.Time == 0)
                return null;

            try
            {
                // Date property
                int year = (file.Date >> 9) + 1980;
                int month = (file.Date >> 5) & 0x0F;
                int day = file.Date & 0x1F;

                // Time property
                int hour = file.Time >> 11;
                int minute = (file.Time >> 5) & 0x3F;
                int second = (file.Time << 1) & 0x3E;

                return new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Get the uncompressed data associated with a file
        /// </summary>
        /// <param name="fileIndex">File index to check</param>
        /// <returns>Byte array representing the data, null on error</returns>
#if NET48
        public byte[] GetFileData(int fileIndex)
#else
        public byte[]? GetFileData(int fileIndex)
#endif
        {
            // If we have an invalid file index
            if (fileIndex < 0 || this.Model.Files == null || fileIndex >= this.Model.Files.Length)
                return null;

            // Get the file header
            var file = this.Model.Files[fileIndex];
            if (file == null || file.FileSize == 0)
                return null;

            // Get the parent folder data
#if NET48
            byte[] folderData = GetUncompressedData((int)file.FolderIndex);
#else
            byte[]? folderData = GetUncompressedData((int)file.FolderIndex);
#endif
            if (folderData == null || folderData.Length == 0)
                return null;

            // Create the output file data
            byte[] fileData = new byte[file.FileSize];
            if (folderData.Length < file.FolderStartOffset + file.FileSize)
                return null;

            // Get the segment that represents this file
            Array.Copy(folderData, file.FolderStartOffset, fileData, 0, file.FileSize);
            return fileData;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();
            Printing.MicrosoftCabinet.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}
