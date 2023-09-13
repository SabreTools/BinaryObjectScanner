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

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.Signature"/>
#if NET48
        public string Signature => _model.Header.Signature;
#else
        public string? Signature => _model.Header?.Signature;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.Reserved1"/>
#if NET48
        public uint Reserved1 => _model.Header.Reserved1;
#else
        public uint? Reserved1 => _model.Header?.Reserved1;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.CabinetSize"/>
#if NET48
        public uint CabinetSize => _model.Header.CabinetSize;
#else
        public uint? CabinetSize => _model.Header?.CabinetSize;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.Reserved2"/>
#if NET48
        public uint Reserved2 => _model.Header.Reserved2;
#else
        public uint? Reserved2 => _model.Header?.Reserved2;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FilesOffset"/>
#if NET48
        public uint FilesOffset => _model.Header.FilesOffset;
#else
        public uint? FilesOffset => _model.Header?.FilesOffset;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.Reserved3"/>
#if NET48
        public uint Reserved3 => _model.Header.Reserved3;
#else
        public uint? Reserved3 => _model.Header?.Reserved3;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.VersionMinor"/>
#if NET48
        public byte VersionMinor => _model.Header.VersionMinor;
#else
        public byte? VersionMinor => _model.Header?.VersionMinor;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.VersionMajor"/>
#if NET48
        public byte VersionMajor => _model.Header.VersionMajor;
#else
        public byte? VersionMajor => _model.Header?.VersionMajor;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FolderCount"/>
#if NET48
        public ushort FolderCount => _model.Header.FolderCount;
#else
        public ushort? FolderCount => _model.Header?.FolderCount;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FileCount"/>
#if NET48
        public ushort FileCount => _model.Header.FileCount;
#else
        public ushort? FileCount => _model.Header?.FileCount;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FileCount"/>
#if NET48
        public SabreTools.Models.MicrosoftCabinet.HeaderFlags Flags => _model.Header.Flags;
#else
        public SabreTools.Models.MicrosoftCabinet.HeaderFlags? Flags => _model.Header?.Flags;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.SetID"/>
#if NET48
        public ushort SetID => _model.Header.SetID;
#else
        public ushort? SetID => _model.Header?.SetID;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.CabinetIndex"/>
#if NET48
        public ushort CabinetIndex => _model.Header.CabinetIndex;
#else
        public ushort? CabinetIndex => _model.Header?.CabinetIndex;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.HeaderReservedSize"/>
#if NET48
        public ushort HeaderReservedSize => _model.Header.HeaderReservedSize;
#else
        public ushort? HeaderReservedSize => _model.Header?.HeaderReservedSize;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FolderReservedSize"/>
#if NET48
        public byte FolderReservedSize => _model.Header.FolderReservedSize;
#else
        public byte? FolderReservedSize => _model.Header?.FolderReservedSize;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.DataReservedSize"/>
#if NET48
        public byte DataReservedSize => _model.Header.DataReservedSize;
#else
        public byte? DataReservedSize => _model.Header?.DataReservedSize;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.ReservedData"/>
#if NET48
        public byte[] ReservedData => _model.Header.ReservedData;
#else
        public byte[]? ReservedData => _model.Header?.ReservedData;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.CabinetPrev"/>
#if NET48
        public string CabinetPrev => _model.Header.CabinetPrev;
#else
        public string? CabinetPrev => _model.Header?.CabinetPrev;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.DiskPrev"/>
#if NET48
        public string DiskPrev => _model.Header.DiskPrev;
#else
        public string? DiskPrev => _model.Header?.DiskPrev;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.CabinetNext"/>
#if NET48
        public string CabinetNext => _model.Header.CabinetNext;
#else
        public string? CabinetNext => _model.Header?.CabinetNext;
#endif

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.DiskNext"/>
#if NET48
        public string DiskNext => _model.Header.DiskNext;
#else
        public string? DiskNext => _model.Header?.DiskNext;
#endif

        #endregion

        #region Folders

        /// <inheritdoc cref="Models.MicrosoftCabinet.Cabinet.Folders"/>
