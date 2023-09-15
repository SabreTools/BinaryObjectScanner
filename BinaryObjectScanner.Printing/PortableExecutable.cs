using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SabreTools.ASN1;
using SabreTools.IO;
using SabreTools.Models.PortableExecutable;
using static SabreTools.Serialization.Extensions;

namespace BinaryObjectScanner.Printing
{
    public static class PortableExecutable
    {
        public static void Print(StringBuilder builder, Executable executable)
        {
            builder.AppendLine("Portable Executable Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            // Stub
            Print(builder, executable.Stub?.Header);

            // Header
            Print(builder, executable.Signature, executable.COFFFileHeader);
            Print(builder, executable.OptionalHeader, executable.SectionTable);

            // Tables
            Print(builder, executable.SectionTable);
            Print(builder, executable.COFFSymbolTable);
            Print(builder, executable.COFFStringTable);
            Print(builder, executable.AttributeCertificateTable);
            Print(builder, executable.DelayLoadDirectoryTable);

            // Named Sections
            Print(builder, executable.BaseRelocationTable, executable.SectionTable);
            Print(builder, executable.DebugTable);
            Print(builder, executable.ExportTable);
            Print(builder, executable.ImportTable, executable.SectionTable);
            Print(builder, executable.ResourceDirectoryTable);
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
        private static void Print(StringBuilder builder, string signature, COFFFileHeader header)
#else
        private static void Print(StringBuilder builder, string? signature, COFFFileHeader? header)
#endif
        {
            builder.AppendLine("  COFF File Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No COFF file header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(signature, "  Signature");
            builder.AppendLine($"  Machine: {header.Machine} (0x{header.Machine:X})");
            builder.AppendLine(header.NumberOfSections, "  Number of sections");
            builder.AppendLine(header.TimeDateStamp, "  Time/Date stamp");
            builder.AppendLine(header.PointerToSymbolTable, "  Pointer to symbol table");
            builder.AppendLine(header.NumberOfSymbols, "  Number of symbols");
            builder.AppendLine(header.SizeOfOptionalHeader, "  Size of optional header");
            builder.AppendLine($"  Characteristics: {header.Characteristics} (0x{header.Characteristics:X})");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, OptionalHeader header, SectionHeader[] table)
#else
        private static void Print(StringBuilder builder, OptionalHeader? header, SectionHeader?[]? table)
#endif
        {
            builder.AppendLine("  Optional Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No optional header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine($"  Magic: {header.Magic} (0x{header.Magic:X})");
            builder.AppendLine(header.MajorLinkerVersion, "  Major linker version");
            builder.AppendLine(header.MinorLinkerVersion, "  Minor linker version");
            builder.AppendLine(header.SizeOfCode, "  Size of code section");
            builder.AppendLine(header.SizeOfInitializedData, "  Size of initialized data");
            builder.AppendLine(header.SizeOfUninitializedData, "  Size of uninitialized data");
            builder.AppendLine(header.AddressOfEntryPoint, "  Address of entry point");
            builder.AppendLine(header.BaseOfCode, "  Base of code");
            if (header.Magic == OptionalHeaderMagicNumber.PE32)
                builder.AppendLine(header.BaseOfData, "  Base of data");

            if (header.Magic == OptionalHeaderMagicNumber.PE32)
                builder.AppendLine(header.ImageBase_PE32, "  Image base");
            else
                builder.AppendLine(header.ImageBase_PE32Plus, "  Image base");
            builder.AppendLine(header.SectionAlignment, "  Section alignment");
            builder.AppendLine(header.FileAlignment, "  File alignment");
            builder.AppendLine(header.MajorOperatingSystemVersion, "  Major operating system version");
            builder.AppendLine(header.MinorOperatingSystemVersion, "  Minor operating system version");
            builder.AppendLine(header.MajorImageVersion, "  Major image version");
            builder.AppendLine(header.MinorImageVersion, "  Minor image version");
            builder.AppendLine(header.MajorSubsystemVersion, "  Major subsystem version");
            builder.AppendLine(header.MinorSubsystemVersion, "  Minor subsystem version");
            builder.AppendLine(header.Win32VersionValue, "  Win32 version value");
            builder.AppendLine(header.SizeOfImage, "  Size of image");
            builder.AppendLine(header.SizeOfHeaders, "  Size of headers");
            builder.AppendLine(header.CheckSum, "  Checksum");
            builder.AppendLine($"  Subsystem: {header.Subsystem} (0x{header.Subsystem:X})");
            builder.AppendLine($"  DLL characteristics: {header.DllCharacteristics} (0x{header.DllCharacteristics:X})");
            if (header.Magic == OptionalHeaderMagicNumber.PE32)
            {
                builder.AppendLine(header.SizeOfStackReserve_PE32, "  Size of stack reserve");
                builder.AppendLine(header.SizeOfStackCommit_PE32, "  Size of stack commit");
                builder.AppendLine(header.SizeOfHeapReserve_PE32, "  Size of heap reserve");
                builder.AppendLine(header.SizeOfHeapCommit_PE32, "  Size of heap commit");
            }
            else
            {
                builder.AppendLine(header.SizeOfStackReserve_PE32Plus, "  Size of stack reserve");
                builder.AppendLine(header.SizeOfStackCommit_PE32Plus, "  Size of stack commit");
                builder.AppendLine(header.SizeOfHeapReserve_PE32Plus, "  Size of heap reserve");
                builder.AppendLine(header.SizeOfHeapCommit_PE32Plus, "  Size of heap commit");
            }
            builder.AppendLine(header.LoaderFlags, "  Loader flags");
            builder.AppendLine(header.NumberOfRvaAndSizes, "  Number of data-directory entries");

            if (header.ExportTable != null)
            {
                builder.AppendLine("    Export Table (1)");
                builder.AppendLine(header.ExportTable.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.ExportTable.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.ExportTable.Size, "      Size");
            }
            if (header.ImportTable != null)
            {
                builder.AppendLine("    Import Table (2)");
                builder.AppendLine(header.ImportTable.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.ImportTable.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.ImportTable.Size, "      Size");
            }
            if (header.ResourceTable != null)
            {
                builder.AppendLine("    Resource Table (3)");
                builder.AppendLine(header.ResourceTable.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.ResourceTable.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.ResourceTable.Size, "      Size");
            }
            if (header.ExceptionTable != null)
            {
                builder.AppendLine("    Exception Table (4)");
                builder.AppendLine(header.ExceptionTable.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.ExceptionTable.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.ExceptionTable.Size, "      Size");
            }
            if (header.CertificateTable != null)
            {
                builder.AppendLine("    Certificate Table (5)");
                builder.AppendLine(header.CertificateTable.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.CertificateTable.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.CertificateTable.Size, "      Size");
            }
            if (header.BaseRelocationTable != null)
            {
                builder.AppendLine("    Base Relocation Table (6)");
                builder.AppendLine(header.BaseRelocationTable.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.BaseRelocationTable.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.BaseRelocationTable.Size, "      Size");
            }
            if (header.Debug != null)
            {
                builder.AppendLine("    Debug Table (7)");
                builder.AppendLine(header.Debug.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.Debug.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.Debug.Size, "      Size");
            }
            if (header.NumberOfRvaAndSizes >= 8)
            {
                builder.AppendLine("    Architecture Table (8)");
                builder.AppendLine("      Virtual address: 0 (0x00000000)");
                builder.AppendLine("      Physical address: 0 (0x00000000)");
                builder.AppendLine("      Size: 0 (0x00000000)");
            }
            if (header.GlobalPtr != null)
            {
                builder.AppendLine("    Global Pointer Register (9)");
                builder.AppendLine(header.GlobalPtr.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.GlobalPtr.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.GlobalPtr.Size, "      Size");
            }
            if (header.ThreadLocalStorageTable != null)
            {
                builder.AppendLine("    Thread Local Storage (TLS) Table (10)");
                builder.AppendLine(header.ThreadLocalStorageTable.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.ThreadLocalStorageTable.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.ThreadLocalStorageTable.Size, "      Size");
            }
            if (header.LoadConfigTable != null)
            {
                builder.AppendLine("    Load Config Table (11)");
                builder.AppendLine(header.LoadConfigTable.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.LoadConfigTable.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.LoadConfigTable.Size, "      Size");
            }
            if (header.BoundImport != null)
            {
                builder.AppendLine("    Bound Import Table (12)");
                builder.AppendLine(header.BoundImport.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.BoundImport.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.BoundImport.Size, "      Size");
            }
            if (header.ImportAddressTable != null)
            {
                builder.AppendLine("    Import Address Table (13)");
                builder.AppendLine(header.ImportAddressTable.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.ImportAddressTable.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.ImportAddressTable.Size, "      Size");
            }
            if (header.DelayImportDescriptor != null)
            {
                builder.AppendLine("    Delay Import Descriptior (14)");
                builder.AppendLine(header.DelayImportDescriptor.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.DelayImportDescriptor.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.DelayImportDescriptor.Size, "      Size");
            }
            if (header.CLRRuntimeHeader != null)
            {
                builder.AppendLine("    CLR Runtime Header (15)");
                builder.AppendLine(header.CLRRuntimeHeader.VirtualAddress, "      Virtual address");
                builder.AppendLine(header.CLRRuntimeHeader.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "      Physical address");
                builder.AppendLine(header.CLRRuntimeHeader.Size, "      Size");
            }
            if (header.NumberOfRvaAndSizes >= 16)
            {
                builder.AppendLine("    Reserved (16)");
                builder.AppendLine("      Virtual address: 0 (0x00000000)");
                builder.AppendLine("      Physical address: 0 (0x00000000)");
                builder.AppendLine("      Size: 0 (0x00000000)");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, SectionHeader[] table)
#else
        private static void Print(StringBuilder builder, SectionHeader?[]? table)
#endif
        {
            builder.AppendLine("  Section Table Information:");
            builder.AppendLine("  -------------------------");
            if (table == null || table.Length == 0)
            {
                builder.AppendLine("  No section table items");
                builder.AppendLine();
                return;
            }

#if NET48
            for (int i = 0; i < table.Length; i++)
#else
                for (int i = 0; i < table!.Length; i++)
#endif
            {
                var entry = table[i];
                builder.AppendLine($"  Section Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.VirtualSize, "    Virtual size");
                builder.AppendLine(entry.VirtualAddress, "    Virtual address");
                builder.AppendLine(entry.VirtualAddress.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "    Physical address");
                builder.AppendLine(entry.SizeOfRawData, "    Size of raw data");
                builder.AppendLine(entry.PointerToRawData, "    Pointer to raw data");
                builder.AppendLine(entry.PointerToRelocations, "    Pointer to relocations");
                builder.AppendLine(entry.PointerToLinenumbers, "    Pointer to linenumbers");
                builder.AppendLine(entry.NumberOfRelocations, "    Number of relocations");
                builder.AppendLine(entry.NumberOfLinenumbers, "    Number of linenumbers");
                builder.AppendLine($"    Characteristics: {entry.Characteristics} (0x{entry.Characteristics:X})");
                // TODO: Add COFFRelocations
                // TODO: Add COFFLineNumbers
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, COFFSymbolTableEntry[] symbolTable)
#else
        private static void Print(StringBuilder builder, COFFSymbolTableEntry?[]? symbolTable)
#endif
        {
            builder.AppendLine("  COFF Symbol Table Information:");
            builder.AppendLine("  -------------------------");
            if (symbolTable == null || symbolTable.Length == 0)
            {
                builder.AppendLine("  No COFF symbol table items");
                builder.AppendLine();
                return;
            }

            int auxSymbolsRemaining = 0;
            int currentSymbolType = 0;

            for (int i = 0; i < symbolTable.Length; i++)
            {
                var entry = symbolTable[i];
                builder.AppendLine($"  COFF Symbol Table Entry {i} (Subtype {currentSymbolType})");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                if (currentSymbolType == 0)
                {
                    if (entry.ShortName != null)
                    {
                        builder.AppendLine(entry.ShortName, "    Short name");
                    }
                    else
                    {
                        builder.AppendLine(entry.Zeroes, "    Zeroes");
                        builder.AppendLine(entry.Offset, "    Offset");
                    }
                    builder.AppendLine(entry.Value, "    Value");
                    builder.AppendLine(entry.SectionNumber, "    Section number");
                    builder.AppendLine($"    Symbol type: {entry.SymbolType} (0x{entry.SymbolType:X})");
                    builder.AppendLine($"    Storage class: {entry.StorageClass} (0x{entry.StorageClass:X})");
                    builder.AppendLine(entry.NumberOfAuxSymbols, "    Number of aux symbols");

                    auxSymbolsRemaining = entry.NumberOfAuxSymbols;
                    if (auxSymbolsRemaining == 0)
                        continue;

                    if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                        && entry.SymbolType == SymbolType.IMAGE_SYM_TYPE_FUNC
                        && entry.SectionNumber > 0)
                    {
                        currentSymbolType = 1;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_FUNCTION
                        && entry.ShortName != null
                        && ((entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x62 && entry.ShortName[2] == 0x66)  // .bf
                            || (entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x65 && entry.ShortName[2] == 0x66))) // .ef
                    {
                        currentSymbolType = 2;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                        && entry.SectionNumber == (ushort)SectionNumber.IMAGE_SYM_UNDEFINED
                        && entry.Value == 0)
                    {
                        currentSymbolType = 3;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_FILE)
                    {
                        // TODO: Symbol name should be ".file"
                        currentSymbolType = 4;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_STATIC)
                    {
                        // TODO: Should have the name of a section (like ".text")
                        currentSymbolType = 5;
                    }
                    else if (entry.StorageClass == StorageClass.IMAGE_SYM_CLASS_CLR_TOKEN)
                    {
                        currentSymbolType = 6;
                    }
                }
                else if (currentSymbolType == 1)
                {
                    builder.AppendLine(entry.AuxFormat1TagIndex, "    Tag index");
                    builder.AppendLine(entry.AuxFormat1TotalSize, "    Total size");
                    builder.AppendLine(entry.AuxFormat1PointerToLinenumber, "    Pointer to linenumber");
                    builder.AppendLine(entry.AuxFormat1PointerToNextFunction, "    Pointer to next function");
                    builder.AppendLine(entry.AuxFormat1Unused, "    Unused");
                    auxSymbolsRemaining--;
                }
                else if (currentSymbolType == 2)
                {
                    builder.AppendLine(entry.AuxFormat2Unused1, "    Unused");
                    builder.AppendLine(entry.AuxFormat2Linenumber, "    Linenumber");
                    builder.AppendLine(entry.AuxFormat2Unused2, "    Unused");
                    builder.AppendLine(entry.AuxFormat2PointerToNextFunction, "    Pointer to next function");
                    builder.AppendLine(entry.AuxFormat2Unused3, "    Unused");
                    auxSymbolsRemaining--;
                }
                else if (currentSymbolType == 3)
                {
                    builder.AppendLine(entry.AuxFormat3TagIndex, "    Tag index");
                    builder.AppendLine(entry.AuxFormat3Characteristics, "    Characteristics");
                    builder.AppendLine(entry.AuxFormat3Unused, "    Unused");
                    auxSymbolsRemaining--;
                }
                else if (currentSymbolType == 4)
                {
                    builder.AppendLine(entry.AuxFormat4FileName, "    File name");
                    auxSymbolsRemaining--;
                }
                else if (currentSymbolType == 5)
                {
                    builder.AppendLine(entry.AuxFormat5Length, "    Length");
                    builder.AppendLine(entry.AuxFormat5NumberOfRelocations, "    Number of relocations");
                    builder.AppendLine(entry.AuxFormat5NumberOfLinenumbers, "    Number of linenumbers");
                    builder.AppendLine(entry.AuxFormat5CheckSum, "    Checksum");
                    builder.AppendLine(entry.AuxFormat5Number, "    Number");
                    builder.AppendLine(entry.AuxFormat5Selection, "    Selection");
                    builder.AppendLine(entry.AuxFormat5Unused, "    Unused");
                    auxSymbolsRemaining--;
                }
                else if (currentSymbolType == 6)
                {
                    builder.AppendLine(entry.AuxFormat6AuxType, "    Aux type");
                    builder.AppendLine(entry.AuxFormat6Reserved1, "    Reserved");
                    builder.AppendLine(entry.AuxFormat6SymbolTableIndex, "    Symbol table index");
                    builder.AppendLine(entry.AuxFormat6Reserved2, "    Reserved");
                    auxSymbolsRemaining--;
                }

                // If we hit the last aux symbol, go back to normal format
                if (auxSymbolsRemaining == 0)
                    currentSymbolType = 0;
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, COFFStringTable stringTable)
#else
        private static void Print(StringBuilder builder, COFFStringTable? stringTable)
#endif
        {
            builder.AppendLine("  COFF String Table Information:");
            builder.AppendLine("  -------------------------");
            if (stringTable?.Strings == null || stringTable.Strings.Length == 0)
            {
                builder.AppendLine("  No COFF string table items");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(stringTable.TotalSize, "  Total size");
            for (int i = 0; i < stringTable.Strings.Length; i++)
            {
#if NET48
                string entry = stringTable.Strings[i];
#else
                string? entry = stringTable.Strings[i];
#endif
                builder.AppendLine($"  COFF String Table Entry {i})");
                builder.AppendLine(entry, "    Value");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, AttributeCertificateTableEntry[] entries)
#else
        private static void Print(StringBuilder builder, AttributeCertificateTableEntry?[]? entries)
#endif
        {
            builder.AppendLine("  Attribute Certificate Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No attribute certificate table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Attribute Certificate Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.Length, "    Length");
                builder.AppendLine($"    Revision: {entry.Revision} (0x{entry.Revision:X})");
                builder.AppendLine($"    Certificate type: {entry.CertificateType} (0x{entry.CertificateType:X})");
                builder.AppendLine();
                if (entry.CertificateType == WindowsCertificateType.WIN_CERT_TYPE_PKCS_SIGNED_DATA)
                {
                    builder.AppendLine("    Certificate Data [Formatted]");
                    builder.AppendLine("    -------------------------");
                    if (entry.Certificate == null)
                    {
                        builder.AppendLine("    INVALID DATA FOUND");
                    }
                    else
                    {
                        var topLevelValues = AbstractSyntaxNotationOne.Parse(entry.Certificate, 0);
                        if (topLevelValues == null)
                        {
                            builder.AppendLine("    INVALID DATA FOUND");
                            builder.AppendLine(entry.Certificate, "    Raw data");
                        }
                        else
                        {
                            foreach (TypeLengthValue tlv in topLevelValues)
                            {
                                string tlvString = tlv.Format(paddingLevel: 4);
                                builder.AppendLine(tlvString);
                            }
                        }
                    }
                }
                else
                {
                    builder.AppendLine("    Certificate Data [Binary]");
                    builder.AppendLine("  -------------------------");
                    try
                    {
                        builder.AppendLine(entry.Certificate, "    Raw data");
                    }
                    catch
                    {
                        builder.AppendLine("    [DATA TOO LARGE TO FORMAT]");
                    }
                }

                builder.AppendLine();
            }
        }

#if NET48
        private static void Print(StringBuilder builder, DelayLoadDirectoryTable table)
#else
        private static void Print(StringBuilder builder, DelayLoadDirectoryTable? table)
#endif
        {
            builder.AppendLine("  Delay-Load Directory Table Information:");
            builder.AppendLine("  -------------------------");
            if (table == null)
            {
                builder.AppendLine("  No delay-load directory table items");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(table.Attributes, "  Attributes");
            builder.AppendLine(table.Name, "  Name RVA");
            builder.AppendLine(table.ModuleHandle, "  Module handle");
            builder.AppendLine(table.DelayImportAddressTable, "  Delay import address table RVA");
            builder.AppendLine(table.DelayImportNameTable, "  Delay import name table RVA");
            builder.AppendLine(table.BoundDelayImportTable, "  Bound delay import table RVA");
            builder.AppendLine(table.UnloadDelayImportTable, "  Unload delay import table RVA");
            builder.AppendLine(table.TimeStamp, "  Timestamp");
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, BaseRelocationBlock[] entries, SectionHeader[] table)
#else
        private static void Print(StringBuilder builder, BaseRelocationBlock?[]? entries, SectionHeader?[]? table)
#endif
        {
            builder.AppendLine("  Base Relocation Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No base relocation table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var baseRelocationTableEntry = entries[i];
                builder.AppendLine($"  Base Relocation Table Entry {i}");
                if (baseRelocationTableEntry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(baseRelocationTableEntry.PageRVA, "    Page RVA");
                builder.AppendLine(baseRelocationTableEntry.PageRVA.ConvertVirtualAddress(table ?? Array.Empty<SectionHeader>()), "    Page physical address");
                builder.AppendLine(baseRelocationTableEntry.BlockSize, "    Block size");

                builder.AppendLine($"    Base Relocation Table {i} Type and Offset Information:");
                builder.AppendLine("    -------------------------");
                if (baseRelocationTableEntry.TypeOffsetFieldEntries == null || baseRelocationTableEntry.TypeOffsetFieldEntries.Length == 0)
                {
                    builder.AppendLine("    No base relocation table type and offset entries");
                    continue;
                }

                for (int j = 0; j < baseRelocationTableEntry.TypeOffsetFieldEntries.Length; j++)
                {
                    var typeOffsetFieldEntry = baseRelocationTableEntry.TypeOffsetFieldEntries[j];
                    builder.AppendLine($"    Type and Offset Entry {j}");
#if NET6_0_OR_GREATER
                            if (typeOffsetFieldEntry == null)
                            {
                                builder.AppendLine("      [NULL]");
                                continue;
                            }
#endif
                    builder.AppendLine($"      Type: {typeOffsetFieldEntry.BaseRelocationType} (0x{typeOffsetFieldEntry.BaseRelocationType:X})");
                    builder.AppendLine(typeOffsetFieldEntry.Offset, "      Offset");
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, DebugTable table)
#else
        private static void Print(StringBuilder builder, DebugTable? table)
#endif
        {
            builder.AppendLine("  Debug Table Information:");
            builder.AppendLine("  -------------------------");
            if (table?.DebugDirectoryTable == null || table.DebugDirectoryTable.Length == 0)
            {
                builder.AppendLine("  No debug table items");
                builder.AppendLine();
                return;
            }

            // TODO: If more sections added, model this after the Export Table
            for (int i = 0; i < table.DebugDirectoryTable.Length; i++)
            {
                var entry = table.DebugDirectoryTable[i];
                builder.AppendLine($"  Debug Directory Table Entry {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.Characteristics, "    Characteristics");
                builder.AppendLine(entry.TimeDateStamp, "    Time/Date stamp");
                builder.AppendLine(entry.MajorVersion, "    Major version");
                builder.AppendLine(entry.MinorVersion, "    Minor version");
                builder.AppendLine($"    Debug type: {entry.DebugType} (0x{entry.DebugType:X})");
                builder.AppendLine(entry.SizeOfData, "    Size of data");
                builder.AppendLine(entry.AddressOfRawData, "    Address of raw data");
                builder.AppendLine(entry.PointerToRawData, "    Pointer to raw data");
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ExportTable table)
#else
        private static void Print(StringBuilder builder, ExportTable? table)
#endif
        {
            builder.AppendLine("  Export Table Information:");
            builder.AppendLine("  -------------------------");
            if (table == null)
            {
                builder.AppendLine("  No export table");
                builder.AppendLine();
                return;
            }

            builder.AppendLine("    Export Directory Table Information:");
            builder.AppendLine("    -------------------------");
            if (table.ExportDirectoryTable == null)
            {
                builder.AppendLine("  No export directory table");
            }
            else
            {
                builder.AppendLine(table.ExportDirectoryTable.ExportFlags, "    Export flags");
                builder.AppendLine(table.ExportDirectoryTable.TimeDateStamp, "    Time/Date stamp");
                builder.AppendLine(table.ExportDirectoryTable.MajorVersion, "    Major version");
                builder.AppendLine(table.ExportDirectoryTable.MinorVersion, "    Minor version");
                builder.AppendLine(table.ExportDirectoryTable.NameRVA, "    Name RVA");
                builder.AppendLine(table.ExportDirectoryTable.Name, "    Name");
                builder.AppendLine(table.ExportDirectoryTable.OrdinalBase, "    Ordinal base");
                builder.AppendLine(table.ExportDirectoryTable.AddressTableEntries, "    Address table entries");
                builder.AppendLine(table.ExportDirectoryTable.NumberOfNamePointers, "    Number of name pointers");
                builder.AppendLine(table.ExportDirectoryTable.ExportAddressTableRVA, "    Export address table RVA");
                builder.AppendLine(table.ExportDirectoryTable.NamePointerRVA, "    Name pointer table RVA");
                builder.AppendLine(table.ExportDirectoryTable.OrdinalTableRVA, "    Ordinal table RVA");
            }
            builder.AppendLine();

            builder.AppendLine("    Export Address Table Information:");
            builder.AppendLine("    -------------------------");
            if (table.ExportAddressTable == null || table.ExportAddressTable.Length == 0)
            {
                builder.AppendLine("    No export address table items");
            }
            else
            {
                for (int i = 0; i < table.ExportAddressTable.Length; i++)
                {
                    var exportAddressTableEntry = table.ExportAddressTable[i];
                    builder.AppendLine($"    Export Address Table Entry {i}");
                    if (exportAddressTableEntry == null)
                    {
                        builder.AppendLine("      [NULL]");
                        continue;
                    }

                    builder.AppendLine(exportAddressTableEntry.ExportRVA, "      Export RVA / Forwarder RVA");
                }
            }
            builder.AppendLine();

            builder.AppendLine("    Name Pointer Table Information:");
            builder.AppendLine("    -------------------------");
            if (table.NamePointerTable?.Pointers == null || table.NamePointerTable.Pointers.Length == 0)
            {
                builder.AppendLine("    No name pointer table items");
            }
            else
            {
                for (int i = 0; i < table.NamePointerTable.Pointers.Length; i++)
                {
                    var namePointerTableEntry = table.NamePointerTable.Pointers[i];
                    builder.AppendLine($"    Name Pointer Table Entry {i}");
                    builder.AppendLine(namePointerTableEntry, "      Pointer");
                }
            }
            builder.AppendLine();

            builder.AppendLine("    Ordinal Table Information:");
            builder.AppendLine("    -------------------------");
            if (table.OrdinalTable?.Indexes == null || table.OrdinalTable.Indexes.Length == 0)
            {
                builder.AppendLine("    No ordinal table items");
            }
            else
            {
                for (int i = 0; i < table.OrdinalTable.Indexes.Length; i++)
                {
                    var ordinalTableEntry = table.OrdinalTable.Indexes[i];
                    builder.AppendLine($"    Ordinal Table Entry {i}");
                    builder.AppendLine(ordinalTableEntry, "      Index");
                }
            }
            builder.AppendLine();

            builder.AppendLine("    Export Name Table Information:");
            builder.AppendLine("    -------------------------");
            if (table.ExportNameTable?.Strings == null || table.ExportNameTable.Strings.Length == 0)
            {
                builder.AppendLine("    No export name table items");
            }
            else
            {
                for (int i = 0; i < table.ExportNameTable.Strings.Length; i++)
                {
                    var exportNameTableEntry = table.ExportNameTable.Strings[i];
                    builder.AppendLine($"    Export Name Table Entry {i}");
                    builder.AppendLine(exportNameTableEntry, "      String");
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ImportTable table, SectionHeader[] sectionTable)
#else
        private static void Print(StringBuilder builder, ImportTable? table, SectionHeader?[]? sectionTable)
#endif
        {
            builder.AppendLine("  Import Table Information:");
            builder.AppendLine("  -------------------------");
            if (table == null)
            {
                builder.AppendLine("  No import table");
                builder.AppendLine();
                return;
            }

            builder.AppendLine();
            builder.AppendLine("    Import Directory Table Information:");
            builder.AppendLine("    -------------------------");
            if (table.ImportDirectoryTable == null || table.ImportDirectoryTable.Length == 0)
            {
                builder.AppendLine("    No import directory table items");
            }
            else
            {
                for (int i = 0; i < table.ImportDirectoryTable.Length; i++)
                {
                    var importDirectoryTableEntry = table.ImportDirectoryTable[i];
                    builder.AppendLine($"    Import Directory Table Entry {i}");
                    if (importDirectoryTableEntry == null)
                    {
                        builder.AppendLine("      [NULL]");
                        continue;
                    }

                    builder.AppendLine(importDirectoryTableEntry.ImportLookupTableRVA, "      Import lookup table RVA");
                    builder.AppendLine(importDirectoryTableEntry.ImportLookupTableRVA.ConvertVirtualAddress(sectionTable ?? Array.Empty<SectionHeader>()), "      Import lookup table Physical Address");
                    builder.AppendLine(importDirectoryTableEntry.TimeDateStamp, "      Time/Date stamp");
                    builder.AppendLine(importDirectoryTableEntry.ForwarderChain, "      Forwarder chain");
                    builder.AppendLine(importDirectoryTableEntry.NameRVA, "      Name RVA");
                    builder.AppendLine(importDirectoryTableEntry.Name, "      Name");
                    builder.AppendLine(importDirectoryTableEntry.ImportAddressTableRVA, "      Import address table RVA");
                    builder.AppendLine(importDirectoryTableEntry.ImportAddressTableRVA.ConvertVirtualAddress(sectionTable ?? Array.Empty<SectionHeader>()), "      Import address table Physical Address");
                }
            }
            builder.AppendLine();

            builder.AppendLine("    Import Lookup Tables Information:");
            builder.AppendLine("    -------------------------");
            if (table.ImportLookupTables == null || table.ImportLookupTables.Count == 0)
            {
                builder.AppendLine("    No import lookup tables");
            }
            else
            {
                foreach (var kvp in table.ImportLookupTables)
                {
                    int index = kvp.Key;
                    var importLookupTable = kvp.Value;

                    builder.AppendLine();
                    builder.AppendLine($"      Import Lookup Table {index} Information:");
                    builder.AppendLine("      -------------------------");
                    if (importLookupTable == null || importLookupTable.Length == 0)
                    {
                        builder.AppendLine("      No import lookup table items");
                        continue;
                    }

                    for (int i = 0; i < importLookupTable.Length; i++)
                    {
                        var importLookupTableEntry = importLookupTable[i];
                        builder.AppendLine($"      Import Lookup Table {index} Entry {i}");
                        if (importLookupTableEntry == null)
                        {
                            builder.AppendLine("        [NULL]");
                            continue;
                        }

                        builder.AppendLine(importLookupTableEntry.OrdinalNameFlag, "        Ordinal/Name flag");
                        if (importLookupTableEntry.OrdinalNameFlag)
                        {
                            builder.AppendLine(importLookupTableEntry.OrdinalNumber, "        Ordinal number");
                        }
                        else
                        {
                            builder.AppendLine(importLookupTableEntry.HintNameTableRVA, "        Hint/Name table RVA");
                            builder.AppendLine(importLookupTableEntry.HintNameTableRVA.ConvertVirtualAddress(sectionTable ?? Array.Empty<SectionHeader>()), "        Hint/Name table Physical Address");
                        }
                    }
                }
            }
            builder.AppendLine();

            builder.AppendLine("    Import Address Tables Information:");
            builder.AppendLine("    -------------------------");
            if (table.ImportAddressTables == null || table.ImportAddressTables.Count == 0)
            {
                builder.AppendLine("    No import address tables");
            }
            else
            {
                foreach (var kvp in table.ImportAddressTables)
                {
                    int index = kvp.Key;
                    var importAddressTable = kvp.Value;

                    builder.AppendLine();
                    builder.AppendLine($"      Import Address Table {index} Information:");
                    builder.AppendLine("      -------------------------");
                    if (importAddressTable == null || importAddressTable.Length == 0)
                    {
                        builder.AppendLine("      No import address table items");
                        continue;
                    }

                    for (int i = 0; i < importAddressTable.Length; i++)
                    {
                        var importAddressTableEntry = importAddressTable[i];
                        builder.AppendLine($"      Import Address Table {index} Entry {i}");
                        if (importAddressTableEntry == null)
                        {
                            builder.AppendLine("        [NULL]");
                            continue;
                        }

                        builder.AppendLine(importAddressTableEntry.OrdinalNameFlag, "        Ordinal/Name flag");
                        if (importAddressTableEntry.OrdinalNameFlag)
                        {
                            builder.AppendLine(importAddressTableEntry.OrdinalNumber, "        Ordinal number");
                        }
                        else
                        {
                            builder.AppendLine(importAddressTableEntry.HintNameTableRVA, "        Hint/Name table RVA");
                            builder.AppendLine(importAddressTableEntry.HintNameTableRVA.ConvertVirtualAddress(sectionTable ?? Array.Empty<SectionHeader>()), "        Hint/Name table Physical Address");
                        }
                    }
                }
            }
            builder.AppendLine();

            builder.AppendLine("    Hint/Name Table Information:");
            builder.AppendLine("    -------------------------");
            if (table.HintNameTable == null || table.HintNameTable.Length == 0)
            {
                builder.AppendLine("    No hint/name table items");
            }
            else
            {
                for (int i = 0; i < table.HintNameTable.Length; i++)
                {
                    var hintNameTableEntry = table.HintNameTable[i];
                    builder.AppendLine($"    Hint/Name Table Entry {i}");
                    if (hintNameTableEntry == null)
                    {
                        builder.AppendLine("        [NULL]");
                        continue;
                    }

                    builder.AppendLine(hintNameTableEntry.Hint, "      Hint");
                    builder.AppendLine(hintNameTableEntry.Name, "      Name");
                }
            }
            builder.AppendLine();
        }

#if NET48
        private static void Print(StringBuilder builder, ResourceDirectoryTable table)
#else
        private static void Print(StringBuilder builder, ResourceDirectoryTable? table)
#endif
        {
            builder.AppendLine("  Resource Directory Table Information:");
            builder.AppendLine("  -------------------------");
            if (table == null)
            {
                builder.AppendLine("  No resource directory table items");
                builder.AppendLine();
                return;
            }

            Print(table, level: 0, types: new List<object>(), builder);
            builder.AppendLine();
        }

        private static void Print(ResourceDirectoryTable table, int level, List<object> types, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            builder.AppendLine(level, $"{padding}Table level");
            builder.AppendLine(table.Characteristics, $"{padding}Characteristics");
            builder.AppendLine(table.TimeDateStamp, $"{padding}Time/Date stamp");
            builder.AppendLine(table.MajorVersion, $"{padding}Major version");
            builder.AppendLine(table.MinorVersion, $"{padding}Minor version");
            builder.AppendLine(table.NumberOfNameEntries, $"{padding}Number of name entries");
            builder.AppendLine(table.NumberOfIDEntries, $"{padding}Number of ID entries");
            builder.AppendLine();

            builder.AppendLine($"{padding}Entries");
            builder.AppendLine($"{padding}-------------------------");
            if (table.NumberOfNameEntries == 0 && table.NumberOfIDEntries == 0)
            {
                builder.AppendLine($"{padding}No entries");
                builder.AppendLine();
            }
            else
            {
                if (table.Entries == null)
                    return;

                for (int i = 0; i < table.Entries.Length; i++)
                {
                    var entry = table.Entries[i];
                    if (entry == null)
                        continue;

                    var newTypes = new List<object>(types ?? new List<object>());
                    if (entry.Name?.UnicodeString != null)
                        newTypes.Add(Encoding.UTF8.GetString(entry.Name.UnicodeString));
                    else
                        newTypes.Add(entry.IntegerID);

                    PrintResourceDirectoryEntry(entry, level + 1, newTypes, builder);
                }
            }
        }

        private static void PrintResourceDirectoryEntry(ResourceDirectoryEntry entry, int level, List<object> types, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            builder.AppendLine(level, $"{padding}Item level");
            if (entry.NameOffset != default)
            {
                builder.AppendLine(entry.NameOffset, $"{padding}Name offset");
                builder.AppendLine(entry.Name?.UnicodeString, $"{padding}Name ({entry.Name?.Length ?? 0})");
            }
            else
            {
                builder.AppendLine(entry.IntegerID, $"{padding}Integer ID");
            }

            if (entry.DataEntry != null)
                PrintResourceDataEntry(entry.DataEntry, level: level + 1, types, builder);
            else if (entry.Subdirectory != null)
                Print(entry.Subdirectory, level: level + 1, types, builder);
        }

        private static void PrintResourceDataEntry(ResourceDataEntry entry, int level, List<object> types, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            // TODO: Use ordered list of base types to determine the shape of the data
            builder.AppendLine($"{padding}Base types: {string.Join(", ", types)}");

            builder.AppendLine(level, $"{padding}Entry level");
            builder.AppendLine(entry.DataRVA, $"{padding}Data RVA");
            builder.AppendLine(entry.Size, $"{padding}Size");
            builder.AppendLine(entry.Codepage, $"{padding}Codepage");
            builder.AppendLine(entry.Reserved, $"{padding}Reserved");

            // TODO: Print out per-type data
            if (types != null && types.Count > 0 && types[0] is uint resourceType)
            {
                switch ((ResourceType)resourceType)
                {
                    case ResourceType.RT_CURSOR:
                        PrintResourceRT_CURSOR(entry, level, builder);
                        break;
                    case ResourceType.RT_BITMAP:
                        PrintResourceRT_BITMAP(entry, level, builder);
                        break;
                    case ResourceType.RT_ICON:
                        PrintResourceRT_ICON(entry, level, builder);
                        break;
                    case ResourceType.RT_MENU:
                        PrintResourceRT_MENU(entry, level, builder);
                        break;
                    case ResourceType.RT_DIALOG:
                        PrintResourceRT_DIALOG(entry, level, builder);
                        break;
                    case ResourceType.RT_STRING:
                        PrintResourceRT_STRING(entry, level, builder);
                        break;
                    case ResourceType.RT_FONTDIR:
                        PrintResourceRT_FONTDIR(entry, level, builder);
                        break;
                    case ResourceType.RT_FONT:
                        PrintResourceRT_FONT(entry, level, builder);
                        break;
                    case ResourceType.RT_ACCELERATOR:
                        PrintResourceRT_ACCELERATOR(entry, level, builder);
                        break;
                    case ResourceType.RT_RCDATA:
                        PrintResourceRT_RCDATA(entry, level, builder);
                        break;
                    case ResourceType.RT_MESSAGETABLE:
                        PrintResourceRT_MESSAGETABLE(entry, level, builder);
                        break;
                    case ResourceType.RT_GROUP_CURSOR:
                        PrintResourceRT_GROUP_CURSOR(entry, level, builder);
                        break;
                    case ResourceType.RT_GROUP_ICON:
                        PrintResourceRT_GROUP_ICON(entry, level, builder);
                        break;
                    case ResourceType.RT_VERSION:
                        PrintResourceRT_VERSION(entry, level, builder);
                        break;
                    case ResourceType.RT_DLGINCLUDE:
                        PrintResourceRT_DLGINCLUDE(entry, level, builder);
                        break;
                    case ResourceType.RT_PLUGPLAY:
                        PrintResourceRT_PLUGPLAY(entry, level, builder);
                        break;
                    case ResourceType.RT_VXD:
                        PrintResourceRT_VXD(entry, level, builder);
                        break;
                    case ResourceType.RT_ANICURSOR:
                        PrintResourceRT_ANICURSOR(entry, level, builder);
                        break;
                    case ResourceType.RT_ANIICON:
                        PrintResourceRT_ANIICON(entry, level, builder);
                        break;
                    case ResourceType.RT_HTML:
                        PrintResourceRT_HTML(entry, level, builder);
                        break;
                    case ResourceType.RT_MANIFEST:
                        PrintResourceRT_MANIFEST(entry, level, builder);
                        break;
                    default:
                        PrintResourceUNKNOWN(entry, level, types[0], builder);
                        break;
                }
            }
            else if (types != null && types.Count > 0 && types[0] is string resourceString)
            {
                PrintResourceUNKNOWN(entry, level, types[0], builder);
            }

            builder.AppendLine();
        }

        private static void PrintResourceRT_CURSOR(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Hardware-dependent cursor resource found, not parsed yet");
        }

        private static void PrintResourceRT_BITMAP(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Bitmap resource found, not parsed yet");
        }

        private static void PrintResourceRT_ICON(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Hardware-dependent icon resource found, not parsed yet");
        }

        private static void PrintResourceRT_MENU(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

#if NET48
            MenuResource menu = null;
#else
            MenuResource? menu = null;
#endif
            try { menu = entry.AsMenu(); } catch { }
            if (menu == null)
            {
                builder.AppendLine($"{padding}Menu resource found, but malformed");
                return;
            }

            if (menu.MenuHeader != null)
            {
                builder.AppendLine(menu.MenuHeader.Version, $"{padding}Version");
                builder.AppendLine(menu.MenuHeader.HeaderSize, $"{padding}Header size");
                builder.AppendLine();
                builder.AppendLine($"{padding}Menu items");
                builder.AppendLine($"{padding}-------------------------");
                if (menu.MenuItems == null || menu.MenuItems.Length == 0)
                {
                    builder.AppendLine($"{padding}No menu items");
                    return;
                }

                for (int i = 0; i < menu.MenuItems.Length; i++)
                {
                    var menuItem = menu.MenuItems[i];
                    builder.AppendLine($"{padding}Menu item {i}");
                    if (menuItem == null)
                    {
                        builder.AppendLine($"{padding}  [NULL]");
                        continue;
                    }

                    if (menuItem.NormalMenuText != null)
                    {
                        builder.AppendLine($"{padding}  Resource info: {menuItem.NormalResInfo} (0x{menuItem.NormalResInfo:X})");
                        builder.AppendLine(menuItem.NormalMenuText, $"{padding}  Menu text");
                    }
                    else
                    {
                        builder.AppendLine($"{padding}  Item type: {menuItem.PopupItemType} (0x{menuItem.PopupItemType:X})");
                        builder.AppendLine($"{padding}  State: {menuItem.PopupState} (0x{menuItem.PopupState:X})");
                        builder.AppendLine(menuItem.PopupID, $"{padding}  ID");
                        builder.AppendLine($"{padding}  Resource info: {menuItem.PopupResInfo} (0x{menuItem.PopupResInfo:X})");
                        builder.AppendLine(menuItem.PopupMenuText, $"{padding}  Menu text");
                    }
                }
            }
            else if (menu.ExtendedMenuHeader != null)
            {
                builder.AppendLine(menu.ExtendedMenuHeader.Version, $"{padding}Version");
                builder.AppendLine(menu.ExtendedMenuHeader.Offset, $"{padding}Offset");
                builder.AppendLine(menu.ExtendedMenuHeader.HelpID, $"{padding}Help ID");
                builder.AppendLine();
                builder.AppendLine($"{padding}Menu items");
                builder.AppendLine($"{padding}-------------------------");
                if (menu.ExtendedMenuHeader.Offset == 0
                    || menu.ExtendedMenuItems == null
                    || menu.ExtendedMenuItems.Length == 0)
                {
                    builder.AppendLine($"{padding}No menu items");
                    return;
                }

                for (int i = 0; i < menu.ExtendedMenuItems.Length; i++)
                {
                    var menuItem = menu.ExtendedMenuItems[i];

                    builder.AppendLine($"{padding}Menu item {i}");
                    if (menuItem == null)
                    {
                        builder.AppendLine($"{padding}  [NULL]");
                        continue;
                    }

                    builder.AppendLine($"{padding}  Item type: {menuItem.ItemType} (0x{menuItem.ItemType:X})");
                    builder.AppendLine($"{padding}  State: {menuItem.State} (0x{menuItem.State:X})");
                    builder.AppendLine(menuItem.ID, $"{padding}  ID");
                    builder.AppendLine($"{padding}  Flags: {menuItem.Flags} (0x{menuItem.Flags:X})");
                    builder.AppendLine(menuItem.MenuText, $"{padding}  Menu text");
                }
            }
            else
            {
                builder.AppendLine($"{padding}Menu resource found, but malformed");
            }
        }

        private static void PrintResourceRT_DIALOG(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

#if NET48
            DialogBoxResource dialogBox = null;
#else
            DialogBoxResource? dialogBox = null;
#endif
            try { dialogBox = entry.AsDialogBox(); } catch { }
            if (dialogBox == null)
            {
                builder.AppendLine($"{padding}Dialog box resource found, but malformed");
                return;
            }

            if (dialogBox.DialogTemplate != null)
            {
                builder.AppendLine($"{padding}Style: {dialogBox.DialogTemplate.Style} (0x{dialogBox.DialogTemplate.Style:X})");
                builder.AppendLine($"{padding}Extended style: {dialogBox.DialogTemplate.ExtendedStyle} (0x{dialogBox.DialogTemplate.ExtendedStyle:X})");
                builder.AppendLine(dialogBox.DialogTemplate.ItemCount, $"{padding}Item count");
                builder.AppendLine(dialogBox.DialogTemplate.PositionX, $"{padding}X-coordinate of upper-left corner");
                builder.AppendLine(dialogBox.DialogTemplate.PositionY, $"{padding}Y-coordinate of upper-left corner");
                builder.AppendLine(dialogBox.DialogTemplate.WidthX, $"{padding}Width of the dialog box");
                builder.AppendLine(dialogBox.DialogTemplate.HeightY, $"{padding}Height of the dialog box");
                builder.AppendLine(dialogBox.DialogTemplate.MenuResource, $"{padding}Menu resource");
                builder.AppendLine(dialogBox.DialogTemplate.MenuResourceOrdinal, $"{padding}Menu resource ordinal");
                builder.AppendLine(dialogBox.DialogTemplate.ClassResource, $"{padding}Class resource");
                builder.AppendLine(dialogBox.DialogTemplate.ClassResourceOrdinal, $"{padding}Class resource ordinal");
                builder.AppendLine(dialogBox.DialogTemplate.TitleResource, $"{padding}Title resource");
                builder.AppendLine(dialogBox.DialogTemplate.PointSizeValue, $"{padding}Point size value");
                builder.AppendLine(dialogBox.DialogTemplate.Typeface, $"{padding}Typeface");
                builder.AppendLine();
                builder.AppendLine($"{padding}Dialog item templates");
                builder.AppendLine($"{padding}-------------------------");
                if (dialogBox.DialogTemplate.ItemCount == 0
                    || dialogBox.DialogItemTemplates == null
                    || dialogBox.DialogItemTemplates.Length == 0)
                {
                    builder.AppendLine($"{padding}No dialog item templates");
                    return;
                }

                for (int i = 0; i < dialogBox.DialogItemTemplates.Length; i++)
                {
                    var dialogItemTemplate = dialogBox.DialogItemTemplates[i];
                    builder.AppendLine($"{padding}Dialog item template {i}");
                    if (dialogItemTemplate == null)
                    {
                        builder.AppendLine($"{padding}  [NULL]");
                        continue;
                    }

                    builder.AppendLine($"{padding}  Style: {dialogItemTemplate.Style} (0x{dialogItemTemplate.Style:X})");
                    builder.AppendLine($"{padding}  Extended style: {dialogItemTemplate.ExtendedStyle} (0x{dialogItemTemplate.ExtendedStyle:X})");
                    builder.AppendLine(dialogItemTemplate.PositionX, $"{padding}  X-coordinate of upper-left corner");
                    builder.AppendLine(dialogItemTemplate.PositionY, $"{padding}  Y-coordinate of upper-left corner");
                    builder.AppendLine(dialogItemTemplate.WidthX, $"{padding}  Width of the control");
                    builder.AppendLine(dialogItemTemplate.HeightY, $"{padding}  Height of the control");
                    builder.AppendLine(dialogItemTemplate.ID, $"{padding}  ID");
                    builder.AppendLine(dialogItemTemplate.ClassResource, $"{padding}  Class resource");
                    builder.AppendLine($"{padding}  Class resource ordinal: {dialogItemTemplate.ClassResourceOrdinal} (0x{dialogItemTemplate.ClassResourceOrdinal:X})");
                    builder.AppendLine(dialogItemTemplate.TitleResource, $"{padding}  Title resource");
                    builder.AppendLine(dialogItemTemplate.TitleResourceOrdinal, $"{padding}  Title resource ordinal");
                    builder.AppendLine(dialogItemTemplate.CreationDataSize, $"{padding}  Creation data size");
                    if (dialogItemTemplate.CreationData != null && dialogItemTemplate.CreationData.Length != 0)
                        builder.AppendLine(dialogItemTemplate.CreationData, $"{padding}  Creation data");
                    else
                        builder.AppendLine($"{padding}  Creation data: [EMPTY]");
                }
            }
            else if (dialogBox.ExtendedDialogTemplate != null)
            {
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.Version, $"{padding}Version");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.Signature, $"{padding}Signature");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.HelpID, $"{padding}Help ID");
                builder.AppendLine($"{padding}Extended style: {dialogBox.ExtendedDialogTemplate.ExtendedStyle} (0x{dialogBox.ExtendedDialogTemplate.ExtendedStyle:X})");
                builder.AppendLine($"{padding}Style: {dialogBox.ExtendedDialogTemplate.Style} (0x{dialogBox.ExtendedDialogTemplate.Style:X})");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.DialogItems, $"{padding}Item count");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.PositionX, $"{padding}X-coordinate of upper-left corner");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.PositionY, $"{padding}Y-coordinate of upper-left corner");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.WidthX, $"{padding}Width of the dialog box");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.HeightY, $"{padding}Height of the dialog box");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.MenuResource, $"{padding}Menu resource");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.MenuResourceOrdinal, $"{padding}Menu resource ordinal");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.ClassResource, $"{padding}Class resource");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.ClassResourceOrdinal, $"{padding}Class resource ordinal");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.TitleResource, $"{padding}Title resource");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.PointSize, $"{padding}Point size");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.Weight, $"{padding}Weight");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.Italic, $"{padding}Italic");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.CharSet, $"{padding}Character set");
                builder.AppendLine(dialogBox.ExtendedDialogTemplate.Typeface, $"{padding}Typeface");
                builder.AppendLine();
                builder.AppendLine($"{padding}Dialog item templates");
                builder.AppendLine($"{padding}-------------------------");
                if (dialogBox.ExtendedDialogTemplate.DialogItems == 0
                    || dialogBox.ExtendedDialogItemTemplates == null
                    || dialogBox.ExtendedDialogItemTemplates.Length == 0)
                {
                    builder.AppendLine($"{padding}No dialog item templates");
                    return;
                }

                for (int i = 0; i < dialogBox.ExtendedDialogItemTemplates.Length; i++)
                {
                    var dialogItemTemplate = dialogBox.ExtendedDialogItemTemplates[i];
                    builder.AppendLine($"{padding}Dialog item template {i}");
                    if (dialogItemTemplate == null)
                    {
                        builder.AppendLine($"{padding}  [NULL]");
                        continue;
                    }

                    builder.AppendLine(dialogItemTemplate.HelpID, $"{padding}  Help ID");
                    builder.AppendLine($"{padding}  Extended style: {dialogItemTemplate.ExtendedStyle} (0x{dialogItemTemplate.ExtendedStyle:X})");
                    builder.AppendLine($"{padding}  Style: {dialogItemTemplate.Style} (0x{dialogItemTemplate.Style:X})");
                    builder.AppendLine(dialogItemTemplate.PositionX, $"{padding}  X-coordinate of upper-left corner");
                    builder.AppendLine(dialogItemTemplate.PositionY, $"{padding}  Y-coordinate of upper-left corner");
                    builder.AppendLine(dialogItemTemplate.WidthX, $"{padding}  Width of the control");
                    builder.AppendLine(dialogItemTemplate.HeightY, $"{padding}  Height of the control");
                    builder.AppendLine(dialogItemTemplate.ID, $"{padding}  ID");
                    builder.AppendLine(dialogItemTemplate.ClassResource, $"{padding}  Class resource");
                    builder.AppendLine($"{padding}  Class resource ordinal: {dialogItemTemplate.ClassResourceOrdinal} (0x{dialogItemTemplate.ClassResourceOrdinal:X})");
                    builder.AppendLine(dialogItemTemplate.TitleResource, $"{padding}  Title resource");
                    builder.AppendLine(dialogItemTemplate.TitleResourceOrdinal, $"{padding}  Title resource ordinal");
                    builder.AppendLine(dialogItemTemplate.CreationDataSize, $"{padding}  Creation data size");
                    if (dialogItemTemplate.CreationData != null && dialogItemTemplate.CreationData.Length != 0)
                        builder.AppendLine(dialogItemTemplate.CreationData, $"{padding}  Creation data");
                    else
                        builder.AppendLine($"{padding}  Creation data: [EMPTY]");
                }
            }
            else
            {
                builder.AppendLine($"{padding}Dialog box resource found, but malformed");
            }
        }

