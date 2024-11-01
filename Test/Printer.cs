using System;
using System.IO;
#if NET452_OR_GREATER || NETCOREAPP
using System.Text;
#endif
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;
using SPrinter = SabreTools.Serialization.Printer;

namespace Test
{
    internal class Printer
    {
        #region Options

        /// <inheritdoc cref="BinaryObjectScanner.Options.IncludeDebug"/>
        public bool IncludeDebug => _options?.IncludeDebug ?? false;

        /// <summary>
        /// Options object for configuration
        /// </summary>
        private readonly BinaryObjectScanner.Options _options;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="includeDebug">Enable including debug information</param>
        public Printer(bool includeDebug)
        {
            _options = new BinaryObjectScanner.Options
            {
                IncludeDebug = includeDebug,
            };

#if NET462_OR_GREATER || NETCOREAPP
            // Register the codepages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        /// <summary>
        /// Wrapper to print information for a single path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="json">Enable JSON output, if supported</param>
        /// <param name="debug">Enable debug output</param>
        public void PrintPathInfo(string path, bool json, bool debug)
        {
            Console.WriteLine($"Checking possible path: {path}");

            // Check if the file or directory exists
            if (File.Exists(path))
            {
                PrintFileInfo(path, json, debug);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in IOExtensions.SafeEnumerateFiles(path, "*", SearchOption.AllDirectories))
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
        private void PrintFileInfo(string file, bool json, bool debug)
        {
            Console.WriteLine($"Attempting to print info for {file}");

            try
            {
                using Stream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Get the extension for certain checks
                string extension = Path.GetExtension(file).ToLower().TrimStart('.');

                // Get the first 16 bytes for matching
                byte[] magic = new byte[16];
                try
                {
                    stream.Read(magic, 0, 16);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception ex)
                {
                    if (IncludeDebug) Console.WriteLine(ex);

                    return;
                }

                // Get the file type
                WrapperType ft = WrapperFactory.GetFileType(magic, extension);

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
                    using var jsw = new StreamWriter(File.OpenWrite($"{filenameBase}.json"));
                    jsw.WriteLine(serializedData);
                }
#endif

                // Create the output data
                var builder = SPrinter.ExportStringBuilder(wrapper);
                if (builder == null)
                {
                    Console.WriteLine("No item information could be generated");
                    return;
                }

                // Write the output data
                Console.WriteLine(builder);
                using var sw = new StreamWriter(File.OpenWrite($"{filenameBase}.txt"));
                sw.WriteLine(builder.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(debug ? ex : "[Exception opening file, please try again]");
                Console.WriteLine();
            }
        }
    }
}