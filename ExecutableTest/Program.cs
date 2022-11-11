using System;
using System.Text;
using System.Xml;
using BurnOutSharp.Builder;

namespace ExecutableTest
{
    public class ExecutableTest
    {
        public static void Main(string[] args)
        {
            // Invalid arguments means nothing to do
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please provide at least one file path");
                Console.ReadLine();
                return;
            }

            // Register the codepages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Loop through the args
            foreach (string arg in args)
            {
                Console.WriteLine($"Checking possible path: {arg}");

                // Check the file exists
                if (!File.Exists(arg))
                {
                    Console.WriteLine($"{arg} does not exist or is not a file, skipping...");
                    continue;
                }

                using (Stream stream = File.OpenRead(arg))
                {
                    // Read the first 4 bytes
                    byte[] magic = stream.ReadBytes(2);
                    if (magic[0] != 'M' || magic[1] != 'Z')
                    {
                        Console.WriteLine("Not a recognized executable format, skipping...");
                        Console.WriteLine();
                        continue;
                    }

                    // Build the executable information
                    Console.WriteLine("Creating MS-DOS executable builder");
                    Console.WriteLine();

                    stream.Seek(0, SeekOrigin.Begin);
                    var msdos = MSDOS.ParseExecutable(stream);
                    if (msdos == null)
                    {
                        Console.WriteLine("Something went wrong parsing MS-DOS executable");
                        Console.WriteLine();
                        continue;
                    }

                    // Print the executable info to screen
                    PrintMSDOS(msdos);

                    // Check for a valid new executable address
                    if (msdos.Header.NewExeHeaderAddr >= stream.Length)
                    {
                        Console.WriteLine("New EXE header address invalid, skipping additional reading...");
                        Console.WriteLine();
                        continue;
                    }

                    // Try to read the executable info
                    stream.Seek(msdos.Header.NewExeHeaderAddr, SeekOrigin.Begin);
                    magic = stream.ReadBytes(4);

                    // New Executable
                    if (magic[0] == 'N' && magic[1] == 'E')
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        var newExecutable = NewExecutable.ParseExecutable(stream);
                        if (newExecutable == null)
                        {
                            Console.WriteLine("Something went wrong parsing New Executable");
                            Console.WriteLine();
                            continue;
                        }

                        // Print the executable info to screen
                        PrintNewExecutable(newExecutable);
                    }
                    
                    // Linear Executable
                    else if (magic[0] == 'L' && (magic[1] == 'E' || magic[1] == 'X'))
                    {
                        Console.WriteLine($"Linear executable found. No parsing currently available.");
                        Console.WriteLine();
                        continue;
                    }

                    // Portable Executable
                    else if (magic[0] == 'P' && magic[1] == 'E' && magic[2] == '\0' && magic[3] == '\0')
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        var portableExecutable = PortableExecutable.ParseExecutable(stream);
                        if (portableExecutable == null)
                        {
                            Console.WriteLine("Something went wrong parsing Portable Executable");
                            Console.WriteLine();
                            continue;
                        }

                        // Print the executable info to screen
                        PrintPortableExecutable(portableExecutable);
                    }

