using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class NCF : WrapperBase<SabreTools.Models.NCF.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life No Cache File (NCF)";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.NCF.Header.Dummy0"/>
#if NET48
        public uint Dummy0 => _model.Header.Dummy0;
#else
        public uint? Dummy0 => _model.Header?.Dummy0;
#endif

        /// <inheritdoc cref="Models.NCF.Header.MajorVersion"/>
#if NET48
        public uint MajorVersion => _model.Header.MajorVersion;
#else
        public uint? MajorVersion => _model.Header?.MajorVersion;
#endif

        /// <inheritdoc cref="Models.NCF.Header.MinorVersion"/>
#if NET48
        public uint MinorVersion => _model.Header.MinorVersion;
#else
        public uint? MinorVersion => _model.Header?.MinorVersion;
#endif

        /// <inheritdoc cref="Models.NCF.Header.CacheID"/>
#if NET48
        public uint CacheID => _model.Header.CacheID;
#else
        public uint? CacheID => _model.Header?.CacheID;
#endif

        /// <inheritdoc cref="Models.NCF.Header.LastVersionPlayed"/>
#if NET48
        public uint LastVersionPlayed => _model.Header.LastVersionPlayed;
#else
        public uint? LastVersionPlayed => _model.Header?.LastVersionPlayed;
#endif

        /// <inheritdoc cref="Models.NCF.Header.Dummy1"/>
#if NET48
        public uint Dummy1 => _model.Header.Dummy1;
#else
        public uint? Dummy1 => _model.Header?.Dummy1;
#endif

        /// <inheritdoc cref="Models.NCF.Header.Dummy2"/>
#if NET48
        public uint Dummy2 => _model.Header.Dummy2;
#else
        public uint? Dummy2 => _model.Header?.Dummy2;
#endif

        /// <inheritdoc cref="Models.NCF.Header.FileSize"/>
#if NET48
        public uint FileSize => _model.Header.FileSize;
#else
        public uint? FileSize => _model.Header?.FileSize;
#endif

        /// <inheritdoc cref="Models.NCF.Header.BlockSize"/>
#if NET48
        public uint BlockSize => _model.Header.BlockSize;
#else
        public uint? BlockSize => _model.Header?.BlockSize;
#endif

        /// <inheritdoc cref="Models.NCF.Header.BlockCount"/>
#if NET48
        public uint BlockCount => _model.Header.BlockCount;
#else
        public uint? BlockCount => _model.Header?.BlockCount;
#endif

        /// <inheritdoc cref="Models.NCF.Header.Dummy3"/>
#if NET48
        public uint Dummy3 => _model.Header.Dummy3;
#else
        public uint? Dummy3 => _model.Header?.Dummy3;
#endif

        #endregion

        #region Directory Header

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Dummy0"/>
#if NET48
        public uint DH_Dummy0 => _model.DirectoryHeader.Dummy0;
#else
        public uint? DH_Dummy0 => _model.DirectoryHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.CacheID"/>
#if NET48
        public uint DH_CacheID => _model.DirectoryHeader.CacheID;
#else
        public uint? DH_CacheID => _model.DirectoryHeader?.CacheID;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.LastVersionPlayed"/>
#if NET48
        public uint DH_LastVersionPlayed => _model.DirectoryHeader.LastVersionPlayed;
#else
        public uint? DH_LastVersionPlayed => _model.DirectoryHeader?.LastVersionPlayed;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.ItemCount"/>
#if NET48
        public uint DH_ItemCount => _model.DirectoryHeader.ItemCount;
#else
        public uint? DH_ItemCount => _model.DirectoryHeader?.ItemCount;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.FileCount"/>
#if NET48
        public uint DH_FileCount => _model.DirectoryHeader.FileCount;
#else
        public uint? DH_FileCount => _model.DirectoryHeader?.FileCount;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.ChecksumDataLength"/>
#if NET48
        public uint DH_ChecksumDataLength => _model.DirectoryHeader.ChecksumDataLength;
#else
        public uint? DH_ChecksumDataLength => _model.DirectoryHeader?.ChecksumDataLength;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.DirectorySize"/>
#if NET48
        public uint DH_DirectorySize => _model.DirectoryHeader.DirectorySize;
#else
        public uint? DH_DirectorySize => _model.DirectoryHeader?.DirectorySize;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.NameSize"/>
#if NET48
        public uint DH_NameSize => _model.DirectoryHeader.NameSize;
#else
        public uint? DH_NameSize => _model.DirectoryHeader?.NameSize;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Info1Count"/>
#if NET48
        public uint DH_Info1Count => _model.DirectoryHeader.Info1Count;
#else
        public uint? DH_Info1Count => _model.DirectoryHeader?.Info1Count;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.CopyCount"/>
#if NET48
        public uint DH_CopyCount => _model.DirectoryHeader.CopyCount;
#else
        public uint? DH_CopyCount => _model.DirectoryHeader?.CopyCount;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.LocalCount"/>
#if NET48
        public uint DH_LocalCount => _model.DirectoryHeader.LocalCount;
#else
        public uint? DH_LocalCount => _model.DirectoryHeader?.LocalCount;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Dummy1"/>