        private static void PrintResourceRT_STRING(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

#if NET48
            Dictionary<int, string> stringTable = null;
#else
            Dictionary<int, string?>? stringTable = null;
#endif
            try { stringTable = entry.AsStringTable(); } catch { }
            if (stringTable == null)
            {
                builder.AppendLine($"{padding}String table resource found, but malformed");
                return;
            }

            foreach (var kvp in stringTable)
            {
                int index = kvp.Key;
#if NET48
                string stringValue = kvp.Value;
#else
                string? stringValue = kvp.Value;
#endif
                builder.AppendLine(stringValue, $"{padding}String entry {index}");
            }
        }

        private static void PrintResourceRT_FONTDIR(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Font directory resource found, not parsed yet");
        }

        private static void PrintResourceRT_FONT(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Font resource found, not parsed yet");
        }

        private static void PrintResourceRT_ACCELERATOR(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

#if NET48
            AcceleratorTableEntry[] acceleratorTable = null;
#else
            AcceleratorTableEntry[]? acceleratorTable = null;
#endif
            try { acceleratorTable = entry.AsAcceleratorTableResource(); } catch { }
            if (acceleratorTable == null)
            {
                builder.AppendLine($"{padding}Accelerator table resource found, but malformed");
                return;
            }

            for (int i = 0; i < acceleratorTable.Length; i++)
            {
                var acceleratorTableEntry = acceleratorTable[i];
                builder.AppendLine($"{padding}Accelerator Table Entry {i}:");
                builder.AppendLine($"{padding}  Flags: {acceleratorTableEntry.Flags} (0x{acceleratorTableEntry.Flags:X})");
                builder.AppendLine(acceleratorTableEntry.Ansi, $"{padding}  Ansi");
                builder.AppendLine(acceleratorTableEntry.Id, $"{padding}  Id");
                builder.AppendLine(acceleratorTableEntry.Padding, $"{padding}  Padding");
            }
        }

