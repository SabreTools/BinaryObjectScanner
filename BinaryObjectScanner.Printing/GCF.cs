using System.Text;
using SabreTools.Models.GCF;

namespace BinaryObjectScanner.Printing
{
    public static class GCF
    {
        public static void Print(StringBuilder builder, File file)
        {
            builder.AppendLine("GCF Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Header
            Print(builder, file.Header);

            // Block Entries
            Print(builder, file.BlockEntryHeader);
            Print(builder, file.BlockEntries);

            // Fragmentation Maps
            Print(builder, file.FragmentationMapHeader);
            Print(builder, file.FragmentationMaps);

            // Block Entry Maps
            Print(builder, file.BlockEntryMapHeader);
            Print(builder, file.BlockEntryMaps);

            // Directory and Directory Maps
            Print(builder, file.DirectoryHeader);
            Print(builder, file.DirectoryEntries);
            // TODO: Should we print out the entire string table?
            Print(builder, file.DirectoryInfo1Entries);
            Print(builder, file.DirectoryInfo2Entries);
            Print(builder, file.DirectoryCopyEntries);
            Print(builder, file.DirectoryLocalEntries);
            Print(builder, file.DirectoryMapHeader);
            Print(builder, file.DirectoryMapEntries);

            // Checksums and Checksum Maps
            Print(builder, file.ChecksumHeader);
            Print(builder, file.ChecksumMapHeader);
            Print(builder, file.ChecksumMapEntries);
            Print(builder, file.ChecksumEntries);

            // Data Blocks
            Print(builder, file.DataBlockHeader);
        }

#if NET48
        private static void Print(StringBuilder builder, Header header)
#else
        private static void Print(StringBuilder builder, Header? header)
#endif
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Dummy 0: {header.Dummy0} (0x{header.Dummy0:X})");
            builder.AppendLine($"  Major version: {header.MajorVersion} (0x{header.MajorVersion:X})");
            builder.AppendLine($"  Minor version: {header.MinorVersion} (0x{header.MinorVersion:X})");
            builder.AppendLine($"  Cache ID: {header.CacheID} (0x{header.CacheID:X})");
            builder.AppendLine($"  Last version played: {header.LastVersionPlayed} (0x{header.LastVersionPlayed:X})");
            builder.AppendLine($"  Dummy 1: {header.Dummy1} (0x{header.Dummy1:X})");
            builder.AppendLine($"  Dummy 2: {header.Dummy2} (0x{header.Dummy2:X})");
            builder.AppendLine($"  File size: {header.FileSize} (0x{header.FileSize:X})");
            builder.AppendLine($"  Block size: {header.BlockSize} (0x{header.BlockSize:X})");
            builder.AppendLine($"  Block count: {header.BlockCount} (0x{header.BlockCount:X})");
            builder.AppendLine($"  Dummy 3: {header.Dummy3} (0x{header.Dummy3:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, BlockEntryHeader header)
#else
        private static void Print(StringBuilder builder, BlockEntryHeader? header)
#endif
        {
            builder.AppendLine("  Block Entry Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No block entry header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Block count: {header.BlockCount} (0x{header.BlockCount:X})");
            builder.AppendLine($"  Blocks used: {header.BlocksUsed} (0x{header.BlocksUsed:X})");
            builder.AppendLine($"  Dummy 0: {header.Dummy0} (0x{header.Dummy0:X})");
            builder.AppendLine($"  Dummy 1: {header.Dummy1} (0x{header.Dummy1:X})");
            builder.AppendLine($"  Dummy 2: {header.Dummy2} (0x{header.Dummy2:X})");
            builder.AppendLine($"  Dummy 3: {header.Dummy3} (0x{header.Dummy3:X})");
            builder.AppendLine($"  Dummy 4: {header.Dummy4} (0x{header.Dummy4:X})");
            builder.AppendLine($"  Checksum: {header.Checksum} (0x{header.Checksum:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, BlockEntry[] entries)
#else
        private static void Print(StringBuilder builder, BlockEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Block Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No block entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Block Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Entry flags: {entry.EntryFlags} (0x{entry.EntryFlags:X})");
                builder.AppendLine($"    File data offset: {entry.FileDataOffset} (0x{entry.FileDataOffset:X})");
                builder.AppendLine($"    File data size: {entry.FileDataSize} (0x{entry.FileDataSize:X})");
                builder.AppendLine($"    First data block index: {entry.FirstDataBlockIndex} (0x{entry.FirstDataBlockIndex:X})");
                builder.AppendLine($"    Next block entry index: {entry.NextBlockEntryIndex} (0x{entry.NextBlockEntryIndex:X})");
                builder.AppendLine($"    Previous block entry index: {entry.PreviousBlockEntryIndex} (0x{entry.PreviousBlockEntryIndex:X})");
                builder.AppendLine($"    Directory index: {entry.DirectoryIndex} (0x{entry.DirectoryIndex:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, FragmentationMapHeader header)
#else
        private static void Print(StringBuilder builder, FragmentationMapHeader? header)
#endif
        {
            builder.AppendLine("  Fragmentation Map Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No fragmentation map header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Block count: {header.BlockCount} (0x{header.BlockCount:X})");
            builder.AppendLine($"  First unused entry: {header.FirstUnusedEntry} (0x{header.FirstUnusedEntry:X})");
            builder.AppendLine($"  Terminator: {header.Terminator} (0x{header.Terminator:X})");
            builder.AppendLine($"  Checksum: {header.Checksum} (0x{header.Checksum:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, FragmentationMap[] entries)
#else
        private static void Print(StringBuilder builder, FragmentationMap?[]? entries)
#endif
        {
            builder.AppendLine("  Fragmentation Maps Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No fragmentation maps");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Fragmentation Map {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Next data block index: {entry.NextDataBlockIndex} (0x{entry.NextDataBlockIndex:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, BlockEntryMapHeader header)
#else
        private static void Print(StringBuilder builder, BlockEntryMapHeader? header)
#endif
        {
            builder.AppendLine("  Block Entry Map Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine($"  No block entry map header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Block count: {header.BlockCount} (0x{header.BlockCount:X})");
            builder.AppendLine($"  First block entry index: {header.FirstBlockEntryIndex} (0x{header.FirstBlockEntryIndex:X})");
            builder.AppendLine($"  Last block entry index: {header.LastBlockEntryIndex} (0x{header.LastBlockEntryIndex:X})");
            builder.AppendLine($"  Dummy 0: {header.Dummy0} (0x{header.Dummy0:X})");
            builder.AppendLine($"  Checksum: {header.Checksum} (0x{header.Checksum:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, BlockEntryMap[] entries)
#else
        private static void Print(StringBuilder builder, BlockEntryMap?[]? entries)
#endif
        {
            builder.AppendLine("  Block Entry Maps Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No block entry maps");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Block Entry Map {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Previous data block index: {entry.PreviousBlockEntryIndex} (0x{entry.PreviousBlockEntryIndex:X})");
                builder.AppendLine($"    Next data block index: {entry.NextBlockEntryIndex} (0x{entry.NextBlockEntryIndex:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DirectoryHeader header)
#else
        private static void Print(StringBuilder builder, DirectoryHeader? header)
#endif
        {
            builder.AppendLine("  Directory Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No directory header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Dummy 0: {header.Dummy0} (0x{header.Dummy0:X})");
            builder.AppendLine($"  Cache ID: {header.CacheID} (0x{header.CacheID:X})");
            builder.AppendLine($"  Last version played: {header.LastVersionPlayed} (0x{header.LastVersionPlayed:X})");
            builder.AppendLine($"  Item count: {header.ItemCount} (0x{header.ItemCount:X})");
            builder.AppendLine($"  File count: {header.FileCount} (0x{header.FileCount:X})");
            builder.AppendLine($"  Dummy 1: {header.Dummy1} (0x{header.Dummy1:X})");
            builder.AppendLine($"  Directory size: {header.DirectorySize} (0x{header.DirectorySize:X})");
            builder.AppendLine($"  Name size: {header.NameSize} (0x{header.NameSize:X})");
            builder.AppendLine($"  Info 1 count: {header.Info1Count} (0x{header.Info1Count:X})");
            builder.AppendLine($"  Copy count: {header.CopyCount} (0x{header.CopyCount:X})");
            builder.AppendLine($"  Local count: {header.LocalCount} (0x{header.LocalCount:X})");
            builder.AppendLine($"  Dummy 2: {header.Dummy2} (0x{header.Dummy2:X})");
            builder.AppendLine($"  Dummy 3: {header.Dummy3} (0x{header.Dummy3:X})");
            builder.AppendLine($"  Checksum: {header.Checksum} (0x{header.Checksum:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DirectoryEntry[] entries)
#else
        private static void Print(StringBuilder builder, DirectoryEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Directory Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directory entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Directory Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Name offset: {entry.NameOffset} (0x{entry.NameOffset:X})");
                builder.AppendLine($"    Name: {entry.Name ?? "[NULL]"}");
                builder.AppendLine($"    Item size: {entry.ItemSize} (0x{entry.ItemSize:X})");
                builder.AppendLine($"    Checksum index: {entry.ChecksumIndex} (0x{entry.ChecksumIndex:X})");
                builder.AppendLine($"    Directory flags: {entry.DirectoryFlags} (0x{entry.DirectoryFlags:X})");
                builder.AppendLine($"    Parent index: {entry.ParentIndex} (0x{entry.ParentIndex:X})");
                builder.AppendLine($"    Next index: {entry.NextIndex} (0x{entry.NextIndex:X})");
                builder.AppendLine($"    First index: {entry.FirstIndex} (0x{entry.FirstIndex:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DirectoryInfo1Entry[] entries)
#else
        private static void Print(StringBuilder builder, DirectoryInfo1Entry?[]? entries)
#endif
        {
            builder.AppendLine("  Directory Info 1 Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directory info 1 entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Directory Info 1 Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Dummy 0: {entry.Dummy0} (0x{entry.Dummy0:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DirectoryInfo2Entry[] entries)
#else
        private static void Print(StringBuilder builder, DirectoryInfo2Entry?[]? entries)
#endif
        {
            builder.AppendLine("  Directory Info 2 Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directory info 2 entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Directory Info 2 Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Dummy 0: {entry.Dummy0} (0x{entry.Dummy0:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DirectoryCopyEntry[] entries)
#else
        private static void Print(StringBuilder builder, DirectoryCopyEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Directory Copy Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directory copy entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Directory Copy Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Directory index: {entry.DirectoryIndex} (0x{entry.DirectoryIndex:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DirectoryLocalEntry[] entries)
#else
        private static void Print(StringBuilder builder, DirectoryLocalEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Directory Local Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directory local entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Directory Local Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Directory index: {entry.DirectoryIndex} (0x{entry.DirectoryIndex:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DirectoryMapHeader header)
#else
        private static void Print(StringBuilder builder, DirectoryMapHeader? header)
#endif
        {
            builder.AppendLine("  Directory Map Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine($"  No directory map header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Dummy 0: {header.Dummy0} (0x{header.Dummy0:X})");
            builder.AppendLine($"  Dummy 1: {header.Dummy1} (0x{header.Dummy1:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DirectoryMapEntry[] entries)
#else
        private static void Print(StringBuilder builder, DirectoryMapEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Directory Map Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directory map entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Directory Map Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    First block index: {entry.FirstBlockIndex} (0x{entry.FirstBlockIndex:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ChecksumHeader header)
#else
        private static void Print(StringBuilder builder, ChecksumHeader? header)
#endif
        {
            builder.AppendLine("  Checksum Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No checksum header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Dummy 0: {header.Dummy0} (0x{header.Dummy0:X})");
            builder.AppendLine($"  Checksum size: {header.ChecksumSize} (0x{header.ChecksumSize:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ChecksumMapHeader header)
#else
        private static void Print(StringBuilder builder, ChecksumMapHeader? header)
#endif
        {
            builder.AppendLine("  Checksum Map Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No checksum map header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Dummy 0: {header.Dummy0} (0x{header.Dummy0:X})");
            builder.AppendLine($"  Dummy 1: {header.Dummy1} (0x{header.Dummy1:X})");
            builder.AppendLine($"  Item count: {header.ItemCount} (0x{header.ItemCount:X})");
            builder.AppendLine($"  Checksum count: {header.ChecksumCount} (0x{header.ChecksumCount:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ChecksumMapEntry[] entries)
#else
        private static void Print(StringBuilder builder, ChecksumMapEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Checksum Map Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No checksum map entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Checksum Map Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Checksum count: {entry.ChecksumCount} (0x{entry.ChecksumCount:X})");
                builder.AppendLine($"    First checksum index: {entry.FirstChecksumIndex} (0x{entry.FirstChecksumIndex:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ChecksumEntry[] entries)
#else
        private static void Print(StringBuilder builder, ChecksumEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Checksum Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No checksum entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Checksum Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Checksum: {entry.Checksum} (0x{entry.Checksum:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DataBlockHeader header)
#else
        private static void Print(StringBuilder builder, DataBlockHeader? header)
#endif
        {
            builder.AppendLine("  Data Block Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No data block header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Last version played: {header.LastVersionPlayed} (0x{header.LastVersionPlayed:X})");
            builder.AppendLine($"  Block count: {header.BlockCount} (0x{header.BlockCount:X})");
            builder.AppendLine($"  Block size: {header.BlockSize} (0x{header.BlockSize:X})");
            builder.AppendLine($"  First block offset: {header.FirstBlockOffset} (0x{header.FirstBlockOffset:X})");
            builder.AppendLine($"  Blocks used: {header.BlocksUsed} (0x{header.BlocksUsed:X})");
            builder.AppendLine($"  Checksum: {header.Checksum} (0x{header.Checksum:X})");
            builder.AppendLine();
        }
    }
}