#if NET48
        public uint DH_Dummy1 => _model.DirectoryHeader.Dummy1;
#else
        public uint? DH_Dummy1 => _model.DirectoryHeader?.Dummy1;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Dummy2"/>
#if NET48
        public uint DH_Dummy2 => _model.DirectoryHeader.Dummy2;
#else
        public uint? DH_Dummy2 => _model.DirectoryHeader?.Dummy2;
#endif

        /// <inheritdoc cref="Models.NCF.DirectoryHeader.Checksum"/>
#if NET48
        public uint DH_Checksum => _model.DirectoryHeader.Checksum;
#else
        public uint? DH_Checksum => _model.DirectoryHeader?.Checksum;
#endif

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryEntries"/>
#if NET48
        public SabreTools.Models.NCF.DirectoryEntry[] DirectoryEntries => _model.DirectoryEntries;
#else
        public SabreTools.Models.NCF.DirectoryEntry?[]? DirectoryEntries => _model.DirectoryEntries;
#endif

        #endregion

        #region Directory Names

        /// <inheritdoc cref="Models.NCF.File.DirectoryNames"/>
#if NET48
        public Dictionary<long, string> DirectoryNames => _model.DirectoryNames;
#else
        public Dictionary<long, string?>? DirectoryNames => _model.DirectoryNames;
#endif

        #endregion

        #region Directory Info 1 Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryInfo1Entries"/>
#if NET48
        public SabreTools.Models.NCF.DirectoryInfo1Entry[] DirectoryInfo1Entries => _model.DirectoryInfo1Entries;
#else
        public SabreTools.Models.NCF.DirectoryInfo1Entry?[]? DirectoryInfo1Entries => _model.DirectoryInfo1Entries;
#endif

        #endregion

        #region Directory Info 2 Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryInfo2Entries"/>
#if NET48
        public SabreTools.Models.NCF.DirectoryInfo2Entry[] DirectoryInfo2Entries => _model.DirectoryInfo2Entries;
#else
        public SabreTools.Models.NCF.DirectoryInfo2Entry?[]? DirectoryInfo2Entries => _model.DirectoryInfo2Entries;
#endif

        #endregion

        #region Directory Copy Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryCopyEntries"/>
#if NET48
        public SabreTools.Models.NCF.DirectoryCopyEntry[] DirectoryCopyEntries => _model.DirectoryCopyEntries;
#else
        public SabreTools.Models.NCF.DirectoryCopyEntry?[]? DirectoryCopyEntries => _model.DirectoryCopyEntries;
#endif

        #endregion

        #region Directory Local Entries

        /// <inheritdoc cref="Models.NCF.File.DirectoryLocalEntries"/>
#if NET48
        public SabreTools.Models.NCF.DirectoryLocalEntry[] DirectoryLocalEntries => _model.DirectoryLocalEntries;
#else
        public SabreTools.Models.NCF.DirectoryLocalEntry?[]? DirectoryLocalEntries => _model.DirectoryLocalEntries;
#endif

        #endregion

        #region Unknown Header

        /// <inheritdoc cref="Models.NCF.UnknownHeader.Dummy0"/>
#if NET48
        public uint UH_Dummy0 => _model.UnknownHeader.Dummy0;
#else
        public uint? UH_Dummy0 => _model.UnknownHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.NCF.UnknownHeader.Dummy1"/>
#if NET48
        public uint UH_Dummy1 => _model.UnknownHeader.Dummy1;
#else
        public uint? UH_Dummy1 => _model.UnknownHeader?.Dummy1;
#endif

        #endregion

        #region Unknown Entries

        /// <inheritdoc cref="Models.NCF.File.UnknownEntries"/>
#if NET48
        public SabreTools.Models.NCF.UnknownEntry[] UnknownEntries => _model.UnknownEntries;
#else
        public SabreTools.Models.NCF.UnknownEntry?[]? UnknownEntries => _model.UnknownEntries;
#endif

        #endregion

        #region Checksum Header

        /// <inheritdoc cref="Models.NCF.ChecksumHeader.Dummy0"/>
#if NET48
        public uint CH_Dummy0 => _model.ChecksumHeader.Dummy0;
#else
        public uint? CH_Dummy0 => _model.ChecksumHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.NCF.ChecksumHeader.ChecksumSize"/>
#if NET48
        public uint CH_ChecksumSize => _model.ChecksumHeader.ChecksumSize;
#else
        public uint? CH_ChecksumSize => _model.ChecksumHeader?.ChecksumSize;
#endif

        #endregion

        #region Checksum Map Header

        /// <inheritdoc cref="Models.NCF.ChecksumMapHeader.Dummy0"/>
#if NET48
        public uint CMH_Dummy0 => _model.ChecksumMapHeader.Dummy0;
#else
        public uint? CMH_Dummy0 => _model.ChecksumMapHeader?.Dummy0;
#endif

        /// <inheritdoc cref="Models.NCF.ChecksumMapHeader.Dummy1"/>
#if NET48
        public uint CMH_Dummy1 => _model.ChecksumMapHeader.Dummy1;
