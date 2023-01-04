using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Register the codepages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Create progress indicator
            var p = new Progress<ProtectionProgress>();
            p.ProgressChanged += Protector.Changed;

            // Set initial values for scanner flags
            bool debug = false, archives = true, packers = true, info = false, extract = false;
            string outputPath = string.Empty;
            var inputPaths = new List<string>();

            // Loop through the arguments to get the flags
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                switch (arg)
                {
                    case "-?":
                    case "-h":
                    case "--help":
                        DisplayHelp();
                        Console.WriteLine("Press enter to close the program...");
                        Console.ReadLine();
                        return;

                    case "-d":
                    case "--debug":
                        debug = true;
                        break;

                    case "-na":
                    case "--no-archives":
                        archives = false;
                        break;

                    case "-np":
                    case "--no-packers":
                        packers = false;
                        break;

                    case "-i":
                    case "--info":
                        info = true;
                        break;

                    case "-x":
                    case "--extract":
                        extract = true;
                        break;

                    case "-o":
                    case "--outdir":
                        outputPath = i + 1 < args.Length ? args[++i] : null;
                        break;

                    default:
                        inputPaths.Add(arg);
                        break;
                }
            }

            // If we have no arguments, show the help
            if (inputPaths.Count == 0)
            {
                DisplayHelp();
                Console.WriteLine("Press enter to close the program...");
                Console.ReadLine();
                return;
            }

            // Create scanner for all paths
            var scanner = new Scanner(archives, packers, debug, p);

            // If we have extraction, check the output path exists and is valid
            if (extract)
            {
                // Null or empty output path
                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    Console.WriteLine("Output directory required for extraction!");
                    Console.WriteLine();
                    DisplayHelp();
                    Console.WriteLine("Press enter to close the program...");
                    Console.ReadLine();
                    return;
                }

                // Malformed output path or invalid location
                try
                {
                    outputPath = Path.GetFullPath(outputPath);
                    Directory.CreateDirectory(outputPath);
                }
                catch
                {
                    Console.WriteLine("Output directory could not be created!");
                    Console.WriteLine();
                    DisplayHelp();
                    Console.WriteLine("Press enter to close the program...");
                    Console.ReadLine();
                    return;
                }
            }

            // Loop through the input paths
            foreach (string inputPath in inputPaths)
            {
                if (info)
                    Printer.PrintPathInfo(inputPath);
                else if (extract)
                    Extractor.ExtractPath(inputPath, outputPath);
                else
                    Protector.GetAndWriteProtections(scanner, inputPath);
            }

            Console.WriteLine("Press enter to close the program...");
            Console.ReadLine();
        }

        /// <summary>
        /// Display help text
        /// </summary>
        private static void DisplayHelp()
        {
            Console.WriteLine("BurnOutSharp Test Program");
            Console.WriteLine();
            Console.WriteLine("test.exe <options> file|directory ...");
            Console.WriteLine();
            Console.WriteLine("Possible options:");
            Console.WriteLine("-?, -h, --help       Display this help text and quit");
            Console.WriteLine("-d, --debug          Enable debug mode");
            Console.WriteLine("-na, --no-archives   Disable scanning archives");
            Console.WriteLine("-np, --no-packers    Disable scanning for packers");
            Console.WriteLine("-i, --info           Print executable info");
            Console.WriteLine("-x, --extract        Extract archive formats");
            Console.WriteLine("-o, --outdir [PATH]  Set output path for extraction (REQUIRED)");
        }
    }
}
