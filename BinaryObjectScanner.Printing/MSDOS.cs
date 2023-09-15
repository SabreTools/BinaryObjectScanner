using System.Text;
using SabreTools.Models.MSDOS;

namespace BinaryObjectScanner.Printing
{
    public static class MSDOS
    {
        public static void Print(StringBuilder builder, Executable executable)
        {
            builder.AppendLine("MS-DOS Executable Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, executable.Header);
            Print(builder, executable.RelocationTable);
        }

#if NET48
        private static void Print(StringBuilder builder, ExecutableHeader header)
#else
        private static void Print(StringBuilder builder, ExecutableHeader? header)
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

            builder.AppendLine(header.Magic, "  Magic number");
            builder.AppendLine(header.LastPageBytes, "  Last page bytes");
            builder.AppendLine(header.Pages, "  Pages");
            builder.AppendLine(header.RelocationItems, "  Relocation items");
            builder.AppendLine(header.HeaderParagraphSize, "  Header paragraph size");
            builder.AppendLine(header.MinimumExtraParagraphs, "  Minimum extra paragraphs");
            builder.AppendLine(header.MaximumExtraParagraphs, "  Maximum extra paragraphs");
            builder.AppendLine(header.InitialSSValue, "  Initial SS value");
            builder.AppendLine(header.InitialSPValue, "  Initial SP value");
            builder.AppendLine(header.Checksum, "  Checksum");
            builder.AppendLine(header.InitialIPValue, "  Initial IP value");
            builder.AppendLine(header.InitialCSValue, "  Initial CS value");
            builder.AppendLine(header.RelocationTableAddr, "  Relocation table address");
            builder.AppendLine(header.OverlayNumber, "  Overlay number");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, RelocationEntry[] entries)
#else
        private static void Print(StringBuilder builder, RelocationEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Relocation Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No relocation table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Relocation Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.Offset, "    Offset");
                builder.AppendLine(entry.Segment, "    Segment");
            }
            builder.AppendLine();
        }
    }
}