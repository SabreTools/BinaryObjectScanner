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
                    var msdos = MSDOS.Create(stream);
                    if (msdos == null)
                    {
                        Console.WriteLine("Something went wrong parsing MS-DOS executable");
                        Console.WriteLine();
                        continue;
                    }

                    // Print the executable info to screen
                    msdos.Print();

                    // Check for a valid new executable address
                    if (msdos.NewExeHeaderAddr >= stream.Length)
                    {
                        Console.WriteLine("New EXE header address invalid, skipping additional reading...");
                        Console.WriteLine();
                        continue;
                    }

                    // Try to read the executable info
                    stream.Seek(msdos.NewExeHeaderAddr, SeekOrigin.Begin);
                    magic = stream.ReadBytes(4);

                    // New Executable
                    if (magic[0] == 'N' && magic[1] == 'E')
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        var newExecutable = NewExecutable.Create(stream);
                        if (newExecutable == null)
                        {
                            Console.WriteLine("Something went wrong parsing New Executable");
                            Console.WriteLine();
                            continue;
                        }

                        // Print the executable info to screen
                        newExecutable.Print();
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
                        var portableExecutable = PortableExecutable.Create(stream);
                        if (portableExecutable == null)
                        {
                            Console.WriteLine("Something went wrong parsing Portable Executable");
                            Console.WriteLine();
                            continue;
                        }

                        // Print the executable info to screen
                        portableExecutable.Print();
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
    }
}