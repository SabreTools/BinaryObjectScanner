using System;
using System.Text;
using SabreTools.Models.CFB;

namespace BinaryObjectScanner.Printing
{
    public static class CFB
    {
        public static void Print(StringBuilder builder, Binary binary)
        {
            builder.AppendLine("Compound File Binary Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, binary.Header);
            Print(builder, binary.FATSectorNumbers, "FAT");
            Print(builder, binary.MiniFATSectorNumbers, "Mini FAT");
            Print(builder, binary.DIFATSectorNumbers, "DIFAT");
            Print(builder, binary.DirectoryEntries);
        }

#if NET48
        private static void Print(StringBuilder builder, FileHeader header)
#else
        private static void Print(StringBuilder builder, FileHeader? header)
#endif
        {
            builder.AppendLine("  File Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No file header");
                return;
            }

            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine(header.CLSID, "  CLSID");
            builder.AppendLine(header.MinorVersion, "  Minor version");
            builder.AppendLine(header.MajorVersion, "  Major version");
            builder.AppendLine(header.ByteOrder, "  Byte order");
            builder.AppendLine($"  Sector shift: {header.SectorShift} (0x{header.SectorShift:X}) => {Math.Pow(2, header.SectorShift)}");
            builder.AppendLine($"  Mini sector shift: {header.MiniSectorShift} (0x{header.MiniSectorShift:X}) => {Math.Pow(2, header.MiniSectorShift)}");
            builder.AppendLine(header.Reserved, "  Reserved");
            builder.AppendLine(header.NumberOfDirectorySectors, "  Number of directory sectors");
            builder.AppendLine(header.NumberOfFATSectors, "  Number of FAT sectors");
            builder.AppendLine(header.FirstDirectorySectorLocation, "  First directory sector location");
            builder.AppendLine(header.TransactionSignatureNumber, "  Transaction signature number");
            builder.AppendLine(header.MiniStreamCutoffSize, "  Mini stream cutoff size");
            builder.AppendLine(header.FirstMiniFATSectorLocation, "  First mini FAT sector location");
            builder.AppendLine(header.NumberOfMiniFATSectors, "  Number of mini FAT sectors");
            builder.AppendLine(header.FirstDIFATSectorLocation, "  First DIFAT sector location");
            builder.AppendLine(header.NumberOfDIFATSectors, "  Number of DIFAT sectors");
            builder.AppendLine("  DIFAT:");
            if (header.DIFAT == null || header.DIFAT.Length == 0)
            {
                builder.AppendLine("  No DIFAT entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < header.DIFAT.Length; i++)
            {
                builder.AppendLine($"    DIFAT Entry {i}: {header.DIFAT[i]} (0x{header.DIFAT[i]:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, SectorNumber[] sectorNumbers, string name)
#else
        private static void Print(StringBuilder builder, SectorNumber?[]? sectorNumbers, string name)
#endif
        {
            builder.AppendLine($"  {name} Sectors Information:");
            builder.AppendLine("  -------------------------");
            if (sectorNumbers == null || sectorNumbers.Length == 0)
            {
                builder.AppendLine($"  No {name} sectors");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < sectorNumbers.Length; i++)
            {
                builder.AppendLine($"  {name} Sector Entry {i}: {sectorNumbers[i]} (0x{sectorNumbers[i]:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DirectoryEntry[] directoryEntries)
#else
        private static void Print(StringBuilder builder, DirectoryEntry?[]? directoryEntries)
#endif
        {
            builder.AppendLine("  Directory Entries Information:");
            builder.AppendLine("  -------------------------");
            if (directoryEntries == null || directoryEntries.Length == 0)
            {
                builder.AppendLine("  No directory entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < directoryEntries.Length; i++)
            {
                var directoryEntry = directoryEntries[i];
                builder.AppendLine($"  Directory Entry {i}");
                if (directoryEntry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(directoryEntry.Name, "    Name");
                builder.AppendLine(directoryEntry.NameLength, "    Name length");
                builder.AppendLine($"    Object type: {directoryEntry.ObjectType} (0x{directoryEntry.ObjectType:X})");
                builder.AppendLine($"    Color flag: {directoryEntry.ColorFlag} (0x{directoryEntry.ColorFlag:X})");
                builder.AppendLine($"    Left sibling ID: {directoryEntry.LeftSiblingID} (0x{directoryEntry.LeftSiblingID:X})");
                builder.AppendLine($"    Right sibling ID: {directoryEntry.RightSiblingID} (0x{directoryEntry.RightSiblingID:X})");
                builder.AppendLine($"    Child ID: {directoryEntry.ChildID} (0x{directoryEntry.ChildID:X})");
                builder.AppendLine(directoryEntry.CLSID, "    CLSID");
                builder.AppendLine(directoryEntry.StateBits, "    State bits");
                builder.AppendLine(directoryEntry.CreationTime, "    Creation time");
                builder.AppendLine(directoryEntry.ModifiedTime, "    Modification time");
                builder.AppendLine(directoryEntry.StartingSectorLocation, "    Staring sector location");
                builder.AppendLine(directoryEntry.StreamSize, "    Stream size");
            }
            builder.AppendLine();
        }

    }
}