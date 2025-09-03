using System;
using System.IO;
using BinaryObjectScanner;

namespace ProtectionScan
{
    class Program
    {
        static void Main(string[] args)
        {
#if NET462_OR_GREATER || NETCOREAPP
            // Register the codepages
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif

            // Create progress indicator
            var fileProgress = new Progress<ProtectionProgress>();
            fileProgress.ProgressChanged += Changed;

            // Get the options from the arguments
            var options = Options.ParseOptions(args);

            // If we have an invalid state
            if (options == null)
            {
                Options.DisplayHelp();
                return;
            }

            // Create scanner for all paths
            var scanner = new Scanner(
                options.ScanArchives,
                options.ScanContents,
                options.ScanPaths,
                options.ScanSubdirectories,
                options.Debug,
                fileProgress);

            // Loop through the input paths
            foreach (string inputPath in options.InputPaths)
            {
                GetAndWriteProtections(scanner, inputPath);
            }
        }

        /// <summary>
        /// Wrapper to get and log protections for a single path
        /// </summary>
        /// <param name="scanner">Scanner object to use</param>
        /// <param name="path">File or directory path</param>
        private static void GetAndWriteProtections(Scanner scanner, string path)
        {
            // Normalize by getting the full path
            path = Path.GetFullPath(path);

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
                try
                {
                    using var sw = new StreamWriter(File.OpenWrite($"exception-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}.txt"));
                    sw.WriteLine(ex);
                }
                catch
                {
                    Console.WriteLine("Could not open exception log file for writing. See original message below:");
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Write the protection results from a single path to file, if possible
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="protections">Dictionary of protections found, if any</param>
        private static void WriteProtectionResultFile(string path, ProtectionDictionary? protections)
        {
            if (protections == null)
            {
                Console.WriteLine($"No protections found for {path}");
                return;
            }

            // Attempt to open a protection file for writing
            StreamWriter? sw = null;
            try
            {
                sw = new StreamWriter(File.OpenWrite($"protection-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}.txt"));
            }
            catch
            {
                Console.WriteLine("Could not open protection log file for writing. Only a console log will be provided.");
            }

            // Sort the keys for consistent output
            string[] keys = [.. protections.Keys];
            Array.Sort(keys);

            // Loop over all keys
            foreach (string key in keys)
            {
                // Skip over files with no protection
                if (protections[key] == null || protections[key].Count == 0)
                    continue;

                // Sort the detected protections for consistent output
                string[] fileProtections = [.. protections[key]];
                Array.Sort(fileProtections);

                // Format and output the line
                string line = $"{key}: {string.Join(", ", fileProtections)}";
                Console.WriteLine(line);
                sw?.WriteLine(line);
            }

            // Dispose of the writer
            sw?.Dispose();
        }

        /// <summary>
        /// Protection progress changed handler
        /// </summary>
        private static void Changed(object? source, ProtectionProgress value)
        {
            string prefix = string.Empty;
            for (int i = 0; i < value.Depth; i++)
            {
                prefix += "--> ";
            }

            Console.WriteLine($"{prefix}{value.Percentage * 100:N2}%: {value.Filename} - {value.Protection}");
        }
    }
}
