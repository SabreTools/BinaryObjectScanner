using System.Text;
using SabreTools.Models.XZP;

namespace BinaryObjectScanner.Printing
{
    public static class XZP
    {
        public static void Print(StringBuilder builder, File file)
        {
            builder.AppendLine("XZP Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, file.Header);
            Print(builder, file.DirectoryEntries, "Directory");
            Print(builder, file.PreloadDirectoryEntries, "Preload Directory");
            Print(builder, file.PreloadDirectoryMappings);
            Print(builder, file.DirectoryItems);
            Print(builder, file.Footer);
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

            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine(header.Version, "  Version");
            builder.AppendLine(header.PreloadDirectoryEntryCount, "  Preload directory entry count");
            builder.AppendLine(header.DirectoryEntryCount, "  Directory entry count");
            builder.AppendLine(header.PreloadBytes, "  Preload bytes");
            builder.AppendLine(header.HeaderLength, "  Header length");
            builder.AppendLine(header.DirectoryItemCount, "  Directory item count");
            builder.AppendLine(header.DirectoryItemOffset, "  Directory item offset");
            builder.AppendLine(header.DirectoryItemLength, "  Directory item length");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DirectoryEntry[] entries, string prefix)
#else
        private static void Print(StringBuilder builder, DirectoryEntry?[]? entries, string prefix)
#endif
        {
            builder.AppendLine($"  {prefix} Entries Information:");
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

                builder.AppendLine(entry.FileNameCRC, "    File name CRC");
                builder.AppendLine(entry.EntryLength, "    Entry length");
                builder.AppendLine(entry.EntryOffset, "    Entry offset");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DirectoryMapping[] entries)
#else
        private static void Print(StringBuilder builder, DirectoryMapping?[]? entries)
#endif
        {
            builder.AppendLine("  Preload Directory Mappings Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No preload directory mappings");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Directory Mapping {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.PreloadDirectoryEntryIndex, "    Preload directory entry index");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DirectoryItem[] entries)
#else
        private static void Print(StringBuilder builder, DirectoryItem?[]? entries)
#endif
        {
            builder.AppendLine("  Directory Items Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directory items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Directory Item {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.FileNameCRC, "    File name CRC");
                builder.AppendLine(entry.NameOffset, "    Name offset");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.TimeCreated, "    Time created");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, Footer footer)
#else
        private static void Print(StringBuilder builder, Footer? footer)
#endif
        {
            builder.AppendLine("  Footer Information:");
            builder.AppendLine("  -------------------------");
            if (footer == null)
            {
                builder.AppendLine("  No header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(footer.FileLength, "  File length");
            builder.AppendLine(footer.Signature, "  Signature");
            builder.AppendLine();
        }
    }
}