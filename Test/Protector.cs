using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using BinaryObjectScanner;

namespace Test
{
    internal static class Protector
    {
        /// <summary>
        /// Wrapper to get and log protections for a single path
        /// </summary>
        /// <param name="scanner">Scanner object to use</param>
        /// <param name="path">File or directory path</param>
        public static void GetAndWriteProtections(Scanner scanner, string path)
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
        private static void WriteProtectionResultFile(string path, ConcurrentDictionary<string, ConcurrentQueue<string>>? protections)
        {
            if (protections == null)
            {
                Console.WriteLine($"No protections found for {path}");
                return;
            }

            using var sw = new StreamWriter(File.OpenWrite($"protection-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}.txt"));
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

        /// <summary>
        /// Protection progress changed handler
        /// </summary>
        public static void Changed(object? source, ProtectionProgress value)
        {
            Console.WriteLine($"{value.Percentage * 100:N2}%: {value.Filename} - {value.Protection}");
        }
    }
}