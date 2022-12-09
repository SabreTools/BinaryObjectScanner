using System.Text;
using BurnOutSharp.Wrappers;
using static BurnOutSharp.Builder.Extensions;

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

                // Check if the file or directory exists
                if (File.Exists(arg))
                {
                    PrintFileInfo(arg);
                }
                else if (Directory.Exists(arg))
                {
                    foreach (string path in Directory.EnumerateFiles(arg, "*", SearchOption.AllDirectories))
                    {
                        PrintFileInfo(path);
                    }
                }
                else
                {
                    Console.WriteLine($"{arg} does not exist, skipping...");
                }
            }
        }
    
        /// <summary>
        /// Print information for a single file, if possible
        /// </summary>
        private static void PrintFileInfo(string file)
        {
            using (Stream stream = File.OpenRead(file))
            {
                // Read the first 4 bytes
                byte[] magic = stream.ReadBytes(2);
                
                if (!IsMSDOS(magic))
                {
                    Console.WriteLine("Not a recognized executable format, skipping...");
                    Console.WriteLine();
                    return;
                }

                // Build the executable information
                Console.WriteLine("Creating MS-DOS executable builder");
                Console.WriteLine();

                stream.Seek(0, SeekOrigin.Begin);
                var msdos = MSDOS.Create(stream);
                if (msdos == null)
                {
                    Console.WriteLine("Something went wrong parsing MS-DOS executable");
                    Console.WriteLine();
                    return;
                }

                // Print the executable info to screen
                msdos.Print();

                // Check for a valid new executable address
                if (msdos.NewExeHeaderAddr >= stream.Length)
                {
                    Console.WriteLine("New EXE header address invalid, skipping additional reading...");
                    Console.WriteLine();
                    return;
                }

                // Try to read the executable info
                stream.Seek(msdos.NewExeHeaderAddr, SeekOrigin.Begin);
                magic = stream.ReadBytes(4);

                // New Executable
                if (IsNE(magic))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var newExecutable = NewExecutable.Create(stream);
                    if (newExecutable == null)
                    {
                        Console.WriteLine("Something went wrong parsing New Executable");
                        Console.WriteLine();
                        return;
                    }

                    // Print the executable info to screen
                    newExecutable.Print();
                }

                // Linear Executable
                else if (IsLE(magic))
                {
                    Console.WriteLine($"Linear executable found. No parsing currently available.");
                    Console.WriteLine();
                    return;
                }

                // Portable Executable
                else if (IsPE(magic))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var portableExecutable = PortableExecutable.Create(stream);
                    if (portableExecutable == null)
                    {
                        Console.WriteLine("Something went wrong parsing Portable Executable");
                        Console.WriteLine();
                        return;
                    }

                    // Print the executable info to screen
                    portableExecutable.Print();
                }

                // Unknown
                else
                {
                    Console.WriteLine($"Unrecognized header signature: {BitConverter.ToString(magic).Replace("-", string.Empty)}");
                    Console.WriteLine();
                    return;
                }
            }
        }

        /// <summary>
        /// Determine if the magic bytes indicate an MS-DOS executable
        /// </summary>
        private static bool IsMSDOS(byte[] magic)
        {
            if (magic == null || magic.Length < 2)
                return false;

            return magic[0] == 'M' && magic[1] == 'Z';
        }

        /// <summary>
        /// Determine if the magic bytes indicate a New Executable
        /// </summary>
        private static bool IsNE(byte[] magic)
        {
            if (magic == null || magic.Length < 2)
                return false;

            return magic[0] == 'N' && magic[1] == 'E';
        }

        /// <summary>
        /// Determine if the magic bytes indicate a Linear Executable
        /// </summary>
        private static bool IsLE(byte[] magic)
        {
            if (magic == null || magic.Length < 2)
                return false;

            return magic[0] == 'L' && (magic[1] == 'E' || magic[1] == 'X');
        }

        /// <summary>
        /// Determine if the magic bytes indicate a Portable Executable
        /// </summary>
        private static bool IsPE(byte[] magic)
        {
            if (magic == null || magic.Length < 4)
                return false;

            return magic[0] == 'P' && magic[1] == 'E' && magic[2] == '\0' && magic[3] == '\0';
        }
    }
}