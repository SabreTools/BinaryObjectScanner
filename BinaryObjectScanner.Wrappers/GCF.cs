using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class GCF : WrapperBase<SabreTools.Models.GCF.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life Game Cache File (GCF)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.GCF.Header.Dummy0"/>
#if NET48
        public uint Dummy0 => _model.Header.Dummy0;
#else
        public uint? Dummy0 => _model.Header?.Dummy0;
#endif

        /// <inheritdoc cref="Models.GCF.Header.MajorVersion"/>
#if NET48
        public uint MajorVersion => _model.Header.MajorVersion;
#else
        public uint? MajorVersion => _model.Header?.MajorVersion;
#endif

        /// <inheritdoc cref="Models.GCF.Header.MinorVersion"/>
#if NET48
        public uint MinorVersion => _model.Header.MinorVersion;
#else
        public uint? MinorVersion => _model.Header?.MinorVersion;
#endif

        /// <inheritdoc cref="Models.GCF.Header.CacheID"/>
#if NET48
        public uint CacheID => _model.Header.CacheID;
#else
        public uint? CacheID => _model.Header?.CacheID;
#endif

        /// <inheritdoc cref="Models.GCF.Header.LastVersionPlayed"/>
#if NET48
        public uint LastVersionPlayed => _model.Header.LastVersionPlayed;
#else
        public uint? LastVersionPlayed => _model.Header?.LastVersionPlayed;
#endif

        /// <inheritdoc cref="Models.GCF.Header.Dummy1"/>
#if NET48
        public uint Dummy1 => _model.Header.Dummy1;
#else
        public uint? Dummy1 => _model.Header?.Dummy1;
#endif

        /// <inheritdoc cref="Models.GCF.Header.Dummy2"/>
#if NET48
        public uint Dummy2 => _model.Header.Dummy2;
#else
        public uint? Dummy2 => _model.Header?.Dummy2;
#endif

        /// <inheritdoc cref="Models.GCF.Header.FileSize"/>
#if NET48
        public uint FileSize => _model.Header.FileSize;
#else
        public uint? FileSize => _model.Header?.FileSize;
#endif

        /// <inheritdoc cref="Models.GCF.Header.BlockSize"/>
#if NET48
        public uint BlockSize => _model.Header.BlockSize;
#else
        public uint? BlockSize => _model.Header?.BlockSize;
#endif

        /// <inheritdoc cref="Models.GCF.Header.BlockCount"/>
#if NET48
        public uint BlockCount => _model.Header.BlockCount;
#else
        public uint? BlockCount => _model.Header?.BlockCount;
#endif

        /// <inheritdoc cref="Models.GCF.Header.Dummy3"/>
#if NET48
        public uint Dummy3 => _model.Header.Dummy3;
#else
        public uint? Dummy3 => _model.Header?.Dummy3;
#endif

        #endregion

        #region Block Entry Header

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.BlockCount"/>
#if NET48
        public uint BEH_BlockCount => _model.BlockEntryHeader.BlockCount;
#else
        public uint? BEH_BlockCount => _model.BlockEntryHeader?.BlockCount;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.BlocksUsed"/>
#if NET48
        public uint BEH_BlocksUsed => _model.BlockEntryHeader.BlocksUsed;
#else
        public uint? BEH_BlocksUsed => _model.BlockEntryHeader?.BlocksUsed;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy0"/>
#if NET48
        public uint BEH_Dummy0 => _model.BlockEntryHeader.Dummy0;
#else
        public uint? BEH_Dummy0 => _model.BlockEntryHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy1"/>
#if NET48
        public uint BEH_Dummy1 => _model.BlockEntryHeader.Dummy1;
#else
        public uint? BEH_Dummy1 => _model.BlockEntryHeader?.Dummy1;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy2"/>
#if NET48
        public uint BEH_Dummy2 => _model.BlockEntryHeader.Dummy2;
#else
        public uint? BEH_Dummy2 => _model.BlockEntryHeader?.Dummy2;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy3"/>
#if NET48
        public uint BEH_Dummy3 => _model.BlockEntryHeader.Dummy3;
#else
        public uint? BEH_Dummy3 => _model.BlockEntryHeader?.Dummy3;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy4"/>
#if NET48
        public uint BEH_Dummy4 => _model.BlockEntryHeader.Dummy4;
