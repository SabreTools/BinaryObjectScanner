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
        public uint Dummy0 => this.Model.Header.Dummy0;
#else
        public uint? Dummy0 => this.Model.Header?.Dummy0;
#endif

        /// <inheritdoc cref="Models.GCF.Header.MajorVersion"/>
#if NET48
        public uint MajorVersion => this.Model.Header.MajorVersion;
#else
        public uint? MajorVersion => this.Model.Header?.MajorVersion;
#endif

        /// <inheritdoc cref="Models.GCF.Header.MinorVersion"/>
#if NET48
        public uint MinorVersion => this.Model.Header.MinorVersion;
#else
        public uint? MinorVersion => this.Model.Header?.MinorVersion;
#endif

        /// <inheritdoc cref="Models.GCF.Header.CacheID"/>
#if NET48
        public uint CacheID => this.Model.Header.CacheID;
#else
        public uint? CacheID => this.Model.Header?.CacheID;
#endif

        /// <inheritdoc cref="Models.GCF.Header.LastVersionPlayed"/>
#if NET48
        public uint LastVersionPlayed => this.Model.Header.LastVersionPlayed;
#else
        public uint? LastVersionPlayed => this.Model.Header?.LastVersionPlayed;
#endif

        /// <inheritdoc cref="Models.GCF.Header.Dummy1"/>
#if NET48
        public uint Dummy1 => this.Model.Header.Dummy1;
#else
        public uint? Dummy1 => this.Model.Header?.Dummy1;
#endif

        /// <inheritdoc cref="Models.GCF.Header.Dummy2"/>
#if NET48
        public uint Dummy2 => this.Model.Header.Dummy2;
#else
        public uint? Dummy2 => this.Model.Header?.Dummy2;
#endif

        /// <inheritdoc cref="Models.GCF.Header.FileSize"/>
#if NET48
        public uint FileSize => this.Model.Header.FileSize;
#else
        public uint? FileSize => this.Model.Header?.FileSize;
#endif

        /// <inheritdoc cref="Models.GCF.Header.BlockSize"/>
#if NET48
        public uint BlockSize => this.Model.Header.BlockSize;
#else
        public uint? BlockSize => this.Model.Header?.BlockSize;
#endif

        /// <inheritdoc cref="Models.GCF.Header.BlockCount"/>
#if NET48
        public uint BlockCount => this.Model.Header.BlockCount;
#else
        public uint? BlockCount => this.Model.Header?.BlockCount;
#endif

        /// <inheritdoc cref="Models.GCF.Header.Dummy3"/>
#if NET48
        public uint Dummy3 => this.Model.Header.Dummy3;
#else
        public uint? Dummy3 => this.Model.Header?.Dummy3;
#endif

        #endregion

        #region Block Entry Header

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.BlockCount"/>
#if NET48
        public uint BEH_BlockCount => this.Model.BlockEntryHeader.BlockCount;
#else
        public uint? BEH_BlockCount => this.Model.BlockEntryHeader?.BlockCount;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.BlocksUsed"/>
#if NET48
        public uint BEH_BlocksUsed => this.Model.BlockEntryHeader.BlocksUsed;
#else
        public uint? BEH_BlocksUsed => this.Model.BlockEntryHeader?.BlocksUsed;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy0"/>
#if NET48
        public uint BEH_Dummy0 => this.Model.BlockEntryHeader.Dummy0;
#else
        public uint? BEH_Dummy0 => this.Model.BlockEntryHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy1"/>
#if NET48
        public uint BEH_Dummy1 => this.Model.BlockEntryHeader.Dummy1;
#else
        public uint? BEH_Dummy1 => this.Model.BlockEntryHeader?.Dummy1;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy2"/>
#if NET48
        public uint BEH_Dummy2 => this.Model.BlockEntryHeader.Dummy2;
#else
        public uint? BEH_Dummy2 => this.Model.BlockEntryHeader?.Dummy2;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy3"/>
#if NET48
        public uint BEH_Dummy3 => this.Model.BlockEntryHeader.Dummy3;
#else
        public uint? BEH_Dummy3 => this.Model.BlockEntryHeader?.Dummy3;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Dummy4"/>
#if NET48
        public uint BEH_Dummy4 => this.Model.BlockEntryHeader.Dummy4;
#else
        public uint? BEH_Dummy4 => this.Model.BlockEntryHeader?.Dummy4;
#endif

        /// <inheritdoc cref="Models.GCF.BlockEntryHeader.Checksum"/>
#if NET48
        public uint BEH_Checksum => this.Model.BlockEntryHeader.Checksum;
#else
        public uint? BEH_Checksum => this.Model.BlockEntryHeader?.Checksum;
#endif

        #endregion

        #region Block Entries

        /// <inheritdoc cref="Models.GCF.File.BlockEntries"/>