        private static void PrintResourceRT_RCDATA(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Application-defined resource found, not parsed yet");

            // Then print the data, if needed
            if (entry.Data == null)
            {
                builder.AppendLine($"{padding}Data: [NULL] (This may indicate a very large resource)");
            }
            else
            {
                int offset = 0;
#if NET48
                byte[] magic = entry.Data.ReadBytes(ref offset, Math.Min(entry.Data.Length, 16));
#else
                byte[]? magic = entry.Data.ReadBytes(ref offset, Math.Min(entry.Data.Length, 16));
#endif

                if (magic == null)
                {
                    // No-op
                }
                else if (magic[0] == 0x4D && magic[1] == 0x5A)
                {
                    builder.AppendLine($"{padding}Data: [Embedded Executable File]"); // TODO: Parse this out and print separately
                }
                else if (magic[0] == 0x4D && magic[1] == 0x53 && magic[2] == 0x46 && magic[3] == 0x54)
                {
                    builder.AppendLine($"{padding}Data: [Embedded OLE Library File]"); // TODO: Parse this out and print separately
                }
                else
                {
                    builder.AppendLine(magic, $"{padding}Data");

                    //if (entry.Data != null)
                    //    builder.AppendLine(entry.Data, $"{padding}Value (Byte Data)");
                    //if (entry.Data != null)
                    //    builder.AppendLine(Encoding.ASCII.GetString(entry.Data), $"{padding}Value (ASCII)");
                    //if (entry.Data != null)
                    //    builder.AppendLine(Encoding.UTF8.GetString(entry.Data), $"{padding}Value (UTF-8)");
                    //if (entry.Data != null)
                    //    builder.AppendLine(Encoding.Unicode.GetString(entry.Data), $"{padding}Value (Unicode)");
                }
            }
        }