                    // Unknown
                    else
                    {
                        Console.WriteLine($"Unrecognized header signature: {BitConverter.ToString(magic).Replace("-", string.Empty)}");
                        Console.WriteLine();
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Pretty print the MS-DOS executable information
        /// </summary>
        private static void PrintMSDOS(BurnOutSharp.Models.MSDOS.Executable executable)
        {
            Console.WriteLine("MS-DOS Executable Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Magic number: {BitConverter.ToString(executable.Header.Magic).Replace("-", string.Empty)}");
            Console.WriteLine($"  Last page bytes: {executable.Header.LastPageBytes}");
            Console.WriteLine($"  Pages: {executable.Header.Pages}");
            Console.WriteLine($"  Relocation items: {executable.Header.RelocationItems}");
            Console.WriteLine($"  Header paragraph size: {executable.Header.HeaderParagraphSize}");
            Console.WriteLine($"  Minimum extra paragraphs: {executable.Header.MinimumExtraParagraphs}");
            Console.WriteLine($"  Maximum extra paragraphs: {executable.Header.MaximumExtraParagraphs}");
            Console.WriteLine($"  Initial SS value: {executable.Header.InitialSSValue}");
            Console.WriteLine($"  Initial SP value: {executable.Header.InitialSPValue}");
            Console.WriteLine($"  Checksum: {executable.Header.Checksum}");
            Console.WriteLine($"  Initial IP value: {executable.Header.InitialIPValue}");
            Console.WriteLine($"  Initial CS value: {executable.Header.InitialCSValue}");
            Console.WriteLine($"  Relocation table address: {executable.Header.RelocationTableAddr}");
            Console.WriteLine($"  Overlay number: {executable.Header.OverlayNumber}");
            Console.WriteLine();

            Console.WriteLine("  Extended Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Reserved words: {string.Join(", ", executable.Header.Reserved1)}");
            Console.WriteLine($"  OEM identifier: {executable.Header.OEMIdentifier}");
            Console.WriteLine($"  OEM information: {executable.Header.OEMInformation}");
            Console.WriteLine($"  Reserved words: {string.Join(", ", executable.Header.Reserved2)}");
            Console.WriteLine($"  New EXE header address: {executable.Header.NewExeHeaderAddr}");
            Console.WriteLine();

            Console.WriteLine("  Relocation Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.Header.RelocationItems == 0 || executable.RelocationTable.Length == 0)
            {
                Console.WriteLine("  No relocation table items");
            }
            else
            {
                for (int i = 0; i < executable.RelocationTable.Length; i++)
                {
                    var entry = executable.RelocationTable[i];
                    Console.WriteLine($"  Relocation Table Entry {i}");
                    Console.WriteLine($"    Offset = {entry.Offset}");
                    Console.WriteLine($"    Segment = {entry.Segment}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Pretty print the New Executable information
        /// </summary>
        private static void PrintNewExecutable(BurnOutSharp.Models.NewExecutable.Executable executable)
        {
            Console.WriteLine("New Executable Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            Console.WriteLine("  MS-DOS Stub Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine("  See 'MS-DOS Executable Information' for details");
            Console.WriteLine();

            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Magic number: {BitConverter.ToString(executable.Header.Magic).Replace("-", string.Empty)}");
            Console.WriteLine($"  Linker version: {executable.Header.LinkerVersion}");
            Console.WriteLine($"  Linker revision: {executable.Header.LinkerRevision}");
            Console.WriteLine($"  Entry table offset: {executable.Header.EntryTableOffset}");
            Console.WriteLine($"  Entry table size: {executable.Header.EntryTableSize}");
            Console.WriteLine($"  CRC checksum: {executable.Header.CrcChecksum}");
            Console.WriteLine($"  Flag word: {executable.Header.FlagWord}");
            Console.WriteLine($"  Automatic data segment number: {executable.Header.AutomaticDataSegmentNumber}");
            Console.WriteLine($"  Initial heap allocation: {executable.Header.InitialHeapAlloc}");
            Console.WriteLine($"  Initial stack allocation: {executable.Header.InitialStackAlloc}");
            Console.WriteLine($"  Initial CS:IP setting: {executable.Header.InitialCSIPSetting}");
            Console.WriteLine($"  Initial SS:SP setting: {executable.Header.InitialSSSPSetting}");
            Console.WriteLine($"  File segment count: {executable.Header.FileSegmentCount}");
            Console.WriteLine($"  Module reference table size: {executable.Header.ModuleReferenceTableSize}");
            Console.WriteLine($"  Non-resident name table size: {executable.Header.NonResidentNameTableSize}");
            Console.WriteLine($"  Segment table offset: {executable.Header.SegmentTableOffset}");
            Console.WriteLine($"  Resource table offset: {executable.Header.ResourceTableOffset}");
            Console.WriteLine($"  Resident name table offset: {executable.Header.ResidentNameTableOffset}");
            Console.WriteLine($"  Module reference table offset: {executable.Header.ModuleReferenceTableOffset}");
            Console.WriteLine($"  Imported names table offset: {executable.Header.ImportedNamesTableOffset}");
            Console.WriteLine($"  Non-resident name table offset: {executable.Header.NonResidentNamesTableOffset}");
            Console.WriteLine($"  Moveable entries count: {executable.Header.MovableEntriesCount}");
            Console.WriteLine($"  Segment alignment shift count: {executable.Header.SegmentAlignmentShiftCount}");
            Console.WriteLine($"  Resource entries count: {executable.Header.ResourceEntriesCount}");
            Console.WriteLine($"  Target operating system: {executable.Header.TargetOperatingSystem}");
            Console.WriteLine($"  Additional flags: {executable.Header.AdditionalFlags}");
            Console.WriteLine($"  Return thunk offset: {executable.Header.ReturnThunkOffset}");
            Console.WriteLine($"  Segment reference thunk offset: {executable.Header.SegmentReferenceThunkOffset}");
            Console.WriteLine($"  Minimum code swap area size: {executable.Header.MinCodeSwapAreaSize}");
            Console.WriteLine($"  Windows SDK revision: {executable.Header.WindowsSDKRevision}");
            Console.WriteLine($"  Windows SDK version: {executable.Header.WindowsSDKVersion}");
            Console.WriteLine();

            Console.WriteLine("  Segment Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.Header.FileSegmentCount == 0 || executable.SegmentTable.Length == 0)
            {
                Console.WriteLine("  No segment table items");
            }
            else
            {
                for (int i = 0; i < executable.SegmentTable.Length; i++)
                {
                    var entry = executable.SegmentTable[i];
                    Console.WriteLine($"  Segment Table Entry {i}");
                    Console.WriteLine($"    Offset = {entry.Offset}");
                    Console.WriteLine($"    Length = {entry.Length}");
                    Console.WriteLine($"    Flag word = {entry.FlagWord}");
                    Console.WriteLine($"    Minimum allocation size = {entry.MinimumAllocationSize}");
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Resource Table Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Alignment shift count: {executable.ResourceTable.AlignmentShiftCount}");
            if (executable.Header.ResourceEntriesCount == 0 || executable.ResourceTable.ResourceTypes.Length == 0)
            {
                Console.WriteLine("  No resource table items");
            }
            else
            {
                for (int i = 0; i < executable.ResourceTable.ResourceTypes.Length; i++)
                {
                    // TODO: If not integer type, print out name
                    var entry = executable.ResourceTable.ResourceTypes[i];
                    Console.WriteLine($"  Resource Table Entry {i}");
                    Console.WriteLine($"    Type ID = {entry.TypeID} (Is Integer Type: {entry.IsIntegerType()})");
                    Console.WriteLine($"    Resource count = {entry.ResourceCount}");
                    Console.WriteLine($"    Reserved = {entry.Reserved}");
                    Console.WriteLine($"    Resources = ");
                    if (entry.ResourceCount == 0 || entry.Resources.Length == 0)
                    {
                        Console.WriteLine("      No resource items");
                    }
                    else
                    {
                        for (int j = 0; j < entry.Resources.Length; j++)
                        {
                            // TODO: If not integer type, print out name
                            var resource = entry.Resources[j];
                            Console.WriteLine($"      Resource Entry {i}");
                            Console.WriteLine($"        Offset = {resource.Offset}");
                            Console.WriteLine($"        Length = {resource.Length}");
                            Console.WriteLine($"        Flag word = {resource.FlagWord}");
                            Console.WriteLine($"        Resource ID = {resource.ResourceID} (Is Integer Type: {resource.IsIntegerType()})");
                            Console.WriteLine($"        Reserved = {resource.Reserved}");
                        }
                    }
                }
            }


            if (executable.ResourceTable.TypeAndNameStrings.Count == 0)
            {
                Console.WriteLine("  No resource table type/name strings");
            }
            else
            {
                foreach (var typeAndNameString in executable.ResourceTable.TypeAndNameStrings)
                {
                    Console.WriteLine($"  Resource Type/Name Offset {typeAndNameString.Key}");
                    Console.WriteLine($"    Length = {typeAndNameString.Value.Length}");
                    Console.WriteLine($"    Text = {Encoding.ASCII.GetString(typeAndNameString.Value.Text)}");
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Resident-Name Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.Header.ResidentNameTableOffset == 0 || executable.ResidentNameTable.Length == 0)
            {
                Console.WriteLine("  No resident-name table items");
            }
            else
            {
                for (int i = 0; i < executable.ResidentNameTable.Length; i++)
                {
                    var entry = executable.ResidentNameTable[i];
                    Console.WriteLine($"  Resident-Name Table Entry {i}");
                    Console.WriteLine($"    Length = {entry.Length}");
                    Console.WriteLine($"    Name string = {Encoding.ASCII.GetString(entry.NameString)}");
                    Console.WriteLine($"    Ordinal number = {entry.OrdinalNumber}");
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Module-Reference Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.Header.ModuleReferenceTableSize == 0 || executable.ModuleReferenceTable.Length == 0)
            {
                Console.WriteLine("  No module-reference table items");
            }
            else
            {
                for (int i = 0; i < executable.ModuleReferenceTable.Length; i++)
                {
                    // TODO: Read the imported names table and print value here
                    var entry = executable.ModuleReferenceTable[i];
                    Console.WriteLine($"  Module-Reference Table Entry {i}");
                    Console.WriteLine($"    Offset = {entry.Offset} (adjusted to be {entry.Offset + executable.Stub.Header.NewExeHeaderAddr + executable.Header.ImportedNamesTableOffset})");
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Imported-Name Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.Header.ImportedNamesTableOffset == 0 || executable.ImportedNameTable.Count == 0)
            {
                Console.WriteLine("  No imported-name table items");
            }
            else
            {
                foreach (var entry in executable.ImportedNameTable)
                {
                    Console.WriteLine($"  Imported-Name Table at Offset {entry.Key}");
                    Console.WriteLine($"    Length = {entry.Value.Length}");
                    Console.WriteLine($"    Name string = {Encoding.ASCII.GetString(entry.Value.NameString)}");
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Entry Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.Header.EntryTableSize == 0 || executable.EntryTable.Length == 0)
            {
                Console.WriteLine("  No entry table items");
            }
            else
            {
                for (int i = 0; i < executable.EntryTable.Length; i++)
                {
                    var entry = executable.EntryTable[i];
                    Console.WriteLine($"  Entry Table Entry {i}");
                    Console.WriteLine($"    Entry count = {entry.EntryCount}");
                    Console.WriteLine($"    Segment indicator = {entry.SegmentIndicator} ({entry.GetEntryType()})");
                    switch (entry.GetEntryType())
                    {
                        case BurnOutSharp.Models.NewExecutable.SegmentEntryType.FixedSegment:
                            Console.WriteLine($"    Flag word = {entry.FixedFlagWord}");
                            Console.WriteLine($"    Offset = {entry.FixedOffset}");
                            break;
                        case BurnOutSharp.Models.NewExecutable.SegmentEntryType.MoveableSegment:
                            Console.WriteLine($"    Flag word = {entry.MoveableFlagWord}");
                            Console.WriteLine($"    Reserved = {entry.MoveableReserved}");
                            Console.WriteLine($"    Segment number = {entry.MoveableSegmentNumber}");
                            Console.WriteLine($"    Offset = {entry.MoveableOffset}");
                            break;
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Nonresident-Name Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.Header.NonResidentNameTableSize == 0 || executable.NonResidentNameTable.Length == 0)
            {
                Console.WriteLine("  No nonresident-name table items");
            }
            else
            {
                for (int i = 0; i < executable.NonResidentNameTable.Length; i++)
                {
                    var entry = executable.NonResidentNameTable[i];
                    Console.WriteLine($"  Nonresident-Name Table Entry {i}");
                    Console.WriteLine($"    Length = {entry.Length}");
                    Console.WriteLine($"    Name string = {Encoding.ASCII.GetString(entry.NameString)}");
                    Console.WriteLine($"    Ordinal number = {entry.OrdinalNumber}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Pretty print the Portable Executable information
        /// </summary>
        private static void PrintPortableExecutable(BurnOutSharp.Models.PortableExecutable.Executable executable)
        {
            Console.WriteLine("Portable Executable Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            Console.WriteLine("  MS-DOS Stub Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine("  See 'MS-DOS Executable Information' for details");
            Console.WriteLine();

            Console.WriteLine("  COFF File Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Signature: {BitConverter.ToString(executable.Signature).Replace("-", string.Empty)}");
            Console.WriteLine($"  Machine: {executable.COFFFileHeader.Machine}");
            Console.WriteLine($"  Number of sections: {executable.COFFFileHeader.NumberOfSections}");
            Console.WriteLine($"  Time/Date stamp: {executable.COFFFileHeader.TimeDateStamp}");
            Console.WriteLine($"  Pointer to symbol table: {executable.COFFFileHeader.PointerToSymbolTable}");
            Console.WriteLine($"  Number of symbols: {executable.COFFFileHeader.NumberOfSymbols}");
            Console.WriteLine($"  Size of optional header: {executable.COFFFileHeader.SizeOfOptionalHeader}");
            Console.WriteLine($"  Characteristics: {executable.COFFFileHeader.Characteristics}");
            Console.WriteLine();

            Console.WriteLine("  Optional Header Information:");
            Console.WriteLine("  -------------------------");
            if (executable.COFFFileHeader.SizeOfOptionalHeader == 0 || executable.OptionalHeader == null)
            {
                Console.WriteLine("  No optional header present");
            }
            else
            {
                Console.WriteLine($"  Magic: {executable.OptionalHeader.Magic}");
                Console.WriteLine($"  Major linker version: {executable.OptionalHeader.MajorLinkerVersion}");
                Console.WriteLine($"  Minor linker version: {executable.OptionalHeader.MinorLinkerVersion}");
                Console.WriteLine($"  Size of code section: {executable.OptionalHeader.SizeOfCode}");
                Console.WriteLine($"  Size of initialized data: {executable.OptionalHeader.SizeOfInitializedData}");
                Console.WriteLine($"  Size of uninitialized data: {executable.OptionalHeader.SizeOfUninitializedData}");
                Console.WriteLine($"  Address of entry point: {executable.OptionalHeader.AddressOfEntryPoint}");
                Console.WriteLine($"  Base of code: {executable.OptionalHeader.BaseOfCode}");
                if (executable.OptionalHeader.Magic == BurnOutSharp.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Base of data: {executable.OptionalHeader.BaseOfData}");

                if (executable.OptionalHeader.Magic == BurnOutSharp.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Image base: {executable.OptionalHeader.ImageBase_PE32}");
                else if (executable.OptionalHeader.Magic == BurnOutSharp.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32Plus)
                    Console.WriteLine($"  Image base: {executable.OptionalHeader.ImageBase_PE32Plus}");
                Console.WriteLine($"  Section alignment: {executable.OptionalHeader.SectionAlignment}");
                Console.WriteLine($"  File alignment: {executable.OptionalHeader.FileAlignment}");
                Console.WriteLine($"  Major operating system version: {executable.OptionalHeader.MajorOperatingSystemVersion}");
                Console.WriteLine($"  Minor operating system version: {executable.OptionalHeader.MinorOperatingSystemVersion}");
                Console.WriteLine($"  Major image version: {executable.OptionalHeader.MajorImageVersion}");
                Console.WriteLine($"  Minor image version: {executable.OptionalHeader.MinorImageVersion}");
                Console.WriteLine($"  Major subsystem version: {executable.OptionalHeader.MajorSubsystemVersion}");
                Console.WriteLine($"  Minor subsystem version: {executable.OptionalHeader.MinorSubsystemVersion}");
                Console.WriteLine($"  Win32 version value: {executable.OptionalHeader.Win32VersionValue}");
                Console.WriteLine($"  Size of image: {executable.OptionalHeader.SizeOfImage}");
                Console.WriteLine($"  Size of headers: {executable.OptionalHeader.SizeOfHeaders}");
                Console.WriteLine($"  Checksum: {executable.OptionalHeader.CheckSum}");
                Console.WriteLine($"  Subsystem: {executable.OptionalHeader.Subsystem}");
                Console.WriteLine($"  DLL characteristics: {executable.OptionalHeader.DllCharacteristics}");
                if (executable.OptionalHeader.Magic == BurnOutSharp.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Size of stack reserve: {executable.OptionalHeader.SizeOfStackReserve_PE32}");
                else if (executable.OptionalHeader.Magic == BurnOutSharp.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32Plus)
                    Console.WriteLine($"  Size of stack reserve: {executable.OptionalHeader.SizeOfStackReserve_PE32Plus}");
                if (executable.OptionalHeader.Magic == BurnOutSharp.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Size of stack commit: {executable.OptionalHeader.SizeOfStackCommit_PE32}");
                else if (executable.OptionalHeader.Magic == BurnOutSharp.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32Plus)
                    Console.WriteLine($"  Size of stack commit: {executable.OptionalHeader.SizeOfStackCommit_PE32Plus}");
                if (executable.OptionalHeader.Magic == BurnOutSharp.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Size of heap reserve: {executable.OptionalHeader.SizeOfHeapReserve_PE32}");
                else if (executable.OptionalHeader.Magic == BurnOutSharp.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32Plus)
                    Console.WriteLine($"  Size of heap reserve: {executable.OptionalHeader.SizeOfHeapReserve_PE32Plus}");
                if (executable.OptionalHeader.Magic == BurnOutSharp.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                    Console.WriteLine($"  Size of heap commit: {executable.OptionalHeader.SizeOfHeapCommit_PE32}");
                else if (executable.OptionalHeader.Magic == BurnOutSharp.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32Plus)
                    Console.WriteLine($"  Size of heap commit: {executable.OptionalHeader.SizeOfHeapCommit_PE32Plus}");
                Console.WriteLine($"  Loader flags: {executable.OptionalHeader.LoaderFlags}");
                Console.WriteLine($"  Number of data-directory entries: {executable.OptionalHeader.NumberOfRvaAndSizes}");
            
                if (executable.OptionalHeader.ExportTable != null)
                {
                    Console.WriteLine("    Export Table (1)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.ExportTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.ExportTable.Size}");
                }
                if (executable.OptionalHeader.ImportTable != null)
                {
                    Console.WriteLine("    Import Table (2)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.ImportTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.ImportTable.Size}");
                }
                if (executable.OptionalHeader.ResourceTable != null)
                {
                    Console.WriteLine("    Resource Table (3)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.ResourceTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.ResourceTable.Size}");
                }
                if (executable.OptionalHeader.ExceptionTable != null)
                {
                    Console.WriteLine("    Exception Table (4)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.ExceptionTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.ExceptionTable.Size}");
                }
                if (executable.OptionalHeader.CertificateTable != null)
                {
                    Console.WriteLine("    Certificate Table (5)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.CertificateTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.CertificateTable.Size}");
                }
                if (executable.OptionalHeader.BaseRelocationTable != null)
                {
                    Console.WriteLine("    Base Relocation Table (6)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.BaseRelocationTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.BaseRelocationTable.Size}");
                }
                if (executable.OptionalHeader.Debug != null)
                {
                    Console.WriteLine("    Debug Table (7)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.Debug.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.Debug.Size}");
                }
                if (executable.OptionalHeader.NumberOfRvaAndSizes >= 8)
                {
                    Console.WriteLine("    Architecture Table (8)");
                    Console.WriteLine($"      Virtual address: 0");
                    Console.WriteLine($"      Size: 0");
                }
                if (executable.OptionalHeader.GlobalPtr != null)
                {
                    Console.WriteLine("    Global Pointer Register (9)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.GlobalPtr.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.GlobalPtr.Size}");
                }
                if (executable.OptionalHeader.ThreadLocalStorageTable != null)
                {
                    Console.WriteLine("    Thread Local Storage (TLS) Table (10)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.ThreadLocalStorageTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.ThreadLocalStorageTable.Size}");
                }
                if (executable.OptionalHeader.LoadConfigTable != null)
                {
                    Console.WriteLine("    Load Config Table (11)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.LoadConfigTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.LoadConfigTable.Size}");
                }
                if (executable.OptionalHeader.BoundImport != null)
                {
                    Console.WriteLine("    Bound Import Table (12)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.BoundImport.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.BoundImport.Size}");
                }
                if (executable.OptionalHeader.ImportAddressTable != null)
                {
                    Console.WriteLine("    Import Address Table (13)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.ImportAddressTable.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.ImportAddressTable.Size}");
                }
                if (executable.OptionalHeader.DelayImportDescriptor != null)
                {
                    Console.WriteLine("    Delay Import Descriptior (14)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.DelayImportDescriptor.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.DelayImportDescriptor.Size}");
                }
                if (executable.OptionalHeader.CLRRuntimeHeader != null)
                {
                    Console.WriteLine("    CLR Runtime Header (15)");
                    Console.WriteLine($"      Virtual address: {executable.OptionalHeader.CLRRuntimeHeader.VirtualAddress}");
                    Console.WriteLine($"      Size: {executable.OptionalHeader.CLRRuntimeHeader.Size}");
                }
                if (executable.OptionalHeader.NumberOfRvaAndSizes >= 16)
                {
                    Console.WriteLine("    Reserved (16)");
                    Console.WriteLine($"      Virtual address: 0");
                    Console.WriteLine($"      Size: 0");
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Section Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.COFFFileHeader.NumberOfSections == 0 || executable.SectionTable.Length == 0)
            {
                Console.WriteLine("  No section table items");
            }
            else
            {
                for (int i = 0; i < executable.SectionTable.Length; i++)
                {
                    var entry = executable.SectionTable[i];
                    Console.WriteLine($"  Section Table Entry {i}");
                    Console.WriteLine($"    Name = {Encoding.UTF8.GetString(entry.Name)}");
                    Console.WriteLine($"    Virtual size = {entry.VirtualSize}");
                    Console.WriteLine($"    Virtual address = {entry.VirtualAddress}");
                    Console.WriteLine($"    Size of raw data = {entry.SizeOfRawData}");
                    Console.WriteLine($"    Pointer to raw data = {entry.PointerToRawData}");
                    Console.WriteLine($"    Pointer to relocations = {entry.PointerToRelocations}");
                    Console.WriteLine($"    Pointer to linenumbers = {entry.PointerToLinenumbers}");
                    Console.WriteLine($"    Number of relocations = {entry.NumberOfRelocations}");
                    Console.WriteLine($"    Number of linenumbers = {entry.NumberOfLinenumbers}");
                    Console.WriteLine($"    Characteristics = {entry.Characteristics}");
                    // TODO: Add COFFRelocations
                    // TODO: Add COFFLineNumbers
                }
            }
            Console.WriteLine();

            Console.WriteLine("  COFF Symbol Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.COFFFileHeader.PointerToSymbolTable == 0
                || executable.COFFFileHeader.NumberOfSymbols == 0
                || executable.COFFSymbolTable.Length == 0)
            {
                Console.WriteLine("  No COFF symbol table items");
            }
            else
            {
                int auxSymbolsRemaining = 0;
                int currentSymbolType = 0;

                for (int i = 0; i < executable.COFFSymbolTable.Length; i++)
                {
                    var entry = executable.COFFSymbolTable[i];
                    Console.WriteLine($"  COFF Symbol Table Entry {i} (Subtype {currentSymbolType})");
                    if (currentSymbolType == 0)
                    {
                        if (entry.ShortName != null)
                        {
                            Console.WriteLine($"    Short name = {Encoding.UTF8.GetString(entry.ShortName)}");
                        }
                        else
                        {
                            Console.WriteLine($"    Zeroes = {entry.Zeroes}");
                            Console.WriteLine($"    Offset = {entry.Offset}");
                        }
                        Console.WriteLine($"    Value = {entry.Value}");
                        Console.WriteLine($"    Section number = {entry.SectionNumber}");
                        Console.WriteLine($"    Symbol type = {entry.SymbolType}");
                        Console.WriteLine($"    Storage class = {entry.StorageClass}");
                        Console.WriteLine($"    Number of aux symbols = {entry.NumberOfAuxSymbols}");

                        auxSymbolsRemaining = entry.NumberOfAuxSymbols;
                        if (auxSymbolsRemaining == 0)
                            continue;

                        if (entry.StorageClass == BurnOutSharp.Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                        && entry.SymbolType == BurnOutSharp.Models.PortableExecutable.SymbolType.IMAGE_SYM_TYPE_FUNC
                        && entry.SectionNumber > 0)
                        {
                            currentSymbolType = 1;
                        }
                        else if (entry.StorageClass == BurnOutSharp.Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_FUNCTION
                            && entry.ShortName != null
                            && ((entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x62 && entry.ShortName[2] == 0x66)  // .bf
                                || (entry.ShortName[0] == 0x2E && entry.ShortName[1] == 0x65 && entry.ShortName[2] == 0x66))) // .ef
                        {
                            currentSymbolType = 2;
                        }
                        else if (entry.StorageClass == BurnOutSharp.Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_EXTERNAL
                            && entry.SectionNumber == (ushort)BurnOutSharp.Models.PortableExecutable.SectionNumber.IMAGE_SYM_UNDEFINED
                            && entry.Value == 0)
                        {
                            currentSymbolType = 3;
                        }
                        else if (entry.StorageClass == BurnOutSharp.Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_FILE)
                        {
                            // TODO: Symbol name should be ".file"
                            currentSymbolType = 4;
                        }
                        else if (entry.StorageClass == BurnOutSharp.Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_STATIC)
                        {
                            // TODO: Should have the name of a section (like ".text")
                            currentSymbolType = 5;
                        }
                        else if (entry.StorageClass == BurnOutSharp.Models.PortableExecutable.StorageClass.IMAGE_SYM_CLASS_CLR_TOKEN)
                        {
                            currentSymbolType = 6;
                        }
                    }
                    else if (currentSymbolType == 1)
                    {
                        Console.WriteLine($"    Tag index = {entry.AuxFormat1TagIndex}");
                        Console.WriteLine($"    Total size = {entry.AuxFormat1TotalSize}");
                        Console.WriteLine($"    Pointer to linenumber = {entry.AuxFormat1PointerToLinenumber}");
                        Console.WriteLine($"    Pointer to next function = {entry.AuxFormat1PointerToNextFunction}");
                        Console.WriteLine($"    Unused = {entry.AuxFormat1Unused}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 2)
                    {
                        Console.WriteLine($"    Unused = {entry.AuxFormat2Unused1}");
                        Console.WriteLine($"    Linenumber = {entry.AuxFormat2Linenumber}");
                        Console.WriteLine($"    Unused = {entry.AuxFormat2Unused2}");
                        Console.WriteLine($"    Pointer to next function = {entry.AuxFormat2PointerToNextFunction}");
                        Console.WriteLine($"    Unused = {entry.AuxFormat2Unused3}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 3)
                    {
                        Console.WriteLine($"    Tag index = {entry.AuxFormat3TagIndex}");
                        Console.WriteLine($"    Characteristics = {entry.AuxFormat3Characteristics}");
                        Console.WriteLine($"    Unused = {BitConverter.ToString(entry.AuxFormat3Unused).Replace("-", string.Empty)}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 4)
                    {
                        Console.WriteLine($"    File name = {Encoding.ASCII.GetString(entry.AuxFormat4FileName)}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 5)
                    {
                        Console.WriteLine($"    Length = {entry.AuxFormat5Length}");
                        Console.WriteLine($"    Number of relocations = {entry.AuxFormat5NumberOfRelocations}");
                        Console.WriteLine($"    Number of linenumbers = {entry.AuxFormat5NumberOfLinenumbers}");
                        Console.WriteLine($"    Checksum = {entry.AuxFormat5CheckSum}");
                        Console.WriteLine($"    Number = {entry.AuxFormat5Number}");
                        Console.WriteLine($"    Selection = {entry.AuxFormat5Selection}");
                        Console.WriteLine($"    Unused = {BitConverter.ToString(entry.AuxFormat5Unused).Replace("-", string.Empty)}");
                        auxSymbolsRemaining--;
                    }
                    else if (currentSymbolType == 6)
                    {
                        Console.WriteLine($"    Aux type = {entry.AuxFormat6AuxType}");
                        Console.WriteLine($"    Reserved = {entry.AuxFormat6Reserved1}");
                        Console.WriteLine($"    Symbol table index = {entry.AuxFormat6SymbolTableIndex}");
                        Console.WriteLine($"    Reserved = {BitConverter.ToString(entry.AuxFormat6Reserved2).Replace("-", string.Empty)}");
                        auxSymbolsRemaining--;
                    }

                    // If we hit the last aux symbol, go back to normal format
                    if (auxSymbolsRemaining == 0)
                        currentSymbolType = 0;
                }

                Console.WriteLine();
                Console.WriteLine("  COFF String Table Information:");
                Console.WriteLine("  -------------------------");
                if (executable.COFFStringTable == null
                    || executable.COFFStringTable.Strings == null
                    || executable.COFFStringTable.Strings.Length == 0)
                {
                    Console.WriteLine("  No COFF string table items");
                }
                else
                {
                    Console.WriteLine($"  Total size: {executable.COFFStringTable.TotalSize}");
                    for (int i = 0; i < executable.COFFStringTable.Strings.Length; i++)
                    {
                        string entry = executable.COFFStringTable.Strings[i];
                        Console.WriteLine($"  COFF String Table Entry {i})");
                        Console.WriteLine($"    Value = {entry}");
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Attribute Certificate Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.OptionalHeader?.CertificateTable == null
                || executable.OptionalHeader.CertificateTable.VirtualAddress == 0
                || executable.AttributeCertificateTable.Length == 0)
            {
                Console.WriteLine("  No attribute certificate table items");
            }
            else
            {
                for (int i = 0; i < executable.AttributeCertificateTable.Length; i++)
                {
                    var entry = executable.AttributeCertificateTable[i];
                    Console.WriteLine($"  Attribute Certificate Table Entry {i}");
                    Console.WriteLine($"    Length = {entry.Length}");
                    Console.WriteLine($"    Revision = {entry.Revision}");
                    Console.WriteLine($"    Certificate type = {entry.CertificateType}");
                    Console.WriteLine($"    Certificate = {BitConverter.ToString(entry.Certificate).Replace("-", string.Empty)}");
                    // TODO: Add certificate type parsing
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Delay-Load Directory Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.OptionalHeader?.DelayImportDescriptor == null
                || executable.OptionalHeader.DelayImportDescriptor.VirtualAddress == 0
                || executable.DelayLoadDirectoryTable == null)
            {
                Console.WriteLine("  No delay-load directory table items");
            }
            else
            {
                Console.WriteLine($"  Attributes = {executable.DelayLoadDirectoryTable.Attributes}");
                Console.WriteLine($"  Name RVA = {executable.DelayLoadDirectoryTable.Name}");
                Console.WriteLine($"  Module handle = {executable.DelayLoadDirectoryTable.ModuleHandle}");
                Console.WriteLine($"  Delay import address table RVA = {executable.DelayLoadDirectoryTable.DelayImportAddressTable}");
                Console.WriteLine($"  Delay import name table RVA = {executable.DelayLoadDirectoryTable.DelayImportNameTable}");
                Console.WriteLine($"  Bound delay import table RVA = {executable.DelayLoadDirectoryTable.BoundDelayImportTable}");
                Console.WriteLine($"  Unload delay import table RVA = {executable.DelayLoadDirectoryTable.UnloadDelayImportTable}");
                Console.WriteLine($"  Timestamp = {executable.DelayLoadDirectoryTable.TimeStamp}");
            }
            Console.WriteLine();

            Console.WriteLine("  Debug Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.OptionalHeader?.Debug == null
                || executable.OptionalHeader.Debug.VirtualAddress == 0
                || executable.DebugTable == null)
            {
                Console.WriteLine("  No debug table items");
            }
            else
            {
                // TODO: If more sections added, model this after the Export Table
                for (int i = 0; i < executable.DebugTable.DebugDirectoryTable.Length; i++)
                {
                    var debugDirectoryEntry = executable.DebugTable.DebugDirectoryTable[i];
                    Console.WriteLine($"  Debug Directory Table Entry {i}");
                    Console.WriteLine($"    Characteristics: {debugDirectoryEntry.Characteristics}");
                    Console.WriteLine($"    Time/Date stamp: {debugDirectoryEntry.TimeDateStamp}");
                    Console.WriteLine($"    Major version: {debugDirectoryEntry.MajorVersion}");
                    Console.WriteLine($"    Minor version: {debugDirectoryEntry.MinorVersion}");
                    Console.WriteLine($"    Debug type: {debugDirectoryEntry.DebugType}");
                    Console.WriteLine($"    Size of data: {debugDirectoryEntry.SizeOfData}");
                    Console.WriteLine($"    Address of raw data: {debugDirectoryEntry.AddressOfRawData}");
                    Console.WriteLine($"    Pointer to raw data: {debugDirectoryEntry.PointerToRawData}");
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Export Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.OptionalHeader?.ExportTable == null
                || executable.OptionalHeader.ExportTable.VirtualAddress == 0
                || executable.ExportTable == null)
            {
                Console.WriteLine("  No export table items");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("    Export Directory Table Information:");
                Console.WriteLine("    -------------------------");
                Console.WriteLine($"    Export flags: {executable.ExportTable.ExportDirectoryTable.ExportFlags}");
                Console.WriteLine($"    Time/Date stamp: {executable.ExportTable.ExportDirectoryTable.TimeDateStamp}");
                Console.WriteLine($"    Major version: {executable.ExportTable.ExportDirectoryTable.MajorVersion}");
                Console.WriteLine($"    Minor version: {executable.ExportTable.ExportDirectoryTable.MinorVersion}");
                Console.WriteLine($"    Name RVA: {executable.ExportTable.ExportDirectoryTable.NameRVA}");
                Console.WriteLine($"    Name: {executable.ExportTable.ExportDirectoryTable.Name}");
                Console.WriteLine($"    Ordinal base: {executable.ExportTable.ExportDirectoryTable.OrdinalBase}");
                Console.WriteLine($"    Address table entries: {executable.ExportTable.ExportDirectoryTable.AddressTableEntries}");
                Console.WriteLine($"    Number of name pointers: {executable.ExportTable.ExportDirectoryTable.NumberOfNamePointers}");
                Console.WriteLine($"    Export address table RVA: {executable.ExportTable.ExportDirectoryTable.ExportAddressTableRVA}");
                Console.WriteLine($"    Name pointer table RVA: {executable.ExportTable.ExportDirectoryTable.NamePointerRVA}");
                Console.WriteLine($"    Ordinal table RVA: {executable.ExportTable.ExportDirectoryTable.OrdinalTableRVA}");
                Console.WriteLine();

                Console.WriteLine("    Export Address Table Information:");
                Console.WriteLine("    -------------------------");
                if (executable.ExportTable.ExportAddressTable == null || executable.ExportTable.ExportAddressTable.Length == 0)
                {
                    Console.WriteLine("    No export address table items");
                }
                else
                {
                    for (int i = 0; i < executable.ExportTable.ExportAddressTable.Length; i++)
                    {
                        var exportAddressTableEntry = executable.ExportTable.ExportAddressTable[i];
                        Console.WriteLine($"    Export Address Table Entry {i}");
                        Console.WriteLine($"      Export RVA / Forwarder RVA: {exportAddressTableEntry.ExportRVA}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Name Pointer Table Information:");
                Console.WriteLine("    -------------------------");
                if (executable.ExportTable.NamePointerTable?.Pointers == null || executable.ExportTable.NamePointerTable.Pointers.Length == 0)
                {
                    Console.WriteLine("    No name pointer table items");
                }
                else
                {
                    for (int i = 0; i < executable.ExportTable.NamePointerTable.Pointers.Length; i++)
                    {
                        var namePointerTableEntry = executable.ExportTable.NamePointerTable.Pointers[i];
                        Console.WriteLine($"    Name Pointer Table Entry {i}");
                        Console.WriteLine($"      Pointer: {namePointerTableEntry}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Ordinal Table Information:");
                Console.WriteLine("    -------------------------");
                if (executable.ExportTable.OrdinalTable?.Indexes == null || executable.ExportTable.OrdinalTable.Indexes.Length == 0)
                {
                    Console.WriteLine("    No ordinal table items");
                }
                else
                {
                    for (int i = 0; i < executable.ExportTable.OrdinalTable.Indexes.Length; i++)
                    {
                        var ordinalTableEntry = executable.ExportTable.OrdinalTable.Indexes[i];
                        Console.WriteLine($"    Ordinal Table Entry {i}");
                        Console.WriteLine($"      Index: {ordinalTableEntry}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Export Name Table Information:");
                Console.WriteLine("    -------------------------");
                if (executable.ExportTable.ExportNameTable?.Strings == null || executable.ExportTable.ExportNameTable.Strings.Length == 0)
                {
                    Console.WriteLine("    No export name table items");
                }
                else
                {
                    for (int i = 0; i < executable.ExportTable.ExportNameTable.Strings.Length; i++)
                    {
                        var exportNameTableEntry = executable.ExportTable.ExportNameTable.Strings[i];
                        Console.WriteLine($"    Export Name Table Entry {i}");
                        Console.WriteLine($"      String: {exportNameTableEntry}");
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Import Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.OptionalHeader?.ImportTable == null
                || executable.OptionalHeader.ImportTable.VirtualAddress == 0
                || executable.ImportTable == null)
            {
                Console.WriteLine("  No import table items");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("    Import Directory Table Information:");
                Console.WriteLine("    -------------------------");
                if (executable.ImportTable.ImportDirectoryTable == null || executable.ImportTable.ImportDirectoryTable.Length == 0)
                {
                    Console.WriteLine("    No import directory table items");
                }
                else
                {
                    for (int i = 0; i < executable.ImportTable.ImportDirectoryTable.Length; i++)
                    {
                        var importDirectoryTableEntry = executable.ImportTable.ImportDirectoryTable[i];
                        Console.WriteLine($"    Import Directory Table Entry {i}");
                        Console.WriteLine($"      Import lookup table RVA: {importDirectoryTableEntry.ImportLookupTableRVA}");
                        Console.WriteLine($"      Time/Date stamp: {importDirectoryTableEntry.TimeDateStamp}");
                        Console.WriteLine($"      Forwarder chain: {importDirectoryTableEntry.ForwarderChain}");
                        Console.WriteLine($"      Name RVA: {importDirectoryTableEntry.NameRVA}");
                        Console.WriteLine($"      Name: {importDirectoryTableEntry.Name}");
                        Console.WriteLine($"      Import address table RVA: {importDirectoryTableEntry.ImportAddressTableRVA}");
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Import Lookup Tables Information:");
                Console.WriteLine("    -------------------------");
                if (executable.ImportTable.ImportLookupTables == null || executable.ImportTable.ImportLookupTables.Count == 0)
                {
                    Console.WriteLine("    No import lookup tables");
                }
                else
                {
                    foreach (var kvp in executable.ImportTable.ImportLookupTables)
                    {
                        int index = kvp.Key;
                        var importLookupTable = kvp.Value;

                        Console.WriteLine();
                        Console.WriteLine($"      Import Lookup Table {index} Information:");
                        Console.WriteLine("      -------------------------");
                        if (importLookupTable == null || importLookupTable.Length == 0)
                        {
                            Console.WriteLine("      No import lookup table items");
                        }
                        else
                        {
                            for (int i = 0; i < importLookupTable.Length; i++)
                            {
                                var importLookupTableEntry = importLookupTable[i];
                                Console.WriteLine($"      Import Lookup Table {index} Entry {i}");
                                Console.WriteLine($"        Ordinal/Name flag: {importLookupTableEntry.OrdinalNameFlag}");
                                if (importLookupTableEntry.OrdinalNameFlag)
                                    Console.WriteLine($"        Ordinal number: {importLookupTableEntry.OrdinalNumber}");
                                else
                                    Console.WriteLine($"        Hint/Name table RVA: {importLookupTableEntry.HintNameTableRVA}");
                            }
                        }
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Import Address Tables Information:");
                Console.WriteLine("    -------------------------");
                if (executable.ImportTable.ImportAddressTables == null || executable.ImportTable.ImportAddressTables.Count == 0)
                {
                    Console.WriteLine("    No import address tables");
                }
                else
                {
                    foreach (var kvp in executable.ImportTable.ImportAddressTables)
                    {
                        int index = kvp.Key;
                        var importAddressTable = kvp.Value;

                        Console.WriteLine();
                        Console.WriteLine($"      Import Address Table {index} Information:");
                        Console.WriteLine("      -------------------------");
                        if (importAddressTable == null || importAddressTable.Length == 0)
                        {
                            Console.WriteLine("      No import address table items");
                        }
                        else
                        {
                            for (int i = 0; i < importAddressTable.Length; i++)
                            {
                                var importLookupTableEntry = importAddressTable[i];
                                Console.WriteLine($"      Import Address Table {index} Entry {i}");
                                if (executable.OptionalHeader.Magic == BurnOutSharp.Models.PortableExecutable.OptionalHeaderMagicNumber.PE32)
                                    Console.WriteLine($"        Address: {importLookupTableEntry.Address_PE32}");
                                else
                                    Console.WriteLine($"        Address: {importLookupTableEntry.Address_PE32Plus}");
                            }
                        }
                    }
                }
                Console.WriteLine();

                Console.WriteLine("    Hint/Name Table Information:");
                Console.WriteLine("    -------------------------");
                if (executable.ImportTable.HintNameTable == null || executable.ImportTable.HintNameTable.Length == 0)
                {
                    Console.WriteLine("    No hint/name table items");
                }
                else
                {
                    for (int i = 0; i < executable.ImportTable.HintNameTable.Length; i++)
                    {
                        var hintNameTableEntry = executable.ImportTable.HintNameTable[i];
                        Console.WriteLine($"    Hint/Name Table Entry {i}");
                        Console.WriteLine($"      Hint: {hintNameTableEntry.Hint}");
                        Console.WriteLine($"      Name: {hintNameTableEntry.Name}");
                    }
                }
            }
            Console.WriteLine();

            Console.WriteLine("  Resource Directory Table Information:");
            Console.WriteLine("  -------------------------");
            if (executable.OptionalHeader?.ResourceTable == null
                || executable.OptionalHeader.ResourceTable.VirtualAddress == 0
                || executable.ResourceDirectoryTable == null)
            {
                Console.WriteLine("  No resource directory table items");
            }
            else
            {
                PrintPortableExecutableResourceDirectoryTable(executable.ResourceDirectoryTable, level: 0, types: new List<object>());
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Pretty print the Portable Executable resource directory table information
        /// </summary>
        private static void PrintPortableExecutableResourceDirectoryTable(BurnOutSharp.Models.PortableExecutable.ResourceDirectoryTable table, int level, List<object> types)
        {
            string padding = new string(' ', (level + 1) * 2);

            Console.WriteLine($"{padding}Table level: {level}");
            Console.WriteLine($"{padding}Characteristics: {table.Characteristics}");
            Console.WriteLine($"{padding}Time/Date stamp: {table.TimeDateStamp}");
            Console.WriteLine($"{padding}Major version: {table.MajorVersion}");
            Console.WriteLine($"{padding}Minor version: {table.MinorVersion}");
            Console.WriteLine($"{padding}Number of name entries: {table.NumberOfNameEntries}");
            Console.WriteLine($"{padding}Number of ID entries: {table.NumberOfIDEntries}");
            Console.WriteLine();

            Console.WriteLine($"{padding}Name entries");
            Console.WriteLine($"{padding}-------------------------");
            if (table.NumberOfNameEntries == 0)
            {
                Console.WriteLine($"{padding}No named entries");
                Console.WriteLine();
            }
            else
            {
                for (int i = 0; i < table.NumberOfNameEntries; i++)
                {
                    var entry = table.NameEntries[i];
                    var newTypes = new List<object>(types);
                    newTypes.Add(Encoding.UTF8.GetString(entry.Name.UnicodeString));
                    PrintPortableExecutableNameResourceDirectoryEntry(entry, level + 1, newTypes);
                }
            }

            Console.WriteLine($"{padding}ID entries");
            Console.WriteLine($"{padding}-------------------------");
            if (table.NumberOfIDEntries == 0)
            {
                Console.WriteLine($"{padding}No ID entries");
                Console.WriteLine();
            }
            else
            {

                for (int i = 0; i < table.NumberOfIDEntries; i++)
                {
                    var entry = table.IDEntries[i];
                    var newTypes = new List<object>(types);
                    newTypes.Add(entry.IntegerID);
                    PrintPortableExecutableIDResourceDirectoryEntry(entry, level + 1, newTypes);
                }
            }
        }

        /// <summary>
        /// Pretty print the Portable Executable name resource directory entry information
        /// </summary>
        private static void PrintPortableExecutableNameResourceDirectoryEntry(BurnOutSharp.Models.PortableExecutable.ResourceDirectoryEntry entry, int level, List<object> types)
        {
            string padding = new string(' ', (level + 1) * 2);

            Console.WriteLine($"{padding}Item level: {level}");
            Console.WriteLine($"{padding}Name offset: {entry.NameOffset}");
            Console.WriteLine($"{padding}Name ({entry.Name.Length}): {Encoding.UTF8.GetString(entry.Name.UnicodeString)}");
            if (entry.DataEntry != null)
                PrintPortableExecutableResourceDataEntry(entry.DataEntry, level: level + 1, types);
            else if (entry.Subdirectory != null)
                PrintPortableExecutableResourceDirectoryTable(entry.Subdirectory, level: level + 1, types);
        }

        /// <summary>
        /// Pretty print the Portable Executable ID resource directory entry information
        /// </summary>
        private static void PrintPortableExecutableIDResourceDirectoryEntry(BurnOutSharp.Models.PortableExecutable.ResourceDirectoryEntry entry, int level, List<object> types)
        {
            string padding = new string(' ', (level + 1) * 2);

            Console.WriteLine($"{padding}Item level: {level}");
            Console.WriteLine($"{padding}Integer ID: {entry.IntegerID}");
            if (entry.DataEntry != null)
                PrintPortableExecutableResourceDataEntry(entry.DataEntry, level: level + 1, types);
            else if (entry.Subdirectory != null)
                PrintPortableExecutableResourceDirectoryTable(entry.Subdirectory, level: level + 1, types);
        }

        /// <summary>
        /// Pretty print the Portable Executable resource data entry information
        /// </summary>
        private static void PrintPortableExecutableResourceDataEntry(BurnOutSharp.Models.PortableExecutable.ResourceDataEntry entry, int level, List<object> types)
        {
            string padding = new string(' ', (level + 1) * 2);

            // TODO: Use ordered list of base types to determine the shape of the data
            //Console.WriteLine($"{padding}Base types: {string.Join(", ", types)}");

            Console.WriteLine($"{padding}Entry level: {level}");
            Console.WriteLine($"{padding}Data RVA: {entry.DataRVA}");
            Console.WriteLine($"{padding}Size: {entry.Size}");
            Console.WriteLine($"{padding}Codepage: {entry.Codepage}");
            Console.WriteLine($"{padding}Reserved: {entry.Reserved}");

            // TODO: Print out per-type data
            if (types != null && types.Count > 0 && types[0] is uint resourceType)
            {
                switch ((BurnOutSharp.Models.PortableExecutable.ResourceType)resourceType)
                {
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_CURSOR:
                        Console.WriteLine($"{padding}Hardware-dependent cursor resource found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_BITMAP:
                        Console.WriteLine($"{padding}Bitmap resource found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_ICON:
                        Console.WriteLine($"{padding}Hardware-dependent icon resource found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_MENU:
                        Console.WriteLine($"{padding}Menu resource found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_DIALOG:
                        var dialogBox = entry.AsDialogBox();
                        if (dialogBox == null)
                        {
                            Console.WriteLine($"{padding}Dialog box resource found, but malformed");
                        }
                        else
                        {
                            if (dialogBox.DialogTemplate != null)
                            {
                                Console.WriteLine($"{padding}Style: {dialogBox.DialogTemplate.Style}");
                                Console.WriteLine($"{padding}Extended style: {dialogBox.DialogTemplate.ExtendedStyle}");
                                Console.WriteLine($"{padding}Item count: {dialogBox.DialogTemplate.ItemCount}");
                                Console.WriteLine($"{padding}X-coordinate of upper-left corner: {dialogBox.DialogTemplate.PositionX}");
                                Console.WriteLine($"{padding}Y-coordinate of upper-left corner: {dialogBox.DialogTemplate.PositionY}");
                                Console.WriteLine($"{padding}Width of the dialog box: {dialogBox.DialogTemplate.WidthX}");
                                Console.WriteLine($"{padding}Height of the dialog box: {dialogBox.DialogTemplate.HeightY}");
                                Console.WriteLine($"{padding}Menu resource: {dialogBox.DialogTemplate.MenuResource ?? "[EMPTY]"}");
                                Console.WriteLine($"{padding}Menu resource ordinal: {dialogBox.DialogTemplate.MenuResourceOrdinal}");
                                Console.WriteLine($"{padding}Class resource: {dialogBox.DialogTemplate.ClassResource ?? "[EMPTY]"}");
                                Console.WriteLine($"{padding}Class resource ordinal: {dialogBox.DialogTemplate.ClassResourceOrdinal}");
                                Console.WriteLine($"{padding}Title resource: {dialogBox.DialogTemplate.TitleResource ?? "[EMPTY]"}");
                                Console.WriteLine($"{padding}Point size value: {dialogBox.DialogTemplate.PointSizeValue}");
                                Console.WriteLine($"{padding}Typeface: {dialogBox.DialogTemplate.Typeface ?? "[EMPTY]"}");
                                Console.WriteLine();
                                Console.WriteLine($"{padding}Dialog item templates");
                                Console.WriteLine($"{padding}-------------------------");
                                if (dialogBox.DialogTemplate.ItemCount == 0
                                    || dialogBox.DialogItemTemplates == null
                                    || dialogBox.DialogItemTemplates.Length == 0)
                                {
                                    Console.WriteLine($"{padding}No dialog item templates");
                                }
                                else
                                {
                                    for (int i = 0; i < dialogBox.DialogItemTemplates.Length; i++)
                                    {
                                        var dialogItemTemplate = dialogBox.DialogItemTemplates[i];

                                        Console.WriteLine($"{padding}Dialog item template {i}");
                                        Console.WriteLine($"{padding}  Style: {dialogItemTemplate.Style}");
                                        Console.WriteLine($"{padding}  Extended style: {dialogItemTemplate.ExtendedStyle}");
                                        Console.WriteLine($"{padding}  X-coordinate of upper-left corner: {dialogItemTemplate.PositionX}");
                                        Console.WriteLine($"{padding}  Y-coordinate of upper-left corner: {dialogItemTemplate.PositionY}");
                                        Console.WriteLine($"{padding}  Width of the control: {dialogItemTemplate.WidthX}");
                                        Console.WriteLine($"{padding}  Height of the control: {dialogItemTemplate.HeightY}");
                                        Console.WriteLine($"{padding}  ID: {dialogItemTemplate.ID}");
                                        Console.WriteLine($"{padding}  Class resource: {dialogItemTemplate.ClassResource ?? "[EMPTY]"}");
                                        Console.WriteLine($"{padding}  Class resource ordinal: {dialogItemTemplate.ClassResourceOrdinal}");
                                        Console.WriteLine($"{padding}  Title resource: {dialogItemTemplate.TitleResource ?? "[EMPTY]"}");
                                        Console.WriteLine($"{padding}  Title resource ordinal: {dialogItemTemplate.TitleResourceOrdinal}");
                                        Console.WriteLine($"{padding}  Creation data size: {dialogItemTemplate.CreationDataSize}");
                                        if (dialogItemTemplate.CreationData != null && dialogItemTemplate.CreationData.Length != 0)
                                            Console.WriteLine($"{padding}  Creation data: {BitConverter.ToString(dialogItemTemplate.CreationData).Replace("-", string.Empty)}");
                                        else
                                            Console.WriteLine($"{padding}  Creation data: [EMPTY]");
                                    }
                                }
                            }
                            else if (dialogBox.ExtendedDialogTemplate != null)
                            {
                                Console.WriteLine($"{padding}Version: {dialogBox.ExtendedDialogTemplate.Version}");
                                Console.WriteLine($"{padding}Signature: {dialogBox.ExtendedDialogTemplate.Signature}");
                                Console.WriteLine($"{padding}Help ID: {dialogBox.ExtendedDialogTemplate.HelpID}");
                                Console.WriteLine($"{padding}Extended style: {dialogBox.ExtendedDialogTemplate.ExtendedStyle}");
                                Console.WriteLine($"{padding}Style: {dialogBox.ExtendedDialogTemplate.Style}");
                                Console.WriteLine($"{padding}Item count: {dialogBox.ExtendedDialogTemplate.DialogItems}");
                                Console.WriteLine($"{padding}X-coordinate of upper-left corner: {dialogBox.ExtendedDialogTemplate.PositionX}");
                                Console.WriteLine($"{padding}Y-coordinate of upper-left corner: {dialogBox.ExtendedDialogTemplate.PositionY}");
                                Console.WriteLine($"{padding}Width of the dialog box: {dialogBox.ExtendedDialogTemplate.WidthX}");
                                Console.WriteLine($"{padding}Height of the dialog box: {dialogBox.ExtendedDialogTemplate.HeightY}");
                                Console.WriteLine($"{padding}Menu resource: {dialogBox.ExtendedDialogTemplate.MenuResource ?? "[EMPTY]"}");
                                Console.WriteLine($"{padding}Menu resource ordinal: {dialogBox.ExtendedDialogTemplate.MenuResourceOrdinal}");
                                Console.WriteLine($"{padding}Class resource: {dialogBox.ExtendedDialogTemplate.ClassResource ?? "[EMPTY]"}");
                                Console.WriteLine($"{padding}Class resource ordinal: {dialogBox.ExtendedDialogTemplate.ClassResourceOrdinal}");
                                Console.WriteLine($"{padding}Title resource: {dialogBox.ExtendedDialogTemplate.TitleResource ?? "[EMPTY]"}");
                                Console.WriteLine($"{padding}Point size: {dialogBox.ExtendedDialogTemplate.PointSize}");
                                Console.WriteLine($"{padding}Weight: {dialogBox.ExtendedDialogTemplate.Weight}");
                                Console.WriteLine($"{padding}Italic: {dialogBox.ExtendedDialogTemplate.Italic}");
                                Console.WriteLine($"{padding}Character set: {dialogBox.ExtendedDialogTemplate.CharSet}");
                                Console.WriteLine($"{padding}Typeface: {dialogBox.ExtendedDialogTemplate.Typeface ?? "[EMPTY]"}");
                                Console.WriteLine();
                                Console.WriteLine($"{padding}Dialog item templates");
                                Console.WriteLine($"{padding}-------------------------");
                                if (dialogBox.ExtendedDialogTemplate.DialogItems == 0
                                    || dialogBox.ExtendedDialogItemTemplates == null
                                    || dialogBox.ExtendedDialogItemTemplates.Length == 0)
                                {
                                    Console.WriteLine($"{padding}No dialog item templates");
                                }
                                else
                                {
                                    for (int i = 0; i < dialogBox.ExtendedDialogItemTemplates.Length; i++)
                                    {
                                        var dialogItemTemplate = dialogBox.ExtendedDialogItemTemplates[i];

                                        Console.WriteLine($"{padding}Dialog item template {i}");
                                        Console.WriteLine($"{padding}  Help ID: {dialogItemTemplate.HelpID}");
                                        Console.WriteLine($"{padding}  Extended style: {dialogItemTemplate.ExtendedStyle}");
                                        Console.WriteLine($"{padding}  Style: {dialogItemTemplate.Style}");
                                        Console.WriteLine($"{padding}  X-coordinate of upper-left corner: {dialogItemTemplate.PositionX}");
                                        Console.WriteLine($"{padding}  Y-coordinate of upper-left corner: {dialogItemTemplate.PositionY}");
                                        Console.WriteLine($"{padding}  Width of the control: {dialogItemTemplate.WidthX}");
                                        Console.WriteLine($"{padding}  Height of the control: {dialogItemTemplate.HeightY}");
                                        Console.WriteLine($"{padding}  ID: {dialogItemTemplate.ID}");
                                        Console.WriteLine($"{padding}  Class resource: {dialogItemTemplate.ClassResource ?? "[EMPTY]"}");
                                        Console.WriteLine($"{padding}  Class resource ordinal: {dialogItemTemplate.ClassResourceOrdinal}");
                                        Console.WriteLine($"{padding}  Title resource: {dialogItemTemplate.TitleResource ?? "[EMPTY]"}");
                                        Console.WriteLine($"{padding}  Title resource ordinal: {dialogItemTemplate.TitleResourceOrdinal}");
                                        Console.WriteLine($"{padding}  Creation data size: {dialogItemTemplate.CreationDataSize}");
                                        if (dialogItemTemplate.CreationData != null && dialogItemTemplate.CreationData.Length != 0)
                                            Console.WriteLine($"{padding}  Creation data: {BitConverter.ToString(dialogItemTemplate.CreationData).Replace("-", string.Empty)}");
                                        else
                                            Console.WriteLine($"{padding}  Creation data: [EMPTY]");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{padding}Dialog box resource found, but malformed");
                            }
                        }
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_STRING:
                        var stringTable = entry.AsStringTable();
                        if (stringTable == null)
                        {
                            Console.WriteLine($"{padding}String table resource found, but malformed");
                        }
                        else
                        {
                            foreach ((int index, string stringValue) in stringTable)
                            {
                                Console.WriteLine($"{padding}String entry {index}: {stringValue}");
                            }
                        }
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_FONTDIR:
                        Console.WriteLine($"{padding}Font directory resource found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_FONT:
                        Console.WriteLine($"{padding}Font resource found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_ACCELERATOR:
                        var acceleratorTable = entry.AsAcceleratorTableResource();
                        if (acceleratorTable == null)
                        {
                            Console.WriteLine($"{padding}Accelerator table resource found, but malformed");
                        }
                        else
                        {
                            for (int i = 0; i < acceleratorTable.Length; i++)
                            {
                                var acceleratorTableEntry = acceleratorTable[i];
                                Console.WriteLine($"{padding}Flags: {acceleratorTableEntry.Flags}");
                                Console.WriteLine($"{padding}Ansi: {acceleratorTableEntry.Ansi}");
                                Console.WriteLine($"{padding}Id: {acceleratorTableEntry.Id}");
                                Console.WriteLine($"{padding}Padding: {acceleratorTableEntry.Padding}");
                            }
                        }
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_RCDATA:
                        Console.WriteLine($"{padding}Application-defined resource found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_MESSAGETABLE:
                        Console.WriteLine($"{padding}Message-table entry found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_GROUP_CURSOR:
                        Console.WriteLine($"{padding}Hardware-independent cursor resource found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_GROUP_ICON:
                        Console.WriteLine($"{padding}Hardware-independent icon resource found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_VERSION:
                        var versionInfo = entry.AsVersionInfo();
                        if (versionInfo == null)
                        {
                            Console.WriteLine($"{padding}Version info resource found, but malformed");
                        }
                        else
                        {
                            Console.WriteLine($"{padding}Length: {versionInfo.Length}");
                            Console.WriteLine($"{padding}Value length: {versionInfo.ValueLength}");
                            Console.WriteLine($"{padding}Resource type: {versionInfo.ResourceType}");
                            Console.WriteLine($"{padding}Key: {versionInfo.Key}");
                            if (versionInfo.ValueLength != 0 && versionInfo.Value != null)
                            {
                                Console.WriteLine($"{padding}[Fixed File Info] Signature: {versionInfo.Value.Signature}");
                                Console.WriteLine($"{padding}[Fixed File Info] Struct version: {versionInfo.Value.StrucVersion}");
                                Console.WriteLine($"{padding}[Fixed File Info] File version (MS): {versionInfo.Value.FileVersionMS}");
                                Console.WriteLine($"{padding}[Fixed File Info] File version (LS): {versionInfo.Value.FileVersionLS}");
                                Console.WriteLine($"{padding}[Fixed File Info] Product version (MS): {versionInfo.Value.ProductVersionMS}");
                                Console.WriteLine($"{padding}[Fixed File Info] Product version (LS): {versionInfo.Value.ProductVersionLS}");
                                Console.WriteLine($"{padding}[Fixed File Info] File flags mask: {versionInfo.Value.FileFlagsMask}");
                                Console.WriteLine($"{padding}[Fixed File Info] File flags: {versionInfo.Value.FileFlags}");
                                Console.WriteLine($"{padding}[Fixed File Info] File OS: {versionInfo.Value.FileOS}");
                                Console.WriteLine($"{padding}[Fixed File Info] Type: {versionInfo.Value.FileType}");
                                Console.WriteLine($"{padding}[Fixed File Info] Subtype: {versionInfo.Value.FileSubtype}");
                                Console.WriteLine($"{padding}[Fixed File Info] File date (MS): {versionInfo.Value.FileDateMS}");
                                Console.WriteLine($"{padding}[Fixed File Info] File date (LS): {versionInfo.Value.FileDateLS}");
                            }
                            if (versionInfo.StringFileInfo != null)
                            {
                                Console.WriteLine($"{padding}[String File Info] Length: {versionInfo.StringFileInfo.Length}");
                                Console.WriteLine($"{padding}[String File Info] Value length: {versionInfo.StringFileInfo.ValueLength}");
                                Console.WriteLine($"{padding}[String File Info] Resource type: {versionInfo.StringFileInfo.ResourceType}");
                                Console.WriteLine($"{padding}[String File Info] Key: {versionInfo.StringFileInfo.Key}");
                                Console.WriteLine($"{padding}Children:");
                                Console.WriteLine($"{padding}-------------------------");
                                if (versionInfo.StringFileInfo.Children == null || versionInfo.StringFileInfo.Children.Length == 0)
                                {
                                    Console.WriteLine($"{padding}No string file info children");
                                }
                                else
                                {
                                    for (int i = 0; i < versionInfo.StringFileInfo.Children.Length; i++)
                                    {
                                        var stringFileInfoChildEntry = versionInfo.StringFileInfo.Children[i];

                                        Console.WriteLine($"{padding}  [String Table {i}] Length: {stringFileInfoChildEntry.Length}");
                                        Console.WriteLine($"{padding}  [String Table {i}] Value length: {stringFileInfoChildEntry.ValueLength}");
                                        Console.WriteLine($"{padding}  [String Table {i}] ResourceType: {stringFileInfoChildEntry.ResourceType}");
                                        Console.WriteLine($"{padding}  [String Table {i}] Key: {stringFileInfoChildEntry.Key}");
                                        Console.WriteLine($"{padding}  [String Table {i}] Children:");
                                        Console.WriteLine($"{padding}  -------------------------");
                                        if (stringFileInfoChildEntry.Children == null || stringFileInfoChildEntry.Children.Length == 0)
                                        {
                                            Console.WriteLine($"{padding}  No string table {i} children");
                                        }
                                        else
                                        {
                                            for (int j = 0; j < stringFileInfoChildEntry.Children.Length; j++)
                                            {
                                                var stringDataEntry = stringFileInfoChildEntry.Children[j];

                                                Console.WriteLine($"{padding}    [String Data {j}] Length: {stringDataEntry.Length}");
                                                Console.WriteLine($"{padding}    [String Data {j}] Value length: {stringDataEntry.ValueLength}");
                                                Console.WriteLine($"{padding}    [String Data {j}] ResourceType: {stringDataEntry.ResourceType}");
                                                Console.WriteLine($"{padding}    [String Data {j}] Key: {stringDataEntry.Key}");
                                                Console.WriteLine($"{padding}    [String Data {j}] Value: {stringDataEntry.Value}");
                                            }
                                        }
                                    }
                                }
                            }
                            if (versionInfo.VarFileInfo != null)
                            {
                                Console.WriteLine($"{padding}[Var File Info] Length: {versionInfo.VarFileInfo.Length}");
                                Console.WriteLine($"{padding}[Var File Info] Value length: {versionInfo.VarFileInfo.ValueLength}");
                                Console.WriteLine($"{padding}[Var File Info] Resource type: {versionInfo.VarFileInfo.ResourceType}");
                                Console.WriteLine($"{padding}[Var File Info] Key: {versionInfo.VarFileInfo.Key}");
                                Console.WriteLine($"{padding}Children:");
                                Console.WriteLine($"{padding}-------------------------");
                                if (versionInfo.VarFileInfo.Children == null || versionInfo.VarFileInfo.Children.Length == 0)
                                {
                                    Console.WriteLine($"{padding}No var file info children");
                                }
                                else
                                {
                                    for (int i = 0; i < versionInfo.VarFileInfo.Children.Length; i++)
                                    {
                                        var varFileInfoChildEntry = versionInfo.VarFileInfo.Children[i];

                                        Console.WriteLine($"{padding}  [String Table {i}] Length: {varFileInfoChildEntry.Length}");
                                        Console.WriteLine($"{padding}  [String Table {i}] Value length: {varFileInfoChildEntry.ValueLength}");
                                        Console.WriteLine($"{padding}  [String Table {i}] ResourceType: {varFileInfoChildEntry.ResourceType}");
                                        Console.WriteLine($"{padding}  [String Table {i}] Key: {varFileInfoChildEntry.Key}");
                                        Console.WriteLine($"{padding}  [String Table {i}] Value: {string.Join(",", varFileInfoChildEntry.Value)}");
                                    }
                                }
                            }
                        }
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_DLGINCLUDE:
                        Console.WriteLine($"{padding}External header resource found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_PLUGPLAY:
                        Console.WriteLine($"{padding}Plug and Play resource found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_VXD:
                        Console.WriteLine($"{padding}VXD found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_ANICURSOR:
                        Console.WriteLine($"{padding}Animated cursor found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_ANIICON:
                        Console.WriteLine($"{padding}Animated icon found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_HTML:
                        Console.WriteLine($"{padding}HTML resource found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_MANIFEST:
                        var assemblyManifest = entry.AsAssemblyManifest();
                        if (assemblyManifest == null)
                        {
                            Console.WriteLine($"{padding}Assembly manifest found, but malformed");
                        }
                        else
                        {
                            Console.WriteLine($"{padding}Manifest version: {assemblyManifest.ManifestVersion}");
                            if (assemblyManifest.AssemblyIdentities != null && assemblyManifest.AssemblyIdentities.Length > 0)
                            {
                                for (int i = 0; i < assemblyManifest.AssemblyIdentities.Length; i++)
                                {
                                    var assemblyIdentity = assemblyManifest.AssemblyIdentities[i];
                                    Console.WriteLine($"{padding}[Assembly Identity {i}] Name: {assemblyIdentity.Name}");
                                    Console.WriteLine($"{padding}[Assembly Identity {i}] Version: {assemblyIdentity.Version}");
                                    Console.WriteLine($"{padding}[Assembly Identity {i}] Type: {assemblyIdentity.Type}");
                                    Console.WriteLine($"{padding}[Assembly Identity {i}] Processor architecture: {assemblyIdentity.ProcessorArchitecture}");
                                    Console.WriteLine($"{padding}[Assembly Identity {i}] Public key token: {assemblyIdentity.PublicKeyToken}");
                                    Console.WriteLine($"{padding}[Assembly Identity {i}] Language: {assemblyIdentity.Language}");
                                }
                            }
                            if (assemblyManifest.Description != null)
                            {
                                Console.WriteLine($"{padding}[Assembly Description] Value: {assemblyManifest.Description.Value}");
                            }
                            if (assemblyManifest.COMInterfaceExternalProxyStub != null && assemblyManifest.COMInterfaceExternalProxyStub.Length > 0)
                            {
                                for (int i = 0; i < assemblyManifest.COMInterfaceExternalProxyStub.Length; i++)
                                {
                                    var comInterfaceExternalProxyStub = assemblyManifest.COMInterfaceExternalProxyStub[i];
                                    Console.WriteLine($"{padding}[COM Interface External Proxy Stub {i}] IID: {comInterfaceExternalProxyStub.IID}");
                                    Console.WriteLine($"{padding}[COM Interface External Proxy Stub {i}] Name: {comInterfaceExternalProxyStub.Name}");
                                    Console.WriteLine($"{padding}[COM Interface External Proxy Stub {i}] TLBID: {comInterfaceExternalProxyStub.TLBID}");
                                    Console.WriteLine($"{padding}[COM Interface External Proxy Stub {i}] Number of methods: {comInterfaceExternalProxyStub.NumMethods}");
                                    Console.WriteLine($"{padding}[COM Interface External Proxy Stub {i}] Proxy stub (CLSID32): {comInterfaceExternalProxyStub.ProxyStubClsid32}");
                                    Console.WriteLine($"{padding}[COM Interface External Proxy Stub {i}] Base interface: {comInterfaceExternalProxyStub.BaseInterface}");
                                }
                            }
                            if (assemblyManifest.Dependency != null && assemblyManifest.Dependency.Length > 0)
                            {
                                for (int i = 0; i < assemblyManifest.Dependency.Length; i++)
                                {
                                    var dependency = assemblyManifest.Dependency[i];
                                    if (dependency.DependentAssembly != null)
                                    {
                                        if (dependency.DependentAssembly.AssemblyIdentity != null)
                                        {
                                            Console.WriteLine($"{padding}[Dependency {i} Assembly Identity] Name: {dependency.DependentAssembly.AssemblyIdentity.Name}");
                                            Console.WriteLine($"{padding}[Dependency {i} Assembly Identity] Version: {dependency.DependentAssembly.AssemblyIdentity.Version}");
                                            Console.WriteLine($"{padding}[Dependency {i} Assembly Identity] Type: {dependency.DependentAssembly.AssemblyIdentity.Type}");
                                            Console.WriteLine($"{padding}[Dependency {i} Assembly Identity] Processor architecture: {dependency.DependentAssembly.AssemblyIdentity.ProcessorArchitecture}");
                                            Console.WriteLine($"{padding}[Dependency {i} Assembly Identity] Public key token: {dependency.DependentAssembly.AssemblyIdentity.PublicKeyToken}");
                                            Console.WriteLine($"{padding}[Dependency {i} Assembly Identity] Language: {dependency.DependentAssembly.AssemblyIdentity.Language}");
                                        }
                                        if (dependency.DependentAssembly.BindingRedirect != null && dependency.DependentAssembly.BindingRedirect.Length > 0)
                                        {
                                            for (int j = 0; j < dependency.DependentAssembly.BindingRedirect.Length; j++)
                                            {
                                                var bindingRedirect = dependency.DependentAssembly.BindingRedirect[j];
                                                Console.WriteLine($"{padding}[Dependency {i} Binding Redirect {j}] Old version: {bindingRedirect.OldVersion}");
                                                Console.WriteLine($"{padding}[Dependency {i} Binding Redirect {j}] New version: {bindingRedirect.NewVersion}");
                                            }
                                        }
                                    }

                                    Console.WriteLine($"{padding}[Dependency {i}] Optional: {dependency.Optional}");
                                }
                            }
                            if (assemblyManifest.File != null && assemblyManifest.File.Length > 0)
                            {
                                for (int i = 0; i < assemblyManifest.File.Length; i++)
                                {
                                    var file = assemblyManifest.File[i];
                                    Console.WriteLine($"{padding}[File {i}] Name: {file.Name}");
                                    Console.WriteLine($"{padding}[File {i}] Hash: {file.Hash}");
                                    Console.WriteLine($"{padding}[File {i}] Hash algorithm: {file.HashAlgorithm}");
                                    Console.WriteLine($"{padding}[File {i}] Size: {file.Size}");

                                    if (file.COMClass != null && file.COMClass.Length > 0)
                                    {
                                        for (int j = 0; j < file.COMClass.Length; j++)
                                        {
                                            var comClass = file.COMClass[j];
                                            Console.WriteLine($"{padding}[File {i} COM Class {j}] CLSID: {comClass.CLSID}");
                                            Console.WriteLine($"{padding}[File {i} COM Class {j}] Threading model: {comClass.ThreadingModel}");
                                            Console.WriteLine($"{padding}[File {i} COM Class {j}] Prog ID: {comClass.ProgID}");
                                            Console.WriteLine($"{padding}[File {i} COM Class {j}] TLBID: {comClass.TLBID}");
                                            Console.WriteLine($"{padding}[File {i} COM Class {j}] Description: {comClass.Description}");

                                            if (comClass.ProgIDs != null && comClass.ProgIDs.Length > 0)
                                            {
                                                for (int k = 0; k < comClass.ProgIDs.Length; k++)
                                                {
                                                    var progId = comClass.ProgIDs[k];
                                                    Console.WriteLine($"{padding}[File {i} COM Class {j} Prog ID {k}] Value: {progId.Value}");
                                                }
                                            }
                                        }
                                    }
                                    if (file.COMInterfaceProxyStub != null && file.COMInterfaceProxyStub.Length > 0)
                                    {
                                        for (int j = 0; j < file.COMInterfaceProxyStub.Length; j++)
                                        {
                                            var comInterfaceProxyStub = file.COMInterfaceProxyStub[j];
                                            Console.WriteLine($"{padding}[File {i} COM Interface Proxy Stub {j}] IID: {comInterfaceProxyStub.IID}");
                                            Console.WriteLine($"{padding}[File {i} COM Interface Proxy Stub {j}] Name: {comInterfaceProxyStub.Name}");
                                            Console.WriteLine($"{padding}[File {i} COM Interface Proxy Stub {j}] TLBID: {comInterfaceProxyStub.TLBID}");
                                            Console.WriteLine($"{padding}[File {i} COM Interface Proxy Stub {j}] Number of methods: {comInterfaceProxyStub.NumMethods}");
                                            Console.WriteLine($"{padding}[File {i} COM Interface Proxy Stub {j}] Proxy stub (CLSID32): {comInterfaceProxyStub.ProxyStubClsid32}");
                                            Console.WriteLine($"{padding}[File {i} COM Interface Proxy Stub {j}] Base interface: {comInterfaceProxyStub.BaseInterface}");
                                        }
                                    }
                                    if (file.Typelib != null && file.Typelib.Length > 0)
                                    {
                                        for (int j = 0; j < file.Typelib.Length; j++)
                                        {
                                            var typeLib = file.Typelib[j];
                                            Console.WriteLine($"{padding}[File {i} Type Lib {j}] TLBID: {typeLib.TLBID}");
                                            Console.WriteLine($"{padding}[File {i} Type Lib {j}] Version: {typeLib.Version}");
                                            Console.WriteLine($"{padding}[File {i} Type Lib {j}] Help directory: {typeLib.HelpDir}");
                                            Console.WriteLine($"{padding}[File {i} Type Lib {j}] Resource ID: {typeLib.ResourceID}");
                                            Console.WriteLine($"{padding}[File {i} Type Lib {j}] Flags: {typeLib.Flags}");
                                        }
                                    }
                                    if (file.WindowClass != null && file.WindowClass.Length > 0)
                                    {
                                        for (int j = 0; j < file.WindowClass.Length; j++)
                                        {
                                            var windowClass = file.WindowClass[j];
                                            Console.WriteLine($"{padding}[File {i} Window Class {j}] Versioned: {windowClass.Versioned}");
                                            Console.WriteLine($"{padding}[File {i} Window Class {j}] Value: {windowClass.Value}");
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
                                        Console.WriteLine($"{padding}Unparsed XML Element {i}: {element.OuterXml}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{padding}Unparsed Item {i}: {thing}");
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        Console.WriteLine($"{padding}Type {(BurnOutSharp.Models.PortableExecutable.ResourceType)resourceType} found, not parsed yet");
                        //Console.WriteLine($"{padding}Data: {BitConverter.ToString(entry.Data).Replace("-", string.Empty)}");
                        //Console.WriteLine($"{padding}Data: {Encoding.Unicode.GetString(entry.Data)}");
                        break;
                }
            }
            else if (types != null && types.Count > 0 && types[0] is string resourceString)
            {
                Console.WriteLine($"{padding}Custom data type: {resourceString}");
                //Console.WriteLine($"{padding}Data: {BitConverter.ToString(entry.Data).Replace("-", string.Empty)}");
                //Console.WriteLine($"{padding}Data: {Encoding.Unicode.GetString(entry.Data)}");
            }

            Console.WriteLine();
        }
    }
}