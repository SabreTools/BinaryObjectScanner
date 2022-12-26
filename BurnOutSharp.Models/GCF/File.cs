using System.Collections.Generic;

namespace BurnOutSharp.Models.GCF
{
    /// <summary>
    /// Half-Life Game Cache File
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    public sealed class File
    {
        /// <summary>
        /// Header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Block entry header data
        /// </summary>
        public BlockEntryHeader BlockEntryHeader { get; set; }

        /// <summary>
        /// Block entries data
        /// </summary>
        public BlockEntry[] BlockEntries { get; set; }

        /// <summary>
        /// Fragmentation map header data
        /// </summary>
        public FragmentationMapHeader FragmentationMapHeader { get; set; }

        /// <summary>
        /// Fragmentation map data
        /// </summary>
        public FragmentationMap[] FragmentationMaps { get; set; }

        /// <summary>
        /// Block entry map header data
        /// </summary>
        /// <remarks>Part of version 5 but not version 6.</remarks>
        public BlockEntryMapHeader BlockEntryMapHeader { get; set; }

        /// <summary>
        /// Block entry map data
        /// </summary>
        /// <remarks>Part of version 5 but not version 6.</remarks>
        public BlockEntryMap[] BlockEntryMaps { get; set; }

        /// <summary>
        /// Directory header data
        /// </summary>
        public DirectoryHeader DirectoryHeader { get; set; }

        /// <summary>
        /// Directory entries data
        /// </summary>
        public DirectoryEntry[] DirectoryEntries { get; set; }

        /// <summary>
        /// Directory names data
        /// </summary>
        public Dictionary<long, string> DirectoryNames { get; set; }

        /// <summary>
        /// Directory info 1 entries data
        /// </summary>
        public DirectoryInfo1Entry[] DirectoryInfo1Entries { get; set; }

        /// <summary>
        /// Directory info 2 entries data
        /// </summary>
        public DirectoryInfo2Entry[] DirectoryInfo2Entries { get; set; }

        /// <summary>
        /// Directory copy entries data
        /// </summary>
        public DirectoryCopyEntry[] DirectoryCopyEntries { get; set; }

        /// <summary>
        /// Directory local entries data
        /// </summary>
        public DirectoryLocalEntry[] DirectoryLocalEntries { get; set; }

        /// <summary>
        /// Directory map header data
        /// </summary>
        public DirectoryMapHeader DirectoryMapHeader { get; set; }

        /// <summary>
        /// Directory map entries data
        /// </summary>
        public DirectoryMapEntry[] DirectoryMapEntries { get; set; }

        /// <summary>
        /// Checksum header data
        /// </summary>
        public ChecksumHeader ChecksumHeader { get; set; }

        /// <summary>
        /// Checksum map header data
        /// </summary>
        public ChecksumMapHeader ChecksumMapHeader { get; set; }

        /// <summary>
        /// Checksum map entries data
        /// </summary>
        public ChecksumMapEntry[] ChecksumMapEntries { get; set; }

        /// <summary>
        /// Checksum entries data
        /// </summary>
        public ChecksumEntry[] ChecksumEntries { get; set; }

        /// <summary>
        /// Data block header data
        /// </summary>
        public DataBlockHeader DataBlockHeader { get; set; }
    }
}