#else
        public uint? BEH_Dummy4 => _model.BlockEntryHeader?.Dummy4;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Checksum"/>
#if NET48
        public uint BEH_Checksum => _model.BlockEntryHeader.Checksum;
#else
        public uint? BEH_Checksum => _model.BlockEntryHeader?.Checksum;
#endif

        #endregion

        #region Block Entries

        /// <inheritdoc cref="Models.GCF.File.BlockEntries"/>
#if NET48
        public SabreTools.Models.GCF.BlockEntry[] BlockEntries => _model.BlockEntries;
#else
        public SabreTools.Models.GCF.BlockEntry?[]? BlockEntries => _model.BlockEntries;
#endif

        #endregion

        #region Fragmentation Map Header

        /// <inheritdoc cref="Models.GCF.FragmentationMapHeader.BlockCount"/>
#if NET48
        public uint FMH_BlockCount => _model.FragmentationMapHeader.BlockCount;
#else
        public uint? FMH_BlockCount => _model.FragmentationMapHeader?.BlockCount;
#endif

        /// <inheritdoc cref="Models.GCF.FragmentationMapHeader.FirstUnusedEntry"/>
#if NET48
        public uint FMH_FirstUnusedEntry => _model.FragmentationMapHeader.FirstUnusedEntry;
#else
        public uint? FMH_FirstUnusedEntry => _model.FragmentationMapHeader?.FirstUnusedEntry;
#endif

        /// <inheritdoc cref="Models.GCF.FragmentationMapHeader.Terminator"/>
#if NET48
        public uint FMH_Terminator => _model.FragmentationMapHeader.Terminator;
#else
        public uint? FMH_Terminator => _model.FragmentationMapHeader?.Terminator;
#endif

        /// <inheritdoc cref="Models.GCF.FragmentationMapHeader.Checksum"/>
#if NET48
        public uint FMH_Checksum => _model.FragmentationMapHeader.Checksum;
#else
        public uint? FMH_Checksum => _model.FragmentationMapHeader?.Checksum;
#endif

        #endregion

        #region Fragmentation Maps

        /// <inheritdoc cref="Models.GCF.File.FragmentationMaps"/>
#if NET48
        public SabreTools.Models.GCF.FragmentationMap[] FragmentationMaps => _model.FragmentationMaps;
#else
        public SabreTools.Models.GCF.FragmentationMap?[]? FragmentationMaps => _model.FragmentationMaps;
#endif

        #endregion

        #region Block Entry Map Header

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.BlockCount"/>
        public uint? BEMH_BlockCount => _model.BlockEntryMapHeader?.BlockCount;

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.FirstBlockEntryIndex"/>
        public uint? BEMH_FirstBlockEntryIndex => _model.BlockEntryMapHeader?.FirstBlockEntryIndex;

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.LastBlockEntryIndex"/>
        public uint? BEMH_LastBlockEntryIndex => _model.BlockEntryMapHeader?.LastBlockEntryIndex;

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.Dummy0"/>
        public uint? BEMH_Dummy0 => _model.BlockEntryMapHeader?.Dummy0;

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.Checksum"/>
        public uint? BEMH_Checksum => _model.BlockEntryMapHeader?.Checksum;

        #endregion

        #region Block Entry Maps

        /// <inheritdoc cref="Models.GCF.File.BlockEntryMaps"/>
#if NET48
        public SabreTools.Models.GCF.BlockEntryMap[] BlockEntryMaps => _model.BlockEntryMaps;
#else
        public SabreTools.Models.GCF.BlockEntryMap?[]? BlockEntryMaps => _model.BlockEntryMaps;
#endif

        #endregion

        #region Directory Header

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Dummy0"/>
#if NET48
        public uint DH_Dummy0 => _model.DirectoryHeader.Dummy0;
#else
        public uint? DH_Dummy0 => _model.DirectoryHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.CacheID"/>
#if NET48
        public uint DH_CacheID => _model.DirectoryHeader.CacheID;
#else
        public uint? DH_CacheID => _model.DirectoryHeader?.CacheID;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.LastVersionPlayed"/>
#if NET48
        public uint DH_LastVersionPlayed => _model.DirectoryHeader.LastVersionPlayed;
#else
        public uint? DH_LastVersionPlayed => _model.DirectoryHeader?.LastVersionPlayed;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.ItemCount"/>
