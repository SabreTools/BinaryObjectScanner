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

            builder.AppendLine($"  Magic: {header.Magic}");
            builder.AppendLine($"  Version: {header.Version} (0x{header.Version:X})");
            builder.AppendLine($"  Files: {header.Files} (0x{header.Files:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, FileEntry[] files)
#else
        private static void Print(StringBuilder builder, FileEntry[]? files)
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

                builder.AppendLine($"    Name size: {entry.NameSize} (0x{entry.NameSize:X})");
                builder.AppendLine($"    Name: {entry.Name}");
                builder.AppendLine($"    Uncompressed size: {entry.UncompressedSize} (0x{entry.UncompressedSize:X})");
                builder.AppendLine($"    Offset: {entry.Offset} (0x{entry.Offset:X})");
                builder.AppendLine($"    Compressed Size: {entry.CompressedSize} (0x{entry.CompressedSize:X})");
            }
        }
    }
}