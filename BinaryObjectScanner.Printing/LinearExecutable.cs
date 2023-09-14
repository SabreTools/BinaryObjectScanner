using System.Text;
using SabreTools.Models.LinearExecutable;

namespace BinaryObjectScanner.Printing
{
    public static class LinearExecutable
    {
        public static void Print(StringBuilder builder, Executable executable)
        {
            builder.AppendLine("New Executable Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Stub
            Print(builder, executable.Stub?.Header);

            // Information Block
            Print(builder, executable.InformationBlock);

            // Tables
            Print(builder, executable.ObjectTable);
            Print(builder, executable.ObjectPageMap);
            Print(builder, executable.ResourceTable);
            Print(builder, executable.ResidentNamesTable);
            Print(builder, executable.EntryTable);
            Print(builder, executable.ModuleFormatDirectivesTable);
            Print(builder, executable.VerifyRecordDirectiveTable);
            Print(builder, executable.FixupPageTable);
            Print(builder, executable.FixupRecordTable);
            Print(builder, executable.ImportModuleNameTable);
            Print(builder, executable.ImportModuleProcedureNameTable);
            Print(builder, executable.PerPageChecksumTable);
            Print(builder, executable.NonResidentNamesTable);

            // Debug
            Print(builder, executable.DebugInformation);
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

            builder.AppendLine("  MS-DOS Stub Extended Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Reserved words: {(header.Reserved1 == null ? "[NULL]" : string.Join(", ", header.Reserved1))}");
            builder.AppendLine($"  OEM identifier: {header.OEMIdentifier} (0x{header.OEMIdentifier:X})");
            builder.AppendLine($"  OEM information: {header.OEMInformation} (0x{header.OEMInformation:X})");
            builder.AppendLine($"  Reserved words: {(header.Reserved2 == null ? "[NULL]" : string.Join(", ", header.Reserved2))}");
            builder.AppendLine($"  New EXE header address: {header.NewExeHeaderAddr} (0x{header.NewExeHeaderAddr:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, InformationBlock block)
#else
        private static void Print(StringBuilder builder, InformationBlock? block)
#endif
        {
            builder.AppendLine("  Information Block Information:");
            builder.AppendLine("  -------------------------");
            if (block == null)
            {
                builder.AppendLine("  No information block");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Signature: {block.Signature}");
            builder.AppendLine($"  Byte order: {block.ByteOrder} (0x{block.ByteOrder:X})");
            builder.AppendLine($"  Word order: {block.WordOrder} (0x{block.WordOrder:X})");
            builder.AppendLine($"  Executable format level: {block.ExecutableFormatLevel} (0x{block.ExecutableFormatLevel:X})");
            builder.AppendLine($"  CPU type: {block.CPUType} (0x{block.CPUType:X})");
            builder.AppendLine($"  Module OS: {block.ModuleOS} (0x{block.ModuleOS:X})");
            builder.AppendLine($"  Module version: {block.ModuleVersion} (0x{block.ModuleVersion:X})");
            builder.AppendLine($"  Module type flags: {block.ModuleTypeFlags} (0x{block.ModuleTypeFlags:X})");
            builder.AppendLine($"  Module number pages: {block.ModuleNumberPages} (0x{block.ModuleNumberPages:X})");
            builder.AppendLine($"  Initial object CS: {block.InitialObjectCS} (0x{block.InitialObjectCS:X})");
            builder.AppendLine($"  Initial EIP: {block.InitialEIP} (0x{block.InitialEIP:X})");
            builder.AppendLine($"  Initial object SS: {block.InitialObjectSS} (0x{block.InitialObjectSS:X})");
            builder.AppendLine($"  Initial ESP: {block.InitialESP} (0x{block.InitialESP:X})");
            builder.AppendLine($"  Memory page size: {block.MemoryPageSize} (0x{block.MemoryPageSize:X})");
            builder.AppendLine($"  Bytes on last page: {block.BytesOnLastPage} (0x{block.BytesOnLastPage:X})");
            builder.AppendLine($"  Fix-up section size: {block.FixupSectionSize} (0x{block.FixupSectionSize:X})");
            builder.AppendLine($"  Fix-up section checksum: {block.FixupSectionChecksum} (0x{block.FixupSectionChecksum:X})");
            builder.AppendLine($"  Loader section size: {block.LoaderSectionSize} (0x{block.LoaderSectionSize:X})");
            builder.AppendLine($"  Loader section checksum: {block.LoaderSectionChecksum} (0x{block.LoaderSectionChecksum:X})");
            builder.AppendLine($"  Object table offset: {block.ObjectTableOffset} (0x{block.ObjectTableOffset:X})");
            builder.AppendLine($"  Object table count: {block.ObjectTableCount} (0x{block.ObjectTableCount:X})");
            builder.AppendLine($"  Object page map offset: {block.ObjectPageMapOffset} (0x{block.ObjectPageMapOffset:X})");
            builder.AppendLine($"  Object iterate data map offset: {block.ObjectIterateDataMapOffset} (0x{block.ObjectIterateDataMapOffset:X})");
            builder.AppendLine($"  Resource table offset: {block.ResourceTableOffset} (0x{block.ResourceTableOffset:X})");
            builder.AppendLine($"  Resource table count: {block.ResourceTableCount} (0x{block.ResourceTableCount:X})");
            builder.AppendLine($"  Resident names table offset: {block.ResidentNamesTableOffset} (0x{block.ResidentNamesTableOffset:X})");
            builder.AppendLine($"  Entry table offset: {block.EntryTableOffset} (0x{block.EntryTableOffset:X})");
            builder.AppendLine($"  Module directives table offset: {block.ModuleDirectivesTableOffset} (0x{block.ModuleDirectivesTableOffset:X})");
            builder.AppendLine($"  Module directives table count: {block.ModuleDirectivesCount} (0x{block.ModuleDirectivesCount:X})");
            builder.AppendLine($"  Fix-up page table offset: {block.FixupPageTableOffset} (0x{block.FixupPageTableOffset:X})");
            builder.AppendLine($"  Fix-up record table offset: {block.FixupRecordTableOffset} (0x{block.FixupRecordTableOffset:X})");
            builder.AppendLine($"  Imported modules name table offset: {block.ImportedModulesNameTableOffset} (0x{block.ImportedModulesNameTableOffset:X})");
            builder.AppendLine($"  Imported modules count: {block.ImportedModulesCount} (0x{block.ImportedModulesCount:X})");
            builder.AppendLine($"  Imported procedure name table count: {block.ImportProcedureNameTableOffset} (0x{block.ImportProcedureNameTableOffset:X})");
            builder.AppendLine($"  Per-page checksum table offset: {block.PerPageChecksumTableOffset} (0x{block.PerPageChecksumTableOffset:X})");
            builder.AppendLine($"  Data pages offset: {block.DataPagesOffset} (0x{block.DataPagesOffset:X})");
            builder.AppendLine($"  Preload page count: {block.PreloadPageCount} (0x{block.PreloadPageCount:X})");
            builder.AppendLine($"  Non-resident names table offset: {block.NonResidentNamesTableOffset} (0x{block.NonResidentNamesTableOffset:X})");
            builder.AppendLine($"  Non-resident names table length: {block.NonResidentNamesTableLength} (0x{block.NonResidentNamesTableLength:X})");
            builder.AppendLine($"  Non-resident names table checksum: {block.NonResidentNamesTableChecksum} (0x{block.NonResidentNamesTableChecksum:X})");
            builder.AppendLine($"  Automatic data object: {block.AutomaticDataObject} (0x{block.AutomaticDataObject:X})");
            builder.AppendLine($"  Debug information offset: {block.DebugInformationOffset} (0x{block.DebugInformationOffset:X})");
            builder.AppendLine($"  Debug information length: {block.DebugInformationLength} (0x{block.DebugInformationLength:X})");
            builder.AppendLine($"  Preload instance pages number: {block.PreloadInstancePagesNumber} (0x{block.PreloadInstancePagesNumber:X})");
            builder.AppendLine($"  Demand instance pages number: {block.DemandInstancePagesNumber} (0x{block.DemandInstancePagesNumber:X})");
            builder.AppendLine($"  Extra heap allocation: {block.ExtraHeapAllocation} (0x{block.ExtraHeapAllocation:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ObjectTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, ObjectTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Object Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No object table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Object Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Virtual segment size: {entry.VirtualSegmentSize} (0x{entry.VirtualSegmentSize:X})");
                builder.AppendLine($"    Relocation base address: {entry.RelocationBaseAddress} (0x{entry.RelocationBaseAddress:X})");
                builder.AppendLine($"    Object flags: {entry.ObjectFlags} (0x{entry.ObjectFlags:X})");
                builder.AppendLine($"    Page table index: {entry.PageTableIndex} (0x{entry.PageTableIndex:X})");
                builder.AppendLine($"    Page table entries: {entry.PageTableEntries} (0x{entry.PageTableEntries:X})");
                builder.AppendLine($"    Reserved: {entry.Reserved} (0x{entry.Reserved:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ObjectPageMapEntry[] entries)
#else
        private static void Print(StringBuilder builder, ObjectPageMapEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Object Page Map Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No object page map entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Object Page Map Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Page data offset: {entry.PageDataOffset} (0x{entry.PageDataOffset:X})");
                builder.AppendLine($"    Data size: {entry.DataSize} (0x{entry.DataSize:X})");
                builder.AppendLine($"    Flags: {entry.Flags} (0x{entry.Flags:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ResourceTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, ResourceTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Resource Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No resource table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Resource Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Type ID: {entry.TypeID} (0x{entry.TypeID:X})");
                builder.AppendLine($"    Name ID: {entry.NameID} (0x{entry.NameID:X})");
                builder.AppendLine($"    Resource size: {entry.ResourceSize} (0x{entry.ResourceSize:X})");
                builder.AppendLine($"    Object number: {entry.ObjectNumber} (0x{entry.ObjectNumber:X})");
                builder.AppendLine($"    Offset: {entry.Offset} (0x{entry.Offset:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ResidentNamesTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, ResidentNamesTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Resident Names Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No resident names table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Resident Names Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                builder.AppendLine($"    Name: {entry.Name ?? "[NULL]"}");
                builder.AppendLine($"    Ordinal number: {entry.OrdinalNumber} (0x{entry.OrdinalNumber:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, EntryTableBundle[] bundles)
#else
        private static void Print(StringBuilder builder, EntryTableBundle?[]? bundles)
#endif
        {
            builder.AppendLine("  Entry Table Information:");
            builder.AppendLine("  -------------------------");
            if (bundles == null || bundles.Length == 0)
            {
                builder.AppendLine("  No entry table bundles");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < bundles.Length; i++)
            {
                var bundle = bundles[i];
                builder.AppendLine($"  Entry Table Bundle {i}");
                if (bundle == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Entries: {bundle.Entries} (0x{bundle.Entries:X})");
                builder.AppendLine($"    Bundle type: {bundle.BundleType} (0x{bundle.BundleType:X})");
                builder.AppendLine();

                builder.AppendLine($"    Entry Table Entries:");
                builder.AppendLine("    -------------------------");
                if (bundle.TableEntries == null || bundle.TableEntries.Length == 0)
                {
                    builder.AppendLine("    No entry table entries");
                    builder.AppendLine();
                    continue;
                }

                for (int j = 0; j < bundle.TableEntries.Length; j++)
                {
                    var entry = bundle.TableEntries[j];
                    builder.AppendLine($"    Entry Table Entry {j}");
                    if (entry == null)
                    {
                        builder.AppendLine("    [NULL]");
                        continue;
                    }

                    switch (bundle.BundleType & ~BundleType.ParameterTypingInformationPresent)
                    {
                        case BundleType.UnusedEntry:
                            builder.AppendLine($"      Unused, empty entry");
                            break;

                        case BundleType.SixteenBitEntry:
                            builder.AppendLine($"      Object number: {entry.SixteenBitObjectNumber} (0x{entry.SixteenBitObjectNumber:X})");
                            builder.AppendLine($"      Entry flags: {entry.SixteenBitEntryFlags} (0x{entry.SixteenBitEntryFlags:X})");
                            builder.AppendLine($"      Offset: {entry.SixteenBitOffset} (0x{entry.SixteenBitOffset:X})");
                            break;

                        case BundleType.TwoEightySixCallGateEntry:
                            builder.AppendLine($"      Object number: {entry.TwoEightySixObjectNumber} (0x{entry.TwoEightySixObjectNumber:X})");
                            builder.AppendLine($"      Entry flags: {entry.TwoEightySixEntryFlags} (0x{entry.TwoEightySixEntryFlags:X})");
                            builder.AppendLine($"      Offset: {entry.TwoEightySixOffset} (0x{entry.TwoEightySixOffset:X})");
                            builder.AppendLine($"      Callgate: {entry.TwoEightySixCallgate} (0x{entry.TwoEightySixCallgate:X})");
                            break;

                        case BundleType.ThirtyTwoBitEntry:
                            builder.AppendLine($"      Object number: {entry.ThirtyTwoBitObjectNumber} (0x{entry.ThirtyTwoBitObjectNumber:X})");
                            builder.AppendLine($"      Entry flags: {entry.ThirtyTwoBitEntryFlags} (0x{entry.ThirtyTwoBitEntryFlags:X})");
                            builder.AppendLine($"      Offset: {entry.ThirtyTwoBitOffset} (0x{entry.ThirtyTwoBitOffset:X})");
                            break;

                        case BundleType.ForwarderEntry:
                            builder.AppendLine($"      Reserved: {entry.ForwarderReserved} (0x{entry.ForwarderReserved:X})");
                            builder.AppendLine($"      Forwarder flags: {entry.ForwarderFlags} (0x{entry.ForwarderFlags:X})");
                            builder.AppendLine($"      Module ordinal number: {entry.ForwarderModuleOrdinalNumber} (0x{entry.ForwarderModuleOrdinalNumber:X})");
                            builder.AppendLine($"      Procedure name offset: {entry.ProcedureNameOffset} (0x{entry.ProcedureNameOffset:X})");
                            builder.AppendLine($"      Import ordinal number: {entry.ImportOrdinalNumber} (0x{entry.ImportOrdinalNumber:X})");
                            break;

                        default:
                            builder.AppendLine($"      Unknown entry type {bundle.BundleType}");
                            break;
                    }
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ModuleFormatDirectivesTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, ModuleFormatDirectivesTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Module Format Directives Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No module format directives table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Moduile Format Directives Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Directive number: {entry.DirectiveNumber} (0x{entry.DirectiveNumber:X})");
                builder.AppendLine($"    Directive data length: {entry.DirectiveDataLength} (0x{entry.DirectiveDataLength:X})");
                builder.AppendLine($"    Directive data offset: {entry.DirectiveDataOffset} (0x{entry.DirectiveDataOffset:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, VerifyRecordDirectiveTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, VerifyRecordDirectiveTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Verify Record Directive Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No verify record directive table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Verify Record Directive Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Entry count: {entry.EntryCount} (0x{entry.EntryCount:X})");
                builder.AppendLine($"    Ordinal index: {entry.OrdinalIndex} (0x{entry.OrdinalIndex:X})");
                builder.AppendLine($"    Version: {entry.Version} (0x{entry.Version:X})");
                builder.AppendLine($"    Object entries count: {entry.ObjectEntriesCount} (0x{entry.ObjectEntriesCount:X})");
                builder.AppendLine($"    Object number in module: {entry.ObjectNumberInModule} (0x{entry.ObjectNumberInModule:X})");
                builder.AppendLine($"    Object load base address: {entry.ObjectLoadBaseAddress} (0x{entry.ObjectLoadBaseAddress:X})");
                builder.AppendLine($"    Object virtual address size: {entry.ObjectVirtualAddressSize} (0x{entry.ObjectVirtualAddressSize:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, FixupPageTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, FixupPageTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Fix-up Page Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No fix-up page table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Fix-up Page Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Offset: {entry.Offset} (0x{entry.Offset:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, FixupRecordTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, FixupRecordTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Fix-up Record Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No fix-up record table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Fix-up Record Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Source type: {entry.SourceType} (0x{entry.SourceType:X})");
                builder.AppendLine($"    Target flags: {entry.TargetFlags} (0x{entry.TargetFlags:X})");

                // Source list flag
                if (entry.SourceType.HasFlag(FixupRecordSourceType.SourceListFlag))
                    builder.AppendLine($"    Source offset list count: {entry.SourceOffsetListCount} (0x{entry.SourceOffsetListCount:X})");
                else
                    builder.AppendLine($"    Source offset: {entry.SourceOffset} (0x{entry.SourceOffset:X})");

                // OBJECT / TRGOFF
                if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.InternalReference))
                {
                    // 16-bit Object Number/Module Ordinal Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                        builder.AppendLine($"    Target object number: {entry.TargetObjectNumberWORD} (0x{entry.TargetObjectNumberWORD:X})");
                    else
                        builder.AppendLine($"    Target object number: {entry.TargetObjectNumberByte} (0x{entry.TargetObjectNumberByte:X})");

                    // 16-bit Selector fixup
                    if (!entry.SourceType.HasFlag(FixupRecordSourceType.SixteenBitSelectorFixup))
                    {
                        // 32-bit Target Offset Flag
                        if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                            builder.AppendLine($"    Target offset: {entry.TargetOffsetDWORD} (0x{entry.TargetOffsetDWORD:X})");
                        else
                            builder.AppendLine($"    Target offset: {entry.TargetOffsetWORD} (0x{entry.TargetOffsetWORD:X})");
                    }
                }

                // MOD ORD# / IMPORT ORD / ADDITIVE
                else if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ImportedReferenceByOrdinal))
                {
                    // 16-bit Object Number/Module Ordinal Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                        builder.AppendLine($"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableWORD} (0x{entry.OrdinalIndexImportModuleNameTableWORD:X})");
                    else
                        builder.AppendLine($"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableByte} (0x{entry.OrdinalIndexImportModuleNameTableByte:X})");

                    // 8-bit Ordinal Flag & 32-bit Target Offset Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.EightBitOrdinalFlag))
                        builder.AppendLine($"    Imported ordinal number: {entry.ImportedOrdinalNumberByte} (0x{entry.ImportedOrdinalNumberByte:X})");
                    else if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                        builder.AppendLine($"    Imported ordinal number: {entry.ImportedOrdinalNumberDWORD} (0x{entry.ImportedOrdinalNumberDWORD:X})");
                    else
                        builder.AppendLine($"    Imported ordinal number: {entry.ImportedOrdinalNumberWORD} (0x{entry.ImportedOrdinalNumberWORD:X})");

                    // Additive Fixup Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
                    {
                        // 32-bit Additive Flag
                        if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                            builder.AppendLine($"    Additive fixup value: {entry.AdditiveFixupValueDWORD} (0x{entry.AdditiveFixupValueDWORD:X})");
                        else
                            builder.AppendLine($"    Additive fixup value: {entry.AdditiveFixupValueWORD} (0x{entry.AdditiveFixupValueWORD:X})");
                    }
                }

                // MOD ORD# / PROCEDURE NAME OFFSET / ADDITIVE
                else if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ImportedReferenceByName))
                {
                    // 16-bit Object Number/Module Ordinal Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                        builder.AppendLine($"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableWORD} (0x{entry.OrdinalIndexImportModuleNameTableWORD:X})");
                    else
                        builder.AppendLine($"    Ordinal index import module name table: {entry.OrdinalIndexImportModuleNameTableByte} (0x{entry.OrdinalIndexImportModuleNameTableByte:X})");

                    // 32-bit Target Offset Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitTargetOffsetFlag))
                        builder.AppendLine($"    Offset import procedure name table: {entry.OffsetImportProcedureNameTableDWORD} (0x{entry.OffsetImportProcedureNameTableDWORD:X})");
                    else
                        builder.AppendLine($"    Offset import procedure name table: {entry.OffsetImportProcedureNameTableWORD} (0x{entry.OffsetImportProcedureNameTableWORD:X})");

                    // Additive Fixup Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
                    {
                        // 32-bit Additive Flag
                        if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                            builder.AppendLine($"    Additive fixup value: {entry.AdditiveFixupValueDWORD} (0x{entry.AdditiveFixupValueDWORD:X})");
                        else
                            builder.AppendLine($"    Additive fixup value: {entry.AdditiveFixupValueWORD} (0x{entry.AdditiveFixupValueWORD:X})");
                    }
                }

                // ORD # / ADDITIVE
                else if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.InternalReferenceViaEntryTable))
                {
                    // 16-bit Object Number/Module Ordinal Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.SixteenBitObjectNumberModuleOrdinalFlag))
                        builder.AppendLine($"    Target object number: {entry.TargetObjectNumberWORD} (0x{entry.TargetObjectNumberWORD:X})");
                    else
                        builder.AppendLine($"    Target object number: {entry.TargetObjectNumberByte} (0x{entry.TargetObjectNumberByte:X})");

                    // Additive Fixup Flag
                    if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.AdditiveFixupFlag))
                    {
                        // 32-bit Additive Flag
                        if (entry.TargetFlags.HasFlag(FixupRecordTargetFlags.ThirtyTwoBitAdditiveFixupFlag))
                            builder.AppendLine($"    Additive fixup value: {entry.AdditiveFixupValueDWORD} (0x{entry.AdditiveFixupValueDWORD:X})");
                        else
                            builder.AppendLine($"    Additive fixup value: {entry.AdditiveFixupValueWORD} (0x{entry.AdditiveFixupValueWORD:X})");
                    }
                }

                // No other top-level flags recognized
                else
                {
                    builder.AppendLine($"    Unknown entry format");
                }

                builder.AppendLine();
                builder.AppendLine($"    Source Offset List:");
                builder.AppendLine("    -------------------------");
                if (entry.SourceOffsetList == null || entry.SourceOffsetList.Length == 0)
                {
                    builder.AppendLine($"    No source offset list entries");
                }
                else
                {
                    for (int j = 0; j < entry.SourceOffsetList.Length; j++)
                    {
                        builder.AppendLine($"    Source Offset List Entry {j}: {entry.SourceOffsetList[j]} (0x{entry.SourceOffsetList[j]:X})");
                    }
                }
                builder.AppendLine();
            }
        }

#if NET48
        private static void Print(StringBuilder builder, ImportModuleNameTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, ImportModuleNameTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Import Module Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No import module name table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Import Module Name Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                builder.AppendLine($"    Name: {entry.Name ?? "[NULL]"}");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ImportModuleProcedureNameTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, ImportModuleProcedureNameTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Import Module Procedure Name Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No import module procedure name table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Import Module Procedure Name Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                builder.AppendLine($"    Name: {entry.Name ?? "[NULL]"}");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, PerPageChecksumTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, PerPageChecksumTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Per-Page Checksum Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No per-page checksum table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($" Per-Page Checksum Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Checksum: {entry.Checksum} (0x{entry.Checksum:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, NonResidentNamesTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, NonResidentNamesTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Non-Resident Names Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No non-resident names table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Non-Resident Names Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine($"    Length: {entry.Length} (0x{entry.Length:X})");
                builder.AppendLine($"    Name: {entry.Name ?? "[NULL]"}");
                builder.AppendLine($"    Ordinal number: {entry.OrdinalNumber} (0x{entry.OrdinalNumber:X})");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DebugInformation di)
#else
        private static void Print(StringBuilder builder, DebugInformation? di)
#endif
        {
            builder.AppendLine("  Debug Information:");
            builder.AppendLine("  -------------------------");
            if (di == null)
            {
                builder.AppendLine("  No debug information");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Signature: {di.Signature ?? "[NULL]"}");
            builder.AppendLine($"  Format type: {di.FormatType} (0x{di.FormatType:X})");
            // Debugger data
            builder.AppendLine();
        }
    }
}