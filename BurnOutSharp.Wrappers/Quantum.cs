using System;
using System.IO;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

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

        #region Files

        /// <inheritdoc cref="Models.Quantum.Archive.FileList"/>
        public Models.Quantum.FileDescriptor[] FileList => _archive.FileList;

        #endregion

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
            var file = FileList[index];

            // TODO: Is it even possible to extract a single file?
            return false;
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
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Signature: {Signature}");
            Console.WriteLine($"  Major version: {MajorVersion}");
            Console.WriteLine($"  Minor version: {MinorVersion}");
            Console.WriteLine($"  File count: {FileCount}");
            Console.WriteLine($"  Table size: {TableSize}");
            Console.WriteLine($"  Compression flags: {CompressionFlags}");
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
                    Console.WriteLine($"    File name size = {fileDescriptor.FileNameSize}");
                    Console.WriteLine($"    File name = {fileDescriptor.FileName ?? "[NULL]"}");
                    Console.WriteLine($"    Comment field size = {fileDescriptor.CommentFieldSize}");
                    Console.WriteLine($"    Comment field = {fileDescriptor.CommentField ?? "[NULL]"}");
                    Console.WriteLine($"    Expanded file size = {fileDescriptor.ExpandedFileSize}");
                    Console.WriteLine($"    File time = {fileDescriptor.FileTime}");
                    Console.WriteLine($"    File date = {fileDescriptor.FileDate}");
                }
            }
            Console.WriteLine();
        }

        #endregion
    }
}