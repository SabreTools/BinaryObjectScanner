using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create progress indicator
            var p = new Progress<ProtectionProgress>();
            p.ProgressChanged += Changed;

            // Set initial values for scanner flags
            bool debug = false, archives = true, packers = true;
            var inputPaths = new List<string>();

            // Loop through the arguments to get the flags
            foreach (string arg in args)
            {
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
            var scanner = new Scanner(p)
            {
                IncludeDebug = debug,
                ScanArchives = archives,
                ScanPackers = packers,
            };

            // Loop through the input paths
            foreach (string inputPath in inputPaths)
            {
                GetAndWriteProtections(scanner, inputPath);
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
        }

        /// <summary>
        /// Wrapper to get and log protections for a single path
        /// </summary>
        /// <param name="scanner">Scanner object to use</param>
        /// <param name="path">File or directory path</param>
        private static void GetAndWriteProtections(Scanner scanner, string path)
        {
            // An invalid path can't be scanned
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                Console.WriteLine($"{path} does not exist, skipping...");
                return;
            }

            try
            {
                var protections = scanner.GetProtections(path);
                WriteProtectionResultFile(path, protections);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(File.OpenWrite($"{DateTime.Now:yyyy-MM-dd_HHmmss}-exception.txt")))
                {
                    sw.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Write the protection results from a single path to file, if possible
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="protections">Dictionary of protections found, if any</param>
        private static void WriteProtectionResultFile(string path, ConcurrentDictionary<string, ConcurrentQueue<string>> protections)
        {
            if (protections == null)
            {
                Console.WriteLine($"No protections found for {path}");
                return;
            }

            using (var sw = new StreamWriter(File.OpenWrite($"{DateTime.Now:yyyy-MM-dd_HHmmss}.txt")))
            {
                foreach (string key in protections.Keys.OrderBy(k => k))
                {
                    // Skip over files with no protection
                    if (protections[key] == null || !protections[key].Any())
                        continue;

                    string line = $"{key}: {string.Join(", ", protections[key].OrderBy(p => p))}";
                    Console.WriteLine(line);
                    sw.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Protection progress changed handler
        /// </summary>
        private static void Changed(object source, ProtectionProgress value)
        {
            Console.WriteLine($"{value.Percentage * 100:N2}%: {value.Filename} - {value.Protection}");
        }
    }
}
