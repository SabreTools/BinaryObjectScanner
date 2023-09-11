using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class GCF : WrapperBase
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string Description => "Half-Life Game Cache File (GCF)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.GCF.Header.Dummy0"/>
        public uint Dummy0 => _file.Header.Dummy0;

        /// <inheritdoc cref="Models.GCF.Header.MajorVersion"/>
        public uint MajorVersion => _file.Header.MajorVersion;

        /// <inheritdoc cref="Models.GCF.Header.MinorVersion"/>
        public uint MinorVersion => _file.Header.MinorVersion;

        /// <inheritdoc cref="Models.GCF.Header.CacheID"/>
        public uint CacheID => _file.Header.CacheID;

        /// <inheritdoc cref="Models.GCF.Header.LastVersionPlayed"/>
        public uint LastVersionPlayed => _file.Header.LastVersionPlayed;

        /// <inheritdoc cref="Models.GCF.Header.Dummy1"/>
        public uint Dummy1 => _file.Header.Dummy1;

        /// <inheritdoc cref="Models.GCF.Header.Dummy2"/>
        public uint Dummy2 => _file.Header.Dummy2;

        /// <inheritdoc cref="Models.GCF.Header.FileSize"/>
        public uint FileSize => _file.Header.FileSize;

        /// <inheritdoc cref="Models.GCF.Header.BlockSize"/>
        public uint BlockSize => _file.Header.BlockSize;

        /// <inheritdoc cref="Models.GCF.Header.BlockCount"/>
        public uint BlockCount => _file.Header.BlockCount;

        /// <inheritdoc cref="Models.GCF.Header.Dummy3"/>
        public uint Dummy3 => _file.Header.Dummy3;

        #endregion

        #region Block Entry Header

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.BlockCount"/>
        public uint BEH_BlockCount => _file.BlockEntryHeader.BlockCount;

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.BlocksUsed"/>
        public uint BEH_BlocksUsed => _file.BlockEntryHeader.BlocksUsed;

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy0"/>
        public uint BEH_Dummy0 => _file.BlockEntryHeader.Dummy0;

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy1"/>
        public uint BEH_Dummy1 => _file.BlockEntryHeader.Dummy1;

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy2"/>
        public uint BEH_Dummy2 => _file.BlockEntryHeader.Dummy2;

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy3"/>
        public uint BEH_Dummy3 => _file.BlockEntryHeader.Dummy3;

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy4"/>
        public uint BEH_Dummy4 => _file.BlockEntryHeader.Dummy4;

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Checksum"/>
        public uint BEH_Checksum => _file.BlockEntryHeader.Checksum;

        #endregion

        #region Block Entries

        /// <inheritdoc cref="Models.GCF.File.BlockEntries"/>
        public SabreTools.Models.GCF.BlockEntry[] BlockEntries => _file.BlockEntries;

        #endregion

        #region Fragmentation Map Header

        /// <inheritdoc cref="Models.GCF.FragmentationMapHeader.BlockCount"/>
        public uint FMH_BlockCount => _file.FragmentationMapHeader.BlockCount;

        /// <inheritdoc cref="Models.GCF.FragmentationMapHeader.FirstUnusedEntry"/>
        public uint FMH_FirstUnusedEntry => _file.FragmentationMapHeader.FirstUnusedEntry;

        /// <inheritdoc cref="Models.GCF.FragmentationMapHeader.Terminator"/>
        public uint FMH_Terminator => _file.FragmentationMapHeader.Terminator;

        /// <inheritdoc cref="Models.GCF.FragmentationMapHeader.Checksum"/>
        public uint FMH_Checksum => _file.FragmentationMapHeader.Checksum;

        #endregion

        #region Fragmentation Maps

        /// <inheritdoc cref="Models.GCF.File.FragmentationMaps"/>
        public SabreTools.Models.GCF.FragmentationMap[] FragmentationMaps => _file.FragmentationMaps;

        #endregion

        #region Block Entry Map Header

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.BlockCount"/>
        public uint? BEMH_BlockCount => _file.BlockEntryMapHeader?.BlockCount;

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.FirstBlockEntryIndex"/>
        public uint? BEMH_FirstBlockEntryIndex => _file.BlockEntryMapHeader?.FirstBlockEntryIndex;

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.LastBlockEntryIndex"/>
        public uint? BEMH_LastBlockEntryIndex => _file.BlockEntryMapHeader?.LastBlockEntryIndex;

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.Dummy0"/>
        public uint? BEMH_Dummy0 => _file.BlockEntryMapHeader?.Dummy0;

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.Checksum"/>
        public uint? BEMH_Checksum => _file.BlockEntryMapHeader?.Checksum;

        #endregion

        #region Block Entry Maps

        /// <inheritdoc cref="Models.GCF.File.BlockEntryMaps"/>
        public SabreTools.Models.GCF.BlockEntryMap[] BlockEntryMaps => _file.BlockEntryMaps;

        #endregion

        #region Directory Header

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Dummy0"/>
        public uint DH_Dummy0 => _file.DirectoryHeader.Dummy0;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.CacheID"/>
        public uint DH_CacheID => _file.DirectoryHeader.CacheID;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.LastVersionPlayed"/>
        public uint DH_LastVersionPlayed => _file.DirectoryHeader.LastVersionPlayed;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.ItemCount"/>
        public uint DH_ItemCount => _file.DirectoryHeader.ItemCount;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.FileCount"/>
        public uint DH_FileCount => _file.DirectoryHeader.FileCount;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Dummy1"/>
        public uint DH_Dummy1 => _file.DirectoryHeader.Dummy1;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.DirectorySize"/>
        public uint DH_DirectorySize => _file.DirectoryHeader.DirectorySize;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.NameSize"/>
        public uint DH_NameSize => _file.DirectoryHeader.NameSize;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Info1Count"/>
        public uint DH_Info1Count => _file.DirectoryHeader.Info1Count;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.CopyCount"/>
        public uint DH_CopyCount => _file.DirectoryHeader.CopyCount;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.LocalCount"/>
        public uint DH_LocalCount => _file.DirectoryHeader.LocalCount;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Dummy2"/>
        public uint DH_Dummy2 => _file.DirectoryHeader.Dummy2;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Dummy3"/>
        public uint DH_Dummy3 => _file.DirectoryHeader.Dummy3;

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Checksum"/>
        public uint DH_Checksum => _file.DirectoryHeader.Checksum;

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryEntries"/>
        public SabreTools.Models.GCF.DirectoryEntry[] DirectoryEntries => _file.DirectoryEntries;

        #endregion

        #region Directory Names

        /// <inheritdoc cref="Models.GCF.File.DirectoryNames"/>
        public System.Collections.Generic.Dictionary<long, string> DirectoryNames => _file.DirectoryNames;

        #endregion

        #region Directory Info 1 Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryInfo1Entries"/>
        public SabreTools.Models.GCF.DirectoryInfo1Entry[] DirectoryInfo1Entries => _file.DirectoryInfo1Entries;

        #endregion

        #region Directory Info 2 Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryInfo2Entries"/>
        public SabreTools.Models.GCF.DirectoryInfo2Entry[] DirectoryInfo2Entries => _file.DirectoryInfo2Entries;

        #endregion

        #region Directory Copy Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryCopyEntries"/>
        public SabreTools.Models.GCF.DirectoryCopyEntry[] DirectoryCopyEntries => _file.DirectoryCopyEntries;

        #endregion

        #region Directory Local Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryLocalEntries"/>
        public SabreTools.Models.GCF.DirectoryLocalEntry[] DirectoryLocalEntries => _file.DirectoryLocalEntries;

        #endregion

        #region Directory Map Header

        /// <inheritdoc cref="Models.GCF.DirectoryMapHeader.Dummy0"/>
        public uint? DMH_Dummy0 => _file.DirectoryMapHeader?.Dummy0;

        /// <inheritdoc cref="Models.GCF.DirectoryMapHeader.Dummy1"/>
        public uint? DMH_Dummy1 => _file.DirectoryMapHeader?.Dummy1;

        #endregion

        #region Directory Map Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryMapEntries"/>
        public SabreTools.Models.GCF.DirectoryMapEntry[] DirectoryMapEntries => _file.DirectoryMapEntries;

        #endregion

        #region Checksum Header

        /// <inheritdoc cref="Models.GCF.ChecksumHeader.Dummy0"/>
        public uint CH_Dummy0 => _file.ChecksumHeader.Dummy0;

        /// <inheritdoc cref="Models.GCF.ChecksumHeader.ChecksumSize"/>
        public uint CH_ChecksumSize => _file.ChecksumHeader.ChecksumSize;

        #endregion

        #region Checksum Map Header

        /// <inheritdoc cref="Models.GCF.ChecksumMapHeader.Dummy0"/>
        public uint CMH_Dummy0 => _file.ChecksumMapHeader.Dummy0;

        /// <inheritdoc cref="Models.GCF.ChecksumMapHeader.Dummy1"/>
        public uint CMH_Dummy1 => _file.ChecksumMapHeader.Dummy1;

        /// <inheritdoc cref="Models.GCF.ChecksumMapHeader.ItemCount"/>
        public uint CMH_ItemCount => _file.ChecksumMapHeader.ItemCount;

        /// <inheritdoc cref="Models.GCF.ChecksumMapHeader.ChecksumCount"/>
        public uint CMH_ChecksumCount => _file.ChecksumMapHeader.ChecksumCount;

        #endregion

        #region Checksum Map Entries

        /// <inheritdoc cref="Models.GCF.File.ChecksumMapEntries"/>
        public SabreTools.Models.GCF.ChecksumMapEntry[] ChecksumMapEntries => _file.ChecksumMapEntries;

        #endregion

        #region Checksum Entries

        /// <inheritdoc cref="Models.GCF.File.ChecksumEntries"/>
        public SabreTools.Models.GCF.ChecksumEntry[] ChecksumEntries => _file.ChecksumEntries;

        #endregion

        #region Data Block Header

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.LastVersionPlayed"/>
        public uint DBH_LastVersionPlayed => _file.DataBlockHeader.LastVersionPlayed;

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.BlockCount"/>
        public uint DBH_BlockCount => _file.DataBlockHeader.BlockCount;

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.BlockSize"/>
        public uint DBH_BlockSize => _file.DataBlockHeader.BlockSize;

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.FirstBlockOffset"/>
        public uint DBH_FirstBlockOffset => _file.DataBlockHeader.FirstBlockOffset;

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.BlocksUsed"/>
        public uint DBH_BlocksUsed => _file.DataBlockHeader.BlocksUsed;

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.Checksum"/>
        public uint DBH_Checksum => _file.DataBlockHeader.Checksum;

        #endregion

        #endregion

        #region Extension Properties

        /// <summary>
        /// Set of all files and their information
        /// </summary>
        public FileInfo[] Files
        {
            get
            {
                // Use the cached value if we have it
                if (_files != null)
                    return _files;

                // Otherwise, scan and build the files
                var files = new List<FileInfo>();
                for (int i = 0; i < DirectoryEntries.Length; i++)
                {
                    // Get the directory entry
                    var directoryEntry = DirectoryEntries[i];
                    var directoryMapEntry = DirectoryMapEntries[i];

                    // If we have a directory, skip for now
                    if (!directoryEntry.DirectoryFlags.HasFlag(SabreTools.Models.GCF.HL_GCF_FLAG.HL_GCF_FLAG_FILE))
                        continue;

                    // Otherwise, start building the file info
                    var fileInfo = new FileInfo()
                    {
                        Size = directoryEntry.ItemSize,
                        Encrypted = directoryEntry.DirectoryFlags.HasFlag(SabreTools.Models.GCF.HL_GCF_FLAG.HL_GCF_FLAG_ENCRYPTED),
                    };
                    var pathParts = new List<string> { directoryEntry.Name };
                    var blockEntries = new List<SabreTools.Models.GCF.BlockEntry>();

                    // Traverse the parent tree
                    uint index = directoryEntry.ParentIndex;
                    while (index != 0xFFFFFFFF)
                    {
                        var parentDirectoryEntry = DirectoryEntries[index];
                        pathParts.Add(parentDirectoryEntry.Name);
                        index = parentDirectoryEntry.ParentIndex;
                    }

                    // Traverse the block entries
                    index = directoryMapEntry.FirstBlockIndex;
                    while (index != DBH_BlockCount)
                    {
                        var nextBlock = BlockEntries[index];
                        blockEntries.Add(nextBlock);
                        index = nextBlock.NextBlockEntryIndex;
                    }

                    // Reverse the path parts because of traversal
                    pathParts.Reverse();

                    // Build the remaining file info
                    fileInfo.Path = Path.Combine(pathParts.ToArray());
                    fileInfo.BlockEntries = blockEntries.ToArray();

                    // Add the file info and continue
                    files.Add(fileInfo);
                }

                // Set and return the file infos
                _files = files.ToArray();
                return _files;
            }
        }

        /// <summary>
        /// Set of all data block offsets
        /// </summary>
        public long[] DataBlockOffsets
        {
            get
            {
                // Use the cached value if we have it
                if (_dataBlockOffsets != null)
                    return _dataBlockOffsets;

                // Otherwise, build the data block set
                _dataBlockOffsets = new long[DBH_BlockCount];
                for (int i = 0; i < DBH_BlockCount; i++)
                {
                    long dataBlockOffset = DBH_FirstBlockOffset + (i * DBH_BlockSize);
                    _dataBlockOffsets[i] = dataBlockOffset;
                }

                // Return the set of data blocks
                return _dataBlockOffsets;
            }
        }

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the GCF
        /// </summary>
        private SabreTools.Models.GCF.File _file;

        /// <summary>
        /// Set of all files and their information
        /// </summary>
        private FileInfo[] _files = null;

        /// <summary>
        /// Set of all data block offsets
        /// </summary>
        private long[] _dataBlockOffsets = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private GCF() { }

        /// <summary>
        /// Create an GCF from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the GCF</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An GCF wrapper on success, null on failure</returns>
        public static GCF Create(byte[] data, int offset)
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
        /// Create a GCF from a Stream
        /// </summary>
        /// <param name="data">Stream representing the GCF</param>
        /// <returns>An GCF wrapper on success, null on failure</returns>
        public static GCF Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.GCF().Deserialize(data);
            if (file == null)
                return null;

            var wrapper = new GCF
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
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("GCF Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Header
            PrintHeader(builder);

            // Block Entries
            PrintBlockEntryHeader(builder);
            PrintBlockEntries(builder);

            // Fragmentation Maps
            PrintFragmentationMapHeader(builder);
            PrintFragmentationMaps(builder);

            // Block Entry Maps
            PrintBlockEntryMapHeader(builder);
            PrintBlockEntryMaps(builder);

            // Directory and Directory Maps
            PrintDirectoryHeader(builder);
            PrintDirectoryEntries(builder);
            // TODO: Should we print out the entire string table?
            PrintDirectoryInfo1Entries(builder);
            PrintDirectoryInfo2Entries(builder);
            PrintDirectoryCopyEntries(builder);
            PrintDirectoryLocalEntries(builder);
            PrintDirectoryMapHeader(builder);
            PrintDirectoryMapEntries(builder);

            // Checksums and Checksum Maps
            PrintChecksumHeader(builder);
            PrintChecksumMapHeader(builder);
            PrintChecksumMapEntries(builder);
            PrintChecksumEntries(builder);

            // Data Blocks
            PrintDataBlockHeader(builder);

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
            builder.AppendLine($"  Dummy 0: {Dummy0} (0x{Dummy0:X})");
            builder.AppendLine($"  Major version: {MajorVersion} (0x{MajorVersion:X})");
            builder.AppendLine($"  Minor version: {MinorVersion} (0x{MinorVersion:X})");
            builder.AppendLine($"  Cache ID: {CacheID} (0x{CacheID:X})");
            builder.AppendLine($"  Last version played: {LastVersionPlayed} (0x{LastVersionPlayed:X})");
            builder.AppendLine($"  Dummy 1: {Dummy1} (0x{Dummy1:X})");
            builder.AppendLine($"  Dummy 2: {Dummy2} (0x{Dummy2:X})");
            builder.AppendLine($"  File size: {FileSize} (0x{FileSize:X})");
            builder.AppendLine($"  Block size: {BlockSize} (0x{BlockSize:X})");
            builder.AppendLine($"  Block count: {BlockCount} (0x{BlockCount:X})");
            builder.AppendLine($"  Dummy 3: {Dummy3} (0x{Dummy3:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print block entry header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintBlockEntryHeader(StringBuilder builder)
        {
            builder.AppendLine("  Block Entry Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Block count: {BEH_BlockCount} (0x{BEH_BlockCount:X})");
            builder.AppendLine($"  Blocks used: {BEH_BlocksUsed} (0x{BEH_BlocksUsed:X})");
            builder.AppendLine($"  Dummy 0: {BEH_Dummy0} (0x{BEH_Dummy0:X})");
            builder.AppendLine($"  Dummy 1: {BEH_Dummy1} (0x{BEH_Dummy1:X})");
            builder.AppendLine($"  Dummy 2: {BEH_Dummy2} (0x{BEH_Dummy2:X})");
            builder.AppendLine($"  Dummy 3: {BEH_Dummy3} (0x{BEH_Dummy3:X})");
            builder.AppendLine($"  Dummy 4: {BEH_Dummy4} (0x{BEH_Dummy4:X})");
            builder.AppendLine($"  Checksum: {BEH_Checksum} (0x{BEH_Checksum:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print block entries information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintBlockEntries(StringBuilder builder)
        {
            builder.AppendLine("  Block Entries Information:");
            builder.AppendLine("  -------------------------");
            if (BlockEntries == null || BlockEntries.Length == 0)
            {
                builder.AppendLine("  No block entries");
            }
            else
            {
                for (int i = 0; i < BlockEntries.Length; i++)
                {
                    var blockEntry = BlockEntries[i];
                    builder.AppendLine($"  Block Entry {i}");
                    builder.AppendLine($"    Entry flags: {blockEntry.EntryFlags} (0x{blockEntry.EntryFlags:X})");
                    builder.AppendLine($"    File data offset: {blockEntry.FileDataOffset} (0x{blockEntry.FileDataOffset:X})");
                    builder.AppendLine($"    File data size: {blockEntry.FileDataSize} (0x{blockEntry.FileDataSize:X})");
                    builder.AppendLine($"    First data block index: {blockEntry.FirstDataBlockIndex} (0x{blockEntry.FirstDataBlockIndex:X})");
                    builder.AppendLine($"    Next block entry index: {blockEntry.NextBlockEntryIndex} (0x{blockEntry.NextBlockEntryIndex:X})");
                    builder.AppendLine($"    Previous block entry index: {blockEntry.PreviousBlockEntryIndex} (0x{blockEntry.PreviousBlockEntryIndex:X})");
                    builder.AppendLine($"    Directory index: {blockEntry.DirectoryIndex} (0x{blockEntry.DirectoryIndex:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print fragmentation map header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFragmentationMapHeader(StringBuilder builder)
        {
            builder.AppendLine("  Fragmentation Map Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Block count: {FMH_BlockCount} (0x{FMH_BlockCount:X})");
            builder.AppendLine($"  First unused entry: {FMH_FirstUnusedEntry} (0x{FMH_FirstUnusedEntry:X})");
            builder.AppendLine($"  Terminator: {FMH_Terminator} (0x{FMH_Terminator:X})");
            builder.AppendLine($"  Checksum: {FMH_Checksum} (0x{FMH_Checksum:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print fragmentation maps information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFragmentationMaps(StringBuilder builder)
        {
            builder.AppendLine("  Fragmentation Maps Information:");
            builder.AppendLine("  -------------------------");
            if (FragmentationMaps == null || FragmentationMaps.Length == 0)
            {
                builder.AppendLine("  No fragmentation maps");
            }
            else
            {
                for (int i = 0; i < FragmentationMaps.Length; i++)
                {
                    var fragmentationMap = FragmentationMaps[i];
                    builder.AppendLine($"  Fragmentation Map {i}");
                    builder.AppendLine($"    Next data block index: {fragmentationMap.NextDataBlockIndex} (0x{fragmentationMap.NextDataBlockIndex:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print block entry map header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintBlockEntryMapHeader(StringBuilder builder)
        {
            builder.AppendLine("  Block Entry Map Header Information:");
            builder.AppendLine("  -------------------------");
            if (_file.BlockEntryMapHeader == null)
            {
                builder.AppendLine($"  No block entry map header");
            }
            else
            {
                builder.AppendLine($"  Block count: {BEMH_BlockCount} (0x{BEMH_BlockCount:X})");
                builder.AppendLine($"  First block entry index: {BEMH_FirstBlockEntryIndex} (0x{BEMH_FirstBlockEntryIndex:X})");
                builder.AppendLine($"  Last block entry index: {BEMH_LastBlockEntryIndex} (0x{BEMH_LastBlockEntryIndex:X})");
                builder.AppendLine($"  Dummy 0: {BEMH_Dummy0} (0x{BEMH_Dummy0:X})");
                builder.AppendLine($"  Checksum: {BEMH_Checksum} (0x{BEMH_Checksum:X})");
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print block entry maps information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintBlockEntryMaps(StringBuilder builder)
        {
            builder.AppendLine("  Block Entry Maps Information:");
            builder.AppendLine("  -------------------------");
            if (BlockEntryMaps == null || BlockEntryMaps.Length == 0)
            {
                builder.AppendLine("  No block entry maps");
            }
            else
            {
                for (int i = 0; i < BlockEntryMaps.Length; i++)
                {
                    var blockEntryMap = BlockEntryMaps[i];
                    builder.AppendLine($"  Block Entry Map {i}");
                    builder.AppendLine($"    Previous data block index: {blockEntryMap.PreviousBlockEntryIndex} (0x{blockEntryMap.PreviousBlockEntryIndex:X})");
                    builder.AppendLine($"    Next data block index: {blockEntryMap.NextBlockEntryIndex} (0x{blockEntryMap.NextBlockEntryIndex:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print directory header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryHeader(StringBuilder builder)
        {
            builder.AppendLine("  Directory Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Dummy 0: {DH_Dummy0} (0x{DH_Dummy0:X})");
            builder.AppendLine($"  Cache ID: {DH_CacheID} (0x{DH_CacheID:X})");
            builder.AppendLine($"  Last version played: {DH_LastVersionPlayed} (0x{DH_LastVersionPlayed:X})");
            builder.AppendLine($"  Item count: {DH_ItemCount} (0x{DH_ItemCount:X})");
            builder.AppendLine($"  File count: {DH_FileCount} (0x{DH_FileCount:X})");
            builder.AppendLine($"  Dummy 1: {DH_Dummy1} (0x{DH_Dummy1:X})");
            builder.AppendLine($"  Directory size: {DH_DirectorySize} (0x{DH_DirectorySize:X})");
            builder.AppendLine($"  Name size: {DH_NameSize} (0x{DH_NameSize:X})");
            builder.AppendLine($"  Info 1 count: {DH_Info1Count} (0x{DH_Info1Count:X})");
            builder.AppendLine($"  Copy count: {DH_CopyCount} (0x{DH_CopyCount:X})");
            builder.AppendLine($"  Local count: {DH_LocalCount} (0x{DH_LocalCount:X})");
            builder.AppendLine($"  Dummy 2: {DH_Dummy2} (0x{DH_Dummy2:X})");
            builder.AppendLine($"  Dummy 3: {DH_Dummy3} (0x{DH_Dummy3:X})");
            builder.AppendLine($"  Checksum: {DH_Checksum} (0x{DH_Checksum:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print directory entries information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryEntries(StringBuilder builder)
        {
            builder.AppendLine("  Directory Entries Information:");
            builder.AppendLine("  -------------------------");
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
            {
                builder.AppendLine("  No directory entries");
            }
            else
            {
                for (int i = 0; i < DirectoryEntries.Length; i++)
                {
                    var directoryEntry = DirectoryEntries[i];
                    builder.AppendLine($"  Directory Entry {i}");
                    builder.AppendLine($"    Name offset: {directoryEntry.NameOffset} (0x{directoryEntry.NameOffset:X})");
                    builder.AppendLine($"    Name: {directoryEntry.Name ?? "[NULL]"}");
                    builder.AppendLine($"    Item size: {directoryEntry.ItemSize} (0x{directoryEntry.ItemSize:X})");
                    builder.AppendLine($"    Checksum index: {directoryEntry.ChecksumIndex} (0x{directoryEntry.ChecksumIndex:X})");
                    builder.AppendLine($"    Directory flags: {directoryEntry.DirectoryFlags} (0x{directoryEntry.DirectoryFlags:X})");
                    builder.AppendLine($"    Parent index: {directoryEntry.ParentIndex} (0x{directoryEntry.ParentIndex:X})");
                    builder.AppendLine($"    Next index: {directoryEntry.NextIndex} (0x{directoryEntry.NextIndex:X})");
                    builder.AppendLine($"    First index: {directoryEntry.FirstIndex} (0x{directoryEntry.FirstIndex:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print directory info 1 entries information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryInfo1Entries(StringBuilder builder)
        {
            builder.AppendLine("  Directory Info 1 Entries Information:");
            builder.AppendLine("  -------------------------");
            if (DirectoryInfo1Entries == null || DirectoryInfo1Entries.Length == 0)
            {
                builder.AppendLine("  No directory info 1 entries");
            }
            else
            {
                for (int i = 0; i < DirectoryInfo1Entries.Length; i++)
                {
                    var directoryInfoEntry = DirectoryInfo1Entries[i];
                    builder.AppendLine($"  Directory Info 1 Entry {i}");
                    builder.AppendLine($"    Dummy 0: {directoryInfoEntry.Dummy0} (0x{directoryInfoEntry.Dummy0:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print directory info 2 entries information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryInfo2Entries(StringBuilder builder)
        {
            builder.AppendLine("  Directory Info 2 Entries Information:");
            builder.AppendLine("  -------------------------");
            if (DirectoryInfo2Entries == null || DirectoryInfo2Entries.Length == 0)
            {
                builder.AppendLine("  No directory info 2 entries");
            }
            else
            {
                for (int i = 0; i < DirectoryInfo2Entries.Length; i++)
                {
                    var directoryInfoEntry = DirectoryInfo2Entries[i];
                    builder.AppendLine($"  Directory Info 2 Entry {i}");
                    builder.AppendLine($"    Dummy 0: {directoryInfoEntry.Dummy0} (0x{directoryInfoEntry.Dummy0:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print directory copy entries information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryCopyEntries(StringBuilder builder)
        {
            builder.AppendLine("  Directory Copy Entries Information:");
            builder.AppendLine(value: "  -------------------------");
            if (DirectoryCopyEntries == null || DirectoryCopyEntries.Length == 0)
            {
                builder.AppendLine("  No directory copy entries");
            }
            else
            {
                for (int i = 0; i < DirectoryCopyEntries.Length; i++)
                {
                    var directoryCopyEntry = DirectoryCopyEntries[i];
                    builder.AppendLine($"  Directory Copy Entry {i}");
                    builder.AppendLine($"    Directory index: {directoryCopyEntry.DirectoryIndex} (0x{directoryCopyEntry.DirectoryIndex:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print directory local entries information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryLocalEntries(StringBuilder builder)
        {
            builder.AppendLine("  Directory Local Entries Information:");
            builder.AppendLine(value: "  -------------------------");
            if (DirectoryLocalEntries == null || DirectoryLocalEntries.Length == 0)
            {
                builder.AppendLine("  No directory local entries");
            }
            else
            {
                for (int i = 0; i < DirectoryLocalEntries.Length; i++)
                {
                    var directoryLocalEntry = DirectoryLocalEntries[i];
                    builder.AppendLine($"  Directory Local Entry {i}");
                    builder.AppendLine($"    Directory index: {directoryLocalEntry.DirectoryIndex} (0x{directoryLocalEntry.DirectoryIndex:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print directory map header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryMapHeader(StringBuilder builder)
        {
            builder.AppendLine("  Directory Map Header Information:");
            builder.AppendLine("  -------------------------");
            if (_file.DirectoryMapHeader == null)
            {
                builder.AppendLine($"  No directory map header");
            }
            else
            {
                builder.AppendLine($"  Dummy 0: {DMH_Dummy0} (0x{DMH_Dummy0:X})");
                builder.AppendLine($"  Dummy 1: {DMH_Dummy1} (0x{DMH_Dummy1:X})");
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print directory map entries information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryMapEntries(StringBuilder builder)
        {
            builder.AppendLine("  Directory Map Entries Information:");
            builder.AppendLine(value: "  -------------------------");
            if (DirectoryMapEntries == null || DirectoryMapEntries.Length == 0)
            {
                builder.AppendLine("  No directory map entries");
            }
            else
            {
                for (int i = 0; i < DirectoryMapEntries.Length; i++)
                {
                    var directoryMapEntry = DirectoryMapEntries[i];
                    builder.AppendLine($"  Directory Map Entry {i}");
                    builder.AppendLine($"    First block index: {directoryMapEntry.FirstBlockIndex} (0x{directoryMapEntry.FirstBlockIndex:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print checksum header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintChecksumHeader(StringBuilder builder)
        {
            builder.AppendLine("  Checksum Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Dummy 0: {CH_Dummy0} (0x{CH_Dummy0:X})");
            builder.AppendLine($"  Checksum size: {CH_ChecksumSize} (0x{CH_ChecksumSize:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print checksum map header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintChecksumMapHeader(StringBuilder builder)
        {
            builder.AppendLine("  Checksum Map Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Dummy 0: {CMH_Dummy0} (0x{CMH_Dummy0:X})");
            builder.AppendLine($"  Dummy 1: {CMH_Dummy1} (0x{CMH_Dummy1:X})");
            builder.AppendLine($"  Item count: {CMH_ItemCount} (0x{CMH_ItemCount:X})");
            builder.AppendLine($"  Checksum count: {CMH_ChecksumCount} (0x{CMH_ChecksumCount:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print checksum map entries information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintChecksumMapEntries(StringBuilder builder)
        {
            builder.AppendLine("  Checksum Map Entries Information:");
            builder.AppendLine(value: "  -------------------------");
            if (ChecksumMapEntries == null || ChecksumMapEntries.Length == 0)
            {
                builder.AppendLine("  No checksum map entries");
            }
            else
            {
                for (int i = 0; i < ChecksumMapEntries.Length; i++)
                {
                    var checksumMapEntry = ChecksumMapEntries[i];
                    builder.AppendLine($"  Checksum Map Entry {i}");
                    builder.AppendLine($"    Checksum count: {checksumMapEntry.ChecksumCount} (0x{checksumMapEntry.ChecksumCount:X})");
                    builder.AppendLine($"    First checksum index: {checksumMapEntry.FirstChecksumIndex} (0x{checksumMapEntry.FirstChecksumIndex:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print checksum entries information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintChecksumEntries(StringBuilder builder)
        {
            builder.AppendLine("  Checksum Entries Information:");
            builder.AppendLine(value: "  -------------------------");
            if (ChecksumEntries == null || ChecksumEntries.Length == 0)
            {
                builder.AppendLine("  No checksum entries");
            }
            else
            {
                for (int i = 0; i < ChecksumEntries.Length; i++)
                {
                    var checksumEntry = ChecksumEntries[i];
                    builder.AppendLine($"  Checksum Entry {i}");
                    builder.AppendLine($"    Checksum: {checksumEntry.Checksum} (0x{checksumEntry.Checksum:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print data block header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDataBlockHeader(StringBuilder builder)
        {
            builder.AppendLine("  Data Block Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Last version played: {DBH_LastVersionPlayed} (0x{DBH_LastVersionPlayed:X})");
            builder.AppendLine($"  Block count: {DBH_BlockCount} (0x{DBH_BlockCount:X})");
            builder.AppendLine($"  Block size: {DBH_BlockSize} (0x{DBH_BlockSize:X})");
            builder.AppendLine($"  First block offset: {DBH_FirstBlockOffset} (0x{DBH_FirstBlockOffset:X})");
            builder.AppendLine($"  Blocks used: {DBH_BlocksUsed} (0x{DBH_BlocksUsed:X})");
            builder.AppendLine($"  Checksum: {DBH_Checksum} (0x{DBH_Checksum:X})");
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_file, _jsonSerializerOptions);

#endif

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the GCF to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
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
        /// Extract a file from the GCF to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no files
            if (Files == null || Files.Length == 0)
                return false;

            // If the files index is invalid
            if (index < 0 || index >= Files.Length)
                return false;

            // Get the file
            var file = Files[index];
            if (file.Size == 0)
                return false;

            // If the file is encrypted -- TODO: Revisit later
            if (file.Encrypted)
                return false;

            // Get all data block offsets needed for extraction
            var dataBlockOffsets = new List<long>();
            for (int i = 0; i < file.BlockEntries.Length; i++)
            {
                var blockEntry = file.BlockEntries[i];

                uint dataBlockIndex = blockEntry.FirstDataBlockIndex;
                long blockEntrySize = blockEntry.FileDataSize;
                while (blockEntrySize > 0)
                {
                    long dataBlockOffset = DataBlockOffsets[dataBlockIndex++];
                    dataBlockOffsets.Add(dataBlockOffset);
                    blockEntrySize -= DBH_BlockSize;
                }
            }

            // Create the filename
            string filename = file.Path;

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
            Directory.CreateDirectory(Path.GetDirectoryName(filename));

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    // Now read the data sequentially and write out while we have data left
                    long fileSize = file.Size;
                    for (int i = 0; i < dataBlockOffsets.Count; i++)
                    {
                        int readSize = (int)Math.Min(DBH_BlockSize, fileSize);

                        byte[] data = ReadFromDataSource((int)dataBlockOffsets[i], readSize);
                        fs.Write(data, 0, data.Length);
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Class to contain all necessary file information
        /// </summary>
        public sealed class FileInfo
        {
            /// <summary>
            /// Full item path
            /// </summary>
            public string Path;

            /// <summary>
            /// File size
            /// </summary>
            public uint Size;

            /// <summary>
            /// Indicates if the block is encrypted
            /// </summary>
            public bool Encrypted;

            /// <summary>
            /// Array of block entries
            /// </summary>
            public SabreTools.Models.GCF.BlockEntry[] BlockEntries;
        }

        #endregion
    }
}