#else
        public uint? CMH_Dummy1 => _model.ChecksumMapHeader?.Dummy1;
#endif

        /// <inheritdoc cref="Models.NCF.ChecksumMapHeader.ItemCount"/>
#if NET48
        public uint CMH_ItemCount => _model.ChecksumMapHeader.ItemCount;
#else
        public uint? CMH_ItemCount => _model.ChecksumMapHeader?.ItemCount;
#endif

        /// <inheritdoc cref="Models.NCF.ChecksumMapHeader.ChecksumCount"/>
#if NET48
        public uint CMH_ChecksumCount => _model.ChecksumMapHeader.ChecksumCount;
#else
        public uint? CMH_ChecksumCount => _model.ChecksumMapHeader?.ChecksumCount;
#endif

        #endregion

        #region Checksum Map Entries

        /// <inheritdoc cref="Models.NCF.File.ChecksumMapEntries"/>
#if NET48
        public SabreTools.Models.NCF.ChecksumMapEntry[] ChecksumMapEntries => _model.ChecksumMapEntries;
#else
        public SabreTools.Models.NCF.ChecksumMapEntry?[]? ChecksumMapEntries => _model.ChecksumMapEntries;
#endif

        #endregion

        #region Checksum Entries

        /// <inheritdoc cref="Models.NCF.File.ChecksumEntries"/>
#if NET48
        public SabreTools.Models.NCF.ChecksumEntry[] ChecksumEntries => _model.ChecksumEntries;
#else
        public SabreTools.Models.NCF.ChecksumEntry?[]? ChecksumEntries => _model.ChecksumEntries;
#endif

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public NCF(SabreTools.Models.NCF.File model, byte[] data, int offset)
#else
        public NCF(SabreTools.Models.NCF.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public NCF(SabreTools.Models.NCF.File model, Stream data)
#else
        public NCF(SabreTools.Models.NCF.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an NCF from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the NCF</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NCF wrapper on success, null on failure</returns>
#if NET48
        public static NCF Create(byte[] data, int offset)
#else
        public static NCF? Create(byte[]? data, int offset)
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
        /// Create a NCF from a Stream
        /// </summary>
        /// <param name="data">Stream representing the NCF</param>
        /// <returns>An NCF wrapper on success, null on failure</returns>
#if NET48
        public static NCF Create(Stream data)
#else
        public static NCF? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.NCF().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new NCF(file, data);
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

            builder.AppendLine("NCF Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Header
            PrintHeader(builder);

            // Directory and Directory Maps
            PrintDirectoryHeader(builder);
            PrintDirectoryEntries(builder);
            // TODO: Should we print out the entire string table?
            PrintDirectoryInfo1Entries(builder);
            PrintDirectoryInfo2Entries(builder);
            PrintDirectoryCopyEntries(builder);
            PrintDirectoryLocalEntries(builder);
            PrintUnknownHeader(builder);
            PrintUnknownEntries(builder);

            // Checksums and Checksum Maps
            PrintChecksumHeader(builder);
            PrintChecksumMapHeader(builder);
            PrintChecksumMapEntries(builder);
            PrintChecksumEntries(builder);

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
            builder.AppendLine($"  Checksum data length: {DH_ChecksumDataLength} (0x{DH_ChecksumDataLength:X})");
            builder.AppendLine($"  Directory size: {DH_DirectorySize} (0x{DH_DirectorySize:X})");
            builder.AppendLine($"  Name size: {DH_NameSize} (0x{DH_NameSize:X})");
            builder.AppendLine($"  Info 1 count: {DH_Info1Count} (0x{DH_Info1Count:X})");
            builder.AppendLine($"  Copy count: {DH_CopyCount} (0x{DH_CopyCount:X})");
            builder.AppendLine($"  Local count: {DH_LocalCount} (0x{DH_LocalCount:X})");
            builder.AppendLine($"  Dummy 1: {DH_Dummy1} (0x{DH_Dummy1:X})");
            builder.AppendLine($"  Dummy 2: {DH_Dummy2} (0x{DH_Dummy2:X})");
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
                    builder.AppendLine($"    Name: {directoryEntry.Name}");
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
        /// Print unknown header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintUnknownHeader(StringBuilder builder)
        {
            builder.AppendLine("  Unknown Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Dummy 0: {UH_Dummy0} (0x{UH_Dummy0:X})");
            builder.AppendLine($"  Dummy 1: {UH_Dummy1} (0x{UH_Dummy1:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print unknown entries information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintUnknownEntries(StringBuilder builder)
        {
            builder.AppendLine("  Unknown Entries Information:");
            builder.AppendLine("  -------------------------");
            if (UnknownEntries == null || UnknownEntries.Length == 0)
            {
                builder.AppendLine("  No unknown entries");
            }
            else
            {
                for (int i = 0; i < UnknownEntries.Length; i++)
                {
                    var unknownEntry = UnknownEntries[i];
                    builder.AppendLine($"  Unknown Entry {i}");
                    if (unknownEntry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine($"    Dummy 0: {unknownEntry.Dummy0} (0x{unknownEntry.Dummy0:X})");
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

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}