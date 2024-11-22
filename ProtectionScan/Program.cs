using System;
using System.IO;
#if NET35_OR_GREATER || NETCOREAPP
using System.Linq;
#endif
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
                options.ScanGameEngines,
                options.ScanPackers,
                options.ScanPaths,
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
                using var sw = new StreamWriter(File.OpenWrite($"exception-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}.txt"));
                sw.WriteLine(ex);
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

            using var sw = new StreamWriter(File.OpenWrite($"protection-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}.txt"));
#if NET20
            var keysArr = new string[protections.Keys.Count];
            protections.Keys.CopyTo(keysArr, 0);
            Array.Sort(keysArr);
            foreach (string key in keysArr)
#else
            foreach (string key in protections.Keys.OrderBy(k => k))
#endif
            {
                // Skip over files with no protection
                if (protections[key] == null || protections[key].Count == 0)
                    continue;

#if NET20
                string[] fileProtections = [.. protections[key]];
                Array.Sort(fileProtections);
#else
                string[] fileProtections = [.. protections[key].OrderBy(p => p)];
#endif
                string line = $"{key}: {string.Join(", ", fileProtections)}";
                Console.WriteLine(line);
                sw.WriteLine(line);
            }
        }

        /// <summary>
        /// Protection progress changed handler
        /// </summary>
        private static void Changed(object? source, ProtectionProgress value)
        {
            Console.WriteLine($"{value.Percentage * 100:N2}%: {value.Filename} - {value.Protection}");
        }
    }
}
