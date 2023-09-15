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

            builder.AppendLine(header.Dummy0, "  Dummy 0");
            builder.AppendLine(header.MajorVersion, "  Major version");
            builder.AppendLine(header.MinorVersion, "  Minor version");
            builder.AppendLine(header.CacheID, "  Cache ID");
            builder.AppendLine(header.LastVersionPlayed, "  Last version played");
            builder.AppendLine(header.Dummy1, "  Dummy 1");
            builder.AppendLine(header.Dummy2, "  Dummy 2");
            builder.AppendLine(header.FileSize, "  File size");
            builder.AppendLine(header.BlockSize, "  Block size");
            builder.AppendLine(header.BlockCount, "  Block count");
            builder.AppendLine(header.Dummy3, "  Dummy 3");
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

            builder.AppendLine(header.BlockCount, "  Block count");
            builder.AppendLine(header.BlocksUsed, "  Blocks used");
            builder.AppendLine(header.Dummy0, "  Dummy 0");
            builder.AppendLine(header.Dummy1, "  Dummy 1");
            builder.AppendLine(header.Dummy2, "  Dummy 2");
            builder.AppendLine(header.Dummy3, "  Dummy 3");
            builder.AppendLine(header.Dummy4, "  Dummy 4");
            builder.AppendLine(header.Checksum, "  Checksum");
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

                builder.AppendLine(entry.EntryFlags, "    Entry flags");
                builder.AppendLine(entry.FileDataOffset, "    File data offset");
                builder.AppendLine(entry.FileDataSize, "    File data size");
                builder.AppendLine(entry.FirstDataBlockIndex, "    First data block index");
                builder.AppendLine(entry.NextBlockEntryIndex, "    Next block entry index");
                builder.AppendLine(entry.PreviousBlockEntryIndex, "    Previous block entry index");
                builder.AppendLine(entry.DirectoryIndex, "    Directory index");
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

            builder.AppendLine(header.BlockCount, "  Block count");
            builder.AppendLine(header.FirstUnusedEntry, "  First unused entry");
            builder.AppendLine(header.Terminator, "  Terminator");
            builder.AppendLine(header.Checksum, "  Checksum");
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

                builder.AppendLine(entry.NextDataBlockIndex, "    Next data block index");
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
                builder.AppendLine("  No block entry map header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.BlockCount, "  Block count");
            builder.AppendLine(header.FirstBlockEntryIndex, "  First block entry index");
            builder.AppendLine(header.LastBlockEntryIndex, "  Last block entry index");
            builder.AppendLine(header.Dummy0, "  Dummy 0");
            builder.AppendLine(header.Checksum, "  Checksum");
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

                builder.AppendLine(entry.PreviousBlockEntryIndex, "    Previous data block index");
                builder.AppendLine(entry.NextBlockEntryIndex, "    Next data block index");
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

            builder.AppendLine(header.Dummy0, "  Dummy 0");
            builder.AppendLine(header.CacheID, "  Cache ID");
            builder.AppendLine(header.LastVersionPlayed, "  Last version played");
            builder.AppendLine(header.ItemCount, "  Item count");
            builder.AppendLine(header.FileCount, "  File count");
            builder.AppendLine(header.Dummy1, "  Dummy 1");
            builder.AppendLine(header.DirectorySize, "  Directory size");
            builder.AppendLine(header.NameSize, "  Name size");
            builder.AppendLine(header.Info1Count, "  Info 1 count");
            builder.AppendLine(header.CopyCount, "  Copy count");
            builder.AppendLine(header.LocalCount, "  Local count");
            builder.AppendLine(header.Dummy2, "  Dummy 2");
            builder.AppendLine(header.Dummy3, "  Dummy 3");
            builder.AppendLine(header.Checksum, "  Checksum");
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

                builder.AppendLine(entry.NameOffset, "    Name offset");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.ItemSize, "    Item size");
                builder.AppendLine(entry.ChecksumIndex, "    Checksum index");
                builder.AppendLine($"    Directory flags: {entry.DirectoryFlags} (0x{entry.DirectoryFlags:X})");
                builder.AppendLine(entry.ParentIndex, "    Parent index");
                builder.AppendLine(entry.NextIndex, "    Next index");
                builder.AppendLine(entry.FirstIndex, "    First index");
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

                builder.AppendLine(entry.Dummy0, "    Dummy 0");
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

                builder.AppendLine(entry.Dummy0, "    Dummy 0");
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

                builder.AppendLine(entry.DirectoryIndex, "    Directory index");
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

                builder.AppendLine(entry.DirectoryIndex, "    Directory index");
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
                builder.AppendLine("  No directory map header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.Dummy0, "  Dummy 0");
            builder.AppendLine(header.Dummy1, "  Dummy 1");
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

                builder.AppendLine(entry.FirstBlockIndex, "    First block index");
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

            builder.AppendLine(header.Dummy0, "  Dummy 0");
            builder.AppendLine(header.ChecksumSize, "  Checksum size");
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

            builder.AppendLine(header.Dummy0, "  Dummy 0");
            builder.AppendLine(header.Dummy1, "  Dummy 1");
            builder.AppendLine(header.ItemCount, "  Item count");
            builder.AppendLine(header.ChecksumCount, "  Checksum count");
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

                builder.AppendLine(entry.ChecksumCount, "    Checksum count");
                builder.AppendLine(entry.FirstChecksumIndex, "    First checksum index");
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

                builder.AppendLine(entry.Checksum, "    Checksum");
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

            builder.AppendLine(header.LastVersionPlayed, "  Last version played");
            builder.AppendLine(header.BlockCount, "  Block count");
            builder.AppendLine(header.BlockSize, "  Block size");
            builder.AppendLine(header.FirstBlockOffset, "  First block offset");
            builder.AppendLine(header.BlocksUsed, "  Blocks used");
            builder.AppendLine(header.Checksum, "  Checksum");
            builder.AppendLine();
        }
    }
}