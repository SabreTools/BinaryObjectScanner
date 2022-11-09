using System.Text;
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
                    Console.WriteLine($"  Segment Table Entry {i}");
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
            }
            Console.WriteLine();

            // TODO: COFFStringTable (Only if COFFSymbolTable?)
            // TODO: AttributeCertificateTable
            // TODO: DelayLoadDirectoryTable

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
            Console.WriteLine($"{padding}Size: {entry.DataRVA}");
            Console.WriteLine($"{padding}Codepage: {entry.Codepage}");
            Console.WriteLine($"{padding}Reserved: {entry.Reserved}");

            // TODO: Print out per-type data
            if (types != null && types.Count > 0 && types[0] is uint resourceType)
            {
                int offset = 0;
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
                        Console.WriteLine($"{padding}Dialog box found, not parsed yet");
                        break;
                    case BurnOutSharp.Models.PortableExecutable.ResourceType.RT_STRING:
                        int stringIndex = 0;
                        Encoding stringEncoding = (entry.Codepage != 0 ? Encoding.GetEncoding((int)entry.Codepage) : Encoding.Unicode);
                        while (offset < entry.Data.Length)
                        {
                            ushort stringLength = entry.Data.ReadUInt16(ref offset);
                            if (stringLength == 0)
                            {
                                Console.WriteLine($"{padding}String entry {stringIndex++} ({stringLength}): [EMPTY]");
                            }
                            else
                            {
                                string fullEncodedString = stringEncoding.GetString(entry.Data, offset, entry.Data.Length - offset);
                                string stringValue = fullEncodedString.Substring(0, stringLength);
                                offset += stringEncoding.GetByteCount(stringValue);
                                stringValue = stringValue.Replace("\n", "\\n").Replace("\r", "\\r");
                                Console.WriteLine($"{padding}String entry {stringIndex++} ({stringLength}): {stringValue}");
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
                        var acceleratorTable = entry.Data.AsAcceleratorTableResource(ref offset);
                        if (acceleratorTable != null)
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
                        Console.WriteLine($"{padding}Version resource found, not parsed yet");
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
                        Console.WriteLine($"{padding}Side-by-Side Assembly Manifest found, not parsed yet");
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