#if NET48
        public uint DH_ItemCount => _model.DirectoryHeader.ItemCount;
#else
        public uint? DH_ItemCount => _model.DirectoryHeader?.ItemCount;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.FileCount"/>
#if NET48
        public uint DH_FileCount => _model.DirectoryHeader.FileCount;
#else
        public uint? DH_FileCount => _model.DirectoryHeader?.FileCount;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Dummy1"/>
#if NET48
        public uint DH_Dummy1 => _model.DirectoryHeader.Dummy1;
#else
        public uint? DH_Dummy1 => _model.DirectoryHeader?.Dummy1;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.DirectorySize"/>
#if NET48
        public uint DH_DirectorySize => _model.DirectoryHeader.DirectorySize;
#else
        public uint? DH_DirectorySize => _model.DirectoryHeader?.DirectorySize;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.NameSize"/>
#if NET48
        public uint DH_NameSize => _model.DirectoryHeader.NameSize;
#else
        public uint? DH_NameSize => _model.DirectoryHeader?.NameSize;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Info1Count"/>
#if NET48
        public uint DH_Info1Count => _model.DirectoryHeader.Info1Count;
#else
        public uint? DH_Info1Count => _model.DirectoryHeader?.Info1Count;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.CopyCount"/>
#if NET48
        public uint DH_CopyCount => _model.DirectoryHeader.CopyCount;
#else
        public uint? DH_CopyCount => _model.DirectoryHeader?.CopyCount;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.LocalCount"/>
#if NET48
        public uint DH_LocalCount => _model.DirectoryHeader.LocalCount;
#else
        public uint? DH_LocalCount => _model.DirectoryHeader?.LocalCount;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Dummy2"/>
#if NET48
        public uint DH_Dummy2 => _model.DirectoryHeader.Dummy2;
#else
        public uint? DH_Dummy2 => _model.DirectoryHeader?.Dummy2;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Dummy3"/>
#if NET48
        public uint DH_Dummy3 => _model.DirectoryHeader.Dummy3;
#else
        public uint? DH_Dummy3 => _model.DirectoryHeader?.Dummy3;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Checksum"/>
#if NET48
        public uint DH_Checksum => _model.DirectoryHeader.Checksum;
#else
        public uint? DH_Checksum => _model.DirectoryHeader?.Checksum;
#endif

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryEntries"/>
#if NET48
        public SabreTools.Models.GCF.DirectoryEntry[] DirectoryEntries => _model.DirectoryEntries;
#else
        public SabreTools.Models.GCF.DirectoryEntry?[]? DirectoryEntries => _model.DirectoryEntries;
#endif

        #endregion

        #region Directory Names

        /// <inheritdoc cref="Models.GCF.File.DirectoryNames"/>
#if NET48
        public Dictionary<long, string> DirectoryNames => _model.DirectoryNames;
#else
        public Dictionary<long, string?>? DirectoryNames => _model.DirectoryNames;
#endif

        #endregion

        #region Directory Info 1 Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryInfo1Entries"/>
#if NET48
        public SabreTools.Models.GCF.DirectoryInfo1Entry[] DirectoryInfo1Entries => _model.DirectoryInfo1Entries;
#else
        public SabreTools.Models.GCF.DirectoryInfo1Entry?[]? DirectoryInfo1Entries => _model.DirectoryInfo1Entries;
#endif

        #endregion

        #region Directory Info 2 Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryInfo2Entries"/>
#if NET48
        public SabreTools.Models.GCF.DirectoryInfo2Entry[] DirectoryInfo2Entries => _model.DirectoryInfo2Entries;
#else
        public SabreTools.Models.GCF.DirectoryInfo2Entry?[]? DirectoryInfo2Entries => _model.DirectoryInfo2Entries;
#endif

        #endregion

        #region Directory Copy Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryCopyEntries"/>
#if NET48
        public SabreTools.Models.GCF.DirectoryCopyEntry[] DirectoryCopyEntries => _model.DirectoryCopyEntries;
#else
        public SabreTools.Models.GCF.DirectoryCopyEntry?[]? DirectoryCopyEntries => _model.DirectoryCopyEntries;
#endif

        #endregion

        #region Directory Local Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryLocalEntries"/>
