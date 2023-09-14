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

            builder.AppendLine($"  Magic number: {header.Magic}");
            builder.AppendLine($"  Last page bytes: {header.LastPageBytes} (0x{header.LastPageBytes:X})");
            builder.AppendLine($"  Pages: {header.Pages} (0x{header.Pages:X})");
            builder.AppendLine($"  Relocation items: {header.RelocationItems} (0x{header.RelocationItems:X})");
            builder.AppendLine($"  Header paragraph size: {header.HeaderParagraphSize} (0x{header.HeaderParagraphSize:X})");
            builder.AppendLine($"  Minimum extra paragraphs: {header.MinimumExtraParagraphs} (0x{header.MinimumExtraParagraphs:X})");
            builder.AppendLine($"  Maximum extra paragraphs: {header.MaximumExtraParagraphs} (0x{header.MaximumExtraParagraphs:X})");
            builder.AppendLine($"  Initial SS value: {header.InitialSSValue} (0x{header.InitialSSValue:X})");
            builder.AppendLine($"  Initial SP value: {header.InitialSPValue} (0x{header.InitialSPValue:X})");
            builder.AppendLine($"  Checksum: {header.Checksum} (0x{header.Checksum:X})");
            builder.AppendLine($"  Initial IP value: {header.InitialIPValue} (0x{header.InitialIPValue:X})");
            builder.AppendLine($"  Initial CS value: {header.InitialCSValue} (0x{header.InitialCSValue:X})");
            builder.AppendLine($"  Relocation table address: {header.RelocationTableAddr} (0x{header.RelocationTableAddr:X})");
            builder.AppendLine($"  Overlay number: {header.OverlayNumber} (0x{header.OverlayNumber:X})");
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
                    builder.AppendLine($"    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Offset: {entry.Offset} (0x{entry.Offset:X})");
                builder.AppendLine($"    Segment: {entry.Segment} (0x{entry.Segment:X})");
            }
            builder.AppendLine();
        }
    }
}