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
                        Console.WriteLine($"Portable executable found. No parsing currently available.");
                        Console.WriteLine();
                        continue;
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

            // TODO: Add table printing
        }
    }
}