#if NET48
        public SabreTools.Models.GCF.DirectoryLocalEntry[] DirectoryLocalEntries => _model.DirectoryLocalEntries;
#else
        public SabreTools.Models.GCF.DirectoryLocalEntry?[]? DirectoryLocalEntries => _model.DirectoryLocalEntries;
#endif

        #endregion

        #region Directory Map Header

        /// <inheritdoc cref="Models.GCF.DirectoryMapHeader.Dummy0"/>
        public uint? DMH_Dummy0 => _model.DirectoryMapHeader?.Dummy0;

        /// <inheritdoc cref="Models.GCF.DirectoryMapHeader.Dummy1"/>
        public uint? DMH_Dummy1 => _model.DirectoryMapHeader?.Dummy1;

        #endregion

        #region Directory Map Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryMapEntries"/>
#if NET48
        public SabreTools.Models.GCF.DirectoryMapEntry[] DirectoryMapEntries => _model.DirectoryMapEntries;
#else
        public SabreTools.Models.GCF.DirectoryMapEntry?[]? DirectoryMapEntries => _model.DirectoryMapEntries;
#endif

        #endregion

        #region Checksum Header

        /// <inheritdoc cref="Models.GCF.ChecksumHeader.Dummy0"/>
#if NET48
        public uint CH_Dummy0 => _model.ChecksumHeader.Dummy0;
#else
        public uint? CH_Dummy0 => _model.ChecksumHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.GCF.ChecksumHeader.ChecksumSize"/>
#if NET48
        public uint CH_ChecksumSize => _model.ChecksumHeader.ChecksumSize;
#else
        public uint? CH_ChecksumSize => _model.ChecksumHeader?.ChecksumSize;
#endif

        #endregion

        #region Checksum Map Header

        /// <inheritdoc cref="Models.GCF.ChecksumMapHeader.Dummy0"/>
#if NET48
        public uint CMH_Dummy0 => _model.ChecksumMapHeader.Dummy0;
#else
        public uint? CMH_Dummy0 => _model.ChecksumMapHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.GCF.ChecksumMapHeader.Dummy1"/>
#if NET48
        public uint CMH_Dummy1 => _model.ChecksumMapHeader.Dummy1;
#else
        public uint? CMH_Dummy1 => _model.ChecksumMapHeader?.Dummy1;
#endif

        /// <inheritdoc cref="Models.GCF.ChecksumMapHeader.ItemCount"/>
#if NET48
        public uint CMH_ItemCount => _model.ChecksumMapHeader.ItemCount;
#else
        public uint? CMH_ItemCount => _model.ChecksumMapHeader?.ItemCount;
#endif

        /// <inheritdoc cref="Models.GCF.ChecksumMapHeader.ChecksumCount"/>
#if NET48
        public uint CMH_ChecksumCount => _model.ChecksumMapHeader.ChecksumCount;
#else
        public uint? CMH_ChecksumCount => _model.ChecksumMapHeader?.ChecksumCount;
#endif

        #endregion

        #region Checksum Map Entries

        /// <inheritdoc cref="Models.GCF.File.ChecksumMapEntries"/>
#if NET48
        public SabreTools.Models.GCF.ChecksumMapEntry[] ChecksumMapEntries => _model.ChecksumMapEntries;
#else
        public SabreTools.Models.GCF.ChecksumMapEntry?[]? ChecksumMapEntries => _model.ChecksumMapEntries;
#endif

        #endregion

        #region Checksum Entries

        /// <inheritdoc cref="Models.GCF.File.ChecksumEntries"/>
#if NET48
        public SabreTools.Models.GCF.ChecksumEntry[] ChecksumEntries => _model.ChecksumEntries;
#else
        public SabreTools.Models.GCF.ChecksumEntry?[]? ChecksumEntries => _model.ChecksumEntries;
#endif

        #endregion

        #region Data Block Header

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.LastVersionPlayed"/>
#if NET48
        public uint DBH_LastVersionPlayed => _model.DataBlockHeader.LastVersionPlayed;
#else
        public uint? DBH_LastVersionPlayed => _model.DataBlockHeader?.LastVersionPlayed;
#endif

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.BlockCount"/>
#if NET48
        public uint DBH_BlockCount => _model.DataBlockHeader.BlockCount;
#else
        public uint? DBH_BlockCount => _model.DataBlockHeader?.BlockCount;
