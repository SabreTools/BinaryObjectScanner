using System;
using System.IO;
using System.Linq;
using BurnOutSharp.Compression.Quantum;

namespace BurnOutSharp.Wrappers
{
    public class Quantum : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.Quantum.Header.Signature"/>
        public string Signature => _archive.Header.Signature;

        /// <inheritdoc cref="Models.Quantum.Header.MajorVersion"/>
        public byte MajorVersion => _archive.Header.MajorVersion;

        /// <inheritdoc cref="Models.Quantum.Header.MinorVersion"/>
        public byte MinorVersion => _archive.Header.MinorVersion;

        /// <inheritdoc cref="Models.Quantum.Header.FileCount"/>
        public ushort FileCount => _archive.Header.FileCount;

        /// <inheritdoc cref="Models.Quantum.Header.TableSize"/>
        public byte TableSize => _archive.Header.TableSize;

        /// <inheritdoc cref="Models.Quantum.Header.CompressionFlags"/>
        public byte CompressionFlags => _archive.Header.CompressionFlags;

        #endregion

        #region File List

        /// <inheritdoc cref="Models.Quantum.Archive.FileList"/>
        public Models.Quantum.FileDescriptor[] FileList => _archive.FileList;

        #endregion

        /// <inheritdoc cref="Models.Quantum.Archive.CompressedDataOffset"/>
        public long CompressedDataOffset => _archive.CompressedDataOffset;

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the archive
        /// </summary>
        private Models.Quantum.Archive _archive;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private Quantum() { }

        /// <summary>
        /// Create a Quantum archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A Quantum archive wrapper on success, null on failure</returns>
        public static Quantum Create(byte[] data, int offset)
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
        /// Create a Quantum archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A Quantum archive wrapper on success, null on failure</returns>
        public static Quantum Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = Builders.Quantum.ParseArchive(data);
            if (archive == null)
                return null;

            var wrapper = new Quantum
            {
                _archive = archive,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Data

        /// <summary>
        /// Extract all files from the Quantum archive to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no files
            if (FileList == null || FileList.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < FileList.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the Quantum archive to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no files
            if (FileCount == 0 || FileList == null || FileList.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= FileList.Length)
                return false;

            // Get the file information
            var fileDescriptor = FileList[index];

            // Read the entire compressed data
            int compressedDataOffset = (int)CompressedDataOffset;
            int compressedDataLength = GetEndOfFile() - compressedDataOffset;
            byte[] compressedData = ReadFromDataSource(compressedDataOffset, compressedDataLength);

            // TODO: Figure out decompression
            // - Single-file archives seem to work
            // - Single-file archives with files that span a window boundary seem to work
            // - The first files in each archive seem to work
            return false;

            // // Setup the decompression state
            // State state = new State();
            // Decompressor.InitState(state, TableSize, CompressionFlags);

            // // Decompress the entire array
            // int decompressedDataLength = (int)FileList.Sum(fd => fd.ExpandedFileSize);
            // byte[] decompressedData = new byte[decompressedDataLength];
            // Decompressor.Decompress(state, compressedData.Length, compressedData, decompressedData.Length, decompressedData);

            // // Read the data
            // int offset = (int)FileList.Take(index).Sum(fd => fd.ExpandedFileSize);
            // byte[] data = new byte[fileDescriptor.ExpandedFileSize];
            // Array.Copy(decompressedData, offset, data, 0, data.Length);

            // // Loop through all files before the current
            // for (int i = 0; i < index; i++)
            // {
            //     // Decompress the next block of data
            //     byte[] tempData = new byte[FileList[i].ExpandedFileSize];
            //     int lastRead = Decompressor.Decompress(state, compressedData.Length, compressedData, tempData.Length, tempData);
            //     compressedData = new ReadOnlySpan<byte>(compressedData, (lastRead), compressedData.Length - (lastRead)).ToArray();
            // }

            // // Read the data
            // byte[] data = new byte[fileDescriptor.ExpandedFileSize];
            // _ = Decompressor.Decompress(state, compressedData.Length, compressedData, data.Length, data);

            // // Create the filename
            // string filename = fileDescriptor.FileName;

            // // If we have an invalid output directory
            // if (string.IsNullOrWhiteSpace(outputDirectory))
            //     return false;

            // // Create the full output path
            // filename = Path.Combine(outputDirectory, filename);

            // // Ensure the output directory is created
            // Directory.CreateDirectory(Path.GetDirectoryName(filename));

            // // Try to write the data
            // try
            // {
            //     // Open the output file for writing
            //     using (Stream fs = File.OpenWrite(filename))
            //     {
            //         fs.Write(data, 0, data.Length);
            //     }
            // }
            // catch
            // {
            //     return false;
            // }

            return true;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("Quantum Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintHeader();
            PrintFileList();
            Console.WriteLine($"  Compressed data offset: {CompressedDataOffset} (0x{CompressedDataOffset:X})");
            Console.WriteLine();
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Signature: {Signature}");
            Console.WriteLine($"  Major version: {MajorVersion} (0x{MajorVersion:X})");
            Console.WriteLine($"  Minor version: {MinorVersion} (0x{MinorVersion:X})");
            Console.WriteLine($"  File count: {FileCount} (0x{FileCount:X})");
            Console.WriteLine($"  Table size: {TableSize} (0x{TableSize:X})");
            Console.WriteLine($"  Compression flags: {CompressionFlags} (0x{CompressionFlags:X})");
            Console.WriteLine();
        }

        /// <summary>
        /// Print file list information
        /// </summary>
        private void PrintFileList()
        {
            Console.WriteLine("  File List Information:");
            Console.WriteLine("  -------------------------");
            if (FileCount == 0 || FileList == null || FileList.Length == 0)
            {
                Console.WriteLine("  No file list items");
            }
            else
            {
                for (int i = 0; i < FileList.Length; i++)
                {
                    var fileDescriptor = FileList[i];
                    Console.WriteLine($"  File Descriptor {i}");
                    Console.WriteLine($"    File name size: {fileDescriptor.FileNameSize} (0x{fileDescriptor.FileNameSize:X})");
                    Console.WriteLine($"    File name: {fileDescriptor.FileName ?? "[NULL]"}");
                    Console.WriteLine($"    Comment field size: {fileDescriptor.CommentFieldSize} (0x{fileDescriptor.CommentFieldSize:X})");
                    Console.WriteLine($"    Comment field: {fileDescriptor.CommentField ?? "[NULL]"}");
                    Console.WriteLine($"    Expanded file size: {fileDescriptor.ExpandedFileSize} (0x{fileDescriptor.ExpandedFileSize:X})");
                    Console.WriteLine($"    File time: {fileDescriptor.FileTime} (0x{fileDescriptor.FileTime:X})");
                    Console.WriteLine($"    File date: {fileDescriptor.FileDate} (0x{fileDescriptor.FileDate:X})");
                    if (fileDescriptor.Unknown != null)
                        Console.WriteLine($"    Unknown (Checksum?): {fileDescriptor.Unknown} (0x{fileDescriptor.Unknown:X})");
                }
            }
            Console.WriteLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_archive, _jsonSerializerOptions);

#endif

        #endregion
    }
}