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

            builder.AppendLine($"  Signature: {header.Signature} (0x{header.Signature:X})");
            builder.AppendLine($"  CLSID: {header.CLSID}");
            builder.AppendLine($"  Minor version: {header.MinorVersion} (0x{header.MinorVersion:X})");
            builder.AppendLine($"  Major version: {header.MajorVersion} (0x{header.MajorVersion:X})");
            builder.AppendLine($"  Byte order: {header.ByteOrder} (0x{header.ByteOrder:X})");
            builder.AppendLine($"  Sector shift: {header.SectorShift} (0x{header.SectorShift:X}) => {Math.Pow(2, header.SectorShift)}");
            builder.AppendLine($"  Mini sector shift: {header.MiniSectorShift} (0x{header.MiniSectorShift:X}) => {Math.Pow(2, header.MiniSectorShift)}");
            builder.AppendLine($"  Reserved: {(header.Reserved == null ? "[NULL]" : BitConverter.ToString(header.Reserved).Replace('-', ' '))}");
            builder.AppendLine($"  Number of directory sectors: {header.NumberOfDirectorySectors} (0x{header.NumberOfDirectorySectors:X})");
            builder.AppendLine($"  Number of FAT sectors: {header.NumberOfFATSectors} (0x{header.NumberOfFATSectors:X})");
            builder.AppendLine($"  First directory sector location: {header.FirstDirectorySectorLocation} (0x{header.FirstDirectorySectorLocation:X})");
            builder.AppendLine($"  Transaction signature number: {header.TransactionSignatureNumber} (0x{header.TransactionSignatureNumber:X})");
            builder.AppendLine($"  Mini stream cutoff size: {header.MiniStreamCutoffSize} (0x{header.MiniStreamCutoffSize:X})");
            builder.AppendLine($"  First mini FAT sector location: {header.FirstMiniFATSectorLocation} (0x{header.FirstMiniFATSectorLocation:X})");
            builder.AppendLine($"  Number of mini FAT sectors: {header.NumberOfMiniFATSectors} (0x{header.NumberOfMiniFATSectors:X})");
            builder.AppendLine($"  First DIFAT sector location: {header.FirstDIFATSectorLocation} (0x{header.FirstDIFATSectorLocation:X})");
            builder.AppendLine($"  Number of DIFAT sectors: {header.NumberOfDIFATSectors} (0x{header.NumberOfDIFATSectors:X})");
            builder.AppendLine($"  DIFAT:");
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

                builder.AppendLine($"    Name: {directoryEntry.Name}");
                builder.AppendLine($"    Name length: {directoryEntry.NameLength} (0x{directoryEntry.NameLength:X})");
                builder.AppendLine($"    Object type: {directoryEntry.ObjectType} (0x{directoryEntry.ObjectType:X})");
                builder.AppendLine($"    Color flag: {directoryEntry.ColorFlag} (0x{directoryEntry.ColorFlag:X})");
                builder.AppendLine($"    Left sibling ID: {directoryEntry.LeftSiblingID} (0x{directoryEntry.LeftSiblingID:X})");
                builder.AppendLine($"    Right sibling ID: {directoryEntry.RightSiblingID} (0x{directoryEntry.RightSiblingID:X})");
                builder.AppendLine($"    Child ID: {directoryEntry.ChildID} (0x{directoryEntry.ChildID:X})");
                builder.AppendLine($"    CLSID: {directoryEntry.CLSID}");
                builder.AppendLine($"    State bits: {directoryEntry.StateBits} (0x{directoryEntry.StateBits:X})");
                builder.AppendLine($"    Creation time: {directoryEntry.CreationTime} (0x{directoryEntry.CreationTime:X})");
                builder.AppendLine($"    Modification time: {directoryEntry.ModifiedTime} (0x{directoryEntry.ModifiedTime:X})");
                builder.AppendLine($"    Staring sector location: {directoryEntry.StartingSectorLocation} (0x{directoryEntry.StartingSectorLocation:X})");
                builder.AppendLine($"    Stream size: {directoryEntry.StreamSize} (0x{directoryEntry.StreamSize:X})");
            }
            builder.AppendLine();
        }

    }
}