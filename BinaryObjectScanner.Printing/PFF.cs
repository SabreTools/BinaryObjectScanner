using System.Text;
using SabreTools.Models.PFF;

namespace BinaryObjectScanner.Printing
{
    public static class PFF
    {
        public static void Print(StringBuilder builder, Archive archive)
        {
            builder.AppendLine("PFF Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, archive.Header);
            Print(builder, archive.Segments);
            Print(builder, archive.Footer);
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

            builder.AppendLine(header.HeaderSize, "  Header size");
            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine(header.NumberOfFiles, "  Number of files");
            builder.AppendLine(header.FileSegmentSize, "  File segment size");
            builder.AppendLine(header.FileListOffset, "  File list offset");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, Segment[] segments)
#else
        private static void Print(StringBuilder builder, Segment?[]? segments)
#endif
        {
            builder.AppendLine("  Segments Information:");
            builder.AppendLine("  -------------------------");
            if (segments == null || segments.Length == 0)
            {
                builder.AppendLine("  No segments");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                builder.AppendLine($"  Segment {i}");
                if (segment == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(segment.Deleted, "    Deleted");
                builder.AppendLine(segment.FileLocation, "    File location");
                builder.AppendLine(segment.FileSize, "    File size");
                builder.AppendLine(segment.PackedDate, "    Packed date");
                builder.AppendLine(segment.FileName, "    File name");
                builder.AppendLine(segment.ModifiedDate, "    Modified date");
                builder.AppendLine(segment.CompressionLevel, "    Compression level");
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
                builder.AppendLine("  No footer");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(footer.SystemIP, "  System IP");
            builder.AppendLine(footer.Reserved, "  Reserved");
            builder.AppendLine(footer.KingTag, "  King tag");
            builder.AppendLine();
        }
    }
}