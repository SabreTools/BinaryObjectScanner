using System;
using System.IO;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

namespace BurnOutSharp.Wrappers
{
    public class BFPK : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.BFPK.Header.Magic"/>
        public uint Magic => _archive.Header.Magic;

        /// <inheritdoc cref="Models.BFPK.Header.Version"/>
        public int Version => _archive.Header.Version;

        /// <inheritdoc cref="Models.BFPK.Header.Files"/>
        public int Files => _archive.Header.Files;

        #endregion

        #region Files

        /// <inheritdoc cref="Models.BFPK.Archive.Files"/>
        public Models.BFPK.FileEntry[] FileTable => _archive.Files;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the archive
        /// </summary>
        private Models.BFPK.Archive _archive;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private BFPK() { }

        /// <summary>
        /// Create a BFPK archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A BFPK archive wrapper on success, null on failure</returns>
        public static BFPK Create(byte[] data, int offset)
        {
            var archive = Builder.BFPK.ParseArchive(data, offset);
            if (archive == null)
                return null;

            var wrapper = new BFPK
            {
                _archive = archive,
                _dataSource = DataSource.ByteArray,
                _byteArrayData = data,
                _byteArrayOffset = offset,
            };
            return wrapper;
        }

        /// <summary>
        /// Create a BFPK archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A BFPK archive wrapper on success, null on failure</returns>
        public static BFPK Create(Stream data)
        {
            var archive = Builder.BFPK.ParseArchive(data);
            if (archive == null)
                return null;

            var wrapper = new BFPK
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
        /// Extract a single file based on index
        /// </summary>
        /// <param name="index">Index of the file to extract</param>
        /// <param name="outputDirectory">Directory to write the file to</param>
        /// <returns>True if the extraction succeeded, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no files
            if (Files == 0 || FileTable == null || FileTable.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= FileTable.Length)
                return false;

            // Get the file information
            var file = FileTable[index];

            // Get the read index and length
            int offset = file.Offset + 4;
            int compressedSize = file.CompressedSize;

            // Some files can lack the length prefix
            if (compressedSize > GetEndOfFile())
            {
                offset -= 4;
                compressedSize = file.UncompressedSize;
            }

            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(outputDirectory);

                // Create the output path
                string filePath = Path.Combine(outputDirectory, file.Name);
                using (FileStream fs = File.OpenWrite(filePath))
                {
                    // Read the data block
                    byte[] data = ReadFromDataSource(offset, compressedSize);

                    // If we have uncompressed data
                    if (compressedSize == file.UncompressedSize)
                    {
                        fs.Write(data, 0, compressedSize);
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(data);
                        ZlibStream zs = new ZlibStream(ms, CompressionMode.Decompress);
                        zs.CopyTo(fs);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("BFPK Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintHeader();
            PrintFileTable();
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Magic: {Magic}");
            Console.WriteLine($"  Version: {Version}");
            Console.WriteLine($"  Files: {Files}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print file table information
        /// </summary>
        private void PrintFileTable()
        {
            Console.WriteLine("  File Table Information:");
            Console.WriteLine("  -------------------------");
            if (Files == 0 || FileTable == null || FileTable.Length == 0)
            {
                Console.WriteLine("  No file table items");
            }
            else
            {
                for (int i = 0; i < FileTable.Length; i++)
                {
                    var entry = FileTable[i];
                    Console.WriteLine($"  File Table Entry {i}");
                    Console.WriteLine($"    Name size = {entry.NameSize}");
                    Console.WriteLine($"    Name = {entry.Name}");
                    Console.WriteLine($"    Uncompressed size = {entry.UncompressedSize}");
                    Console.WriteLine($"    Offset = {entry.Offset}");
                    Console.WriteLine($"    Compressed Size = {entry.CompressedSize}");
                }
            }
            Console.WriteLine();
        }

        #endregion
    }
}