#endif

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.BlockSize"/>
#if NET48
        public uint DBH_BlockSize => _model.DataBlockHeader.BlockSize;
#else
        public uint? DBH_BlockSize => _model.DataBlockHeader?.BlockSize;
#endif

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.FirstBlockOffset"/>
#if NET48
        public uint DBH_FirstBlockOffset => _model.DataBlockHeader.FirstBlockOffset;
#else
        public uint? DBH_FirstBlockOffset => _model.DataBlockHeader?.FirstBlockOffset;
#endif

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.BlocksUsed"/>
#if NET48
        public uint DBH_BlocksUsed => _model.DataBlockHeader.BlocksUsed;
#else
        public uint? DBH_BlocksUsed => _model.DataBlockHeader?.BlocksUsed;
#endif

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.Checksum"/>
#if NET48
        public uint DBH_Checksum => _model.DataBlockHeader.Checksum;
#else
        public uint? DBH_Checksum => _model.DataBlockHeader?.Checksum;
#endif

        #endregion

        #endregion

        #region Extension Properties

        /// <summary>
        /// Set of all files and their information
        /// </summary>
#if NET48
        public FileInfo[] Files
#else
        public FileInfo[]? Files
#endif
        {
            get
            {
                // Use the cached value if we have it
                if (_files != null)
                    return _files;

                // If we don't have a required property
                if (DirectoryEntries == null || DirectoryMapEntries == null || BlockEntries == null)
                    return null;

                // Otherwise, scan and build the files
                var files = new List<FileInfo>();
                for (int i = 0; i < DirectoryEntries.Length; i++)
                {
                    // Get the directory entry
                    var directoryEntry = DirectoryEntries[i];
                    var directoryMapEntry = DirectoryMapEntries[i];
                    if (directoryEntry == null || directoryMapEntry == null)
                        continue;

                    // If we have a directory, skip for now
                    if (!directoryEntry.DirectoryFlags.HasFlag(SabreTools.Models.GCF.HL_GCF_FLAG.HL_GCF_FLAG_FILE))
                        continue;

                    // Otherwise, start building the file info
                    var fileInfo = new FileInfo()
                    {
                        Size = directoryEntry.ItemSize,
                        Encrypted = directoryEntry.DirectoryFlags.HasFlag(SabreTools.Models.GCF.HL_GCF_FLAG.HL_GCF_FLAG_ENCRYPTED),
                    };
                    var pathParts = new List<string> { directoryEntry.Name ?? string.Empty };
#if NET48
                    var blockEntries = new List<SabreTools.Models.GCF.BlockEntry>();
#else
                    var blockEntries = new List<SabreTools.Models.GCF.BlockEntry?>();
#endif

                    // Traverse the parent tree
                    uint index = directoryEntry.ParentIndex;
                    while (index != 0xFFFFFFFF)
                    {
                        var parentDirectoryEntry = DirectoryEntries[index];
                        if (parentDirectoryEntry == null)
                            break;

                        pathParts.Add(parentDirectoryEntry.Name ?? string.Empty);
                        index = parentDirectoryEntry.ParentIndex;
                    }

                    // Traverse the block entries
                    index = directoryMapEntry.FirstBlockIndex;
                    while (index != DBH_BlockCount)
                    {
                        var nextBlock = BlockEntries[index];
                        if (nextBlock == null)
                            break;

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
#if NET48
        public long[] DataBlockOffsets
#else
        public long[]? DataBlockOffsets
#endif
        {
            get
            {
                // Use the cached value if we have it
                if (_dataBlockOffsets != null)
                    return _dataBlockOffsets;

#if NET6_0_OR_GREATER
                // If we don't have a block count, offset, or size
                if (DBH_BlockCount == null || DBH_FirstBlockOffset == null || DBH_BlockSize == null)
                    return null;
#endif

                // Otherwise, build the data block set
#if NET48
                _dataBlockOffsets = new long[DBH_BlockCount];
#else
                _dataBlockOffsets = new long[DBH_BlockCount.Value];
#endif
                for (int i = 0; i < DBH_BlockCount; i++)
                {
#if NET48
                    long dataBlockOffset = DBH_FirstBlockOffset + (i * DBH_BlockSize);
#else
                    long dataBlockOffset = DBH_FirstBlockOffset.Value + (i * DBH_BlockSize.Value);
#endif
                    _dataBlockOffsets[i] = dataBlockOffset;
                }

                // Return the set of data blocks
                return _dataBlockOffsets;
            }
        }

        #endregion

        #region Instance Variables

        /// <summary>
        /// Set of all files and their information
        /// </summary>
#if NET48
        private FileInfo[] _files = null;
#else
        private FileInfo[]? _files = null;
#endif

        /// <summary>
        /// Set of all data block offsets
        /// </summary>
#if NET48
        private long[] _dataBlockOffsets = null;
#else
        private long[]? _dataBlockOffsets = null;
#endif

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public GCF(SabreTools.Models.GCF.File model, byte[] data, int offset)
#else
        public GCF(SabreTools.Models.GCF.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public GCF(SabreTools.Models.GCF.File model, Stream data)
#else
        public GCF(SabreTools.Models.GCF.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an GCF from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the GCF</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An GCF wrapper on success, null on failure</returns>
#if NET48
        public static GCF Create(byte[] data, int offset)
#else
        public static GCF? Create(byte[]? data, int offset)
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
        /// Create a GCF from a Stream
        /// </summary>
        /// <param name="data">Stream representing the GCF</param>
        /// <returns>An GCF wrapper on success, null on failure</returns>
#if NET48
        public static GCF Create(Stream data)
#else
        public static GCF? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.GCF().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new GCF(file, data);
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
                    if (blockEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (fragmentationMap == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
            if (_model.BlockEntryMapHeader == null)
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
                    if (blockEntryMap == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (directoryEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (directoryInfoEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
                    if (directoryInfoEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
            builder.AppendLine("  -------------------------");
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
                    if (directoryCopyEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
            builder.AppendLine("  -------------------------");
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
                    if (directoryLocalEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
            if (_model.DirectoryMapHeader == null)
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
            builder.AppendLine("  -------------------------");
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
                    if (directoryMapEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
            builder.AppendLine("  -------------------------");
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
                    if (checksumMapEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
            builder.AppendLine("  -------------------------");
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
                    if (checksumEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

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
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

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
            if (Files == null || Files.Length == 0 || DataBlockOffsets == null)
                return false;

            // If the files index is invalid
            if (index < 0 || index >= Files.Length)
                return false;

            // Get the file
            var file = Files[index];
            if (file?.BlockEntries == null || file.Size == 0)
                return false;

            // If the file is encrypted -- TODO: Revisit later
            if (file.Encrypted)
                return false;

            // Get all data block offsets needed for extraction
            var dataBlockOffsets = new List<long>();
            for (int i = 0; i < file.BlockEntries.Length; i++)
            {
                var blockEntry = file.BlockEntries[i];
                if (blockEntry == null)
                    continue;

                uint dataBlockIndex = blockEntry.FirstDataBlockIndex;
                long blockEntrySize = blockEntry.FileDataSize;
                while (blockEntrySize > 0)
                {
                    long dataBlockOffset = DataBlockOffsets[dataBlockIndex++];
                    dataBlockOffsets.Add(dataBlockOffset);
#if NET48
                    blockEntrySize -= DBH_BlockSize;
#else
                    blockEntrySize -= DBH_BlockSize ?? 0;
#endif
                }
            }

            // Create the filename
#if NET48
            string filename = file.Path;
#else
            string? filename = file.Path;
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
                    // Now read the data sequentially and write out while we have data left
                    long fileSize = file.Size;
                    for (int i = 0; i < dataBlockOffsets.Count; i++)
                    {
#if NET48
                        int readSize = (int)Math.Min(DBH_BlockSize, fileSize);
                        byte[] data = ReadFromDataSource((int)dataBlockOffsets[i], readSize);
#else
                        int readSize = (int)Math.Min(DBH_BlockSize ?? 0, fileSize);
                        byte[]? data = ReadFromDataSource((int)dataBlockOffsets[i], readSize);
#endif
                        if (data == null)
                            return false;

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
#if NET48
            public string Path;
#else
            public string? Path;
#endif

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
#if NET48
            public SabreTools.Models.GCF.BlockEntry[] BlockEntries;
#else
            public SabreTools.Models.GCF.BlockEntry?[]? BlockEntries;
#endif
        }

        #endregion
    }
}