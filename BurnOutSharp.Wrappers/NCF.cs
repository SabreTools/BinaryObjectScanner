using System;
using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class NCF : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.NCF.Header.Dummy0"/>
        public uint Dummy0 => _file.Header.Dummy0;

        /// <inheritdoc cref="Models.NCF.Header.MajorVersion"/>
        public uint MajorVersion => _file.Header.MajorVersion;

        /// <inheritdoc cref="Models.NCF.Header.MinorVersion"/>
        public uint MinorVersion => _file.Header.MinorVersion;

        /// <inheritdoc cref="Models.NCF.Header.CacheID"/>
        public uint CacheID => _file.Header.CacheID;

        /// <inheritdoc cref="Models.NCF.Header.LastVersionPlayed"/>
        public uint LastVersionPlayed => _file.Header.LastVersionPlayed;

        /// <inheritdoc cref="Models.NCF.Header.Dummy1"/>
        public uint Dummy1 => _file.Header.Dummy1;

        /// <inheritdoc cref="Models.NCF.Header.Dummy2"/>
        public uint Dummy2 => _file.Header.Dummy2;

        /// <inheritdoc cref="Models.NCF.Header.FileSize"/>
        public uint FileSize => _file.Header.FileSize;

        /// <inheritdoc cref="Models.NCF.Header.BlockSize"/>
        public uint BlockSize => _file.Header.BlockSize;

        /// <inheritdoc cref="Models.NCF.Header.BlockCount"/>
        public uint BlockCount => _file.Header.BlockCount;

        /// <inheritdoc cref="Models.NCF.Header.Dummy3"/>
        public uint Dummy3 => _file.Header.Dummy3;

        #endregion

        #region Directory Header

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Dummy0"/>
        public uint DH_Dummy0 => _file.DirectoryHeader.Dummy0;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.CacheID"/>
        public uint DH_CacheID => _file.DirectoryHeader.CacheID;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.LastVersionPlayed"/>
        public uint DH_LastVersionPlayed => _file.DirectoryHeader.LastVersionPlayed;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.ItemCount"/>
        public uint DH_ItemCount => _file.DirectoryHeader.ItemCount;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.FileCount"/>
        public uint DH_FileCount => _file.DirectoryHeader.FileCount;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.ChecksumDataLength"/>
        public uint DH_ChecksumDataLength => _file.DirectoryHeader.ChecksumDataLength;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.DirectorySize"/>
        public uint DH_DirectorySize => _file.DirectoryHeader.DirectorySize;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.NameSize"/>
        public uint DH_NameSize => _file.DirectoryHeader.NameSize;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Info1Count"/>
        public uint DH_Info1Count => _file.DirectoryHeader.Info1Count;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.CopyCount"/>
        public uint DH_CopyCount => _file.DirectoryHeader.CopyCount;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.LocalCount"/>
        public uint DH_LocalCount => _file.DirectoryHeader.LocalCount;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Dummy1"/>
        public uint DH_Dummy1 => _file.DirectoryHeader.Dummy1;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Dummy2"/>
        public uint DH_Dummy2 => _file.DirectoryHeader.Dummy2;

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Checksum"/>
        public uint DH_Checksum => _file.DirectoryHeader.Checksum;

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryEntries"/>
        public Models.NCF.DirectoryEntry[] DirectoryEntries => _file.DirectoryEntries;

        #endregion

        #region Directory Names

        /// <inheritdoc cref="Models.NCF.File.DirectoryNames"/>
        public System.Collections.Generic.Dictionary<long, string> DirectoryNames => _file.DirectoryNames;

        #endregion

        #region Directory Info 1 Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryInfo1Entries"/>
        public Models.NCF.DirectoryInfo1Entry[] DirectoryInfo1Entries => _file.DirectoryInfo1Entries;

        #endregion

        #region Directory Info 2 Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryInfo2Entries"/>
        public Models.NCF.DirectoryInfo2Entry[] DirectoryInfo2Entries => _file.DirectoryInfo2Entries;

        #endregion

        #region Directory Copy Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryCopyEntries"/>
        public Models.NCF.DirectoryCopyEntry[] DirectoryCopyEntries => _file.DirectoryCopyEntries;

        #endregion

        #region Directory Local Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryLocalEntries"/>
        public Models.NCF.DirectoryLocalEntry[] DirectoryLocalEntries => _file.DirectoryLocalEntries;

        #endregion

        #region Unknown Header

        /// <inheritdoc cref="Models.NCF.UnknownHeader.Dummy0"/>
        public uint UH_Dummy0 => _file.UnknownHeader.Dummy0;

        /// <inheritdoc cref="Models.NCF.UnknownHeader.Dummy1"/>
        public uint UH_Dummy1 => _file.UnknownHeader.Dummy1;

        #endregion

        #region Unknown Entries

        /// <inheritdoc cref="Models.NCF.File.UnknownEntries"/>
        public Models.NCF.UnknownEntry[] UnknownEntries => _file.UnknownEntries;

        #endregion

        #region Checksum Header

        /// <inheritdoc cref="Models.NCF.ChecksumHeader.Dummy0"/>
        public uint CH_Dummy0 => _file.ChecksumHeader.Dummy0;

        /// <inheritdoc cref="Models.NCF.ChecksumHeader.ChecksumSize"/>
        public uint CH_ChecksumSize => _file.ChecksumHeader.ChecksumSize;

        #endregion

        #region Checksum Map Header

        /// <inheritdoc cref="Models.NCF.ChecksumMapHeader.Dummy0"/>
        public uint CMH_Dummy0 => _file.ChecksumMapHeader.Dummy0;

        /// <inheritdoc cref="Models.NCF.ChecksumMapHeader.Dummy1"/>
        public uint CMH_Dummy1 => _file.ChecksumMapHeader.Dummy1;

        /// <inheritdoc cref="Models.NCF.ChecksumMapHeader.ItemCount"/>
        public uint CMH_ItemCount => _file.ChecksumMapHeader.ItemCount;

        /// <inheritdoc cref="Models.NCF.ChecksumMapHeader.ChecksumCount"/>
        public uint CMH_ChecksumCount => _file.ChecksumMapHeader.ChecksumCount;

        #endregion

        #region Checksum Map Entries

        /// <inheritdoc cref="Models.NCF.File.ChecksumMapEntries"/>
        public Models.NCF.ChecksumMapEntry[] ChecksumMapEntries => _file.ChecksumMapEntries;

        #endregion

        #region Checksum Entries

        /// <inheritdoc cref="Models.NCF.File.ChecksumEntries"/>
        public Models.NCF.ChecksumEntry[] ChecksumEntries => _file.ChecksumEntries;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the NCF
        /// </summary>
        private Models.NCF.File _file;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private NCF() { }

        /// <summary>
        /// Create an NCF from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the NCF</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NCF wrapper on success, null on failure</returns>
        public static NCF Create(byte[] data, int offset)
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
        /// Create a NCF from a Stream
        /// </summary>
        /// <param name="data">Stream representing the NCF</param>
        /// <returns>An NCF wrapper on success, null on failure</returns>
        public static NCF Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = Builders.NCF.ParseFile(data);
            if (file == null)
                return null;

            var wrapper = new NCF
            {
                _file = file,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("NCF Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            // Header
            PrintHeader();

            // Directory and Directory Maps
            PrintDirectoryHeader();
            PrintDirectoryEntries();
            // TODO: Should we print out the entire string table?
            PrintDirectoryInfo1Entries();
            PrintDirectoryInfo2Entries();
            PrintDirectoryCopyEntries();
            PrintDirectoryLocalEntries();
            PrintUnknownHeader();
            PrintUnknownEntries();

            // Checksums and Checksum Maps
            PrintChecksumHeader();
            PrintChecksumMapHeader();
            PrintChecksumMapEntries();
            PrintChecksumEntries();
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Dummy 0: {Dummy0}");
            Console.WriteLine($"  Major version: {MajorVersion}");
            Console.WriteLine($"  Minor version: {MinorVersion}");
            Console.WriteLine($"  Cache ID: {CacheID}");
            Console.WriteLine($"  Last version played: {LastVersionPlayed}");
            Console.WriteLine($"  Dummy 1: {Dummy1}");
            Console.WriteLine($"  Dummy 2: {Dummy2}");
            Console.WriteLine($"  File size: {FileSize}");
            Console.WriteLine($"  Block size: {BlockSize}");
            Console.WriteLine($"  Block count: {BlockCount}");
            Console.WriteLine($"  Dummy 3: {Dummy3}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print directory header information
        /// </summary>
        private void PrintDirectoryHeader()
        {
            Console.WriteLine("  Directory Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Dummy 0: {DH_Dummy0}");
            Console.WriteLine($"  Cache ID: {DH_CacheID}");
            Console.WriteLine($"  Last version played: {DH_LastVersionPlayed}");
            Console.WriteLine($"  Item count: {DH_ItemCount}");
            Console.WriteLine($"  File count: {DH_FileCount}");
            Console.WriteLine($"  Checksum data length: {DH_ChecksumDataLength}");
            Console.WriteLine($"  Directory size: {DH_DirectorySize}");
            Console.WriteLine($"  Name size: {DH_NameSize}");
            Console.WriteLine($"  Info 1 count: {DH_Info1Count}");
            Console.WriteLine($"  Copy count: {DH_CopyCount}");
            Console.WriteLine($"  Local count: {DH_LocalCount}");
            Console.WriteLine($"  Dummy 1: {DH_Dummy1}");
            Console.WriteLine($"  Dummy 2: {DH_Dummy2}");
            Console.WriteLine($"  Checksum: {DH_Checksum}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print directory entries information
        /// </summary>
        private void PrintDirectoryEntries()
        {
            Console.WriteLine("  Directory Entries Information:");
            Console.WriteLine("  -------------------------");
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
            {
                Console.WriteLine("  No directory entries");
            }
            else
            {
                for (int i = 0; i < DirectoryEntries.Length; i++)
                {
                    var directoryEntry = DirectoryEntries[i];
                    Console.WriteLine($"  Directory Entry {i}");
                    Console.WriteLine($"    Name offset: {directoryEntry.NameOffset}");
                    Console.WriteLine($"    Name: {directoryEntry.Name}");
                    Console.WriteLine($"    Item size: {directoryEntry.ItemSize}");
                    Console.WriteLine($"    Checksum index: {directoryEntry.ChecksumIndex}");
                    Console.WriteLine($"    Directory flags: {directoryEntry.DirectoryFlags}");
                    Console.WriteLine($"    Parent index: {directoryEntry.ParentIndex}");
                    Console.WriteLine($"    Next index: {directoryEntry.NextIndex}");
                    Console.WriteLine($"    First index: {directoryEntry.FirstIndex}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print directory info 1 entries information
        /// </summary>
        private void PrintDirectoryInfo1Entries()
        {
            Console.WriteLine("  Directory Info 1 Entries Information:");
            Console.WriteLine("  -------------------------");
            if (DirectoryInfo1Entries == null || DirectoryInfo1Entries.Length == 0)
            {
                Console.WriteLine("  No directory info 1 entries");
            }
            else
            {
                for (int i = 0; i < DirectoryInfo1Entries.Length; i++)
                {
                    var directoryInfoEntry = DirectoryInfo1Entries[i];
                    Console.WriteLine($"  Directory Info 1 Entry {i}");
                    Console.WriteLine($"    Dummy 0: {directoryInfoEntry.Dummy0}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print directory info 2 entries information
        /// </summary>
        private void PrintDirectoryInfo2Entries()
        {
            Console.WriteLine("  Directory Info 2 Entries Information:");
            Console.WriteLine("  -------------------------");
            if (DirectoryInfo2Entries == null || DirectoryInfo2Entries.Length == 0)
            {
                Console.WriteLine("  No directory info 2 entries");
            }
            else
            {
                for (int i = 0; i < DirectoryInfo2Entries.Length; i++)
                {
                    var directoryInfoEntry = DirectoryInfo2Entries[i];
                    Console.WriteLine($"  Directory Info 2 Entry {i}");
                    Console.WriteLine($"    Dummy 0: {directoryInfoEntry.Dummy0}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print directory copy entries information
        /// </summary>
        private void PrintDirectoryCopyEntries()
        {
            Console.WriteLine("  Directory Copy Entries Information:");
            Console.WriteLine(value: "  -------------------------");
            if (DirectoryCopyEntries == null || DirectoryCopyEntries.Length == 0)
            {
                Console.WriteLine("  No directory copy entries");
            }
            else
            {
                for (int i = 0; i < DirectoryCopyEntries.Length; i++)
                {
                    var directoryCopyEntry = DirectoryCopyEntries[i];
                    Console.WriteLine($"  Directory Copy Entry {i}");
                    Console.WriteLine($"    Directory index: {directoryCopyEntry.DirectoryIndex}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print directory local entries information
        /// </summary>
        private void PrintDirectoryLocalEntries()
        {
            Console.WriteLine("  Directory Local Entries Information:");
            Console.WriteLine(value: "  -------------------------");
            if (DirectoryLocalEntries == null || DirectoryLocalEntries.Length == 0)
            {
                Console.WriteLine("  No directory local entries");
            }
            else
            {
                for (int i = 0; i < DirectoryLocalEntries.Length; i++)
                {
                    var directoryLocalEntry = DirectoryLocalEntries[i];
                    Console.WriteLine($"  Directory Local Entry {i}");
                    Console.WriteLine($"    Directory index: {directoryLocalEntry.DirectoryIndex}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print unknown header information
        /// </summary>
        private void PrintUnknownHeader()
        {
            Console.WriteLine("  Unknown Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Dummy 0: {UH_Dummy0}");
            Console.WriteLine($"  Dummy 1: {UH_Dummy1}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print unknown entries information
        /// </summary>
        private void PrintUnknownEntries()
        {
            Console.WriteLine("  Unknown Entries Information:");
            Console.WriteLine(value: "  -------------------------");
            if (UnknownEntries == null || UnknownEntries.Length == 0)
            {
                Console.WriteLine("  No unknown entries");
            }
            else
            {
                for (int i = 0; i < UnknownEntries.Length; i++)
                {
                    var unknownEntry = UnknownEntries[i];
                    Console.WriteLine($"  Unknown Entry {i}");
                    Console.WriteLine($"    Dummy 0: {unknownEntry.Dummy0}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print checksum header information
        /// </summary>
        private void PrintChecksumHeader()
        {
            Console.WriteLine("  Checksum Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Dummy 0: {CH_Dummy0}");
            Console.WriteLine($"  Checksum size: {CH_ChecksumSize}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print checksum map header information
        /// </summary>
        private void PrintChecksumMapHeader()
        {
            Console.WriteLine("  Checksum Map Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Dummy 0: {CMH_Dummy0}");
            Console.WriteLine($"  Dummy 1: {CMH_Dummy1}");
            Console.WriteLine($"  Item count: {CMH_ItemCount}");
            Console.WriteLine($"  Checksum count: {CMH_ChecksumCount}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print checksum map entries information
        /// </summary>
        private void PrintChecksumMapEntries()
        {
            Console.WriteLine("  Checksum Map Entries Information:");
            Console.WriteLine(value: "  -------------------------");
            if (ChecksumMapEntries == null || ChecksumMapEntries.Length == 0)
            {
                Console.WriteLine("  No checksum map entries");
            }
            else
            {
                for (int i = 0; i < ChecksumMapEntries.Length; i++)
                {
                    var checksumMapEntry = ChecksumMapEntries[i];
                    Console.WriteLine($"  Checksum Map Entry {i}");
                    Console.WriteLine($"    Checksum count: {checksumMapEntry.ChecksumCount}");
                    Console.WriteLine($"    First checksum index: {checksumMapEntry.FirstChecksumIndex}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print checksum entries information
        /// </summary>
        private void PrintChecksumEntries()
        {
            Console.WriteLine("  Checksum Entries Information:");
            Console.WriteLine(value: "  -------------------------");
            if (ChecksumEntries == null || ChecksumEntries.Length == 0)
            {
                Console.WriteLine("  No checksum entries");
            }
            else
            {
                for (int i = 0; i < ChecksumEntries.Length; i++)
                {
                    var checksumEntry = ChecksumEntries[i];
                    Console.WriteLine($"  Checksum Entry {i}");
                    Console.WriteLine($"    Checksum: {checksumEntry.Checksum}");
                }
            }
            Console.WriteLine();
        }

        #endregion
    }
}