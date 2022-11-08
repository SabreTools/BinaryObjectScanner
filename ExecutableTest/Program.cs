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

                    // Try to read the new executable info
                    stream.Seek(msdos.Header.NewExeHeaderAddr, SeekOrigin.Begin);
                    magic = stream.ReadBytes(4);

                    // New Executable
                    if (magic[0] == 'N' && magic[1] == 'E')
                    {
                        Console.WriteLine($"New executable found. No parsing currently available.");
                        Console.WriteLine();
                        continue;

                        // TODO: Implement NE reading
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
        /// <param name="executable"></param>
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
                    Console.WriteLine($"  Relocation Table Entry {i} - Offset = {entry.Offset}, Segment = {entry.Segment}");
                }
            }
            Console.WriteLine();
        }
    }
}