#if NET48
        public SabreTools.Models.MicrosoftCabinet.CFFOLDER[] Folders => _model.Folders;
#else
        public SabreTools.Models.MicrosoftCabinet.CFFOLDER?[]? Folders => _model.Folders;
#endif

        #endregion

        #region Files

        /// <inheritdoc cref="Models.MicrosoftCabinet.Cabinet.Files"/>
#if NET48
        public SabreTools.Models.MicrosoftCabinet.CFFILE[] Files => _model.Files;
#else
        public SabreTools.Models.MicrosoftCabinet.CFFILE?[]? Files => _model.Files;
#endif

        #endregion

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
            if (folderIndex < 0 || Folders == null || folderIndex >= Folders.Length)
                return null;

            // Get the folder header
            var folder = Folders[folderIndex];
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
            if (Files == null || Files.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < Files.Length; i++)
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
            if (index < 0 || Files == null || index >= Files.Length)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Ensure the directory exists
            Directory.CreateDirectory(outputDirectory);

            // Get the file header
            var file = Files[index];
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
            if (fileIndex < 0 || Files == null || fileIndex >= Files.Length)
                return null;

            // Get the file header
            var file = Files[fileIndex];
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
            if (fileIndex < 0 || Files == null || fileIndex >= Files.Length)
                return null;

            // Get the file header
            var file = Files[fileIndex];
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

            builder.AppendLine("Microsoft Cabinet Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintHeader(builder);
            PrintFolders(builder);
            PrintFiles(builder);

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
            builder.AppendLine($"  Reserved 1: {Reserved1} (0x{Reserved1:X})");
            builder.AppendLine($"  Cabinet size: {CabinetSize} (0x{CabinetSize:X})");
            builder.AppendLine($"  Reserved 2: {Reserved2} (0x{Reserved2:X})");
            builder.AppendLine($"  Files offset: {FilesOffset} (0x{FilesOffset:X})");
            builder.AppendLine($"  Reserved 3: {Reserved3} (0x{Reserved3:X})");
            builder.AppendLine($"  Minor version: {VersionMinor} (0x{VersionMinor:X})");
            builder.AppendLine($"  Major version: {VersionMajor} (0x{VersionMajor:X})");
            builder.AppendLine($"  Folder count: {FolderCount} (0x{FolderCount:X})");
            builder.AppendLine($"  File count: {FileCount} (0x{FileCount:X})");
            builder.AppendLine($"  Flags: {Flags} (0x{Flags:X})");
            builder.AppendLine($"  Set ID: {SetID} (0x{SetID:X})");
            builder.AppendLine($"  Cabinet index: {CabinetIndex} (0x{CabinetIndex:X})");

#if NET48
            if (Flags.HasFlag(SabreTools.Models.MicrosoftCabinet.HeaderFlags.RESERVE_PRESENT))
#else
            if (Flags != null && Flags.Value.HasFlag(SabreTools.Models.MicrosoftCabinet.HeaderFlags.RESERVE_PRESENT))
#endif
            {
                builder.AppendLine($"  Header reserved size: {HeaderReservedSize} (0x{HeaderReservedSize:X})");
                builder.AppendLine($"  Folder reserved size: {FolderReservedSize} (0x{FolderReservedSize:X})");
                builder.AppendLine($"  Data reserved size: {DataReservedSize} (0x{DataReservedSize:X})");
                if (ReservedData == null)
                    builder.AppendLine($"  Reserved data = [NULL]");
                else
                    builder.AppendLine($"  Reserved data = {BitConverter.ToString(ReservedData).Replace("-", " ")}");
            }

#if NET48
            if (Flags.HasFlag(SabreTools.Models.MicrosoftCabinet.HeaderFlags.PREV_CABINET))
#else
            if (Flags != null && Flags.Value.HasFlag(SabreTools.Models.MicrosoftCabinet.HeaderFlags.PREV_CABINET))
#endif
            {
                builder.AppendLine($"  Previous cabinet: {CabinetPrev}");
                builder.AppendLine($"  Previous disk: {DiskPrev}");
            }

#if NET48
            if (Flags.HasFlag(SabreTools.Models.MicrosoftCabinet.HeaderFlags.NEXT_CABINET))
#else
            if (Flags != null && Flags.Value.HasFlag(SabreTools.Models.MicrosoftCabinet.HeaderFlags.NEXT_CABINET))
#endif
            {
                builder.AppendLine($"  Next cabinet: {CabinetNext}");
                builder.AppendLine($"  Next disk: {DiskNext}");
            }

            builder.AppendLine();
        }

        /// <summary>
        /// Print folders information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFolders(StringBuilder builder)
        {
            builder.AppendLine("  Folders:");
            builder.AppendLine("  -------------------------");
            if (FolderCount == 0 || Folders == null || Folders.Length == 0)
            {
                builder.AppendLine("  No folders");
            }
            else
            {
                for (int i = 0; i < Folders.Length; i++)
                {
                    var entry = Folders[i];
                    builder.AppendLine($"  Folder {i}");
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine($"    Cab start offset = {entry.CabStartOffset} (0x{entry.CabStartOffset:X})");
                    builder.AppendLine($"    Data count = {entry.DataCount} (0x{entry.DataCount:X})");
                    builder.AppendLine($"    Compression type = {entry.CompressionType} (0x{entry.CompressionType:X})");
                    builder.AppendLine($"    Masked compression type = {entry.CompressionType & SabreTools.Models.MicrosoftCabinet.CompressionType.MASK_TYPE}");
                    builder.AppendLine($"    Reserved data = {(entry.ReservedData == null ? "[NULL]" : BitConverter.ToString(entry.ReservedData).Replace("-", " "))}");
                    builder.AppendLine();

                    builder.AppendLine("    Data Blocks");
                    builder.AppendLine("    -------------------------");
                    if (entry.DataBlocks == null || entry.DataBlocks.Length == 0)
                    {
                        builder.AppendLine("    No data blocks");
                    }
                    else
                    {
                        for (int j = 0; j < entry.DataBlocks.Length; j++)
                        {
                            var dataBlock = entry.DataBlocks[j];
                            builder.AppendLine($"    Data Block {j}");
                            if (dataBlock == null)
                            {
                                builder.AppendLine("      [NULL]");
                                continue;
                            }

                            builder.AppendLine($"      Checksum = {dataBlock.Checksum} (0x{dataBlock.Checksum:X})");
                            builder.AppendLine($"      Compressed size = {dataBlock.CompressedSize} (0x{dataBlock.CompressedSize:X})");
                            builder.AppendLine($"      Uncompressed size = {dataBlock.UncompressedSize} (0x{dataBlock.UncompressedSize:X})");
                            builder.AppendLine($"      Reserved data = {(dataBlock.ReservedData == null ? "[NULL]" : BitConverter.ToString(dataBlock.ReservedData).Replace("-", " "))}");
                            //builder.AppendLine($"      Compressed data = {BitConverter.ToString(dataBlock.CompressedData).Replace("-", " ")}");
                        }
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print files information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFiles(StringBuilder builder)
        {
            builder.AppendLine("  Files:");
            builder.AppendLine("  -------------------------");
            if (FileCount == 0 || Files == null || Files.Length == 0)
            {
                builder.AppendLine("  No files");
            }
            else
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    var entry = Files[i];
                    builder.AppendLine($"  File {i}");
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine($"    File size = {entry.FileSize} (0x{entry.FileSize:X})");
                    builder.AppendLine($"    Folder start offset = {entry.FolderStartOffset} (0x{entry.FolderStartOffset:X})");
                    builder.AppendLine($"    Folder index = {entry.FolderIndex} (0x{entry.FolderIndex:X})");
                    builder.AppendLine($"    Date = {entry.Date} (0x{entry.Date:X})");
                    builder.AppendLine($"    Time = {entry.Time} (0x{entry.Time:X})");
                    builder.AppendLine($"    Attributes = {entry.Attributes} (0x{entry.Attributes:X})");
                    builder.AppendLine($"    Name = {entry.Name ?? "[NULL]"}");
                }
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}