#if NET48
        public SabreTools.Models.GCF.BlockEntry[] BlockEntries => this.Model.BlockEntries;
#else
        public SabreTools.Models.GCF.BlockEntry?[]? BlockEntries => this.Model.BlockEntries;
#endif

        #endregion

        #region Fragmentation Map Header

        /// <inheritdoc cref="Models.GCF.FragmentationMapHeader.BlockCount"/>
#if NET48
        public uint FMH_BlockCount => this.Model.FragmentationMapHeader.BlockCount;
#else
        public uint? FMH_BlockCount => this.Model.FragmentationMapHeader?.BlockCount;
#endif

        /// <inheritdoc cref="Models.GCF.FragmentationMapHeader.FirstUnusedEntry"/>
#if NET48
        public uint FMH_FirstUnusedEntry => this.Model.FragmentationMapHeader.FirstUnusedEntry;
#else
        public uint? FMH_FirstUnusedEntry => this.Model.FragmentationMapHeader?.FirstUnusedEntry;
#endif

        /// <inheritdoc cref="Models.GCF.FragmentationMapHeader.Terminator"/>
#if NET48
        public uint FMH_Terminator => this.Model.FragmentationMapHeader.Terminator;
#else
        public uint? FMH_Terminator => this.Model.FragmentationMapHeader?.Terminator;
#endif

        /// <inheritdoc cref="Models.GCF.FragmentationMapHeader.Checksum"/>
#if NET48
        public uint FMH_Checksum => this.Model.FragmentationMapHeader.Checksum;
#else
        public uint? FMH_Checksum => this.Model.FragmentationMapHeader?.Checksum;
#endif

        #endregion

        #region Fragmentation Maps

        /// <inheritdoc cref="Models.GCF.File.FragmentationMaps"/>
#if NET48
        public SabreTools.Models.GCF.FragmentationMap[] FragmentationMaps => this.Model.FragmentationMaps;
#else
        public SabreTools.Models.GCF.FragmentationMap?[]? FragmentationMaps => this.Model.FragmentationMaps;
#endif

        #endregion

        #region Block Entry Map Header

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.BlockCount"/>
        public uint? BEMH_BlockCount => this.Model.BlockEntryMapHeader?.BlockCount;

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.FirstBlockEntryIndex"/>
        public uint? BEMH_FirstBlockEntryIndex => this.Model.BlockEntryMapHeader?.FirstBlockEntryIndex;

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.LastBlockEntryIndex"/>
        public uint? BEMH_LastBlockEntryIndex => this.Model.BlockEntryMapHeader?.LastBlockEntryIndex;

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.Dummy0"/>
        public uint? BEMH_Dummy0 => this.Model.BlockEntryMapHeader?.Dummy0;

        /// <inheritdoc cref="Models.GCF.BlockEntryMapHeader.Checksum"/>
        public uint? BEMH_Checksum => this.Model.BlockEntryMapHeader?.Checksum;

        #endregion

        #region Block Entry Maps

        /// <inheritdoc cref="Models.GCF.File.BlockEntryMaps"/>
#if NET48
        public SabreTools.Models.GCF.BlockEntryMap[] BlockEntryMaps => this.Model.BlockEntryMaps;
#else
        public SabreTools.Models.GCF.BlockEntryMap?[]? BlockEntryMaps => this.Model.BlockEntryMaps;
#endif

        #endregion

        #region Directory Header

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Dummy0"/>
#if NET48
        public uint DH_Dummy0 => this.Model.DirectoryHeader.Dummy0;
#else
        public uint? DH_Dummy0 => this.Model.DirectoryHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.CacheID"/>
#if NET48
        public uint DH_CacheID => this.Model.DirectoryHeader.CacheID;
#else
        public uint? DH_CacheID => this.Model.DirectoryHeader?.CacheID;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.LastVersionPlayed"/>
#if NET48
        public uint DH_LastVersionPlayed => this.Model.DirectoryHeader.LastVersionPlayed;
#else
        public uint? DH_LastVersionPlayed => this.Model.DirectoryHeader?.LastVersionPlayed;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.ItemCount"/>
#if NET48
        public uint DH_ItemCount => this.Model.DirectoryHeader.ItemCount;
#else
        public uint? DH_ItemCount => this.Model.DirectoryHeader?.ItemCount;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.FileCount"/>
#if NET48
        public uint DH_FileCount => this.Model.DirectoryHeader.FileCount;
#else
        public uint? DH_FileCount => this.Model.DirectoryHeader?.FileCount;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Dummy1"/>
#if NET48
        public uint DH_Dummy1 => this.Model.DirectoryHeader.Dummy1;
#else
        public uint? DH_Dummy1 => this.Model.DirectoryHeader?.Dummy1;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.DirectorySize"/>
