using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.Wrappers
{
    public class MicrosoftCabinet : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.Signature"/>
        public string Signature => _cabinet.Header.Signature;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.Reserved1"/>
        public uint Reserved1 => _cabinet.Header.Reserved1;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.CabinetSize"/>
        public uint CabinetSize => _cabinet.Header.CabinetSize;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.Reserved2"/>
        public uint Reserved2 => _cabinet.Header.Reserved2;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FilesOffset"/>
        public uint FilesOffset => _cabinet.Header.FilesOffset;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.Reserved3"/>
        public uint Reserved3 => _cabinet.Header.Reserved3;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.VersionMinor"/>
        public byte VersionMinor => _cabinet.Header.VersionMinor;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.VersionMajor"/>
        public byte VersionMajor => _cabinet.Header.VersionMajor;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FolderCount"/>
        public ushort FolderCount => _cabinet.Header.FolderCount;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FileCount"/>
        public ushort FileCount => _cabinet.Header.FileCount;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FileCount"/>
        public Models.MicrosoftCabinet.HeaderFlags Flags => _cabinet.Header.Flags;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.SetID"/>
        public ushort SetID => _cabinet.Header.SetID;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.CabinetIndex"/>
        public ushort CabinetIndex => _cabinet.Header.CabinetIndex;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.HeaderReservedSize"/>
        public ushort HeaderReservedSize => _cabinet.Header.HeaderReservedSize;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.FolderReservedSize"/>
        public byte FolderReservedSize => _cabinet.Header.FolderReservedSize;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.DataReservedSize"/>
        public byte DataReservedSize => _cabinet.Header.DataReservedSize;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.ReservedData"/>
        public byte[] ReservedData => _cabinet.Header.ReservedData;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.CabinetPrev"/>
        public string CabinetPrev => _cabinet.Header.CabinetPrev;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.DiskPrev"/>
        public string DiskPrev => _cabinet.Header.DiskPrev;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.CabinetNext"/>
        public string CabinetNext => _cabinet.Header.CabinetNext;

        /// <inheritdoc cref="Models.MicrosoftCabinet.CFHEADER.DiskNext"/>
        public string DiskNext => _cabinet.Header.DiskNext;

        #endregion

        #region Folders

        /// <inheritdoc cref="Models.MicrosoftCabinet.Cabinet.Folders"/>
        public Models.MicrosoftCabinet.CFFOLDER[] Folders => _cabinet.Folders;

        #endregion

        #region Files

        /// <inheritdoc cref="Models.MicrosoftCabinet.Cabinet.Files"/>
        public Models.MicrosoftCabinet.CFFILE[] Files => _cabinet.Files;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the cabinet
        /// </summary>
        private Models.MicrosoftCabinet.Cabinet _cabinet;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private MicrosoftCabinet() { }

        /// <summary>
        /// Create a Microsoft Cabinet from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the cabinet</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
        public static MicrosoftCabinet Create(byte[] data, int offset)
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
        public static MicrosoftCabinet Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var cabinet = Builders.MicrosoftCabinet.ParseCabinet(data);
            if (cabinet == null)
                return null;

            var wrapper = new MicrosoftCabinet
            {
                _cabinet = cabinet,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
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
        public byte[] GetUncompressedData(int folderIndex)
        {
            // If we have an invalid folder index
            if (folderIndex < 0 || folderIndex >= Folders.Length)
                return null;

            // Get the folder header
            var folder = Folders[folderIndex];
            if (folder == null)
                return null;

            // If we have invalid data blocks
            if (folder.DataBlocks == null || folder.DataBlocks.Count == 0)
                return null;

            // Store the last decompressed block for MS-ZIP
            byte[] lastDecompressed = null;

            List<byte> data = new List<byte>();
            foreach (var dataBlock in folder.DataBlocks.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value))
            {
                byte[] decompressed = null;
                switch (folder.CompressionType)
                {
                    case Models.MicrosoftCabinet.CompressionType.TYPE_NONE:
                        decompressed = dataBlock.CompressedData;
                        break;
                    case Models.MicrosoftCabinet.CompressionType.TYPE_MSZIP:
                        // TODO: UNIMPLEMENTED
                        decompressed = dataBlock.CompressedData;
                        break;
                    case Models.MicrosoftCabinet.CompressionType.TYPE_QUANTUM:
                        // TODO: UNIMPLEMENTED
                        decompressed = dataBlock.CompressedData;
                        break;
                    case Models.MicrosoftCabinet.CompressionType.TYPE_LZX:
                        // TODO: UNIMPLEMENTED
                        decompressed = dataBlock.CompressedData;
                        break;
                    default:
                        return null;
                }

                lastDecompressed = decompressed;
                if (decompressed != null)
                    data.AddRange(decompressed);
            }

            return data.ToArray();
        }

        #endregion

        #region Files

        /// <summary>
        /// Extract a single file to an output directory
        /// </summary>
        /// <param name="fileIndex">File index to check</param>
        /// <param name="outputDirectory">Output directory to use for writing</param>
        /// <returns>Byte array representing the data, null on error</returns>
        public bool ExtractFile(int fileIndex, string outputDirectory)
        {
            // If we have an invalid file index
            if (fileIndex < 0 || fileIndex >= Files.Length)
                return false;

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Ensure the directory exists
            Directory.CreateDirectory(outputDirectory);

            // Get the file header
            var file = Files[fileIndex];
            if (file == null || file.FileSize == 0)
                return false;

            // Create the output filename
            string fileName = Path.Combine(outputDirectory, file.Name);

            // Get the file data, if possible
            byte[] fileData = GetFileData(fileIndex);
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
            if (fileIndex < 0 || fileIndex >= Files.Length)
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
        public byte[] GetFileData(int fileIndex)
        {
            // If we have an invalid file index
            if (fileIndex < 0 || fileIndex >= Files.Length)
                return null;

            // Get the file header
            var file = Files[fileIndex];
            if (file == null || file.FileSize == 0)
                return null;

            // Get the parent folder data
            byte[] folderData = GetUncompressedData((int)file.FolderIndex);
            if (folderData == null)
                return null;

            // Get the segment that represents this file
            byte[] fileData = new byte[file.FileSize];
            Array.Copy(folderData, file.FolderStartOffset, fileData, 0, file.FileSize);
            return fileData;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("Microsoft Cabinet Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintHeader();
            PrintFolders();
            PrintFiles();
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Signature: {Signature}");
            Console.WriteLine($"  Reserved 1: {Reserved1}");
            Console.WriteLine($"  Cabinet size: {CabinetSize}");
            Console.WriteLine($"  Reserved 2: {Reserved2}");
            Console.WriteLine($"  Files offset: {FilesOffset}");
            Console.WriteLine($"  Reserved 3: {Reserved3}");
            Console.WriteLine($"  Minor version: {VersionMinor}");
            Console.WriteLine($"  Major version: {VersionMajor}");
            Console.WriteLine($"  Folder count: {FolderCount}");
            Console.WriteLine($"  File count: {FileCount}");
            Console.WriteLine($"  Flags: {Flags}");
            Console.WriteLine($"  Set ID: {SetID}");
            Console.WriteLine($"  Cabinet index: {CabinetIndex}");

            if (Flags.HasFlag(Models.MicrosoftCabinet.HeaderFlags.RESERVE_PRESENT))
            {
                Console.WriteLine($"  Header reserved size: {HeaderReservedSize}");
                Console.WriteLine($"  Folder reserved size: {FolderReservedSize}");
                Console.WriteLine($"  Data reserved size: {DataReservedSize}");
                if (ReservedData == null)
                    Console.WriteLine($"  Reserved data = [NULL]");
                else
                    Console.WriteLine($"  Reserved data = {BitConverter.ToString(ReservedData).Replace("-", " ")}");
            }

            if (Flags.HasFlag(Models.MicrosoftCabinet.HeaderFlags.PREV_CABINET))
            {
                Console.WriteLine($"  Previous cabinet: {CabinetPrev}");
                Console.WriteLine($"  Previous disk: {DiskPrev}");
            }

            if (Flags.HasFlag(Models.MicrosoftCabinet.HeaderFlags.NEXT_CABINET))
            {
                Console.WriteLine($"  Next cabinet: {CabinetNext}");
                Console.WriteLine($"  Next disk: {DiskNext}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Print folders information
        /// </summary>
        private void PrintFolders()
        {
            Console.WriteLine("  Folders:");
            Console.WriteLine("  -------------------------");
            if (FolderCount == 0 || Folders == null || Folders.Length == 0)
            {
                Console.WriteLine("  No folders");
            }
            else
            {
                for (int i = 0; i < Folders.Length; i++)
                {
                    var entry = Folders[i];
                    Console.WriteLine($"  Folder {i}");
                    Console.WriteLine($"    Cab start offset = {entry.CabStartOffset}");
                    Console.WriteLine($"    Data count = {entry.DataCount}");
                    Console.WriteLine($"    Compression type = {entry.CompressionType}");
                    if (entry.ReservedData == null)
                        Console.WriteLine($"    Reserved data = [NULL]");
                    else
                        Console.WriteLine($"    Reserved data = {BitConverter.ToString(entry.ReservedData).Replace("-", " ")}");
                    Console.WriteLine();

                    Console.WriteLine("    Data Blocks");
                    Console.WriteLine("    -------------------------");
                    if (entry.DataBlocks == null || entry.DataBlocks.Count == 0)
                    {
                        Console.WriteLine("    No data blocks");
                    }
                    else
                    {
                        foreach (var block in entry.DataBlocks)
                        {
                            Console.WriteLine($"    Data Block at offset {block.Key}");
                            Console.WriteLine($"      Checksum = {block.Value.Checksum}");
                            Console.WriteLine($"      Compressed size = {block.Value.CompressedSize}");
                            Console.WriteLine($"      Uncompressed size = {block.Value.UncompressedSize}");
                            if (block.Value.ReservedData == null)
                                Console.WriteLine($"      Reserved data = [NULL]");
                            else
                                Console.WriteLine($"      Reserved data = {BitConverter.ToString(block.Value.ReservedData).Replace("-", " ")}");
                            //Console.WriteLine($"      Compressed data = {BitConverter.ToString(block.Value.CompressedData).Replace("-", " ")}");
                        }
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print files information
        /// </summary>
        private void PrintFiles()
        {
            Console.WriteLine("  Files:");
            Console.WriteLine("  -------------------------");
            if (FileCount == 0 || Files == null || Files.Length == 0)
            {
                Console.WriteLine("  No files");
            }
            else
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    var entry = Files[i];
                    Console.WriteLine($"  File {i}");
                    Console.WriteLine($"    File size = {entry.FileSize}");
                    Console.WriteLine($"    Folder start offset = {entry.FolderStartOffset}");
                    Console.WriteLine($"    Folder index = {entry.FolderIndex}");
                    Console.WriteLine($"    Date = {entry.Date}");
                    Console.WriteLine($"    Time = {entry.Time}");
                    Console.WriteLine($"    Attributes = {entry.Attributes}");
                    Console.WriteLine($"    Name = {entry.Name}");
                }
            }
            Console.WriteLine();
        }

        #endregion
    }
}