        private static void PrintResourceRT_MESSAGETABLE(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

#if NET48
            MessageResourceData messageTable = null;
#else
            MessageResourceData? messageTable = null;
#endif
            try { messageTable = entry.AsMessageResourceData(); } catch { }
            if (messageTable == null)
            {
                builder.AppendLine($"{padding}Message resource data found, but malformed");
                return;
            }

            builder.AppendLine(messageTable.NumberOfBlocks, $"{padding}Number of blocks");
            builder.AppendLine();
            builder.AppendLine($"{padding}Message resource blocks");
            builder.AppendLine($"{padding}-------------------------");
            if (messageTable.NumberOfBlocks == 0
                || messageTable.Blocks == null
                || messageTable.Blocks.Length == 0)
            {
                builder.AppendLine($"{padding}No message resource blocks");
            }
            else
            {
                for (int i = 0; i < messageTable.Blocks.Length; i++)
                {
                    var messageResourceBlock = messageTable.Blocks[i];
                    builder.AppendLine($"{padding}Message resource block {i}");
                    if (messageResourceBlock == null)
                    {
                        builder.AppendLine($"{padding}  [NULL]");
                        continue;
                    }

                    builder.AppendLine(messageResourceBlock.LowId, $"{padding}  Low ID");
                    builder.AppendLine(messageResourceBlock.HighId, $"{padding}  High ID");
                    builder.AppendLine(messageResourceBlock.OffsetToEntries, $"{padding}  Offset to entries");
                }
            }
            builder.AppendLine();

            builder.AppendLine($"{padding}Message resource entries");
            builder.AppendLine($"{padding}-------------------------");
            if (messageTable.Entries == null || messageTable.Entries.Count == 0)
            {
                builder.AppendLine($"{padding}No message resource entries");
            }
            else
            {
                foreach (var kvp in messageTable.Entries)
                {
                    uint index = kvp.Key;
                    var messageResourceEntry = kvp.Value;
                    builder.AppendLine($"{padding}Message resource entry {index}");
                    if (messageResourceEntry == null)
                    {
                        builder.AppendLine($"{padding}  [NULL]");
                        continue;
                    }

                    builder.AppendLine(messageResourceEntry.Length, $"{padding}  Length");
                    builder.AppendLine(messageResourceEntry.Flags, $"{padding}  Flags");
                    builder.AppendLine(messageResourceEntry.Text, $"{padding}  Text");
                }
            }
        }

