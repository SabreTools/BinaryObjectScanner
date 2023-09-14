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
            PrintFiles(builder, cabinet.Files);
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

            builder.AppendLine($"  Signature: {header.Signature}");
            builder.AppendLine($"  Reserved 1: {header.Reserved1} (0x{header.Reserved1:X})");
            builder.AppendLine($"  Cabinet size: {header.CabinetSize} (0x{header.CabinetSize:X})");
            builder.AppendLine($"  Reserved 2: {header.Reserved2} (0x{header.Reserved2:X})");
            builder.AppendLine($"  Files offset: {header.FilesOffset} (0x{header.FilesOffset:X})");
            builder.AppendLine($"  Reserved 3: {header.Reserved3} (0x{header.Reserved3:X})");
            builder.AppendLine($"  Minor version: {header.VersionMinor} (0x{header.VersionMinor:X})");
            builder.AppendLine($"  Major version: {header.VersionMajor} (0x{header.VersionMajor:X})");
            builder.AppendLine($"  Folder count: {header.FolderCount} (0x{header.FolderCount:X})");
            builder.AppendLine($"  File count: {header.FileCount} (0x{header.FileCount:X})");
            builder.AppendLine($"  Flags: {header.Flags} (0x{header.Flags:X})");
            builder.AppendLine($"  Set ID: {header.SetID} (0x{header.SetID:X})");
            builder.AppendLine($"  Cabinet index: {header.CabinetIndex} (0x{header.CabinetIndex:X})");

            if (header.Flags.HasFlag(HeaderFlags.RESERVE_PRESENT))
            {
                builder.AppendLine($"  Header reserved size: {header.HeaderReservedSize} (0x{header.HeaderReservedSize:X})");
                builder.AppendLine($"  Folder reserved size: {header.FolderReservedSize} (0x{header.FolderReservedSize:X})");
                builder.AppendLine($"  Data reserved size: {header.DataReservedSize} (0x{header.DataReservedSize:X})");
                builder.AppendLine($"  Reserved data = {(header.ReservedData == null ? "[NULL]" : BitConverter.ToString(header.ReservedData).Replace(" - ", " "))}");
            }

            if (header.Flags.HasFlag(HeaderFlags.PREV_CABINET))
            {
                builder.AppendLine($"  Previous cabinet: {header.CabinetPrev}");
                builder.AppendLine($"  Previous disk: {header.DiskPrev}");
            }

            if (header.Flags.HasFlag(HeaderFlags.NEXT_CABINET))
            {
                builder.AppendLine($"  Next cabinet: {header.CabinetNext}");
                builder.AppendLine($"  Next disk: {header.DiskNext}");
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

                builder.AppendLine($"    Cab start offset = {entry.CabStartOffset} (0x{entry.CabStartOffset:X})");
                builder.AppendLine($"    Data count = {entry.DataCount} (0x{entry.DataCount:X})");
                builder.AppendLine($"    Compression type = {entry.CompressionType} (0x{entry.CompressionType:X})");
                builder.AppendLine($"    Masked compression type = {entry.CompressionType & CompressionType.MASK_TYPE}");
                builder.AppendLine($"    Reserved data = {(entry.ReservedData == null ? "[NULL]" : BitConverter.ToString(entry.ReservedData).Replace("-", " "))}");
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

                    builder.AppendLine($"      Checksum = {dataBlock.Checksum} (0x{dataBlock.Checksum:X})");
                    builder.AppendLine($"      Compressed size = {dataBlock.CompressedSize} (0x{dataBlock.CompressedSize:X})");
                    builder.AppendLine($"      Uncompressed size = {dataBlock.UncompressedSize} (0x{dataBlock.UncompressedSize:X})");
                    builder.AppendLine($"      Reserved data = {(dataBlock.ReservedData == null ? "[NULL]" : BitConverter.ToString(dataBlock.ReservedData).Replace("-", " "))}");
                    //builder.AppendLine($"      Compressed data = {BitConverter.ToString(dataBlock.CompressedData).Replace("-", " ")}");
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void PrintFiles(StringBuilder builder, CFFILE[] entries)
#else
        private static void PrintFiles(StringBuilder builder, CFFILE?[]? entries)
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

                builder.AppendLine($"    File size = {entry.FileSize} (0x{entry.FileSize:X})");
                builder.AppendLine($"    Folder start offset = {entry.FolderStartOffset} (0x{entry.FolderStartOffset:X})");
                builder.AppendLine($"    Folder index = {entry.FolderIndex} (0x{entry.FolderIndex:X})");
                builder.AppendLine($"    Date = {entry.Date} (0x{entry.Date:X})");
                builder.AppendLine($"    Time = {entry.Time} (0x{entry.Time:X})");
                builder.AppendLine($"    Attributes = {entry.Attributes} (0x{entry.Attributes:X})");
                builder.AppendLine($"    Name = {entry.Name ?? "[NULL]"}");
            }
            builder.AppendLine();
        }
    }
}