using System.Text;
using SabreTools.Models.BFPK;

namespace BinaryObjectScanner.Printing
{
    public static class BFPK
    {
        public static void Print(StringBuilder builder, Archive archive)
        {
            builder.AppendLine("BFPK Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, archive.Header);
            Print(builder, archive.Files);
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

            builder.AppendLine(header.Magic, "  Magic");
            builder.AppendLine(header.Version, "  Version");
            builder.AppendLine(header.Files, "  Files");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, FileEntry[] files)
#else
        private static void Print(StringBuilder builder, FileEntry?[]? files)
#endif
        {
            builder.AppendLine("  File Table Information:");
            builder.AppendLine("  -------------------------");
            if (files == null || files.Length == 0)
            {
                builder.AppendLine("  No file table items");
                return;
            }

            for (int i = 0; i < files.Length; i++)
            {
                var entry = files[i];
                builder.AppendLine($"  File Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.NameSize, "    Name size");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.UncompressedSize, "    Uncompressed size");
                builder.AppendLine(entry.Offset, "    Offset");
                builder.AppendLine(entry.CompressedSize, "    Compressed size");
            }
        }
    }
}