        private static void PrintResourceRT_GROUP_CURSOR(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Hardware-independent cursor resource found, not parsed yet");
        }

        private static void PrintResourceRT_GROUP_ICON(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Hardware-independent icon resource found, not parsed yet");
        }

        private static void PrintResourceRT_VERSION(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

#if NET48
            VersionInfo versionInfo = null;
#else
            VersionInfo? versionInfo = null;
#endif
            try { versionInfo = entry.AsVersionInfo(); } catch { }
            if (versionInfo == null)
            {
                builder.AppendLine($"{padding}Version info resource found, but malformed");
                return;
            }

            builder.AppendLine(versionInfo.Length, $"{padding}Length");
            builder.AppendLine(versionInfo.ValueLength, $"{padding}Value length");
            builder.AppendLine($"{padding}Resource type: {versionInfo.ResourceType} (0x{versionInfo.ResourceType:X})");
            builder.AppendLine(versionInfo.Key, $"{padding}Key");
            if (versionInfo.ValueLength != 0 && versionInfo.Value != null)
            {
                builder.AppendLine(versionInfo.Value.Signature, $"{padding}[Fixed File Info] Signature");
                builder.AppendLine(versionInfo.Value.StrucVersion, $"{padding}[Fixed File Info] Struct version");
                builder.AppendLine(versionInfo.Value.FileVersionMS, $"{padding}[Fixed File Info] File version (MS)");
                builder.AppendLine(versionInfo.Value.FileVersionLS, $"{padding}[Fixed File Info] File version (LS)");
                builder.AppendLine(versionInfo.Value.ProductVersionMS, $"{padding}[Fixed File Info] Product version (MS)");
                builder.AppendLine(versionInfo.Value.ProductVersionLS, $"{padding}[Fixed File Info] Product version (LS)");
                builder.AppendLine(versionInfo.Value.FileFlagsMask, $"{padding}[Fixed File Info] File flags mask");
                builder.AppendLine($"{padding}[Fixed File Info] File flags: {versionInfo.Value.FileFlags} (0x{versionInfo.Value.FileFlags:X})");
                builder.AppendLine($"{padding}[Fixed File Info] File OS: {versionInfo.Value.FileOS} (0x{versionInfo.Value.FileOS:X})");
                builder.AppendLine($"{padding}[Fixed File Info] Type: {versionInfo.Value.FileType} (0x{versionInfo.Value.FileType:X})");
                builder.AppendLine($"{padding}[Fixed File Info] Subtype: {versionInfo.Value.FileSubtype} (0x{versionInfo.Value.FileSubtype:X})");
                builder.AppendLine(versionInfo.Value.FileDateMS, $"{padding}[Fixed File Info] File date (MS)");
                builder.AppendLine(versionInfo.Value.FileDateLS, $"{padding}[Fixed File Info] File date (LS)");
            }

            if (versionInfo.StringFileInfo != null)
            {
                builder.AppendLine(versionInfo.StringFileInfo.Length, $"{padding}[String File Info] Length");
                builder.AppendLine(versionInfo.StringFileInfo.ValueLength, $"{padding}[String File Info] Value length");
                builder.AppendLine($"{padding}[String File Info] Resource type: {versionInfo.StringFileInfo.ResourceType} (0x{versionInfo.StringFileInfo.ResourceType:X})");
                builder.AppendLine(versionInfo.StringFileInfo.Key, $"{padding}[String File Info] Key");
                builder.AppendLine($"{padding}Children:");
                builder.AppendLine($"{padding}-------------------------");
                if (versionInfo.StringFileInfo.Children == null || versionInfo.StringFileInfo.Children.Length == 0)
                {
                    builder.AppendLine($"{padding}No string file info children");
                }
                else
                {
                    for (int i = 0; i < versionInfo.StringFileInfo.Children.Length; i++)
                    {
                        var stringFileInfoChildEntry = versionInfo.StringFileInfo.Children[i];
                        if (stringFileInfoChildEntry == null)
                        {
                            builder.AppendLine($"{padding}  [String Table {i}] [NULL]");
                            continue;
                        }

                        builder.AppendLine(stringFileInfoChildEntry.Length, $"{padding}  [String Table {i}] Length");
                        builder.AppendLine(stringFileInfoChildEntry.ValueLength, $"{padding}  [String Table {i}] Value length");
                        builder.AppendLine($"{padding}  [String Table {i}] ResourceType: {stringFileInfoChildEntry.ResourceType} (0x{stringFileInfoChildEntry.ResourceType:X})");
                        builder.AppendLine(stringFileInfoChildEntry.Key, $"{padding}  [String Table {i}] Key");
                        builder.AppendLine($"{padding}  [String Table {i}] Children:");
                        builder.AppendLine($"{padding}  -------------------------");
                        if (stringFileInfoChildEntry.Children == null || stringFileInfoChildEntry.Children.Length == 0)
                        {
                            builder.AppendLine($"{padding}  No string table {i} children");
                        }
                        else
                        {
                            for (int j = 0; j < stringFileInfoChildEntry.Children.Length; j++)
                            {
                                var stringDataEntry = stringFileInfoChildEntry.Children[j];
                                if (stringDataEntry == null)
                                {
                                    builder.AppendLine($"{padding}    [String Data {j}] [NULL]");
                                    continue;
                                }

                                builder.AppendLine(stringDataEntry.Length, $"{padding}    [String Data {j}] Length");
                                builder.AppendLine(stringDataEntry.ValueLength, $"{padding}    [String Data {j}] Value length");
                                builder.AppendLine($"{padding}    [String Data {j}] ResourceType: {stringDataEntry.ResourceType} (0x{stringDataEntry.ResourceType:X})");
                                builder.AppendLine(stringDataEntry.Key, $"{padding}    [String Data {j}] Key");
                                builder.AppendLine(stringDataEntry.Value, $"{padding}    [String Data {j}] Value");
                            }
                        }
                    }
                }
            }

            if (versionInfo.VarFileInfo != null)
            {
                builder.AppendLine(versionInfo.VarFileInfo.Length, $"{padding}[Var File Info] Length");
                builder.AppendLine(versionInfo.VarFileInfo.ValueLength, $"{padding}[Var File Info] Value length");
                builder.AppendLine($"{padding}[Var File Info] Resource type: {versionInfo.VarFileInfo.ResourceType} (0x{versionInfo.VarFileInfo.ResourceType:X})");
                builder.AppendLine(versionInfo.VarFileInfo.Key, $"{padding}[Var File Info] Key");
                builder.AppendLine($"{padding}Children:");
                builder.AppendLine($"{padding}-------------------------");
                if (versionInfo.VarFileInfo.Children == null || versionInfo.VarFileInfo.Children.Length == 0)
                {
                    builder.AppendLine($"{padding}No var file info children");
                }
                else
                {
                    for (int i = 0; i < versionInfo.VarFileInfo.Children.Length; i++)
                    {
                        var varFileInfoChildEntry = versionInfo.VarFileInfo.Children[i];
                        if (varFileInfoChildEntry == null)
                        {
                            builder.AppendLine($"{padding}  [String Table {i}] [NULL]");
                            continue;
                        }

                        builder.AppendLine(varFileInfoChildEntry.Length, $"{padding}  [String Table {i}] Length");
                        builder.AppendLine(varFileInfoChildEntry.ValueLength, $"{padding}  [String Table {i}] Value length");
                        builder.AppendLine($"{padding}  [String Table {i}] ResourceType: {varFileInfoChildEntry.ResourceType} (0x{varFileInfoChildEntry.ResourceType:X})");
                        builder.AppendLine(varFileInfoChildEntry.Key, $"{padding}  [String Table {i}] Key");
                        builder.AppendLine(varFileInfoChildEntry.Value, $"{padding}  [String Table {i}] Value");
                    }
                }
            }
        }