#if NET48
        public uint DH_DirectorySize => this.Model.DirectoryHeader.DirectorySize;
#else
        public uint? DH_DirectorySize => this.Model.DirectoryHeader?.DirectorySize;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.NameSize"/>
#if NET48
        public uint DH_NameSize => this.Model.DirectoryHeader.NameSize;
#else
        public uint? DH_NameSize => this.Model.DirectoryHeader?.NameSize;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Info1Count"/>
#if NET48
        public uint DH_Info1Count => this.Model.DirectoryHeader.Info1Count;
#else
        public uint? DH_Info1Count => this.Model.DirectoryHeader?.Info1Count;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.CopyCount"/>
#if NET48
        public uint DH_CopyCount => this.Model.DirectoryHeader.CopyCount;
#else
        public uint? DH_CopyCount => this.Model.DirectoryHeader?.CopyCount;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.LocalCount"/>
#if NET48
        public uint DH_LocalCount => this.Model.DirectoryHeader.LocalCount;
#else
        public uint? DH_LocalCount => this.Model.DirectoryHeader?.LocalCount;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Dummy2"/>
#if NET48
        public uint DH_Dummy2 => this.Model.DirectoryHeader.Dummy2;
#else
        public uint? DH_Dummy2 => this.Model.DirectoryHeader?.Dummy2;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Dummy3"/>
#if NET48
        public uint DH_Dummy3 => this.Model.DirectoryHeader.Dummy3;
#else
        public uint? DH_Dummy3 => this.Model.DirectoryHeader?.Dummy3;
#endif

        /// <inheritdoc cref="Models.GCF.DirectoryHeader.Checksum"/>
#if NET48
        public uint DH_Checksum => this.Model.DirectoryHeader.Checksum;
#else
        public uint? DH_Checksum => this.Model.DirectoryHeader?.Checksum;
#endif

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryEntries"/>
#if NET48
        public SabreTools.Models.GCF.DirectoryEntry[] DirectoryEntries => this.Model.DirectoryEntries;
#else
        public SabreTools.Models.GCF.DirectoryEntry?[]? DirectoryEntries => this.Model.DirectoryEntries;
#endif

        #endregion

        #region Directory Names

        /// <inheritdoc cref="Models.GCF.File.DirectoryNames"/>
#if NET48
        public Dictionary<long, string> DirectoryNames => this.Model.DirectoryNames;
#else
        public Dictionary<long, string?>? DirectoryNames => this.Model.DirectoryNames;
#endif

        #endregion

        #region Directory Info 1 Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryInfo1Entries"/>
#if NET48
        public SabreTools.Models.GCF.DirectoryInfo1Entry[] DirectoryInfo1Entries => this.Model.DirectoryInfo1Entries;
#else
        public SabreTools.Models.GCF.DirectoryInfo1Entry?[]? DirectoryInfo1Entries => this.Model.DirectoryInfo1Entries;
#endif

        #endregion

        #region Directory Info 2 Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryInfo2Entries"/>
#if NET48
        public SabreTools.Models.GCF.DirectoryInfo2Entry[] DirectoryInfo2Entries => this.Model.DirectoryInfo2Entries;
#else
        public SabreTools.Models.GCF.DirectoryInfo2Entry?[]? DirectoryInfo2Entries => this.Model.DirectoryInfo2Entries;
#endif

        #endregion

        #region Directory Copy Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryCopyEntries"/>
#if NET48
        public SabreTools.Models.GCF.DirectoryCopyEntry[] DirectoryCopyEntries => this.Model.DirectoryCopyEntries;
#else
        public SabreTools.Models.GCF.DirectoryCopyEntry?[]? DirectoryCopyEntries => this.Model.DirectoryCopyEntries;
#endif

        #endregion

        #region Directory Local Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryLocalEntries"/>
#if NET48
        public SabreTools.Models.GCF.DirectoryLocalEntry[] DirectoryLocalEntries => this.Model.DirectoryLocalEntries;
#else
        public SabreTools.Models.GCF.DirectoryLocalEntry?[]? DirectoryLocalEntries => this.Model.DirectoryLocalEntries;
#endif

        #endregion

        #region Directory Map Header

        /// <inheritdoc cref="Models.GCF.DirectoryMapHeader.Dummy0"/>
        public uint? DMH_Dummy0 => this.Model.DirectoryMapHeader?.Dummy0;

        /// <inheritdoc cref="Models.GCF.DirectoryMapHeader.Dummy1"/>
        public uint? DMH_Dummy1 => this.Model.DirectoryMapHeader?.Dummy1;

        #endregion

        #region Directory Map Entries

        /// <inheritdoc cref="Models.GCF.File.DirectoryMapEntries"/>
