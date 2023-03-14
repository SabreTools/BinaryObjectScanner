using System;
using System.IO;
using System.Text;
using BinaryObjectScanner.Utilities;
using BinaryObjectScanner.Wrappers;

namespace Test
{
    internal static class Printer
    {
        /// <summary>
        /// Wrapper to print information for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="json">Enable JSON output, if supported</param>
        /// <param name="debug">Enable debug output</param>
        public static void PrintPathInfo(string path, bool json, bool debug)
        {
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                PrintFileInfo(path, json, debug);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    PrintFileInfo(file, json, debug);
                }
            }
            else
            {
                Console.WriteLine($"{path} does not exist, skipping...");
            }
        }

        /// <summary>
        /// Print information for a single file, if possible
        /// </summary>
        private static void PrintFileInfo(string file, bool json, bool debug)
        {
            Console.WriteLine($"Attempting to print info for {file}");

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Read the first 8 bytes
                byte[] magic = stream.ReadBytes(8);
                stream.Seek(0, SeekOrigin.Begin);

                // Get the file type
                SupportedFileType ft = FileTypes.GetFileType(magic);
                if (ft == SupportedFileType.UNKNOWN)
                {
                    string extension = Path.GetExtension(file).TrimStart('.');
                    ft = FileTypes.GetFileType(extension);
                }

                // Print out the file format
                Console.WriteLine($"File format found: {ft}");

                // Setup the wrapper to print
                var wrapper = WrapperFactory.CreateWrapper(ft, stream);

                // If we don't have a wrapper
                if (wrapper == null)
                {
                    Console.WriteLine($"Either {ft} is not supported or something went wrong during parsing!");
                    Console.WriteLine();
                    return;
                }

                // Print the wrapper name
                Console.WriteLine($"{wrapper.Description} wrapper created successfully!");

                // Get the base info output name
                string filenameBase = $"info-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}";

#if NET6_0_OR_GREATER
                // If we have the JSON flag
                if (json)
                {
                    // Create the output data
                    string serializedData = wrapper.ExportJSON();
                    Console.WriteLine(serializedData);

                    // Write the output data
                    using (var sw = new StreamWriter(File.OpenWrite($"{filenameBase}.json")))
                    {
                        sw.WriteLine(serializedData);
                    }
                }
#endif
                // Create the output data
                StringBuilder builder = wrapper.PrettyPrint();
                Console.WriteLine(builder);

                // Write the output data
                using (var sw = new StreamWriter(File.OpenWrite($"{filenameBase}.txt")))
                {
                    sw.WriteLine(builder.ToString());
                }
            }
        }
    }
}