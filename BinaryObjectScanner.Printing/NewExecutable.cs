using System.Collections.Generic;
using System.Text;
using SabreTools.Models.NewExecutable;
using static SabreTools.Serialization.Extensions;

namespace BinaryObjectScanner.Printing
{
    public static class NewExecutable
    {
        public static void Print(StringBuilder builder, Executable executable)
        {
            builder.AppendLine("New Executable Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Stub
            Print(builder, executable.Stub?.Header);

            // Header
            Print(builder, executable.Header);

            // Tables
            Print(builder, executable.SegmentTable);
            Print(builder, executable.ResourceTable);
            Print(builder, executable.ResidentNameTable);
            Print(builder, executable.ModuleReferenceTable, executable.Stub?.Header, executable.Header);
            Print(builder, executable.ImportedNameTable);
            Print(builder, executable.EntryTable);
            Print(builder, executable.NonResidentNameTable);
        }

#if NET48
        private static void Print(StringBuilder builder, SabreTools.Models.MSDOS.ExecutableHeader header)
#else
        private static void Print(StringBuilder builder, SabreTools.Models.MSDOS.ExecutableHeader? header)
#endif
        {
            builder.AppendLine("  MS-DOS Stub Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No MS-DOS stub header");
                builder.AppendLine();
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

            builder.AppendLine("  MS-DOS Stub Extended Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.Reserved1, "  Reserved words");
            builder.AppendLine(header.OEMIdentifier, "  OEM identifier");
            builder.AppendLine(header.OEMInformation, "  OEM information");
            builder.AppendLine(header.Reserved2, "  Reserved words");
            builder.AppendLine(header.NewExeHeaderAddr, "  New EXE header address");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ExecutableHeader header)
#else
        private static void Print(StringBuilder builder, ExecutableHeader? header)
#endif
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.Magic, "  Magic number");
            builder.AppendLine(header.LinkerVersion, "  Linker version");
            builder.AppendLine(header.LinkerRevision, "  Linker revision");
            builder.AppendLine(header.EntryTableOffset, "  Entry table offset");
            builder.AppendLine(header.EntryTableSize, "  Entry table size");
            builder.AppendLine(header.CrcChecksum, "  CRC checksum");
            builder.AppendLine($"  Flag word: {header.FlagWord} (0x{header.FlagWord:X})");
            builder.AppendLine(header.AutomaticDataSegmentNumber, "  Automatic data segment number");
            builder.AppendLine(header.InitialHeapAlloc, "  Initial heap allocation");
            builder.AppendLine(header.InitialStackAlloc, "  Initial stack allocation");
            builder.AppendLine(header.InitialCSIPSetting, "  Initial CS:IP setting");
            builder.AppendLine(header.InitialSSSPSetting, "  Initial SS:SP setting");
            builder.AppendLine(header.FileSegmentCount, "  File segment count");
            builder.AppendLine(header.ModuleReferenceTableSize, "  Module reference table size");
            builder.AppendLine(header.NonResidentNameTableSize, "  Non-resident name table size");
            builder.AppendLine(header.SegmentTableOffset, "  Segment table offset");
            builder.AppendLine(header.ResourceTableOffset, "  Resource table offset");
            builder.AppendLine(header.ResidentNameTableOffset, "  Resident name table offset");
            builder.AppendLine(header.ModuleReferenceTableOffset, "  Module reference table offset");
            builder.AppendLine(header.ImportedNamesTableOffset, "  Imported names table offset");
            builder.AppendLine(header.NonResidentNamesTableOffset, "  Non-resident name table offset");
            builder.AppendLine(header.MovableEntriesCount, "  Moveable entries count");
            builder.AppendLine(header.SegmentAlignmentShiftCount, "  Segment alignment shift count");
            builder.AppendLine(header.ResourceEntriesCount, "  Resource entries count");
            builder.AppendLine($"  Target operating system: {header.TargetOperatingSystem} (0x{header.TargetOperatingSystem:X})");
            builder.AppendLine($"  Additional flags: {header.AdditionalFlags} (0x{header.AdditionalFlags:X})");
            builder.AppendLine(header.ReturnThunkOffset, "  Return thunk offset");
            builder.AppendLine(header.SegmentReferenceThunkOffset, "  Segment reference thunk offset");
            builder.AppendLine(header.MinCodeSwapAreaSize, "  Minimum code swap area size");
            builder.AppendLine(header.WindowsSDKRevision, "  Windows SDK revision");
            builder.AppendLine(header.WindowsSDKVersion, "  Windows SDK version");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, SegmentTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, SegmentTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Segment Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No segment table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Segment Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.Offset, "    Offset");
                builder.AppendLine(entry.Length, "    Length");
                builder.AppendLine($"    Flag word: {entry.FlagWord} (0x{entry.FlagWord:X})");
                builder.AppendLine(entry.MinimumAllocationSize, "    Minimum allocation size");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ResourceTable table)
#else
        private static void Print(StringBuilder builder, ResourceTable? table)
#endif
        {
            builder.AppendLine("  Resource Table Information:");
            builder.AppendLine("  -------------------------");
            if (table == null)
            {
                builder.AppendLine("  No resource table");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(table.AlignmentShiftCount, "  Alignment shift count");
            if (table.ResourceTypes == null || table.ResourceTypes.Length == 0)
            {
                builder.AppendLine("  No resource table items");
            }
            else
            {
                for (int i = 0; i < table.ResourceTypes.Length; i++)
                {
                    // TODO: If not integer type, print out name
                    var entry = table.ResourceTypes[i];
                    builder.AppendLine($"  Resource Table Entry {i}");
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    builder.AppendLine(entry.TypeID, "    Type ID");
                    builder.AppendLine(entry.ResourceCount, "    Resource count");
                    builder.AppendLine(entry.Reserved, "    Reserved");
                    builder.AppendLine("    Resources = ");
                    if (entry.ResourceCount == 0 || entry.Resources == null || entry.Resources.Length == 0)
                    {
                        builder.AppendLine("      No resource items");
                    }
                    else
                    {
                        for (int j = 0; j < entry.Resources.Length; j++)
                        {
                            // TODO: If not integer type, print out name
                            var resource = entry.Resources[j];
                            builder.AppendLine($"      Resource Entry {i}");
                            if (resource == null)
                            {
                                builder.AppendLine("      [NULL]");
                                continue;
                            }

                            builder.AppendLine(resource.Offset, "        Offset");
                            builder.AppendLine(resource.Length, "        Length");
                            builder.AppendLine($"        Flag word: {resource.FlagWord} (0x{resource.FlagWord:X})");
                            builder.AppendLine(resource.ResourceID, "        Resource ID");
                            builder.AppendLine(resource.Reserved, "        Reserved");
                        }
                    }
                }
            }

            if (table.TypeAndNameStrings == null || table.TypeAndNameStrings.Count == 0)
            {
                builder.AppendLine("  No resource table type/name strings");
            }
            else
            {
                foreach (var typeAndNameString in table.TypeAndNameStrings)
                {
                    builder.AppendLine($"  Resource Type/Name Offset {typeAndNameString.Key}");
#if NET6_0_OR_GREATER
                    if (typeAndNameString.Value == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }
#endif
                    builder.AppendLine(typeAndNameString.Value.Length, "    Length");
                    builder.AppendLine($"    Text: {(typeAndNameString.Value.Text != null ? Encoding.ASCII.GetString(typeAndNameString.Value.Text).TrimEnd('\0') : "[EMPTY]")}");
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ResidentNameTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, ResidentNameTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Resident-Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No resident-name table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Resident-Name Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.Length, "    Length");
                builder.AppendLine($"    Name string: {(entry.NameString != null ? Encoding.ASCII.GetString(entry.NameString).TrimEnd('\0') : "[EMPTY]")}");
                builder.AppendLine(entry.OrdinalNumber, "    Ordinal number");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ModuleReferenceTableEntry[] entries, SabreTools.Models.MSDOS.ExecutableHeader stub, ExecutableHeader header)
#else
        private static void Print(StringBuilder builder, ModuleReferenceTableEntry?[]? entries, SabreTools.Models.MSDOS.ExecutableHeader? stub, ExecutableHeader? header)
#endif
        {
            builder.AppendLine("  Module-Reference Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No module-reference table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                // TODO: Read the imported names table and print value here
                var entry = entries[i];
                builder.AppendLine($"  Module-Reference Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

#if NET48
                builder.AppendLine($"    Offset: {entry.Offset} (adjusted to be {entry.Offset + stub.NewExeHeaderAddr + header.ImportedNamesTableOffset})");
#else
                builder.AppendLine($"    Offset: {entry.Offset} (adjusted to be {entry.Offset + (stub?.NewExeHeaderAddr ?? 0) + (header?.ImportedNamesTableOffset ?? 0)})");
#endif
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, Dictionary<ushort, ImportedNameTableEntry> entries)
#else
        private static void Print(StringBuilder builder, Dictionary<ushort, ImportedNameTableEntry?>? entries)
#endif
        {
            builder.AppendLine("  Imported-Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Count == 0)
            {
                builder.AppendLine("  No imported-name table items");
                builder.AppendLine();
                return;
            }

            foreach (var entry in entries)
            {
                builder.AppendLine($"  Imported-Name Table at Offset {entry.Key}");
#if NET6_0_OR_GREATER
                    if (entry.Value == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }
#endif
                builder.AppendLine(entry.Value.Length, "    Length");
                builder.AppendLine($"    Name string: {(entry.Value.NameString != null ? Encoding.ASCII.GetString(entry.Value.NameString) : "[EMPTY]")}");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, EntryTableBundle[] entries)
#else
        private static void Print(StringBuilder builder, EntryTableBundle?[]? entries)
#endif
        {
            builder.AppendLine("  Entry Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No entry table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Entry Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.EntryCount, "    Entry count");
                builder.AppendLine(entry.SegmentIndicator, "    Segment indicator");
                switch (entry.GetEntryType())
                {
                    case SegmentEntryType.FixedSegment:
                        builder.AppendLine($"    Flag word: {entry.FixedFlagWord} (0x{entry.FixedFlagWord:X})");
                        builder.AppendLine(entry.FixedOffset, "    Offset");
                        break;
                    case SegmentEntryType.MoveableSegment:
                        builder.AppendLine($"    Flag word: {entry.MoveableFlagWord} (0x{entry.MoveableFlagWord:X})");
                        builder.AppendLine(entry.MoveableReserved, "    Reserved");
                        builder.AppendLine(entry.MoveableSegmentNumber, "    Segment number");
                        builder.AppendLine(entry.MoveableOffset, "    Offset");
                        break;
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, NonResidentNameTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, NonResidentNameTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Nonresident-Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No nonresident-name table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Nonresident-Name Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.Length, "    Length");
                builder.AppendLine($"    Name string: {(entry.NameString != null ? Encoding.ASCII.GetString(entry.NameString) : "[EMPTY]")}");
                builder.AppendLine(entry.OrdinalNumber, "    Ordinal number");
            }
            builder.AppendLine();
        }
    }
}