#if NET48
        public SabreTools.Models.GCF.DirectoryMapEntry[] DirectoryMapEntries => this.Model.DirectoryMapEntries;
#else
        public SabreTools.Models.GCF.DirectoryMapEntry?[]? DirectoryMapEntries => this.Model.DirectoryMapEntries;
#endif

        #endregion

        #region Checksum Header

        /// <inheritdoc cref="Models.GCF.ChecksumHeader.Dummy0"/>
#if NET48
        public uint CH_Dummy0 => this.Model.ChecksumHeader.Dummy0;
#else
        public uint? CH_Dummy0 => this.Model.ChecksumHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.GCF.ChecksumHeader.ChecksumSize"/>
#if NET48
        public uint CH_ChecksumSize => this.Model.ChecksumHeader.ChecksumSize;
#else
        public uint? CH_ChecksumSize => this.Model.ChecksumHeader?.ChecksumSize;
#endif

        #endregion

        #region Checksum Map Header

        /// <inheritdoc cref="Models.GCF.ChecksumMapHeader.Dummy0"/>
#if NET48
        public uint CMH_Dummy0 => this.Model.ChecksumMapHeader.Dummy0;
#else
        public uint? CMH_Dummy0 => this.Model.ChecksumMapHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.GCF.ChecksumMapHeader.Dummy1"/>
#if NET48
        public uint CMH_Dummy1 => this.Model.ChecksumMapHeader.Dummy1;
#else
        public uint? CMH_Dummy1 => this.Model.ChecksumMapHeader?.Dummy1;
#endif

        /// <inheritdoc cref="Models.GCF.ChecksumMapHeader.ItemCount"/>
#if NET48
        public uint CMH_ItemCount => this.Model.ChecksumMapHeader.ItemCount;
#else
        public uint? CMH_ItemCount => this.Model.ChecksumMapHeader?.ItemCount;
#endif

        /// <inheritdoc cref="Models.GCF.ChecksumMapHeader.ChecksumCount"/>
#if NET48
        public uint CMH_ChecksumCount => this.Model.ChecksumMapHeader.ChecksumCount;
#else
        public uint? CMH_ChecksumCount => this.Model.ChecksumMapHeader?.ChecksumCount;
#endif

        #endregion

        #region Checksum Map Entries

        /// <inheritdoc cref="Models.GCF.File.ChecksumMapEntries"/>
#if NET48
        public SabreTools.Models.GCF.ChecksumMapEntry[] ChecksumMapEntries => this.Model.ChecksumMapEntries;
#else
        public SabreTools.Models.GCF.ChecksumMapEntry?[]? ChecksumMapEntries => this.Model.ChecksumMapEntries;
#endif

        #endregion

        #region Checksum Entries

        /// <inheritdoc cref="Models.GCF.File.ChecksumEntries"/>
#if NET48
        public SabreTools.Models.GCF.ChecksumEntry[] ChecksumEntries => this.Model.ChecksumEntries;
#else
        public SabreTools.Models.GCF.ChecksumEntry?[]? ChecksumEntries => this.Model.ChecksumEntries;
#endif

        #endregion

        #region Data Block Header

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.LastVersionPlayed"/>
#if NET48
        public uint DBH_LastVersionPlayed => this.Model.DataBlockHeader.LastVersionPlayed;
#else
        public uint? DBH_LastVersionPlayed => this.Model.DataBlockHeader?.LastVersionPlayed;
#endif

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.BlockCount"/>
#if NET48
        public uint DBH_BlockCount => this.Model.DataBlockHeader.BlockCount;
#else
        public uint? DBH_BlockCount => this.Model.DataBlockHeader?.BlockCount;
#endif

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.BlockSize"/>
#if NET48
        public uint DBH_BlockSize => this.Model.DataBlockHeader.BlockSize;
#else
        public uint? DBH_BlockSize => this.Model.DataBlockHeader?.BlockSize;
#endif

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.FirstBlockOffset"/>
#if NET48
        public uint DBH_FirstBlockOffset => this.Model.DataBlockHeader.FirstBlockOffset;
#else
        public uint? DBH_FirstBlockOffset => this.Model.DataBlockHeader?.FirstBlockOffset;
#endif

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.BlocksUsed"/>
#if NET48
        public uint DBH_BlocksUsed => this.Model.DataBlockHeader.BlocksUsed;
#else
        public uint? DBH_BlocksUsed => this.Model.DataBlockHeader?.BlocksUsed;
#endif

        /// <inheritdoc cref="Models.GCF.DataBlockHeader.Checksum"/>
#if NET48
        public uint DBH_Checksum => this.Model.DataBlockHeader.Checksum;
#else
        public uint? DBH_Checksum => this.Model.DataBlockHeader?.Checksum;
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
            Printing.GCF.Print(builder, this.Model);
            return builder;
        }

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