        private static void PrintResourceRT_DLGINCLUDE(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}External header resource found, not parsed yet");
        }

        private static void PrintResourceRT_PLUGPLAY(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Plug and Play resource found, not parsed yet");
        }

        private static void PrintResourceRT_VXD(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}VXD found, not parsed yet");
        }

        private static void PrintResourceRT_ANICURSOR(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Animated cursor found, not parsed yet");
        }

        private static void PrintResourceRT_ANIICON(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}Animated icon found, not parsed yet");
        }

        private static void PrintResourceRT_HTML(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);
            builder.AppendLine($"{padding}HTML resource found, not parsed yet");

            //if (entry.Data != null)
            //    builder.AppendLine(Encoding.ASCII.GetString(entry.Data), $"{padding}Value (ASCII)");
            //if (entry.Data != null)
            //    builder.AppendLine(Encoding.UTF8.GetString(entry.Data), $"{padding}Value (UTF-8)");
            //if (entry.Data != null)
            //    builder.AppendLine(Encoding.Unicode.GetString(entry.Data), $"{padding}Value (Unicode)");
        }

        private static void PrintResourceRT_MANIFEST(ResourceDataEntry entry, int level, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

#if NET48
            AssemblyManifest assemblyManifest = null;
#else
            AssemblyManifest? assemblyManifest = null;
#endif
            try { assemblyManifest = entry.AsAssemblyManifest(); } catch { }
            if (assemblyManifest == null)
            {
                builder.AppendLine($"{padding}Assembly manifest found, but malformed");
                return;
            }

            builder.AppendLine(assemblyManifest.ManifestVersion, $"{padding}Manifest version");
            if (assemblyManifest.AssemblyIdentities != null && assemblyManifest.AssemblyIdentities.Length > 0)
            {
                for (int i = 0; i < assemblyManifest.AssemblyIdentities.Length; i++)
                {
                    var assemblyIdentity = assemblyManifest.AssemblyIdentities[i];
                    if (assemblyIdentity == null)
                    {
                        builder.AppendLine($"{padding}  [Assembly Identity {i}] [NULL]");
                        continue;
                    }

                    builder.AppendLine(assemblyIdentity.Name, $"{padding}[Assembly Identity {i}] Name");
                    builder.AppendLine(assemblyIdentity.Version, $"{padding}[Assembly Identity {i}] Version");
                    builder.AppendLine(assemblyIdentity.Type, $"{padding}[Assembly Identity {i}] Type");
                    builder.AppendLine(assemblyIdentity.ProcessorArchitecture, $"{padding}[Assembly Identity {i}] Processor architecture");
                    builder.AppendLine(assemblyIdentity.PublicKeyToken, $"{padding}[Assembly Identity {i}] Public key token");
                    builder.AppendLine(assemblyIdentity.Language, $"{padding}[Assembly Identity {i}] Language");
                }
            }

            if (assemblyManifest.Description != null)
                builder.AppendLine(assemblyManifest.Description.Value, $"{padding}[Assembly Description] Value");

            if (assemblyManifest.COMInterfaceExternalProxyStub != null && assemblyManifest.COMInterfaceExternalProxyStub.Length > 0)
            {
                for (int i = 0; i < assemblyManifest.COMInterfaceExternalProxyStub.Length; i++)
                {
                    var comInterfaceExternalProxyStub = assemblyManifest.COMInterfaceExternalProxyStub[i];
                    if (comInterfaceExternalProxyStub == null)
                    {
                        builder.AppendLine($"{padding}  [COM Interface External Proxy Stub {i}] [NULL]");
                        continue;
                    }

                    builder.AppendLine(comInterfaceExternalProxyStub.IID, $"{padding}[COM Interface External Proxy Stub {i}] IID");
                    builder.AppendLine(comInterfaceExternalProxyStub.Name, $"{padding}[COM Interface External Proxy Stub {i}] Name");
                    builder.AppendLine(comInterfaceExternalProxyStub.TLBID, $"{padding}[COM Interface External Proxy Stub {i}] TLBID");
                    builder.AppendLine(comInterfaceExternalProxyStub.NumMethods, $"{padding}[COM Interface External Proxy Stub {i}] Number of methods");
                    builder.AppendLine(comInterfaceExternalProxyStub.ProxyStubClsid32, $"{padding}[COM Interface External Proxy Stub {i}] Proxy stub (CLSID32)");
                    builder.AppendLine(comInterfaceExternalProxyStub.BaseInterface, $"{padding}[COM Interface External Proxy Stub {i}] Base interface");
                }
            }

            if (assemblyManifest.Dependency != null && assemblyManifest.Dependency.Length > 0)
            {
                for (int i = 0; i < assemblyManifest.Dependency.Length; i++)
                {
                    var dependency = assemblyManifest.Dependency[i];
                    if (dependency?.DependentAssembly != null)
                    {
                        if (dependency.DependentAssembly.AssemblyIdentity != null)
                        {
                            builder.AppendLine(dependency.DependentAssembly.AssemblyIdentity.Name, $"{padding}[Dependency {i} Assembly Identity] Name");
                            builder.AppendLine(dependency.DependentAssembly.AssemblyIdentity.Version, $"{padding}[Dependency {i} Assembly Identity] Version");
                            builder.AppendLine(dependency.DependentAssembly.AssemblyIdentity.Type, $"{padding}[Dependency {i} Assembly Identity] Type");
                            builder.AppendLine(dependency.DependentAssembly.AssemblyIdentity.ProcessorArchitecture, $"{padding}[Dependency {i} Assembly Identity] Processor architecture");
                            builder.AppendLine(dependency.DependentAssembly.AssemblyIdentity.PublicKeyToken, $"{padding}[Dependency {i} Assembly Identity] Public key token");
                            builder.AppendLine(dependency.DependentAssembly.AssemblyIdentity.Language, $"{padding}[Dependency {i} Assembly Identity] Language");
                        }
                        if (dependency.DependentAssembly.BindingRedirect != null && dependency.DependentAssembly.BindingRedirect.Length > 0)
                        {
                            for (int j = 0; j < dependency.DependentAssembly.BindingRedirect.Length; j++)
                            {
                                var bindingRedirect = dependency.DependentAssembly.BindingRedirect[j];
                                if (bindingRedirect == null)
                                {
                                    builder.AppendLine($"{padding}[Dependency {i} Binding Redirect {j}] [NULL]");
                                    continue;
                                }

                                builder.AppendLine(bindingRedirect.OldVersion, $"{padding}[Dependency {i} Binding Redirect {j}] Old version");
                                builder.AppendLine(bindingRedirect.NewVersion, $"{padding}[Dependency {i} Binding Redirect {j}] New version");
                            }
                        }
                    }

                    if (dependency != null)
                        builder.AppendLine(dependency.Optional, $"{padding}[Dependency {i}] Optional");
                }
            }

            if (assemblyManifest.File != null && assemblyManifest.File.Length > 0)
            {
                for (int i = 0; i < assemblyManifest.File.Length; i++)
                {
                    var file = assemblyManifest.File[i];
                    if (file == null)
                    {
                        builder.AppendLine($"{padding}[File {i}] [NULL]");
                        continue;
                    }

                    builder.AppendLine(file.Name, $"{padding}[File {i}] Name");
                    builder.AppendLine(file.Hash, $"{padding}[File {i}] Hash");
                    builder.AppendLine(file.HashAlgorithm, $"{padding}[File {i}] Hash algorithm");
                    builder.AppendLine(file.Size, $"{padding}[File {i}] Size");

                    if (file.COMClass != null && file.COMClass.Length > 0)
                    {
                        for (int j = 0; j < file.COMClass.Length; j++)
                        {
                            var comClass = file.COMClass[j];
                            if (comClass == null)
                            {
                                builder.AppendLine($"{padding}[File {i} COM Class {j}] [NULL]");
                                continue;
                            }

                            builder.AppendLine(comClass.CLSID, $"{padding}[File {i} COM Class {j}] CLSID");
                            builder.AppendLine(comClass.ThreadingModel, $"{padding}[File {i} COM Class {j}] Threading model");
                            builder.AppendLine(comClass.ProgID, $"{padding}[File {i} COM Class {j}] Prog ID");
                            builder.AppendLine(comClass.TLBID, $"{padding}[File {i} COM Class {j}] TLBID");
                            builder.AppendLine(comClass.Description, $"{padding}[File {i} COM Class {j}] Description");

                            if (comClass.ProgIDs != null && comClass.ProgIDs.Length > 0)
                            {
                                for (int k = 0; k < comClass.ProgIDs.Length; k++)
                                {
                                    var progId = comClass.ProgIDs[k];
                                    if (progId == null)
                                    {
                                        builder.AppendLine($"{padding}[File {i} COM Class {j} Prog ID {k}] [NULL]");
                                        continue;
                                    }

                                    builder.AppendLine(progId.Value, $"{padding}[File {i} COM Class {j} Prog ID {k}] Value");
                                }
                            }
                        }
                    }

                    if (file.COMInterfaceProxyStub != null && file.COMInterfaceProxyStub.Length > 0)
                    {
                        for (int j = 0; j < file.COMInterfaceProxyStub.Length; j++)
                        {
                            var comInterfaceProxyStub = file.COMInterfaceProxyStub[j];
                            if (comInterfaceProxyStub == null)
                            {
                                builder.AppendLine($"{padding}[File {i} COM Interface Proxy Stub {j}] [NULL]");
                                continue;
                            }

                            builder.AppendLine(comInterfaceProxyStub.IID, $"{padding}[File {i} COM Interface Proxy Stub {j}] IID");
                            builder.AppendLine(comInterfaceProxyStub.Name, $"{padding}[File {i} COM Interface Proxy Stub {j}] Name");
                            builder.AppendLine(comInterfaceProxyStub.TLBID, $"{padding}[File {i} COM Interface Proxy Stub {j}] TLBID");
                            builder.AppendLine(comInterfaceProxyStub.NumMethods, $"{padding}[File {i} COM Interface Proxy Stub {j}] Number of methods");
                            builder.AppendLine(comInterfaceProxyStub.ProxyStubClsid32, $"{padding}[File {i} COM Interface Proxy Stub {j}] Proxy stub (CLSID32)");
                            builder.AppendLine(comInterfaceProxyStub.BaseInterface, $"{padding}[File {i} COM Interface Proxy Stub {j}] Base interface");
                        }
                    }

                    if (file.Typelib != null && file.Typelib.Length > 0)
                    {
                        for (int j = 0; j < file.Typelib.Length; j++)
                        {
                            var typeLib = file.Typelib[j];
                            if (typeLib == null)
                            {
                                builder.AppendLine($"{padding}[File {i} Type Lib {j}] [NULL]");
                                continue;
                            }

                            builder.AppendLine(typeLib.TLBID, $"{padding}[File {i} Type Lib {j}] TLBID");
                            builder.AppendLine(typeLib.Version, $"{padding}[File {i} Type Lib {j}] Version");
                            builder.AppendLine(typeLib.HelpDir, $"{padding}[File {i} Type Lib {j}] Help directory");
                            builder.AppendLine(typeLib.ResourceID, $"{padding}[File {i} Type Lib {j}] Resource ID");
                            builder.AppendLine(typeLib.Flags, $"{padding}[File {i} Type Lib {j}] Flags");
                        }
                    }

                    if (file.WindowClass != null && file.WindowClass.Length > 0)
                    {
                        for (int j = 0; j < file.WindowClass.Length; j++)
                        {
                            var windowClass = file.WindowClass[j];
                            if (windowClass == null)
                            {
                                builder.AppendLine($"{padding}[File {i} Window Class {j}] [NULL]");
                                continue;
                            }

                            builder.AppendLine(windowClass.Versioned, $"{padding}[File {i} Window Class {j}] Versioned");
                            builder.AppendLine(windowClass.Value, $"{padding}[File {i} Window Class {j}] Value");
                        }
                    }
                }
            }

            if (assemblyManifest.EverythingElse != null && assemblyManifest.EverythingElse.Length > 0)
            {
                for (int i = 0; i < assemblyManifest.EverythingElse.Length; i++)
                {
                    var thing = assemblyManifest.EverythingElse[i];
                    if (thing is XmlElement element)
                    {
                        builder.AppendLine(element.OuterXml, $"{padding}Unparsed XML Element {i}");
                    }
                    else
                    {
                        builder.AppendLine($"{padding}Unparsed Item {i}: {thing ?? "[NULL]"}");
                    }
                }
            }
        }

        private static void PrintResourceUNKNOWN(ResourceDataEntry entry, int level, object resourceType, StringBuilder builder)
        {
            string padding = new string(' ', (level + 1) * 2);

            // Print the type first
            if (resourceType is uint numericType)
                builder.AppendLine($"{padding}Type {(ResourceType)numericType} found, not parsed yet");
            else if (resourceType is string stringType)
                builder.AppendLine($"{padding}Type {stringType} found, not parsed yet");
            else
                builder.AppendLine($"{padding}Unknown type {resourceType} found, not parsed yet");

            // Then print the data, if needed
            if (entry.Data == null)
            {
                builder.AppendLine($"{padding}Data: [NULL] (This may indicate a very large resource)");
            }
            else
            {
                int offset = 0;
#if NET48
                byte[] magic = entry.Data.ReadBytes(ref offset, Math.Min(entry.Data.Length, 16));
#else
                byte[]? magic = entry.Data.ReadBytes(ref offset, Math.Min(entry.Data.Length, 16));
#endif

                if (magic == null)
                {
                    // No-op
                }
                else if (magic[0] == 0x4D && magic[1] == 0x5A)
                {
                    builder.AppendLine($"{padding}Data: [Embedded Executable File]"); // TODO: Parse this out and print separately
                }
                else if (magic[0] == 0x4D && magic[1] == 0x53 && magic[2] == 0x46 && magic[3] == 0x54)
                {
                    builder.AppendLine($"{padding}Data: [Embedded OLE Library File]"); // TODO: Parse this out and print separately
                }
                else
                {
                    builder.AppendLine(magic, $"{padding}Data");

                    //if (entry.Data != null)
                    //    builder.AppendLine(entry.Data, $"{padding}Value (Byte Data)");
                    //if (entry.Data != null)
                    //    builder.AppendLine(Encoding.ASCII.GetString(entry.Data), $"{padding}Value (ASCII)");
                    //if (entry.Data != null)
                    //    builder.AppendLine(Encoding.UTF8.GetString(entry.Data), $"{padding}Value (UTF-8)");
                    //if (entry.Data != null)
                    //    builder.AppendLine(Encoding.Unicode.GetString(entry.Data), $"{padding}Value (Unicode)");
                }
            }
        }
    }
}