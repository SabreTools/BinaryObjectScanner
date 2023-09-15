using System;
using System.Text;
using SabreTools.Models.MicrosoftCabinet;

namespace BinaryObjectScanner.Printing
{
    public static class MicrosoftCabinet
    {
        public static void Print(StringBuilder builder, Cabinet cabinet)
        {
            builder.AppendLine("Microsoft Cabinet Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, cabinet.Header);
            Print(builder, cabinet.Folders);
            Print(builder, cabinet.Files);
        }

#if NET48
        private static void Print(StringBuilder builder, CFHEADER header)
#else
        private static void Print(StringBuilder builder, CFHEADER? header)
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
            builder.AppendLine(header.Reserved1, "  Reserved 1");
            builder.AppendLine(header.CabinetSize, "  Cabinet size");
            builder.AppendLine(header.Reserved2, "  Reserved 2");
            builder.AppendLine(header.FilesOffset, "  Files offset");
            builder.AppendLine(header.Reserved3, "  Reserved 3");
            builder.AppendLine(header.VersionMinor, "  Minor version");
            builder.AppendLine(header.VersionMajor, "  Major version");
            builder.AppendLine(header.FolderCount, "  Folder count");
            builder.AppendLine(header.FileCount, "  File count");
            builder.AppendLine($"  Flags: {header.Flags} (0x{header.Flags:X})");
            builder.AppendLine(header.SetID, "  Set ID");
            builder.AppendLine(header.CabinetIndex, "  Cabinet index");

            if (header.Flags.HasFlag(HeaderFlags.RESERVE_PRESENT))
            {
                builder.AppendLine(header.HeaderReservedSize, "  Header reserved size");
                builder.AppendLine(header.FolderReservedSize, "  Folder reserved size");
                builder.AppendLine(header.DataReservedSize, "  Data reserved size");
                builder.AppendLine(header.ReservedData, "  Reserved data");
            }

            if (header.Flags.HasFlag(HeaderFlags.PREV_CABINET))
            {
                builder.AppendLine(header.CabinetPrev, "  Previous cabinet");
                builder.AppendLine(header.DiskPrev, "  Previous disk");
            }

            if (header.Flags.HasFlag(HeaderFlags.NEXT_CABINET))
            {
                builder.AppendLine(header.CabinetNext, "  Next cabinet");
                builder.AppendLine(header.DiskNext, "  Next disk");
            }

            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, CFFOLDER[] entries)
#else
        private static void Print(StringBuilder builder, CFFOLDER?[]? entries)
#endif
        {
            builder.AppendLine("  Folders:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No folders");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Folder {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.CabStartOffset, "    Cab start offset");
                builder.AppendLine(entry.DataCount, "    Data count");
                builder.AppendLine($"    Compression type: {entry.CompressionType} (0x{entry.CompressionType:X})");
                builder.AppendLine($"    Masked compression type: {entry.CompressionType & CompressionType.MASK_TYPE}");
                builder.AppendLine(entry.ReservedData, "    Reserved data");
                builder.AppendLine();

                builder.AppendLine("    Data Blocks");
                builder.AppendLine("    -------------------------");
                if (entry.DataBlocks == null || entry.DataBlocks.Length == 0)
                {
                    builder.AppendLine("    No data blocks");
                    continue;
                }

                for (int j = 0; j < entry.DataBlocks.Length; j++)
                {
                    var dataBlock = entry.DataBlocks[j];
                    builder.AppendLine($"    Data Block {j}");
                    if (dataBlock == null)
                    {
                        builder.AppendLine("      [NULL]");
                        continue;
                    }

                    builder.AppendLine(dataBlock.Checksum, "      Checksum");
                    builder.AppendLine(dataBlock.CompressedSize, "      Compressed size");
                    builder.AppendLine(dataBlock.UncompressedSize, "      Uncompressed size");
                    builder.AppendLine(dataBlock.ReservedData, "      Reserved data");
                    //builder.AppendLine(dataBlock.CompressedData, "      Compressed data");
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, CFFILE[] entries)
#else
        private static void Print(StringBuilder builder, CFFILE?[]? entries)
#endif
        {
            builder.AppendLine("  Files:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No files");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  File {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.FileSize, "    File size");
                builder.AppendLine(entry.FolderStartOffset, "    Folder start offset");
                builder.AppendLine($"    Folder index: {entry.FolderIndex} (0x{entry.FolderIndex:X})");
                builder.AppendLine(entry.Date, "    Date");
                builder.AppendLine(entry.Time, "    Time");
                builder.AppendLine($"    Attributes: {entry.Attributes} (0x{entry.Attributes:X})");
                builder.AppendLine(entry.Name, "    Name");
            }
            builder.AppendLine();
        }
    }
}