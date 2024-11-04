using System;
using System.Collections.Generic;
using System.IO;

namespace ExtractionTool
{
    /// <summary>
    /// Set of options for the test executable
    /// </summary>
    internal sealed class Options
    {
        #region Properties

        /// <summary>
        /// Enable debug output for relevant operations
        /// </summary>
        public bool Debug { get; private set; } = false;

        /// <summary>
        /// Set of input paths to use for operations
        /// </summary>
        public List<string> InputPaths { get; private set; } = [];

        /// <summary>
        /// Output path for archive extraction
        /// </summary>
        public string OutputPath { get; private set; } = string.Empty;

        #endregion

        /// <summary>
        /// Parse commandline arguments into an Options object
        /// </summary>
        public static Options? ParseOptions(string[] args)
        {
            // If we have invalid arguments
            if (args == null || args.Length == 0)
                return null;

            // Create an Options object
            var options = new Options();

            // Parse the options and paths
            for (int index = 0; index < args.Length; index++)
            {
                string arg = args[index];
                switch (arg)
                {
                    case "-?":
                    case "-h":
                    case "--help":
                        return null;

                    case "-d":
                    case "--debug":
                        options.Debug = true;
                        break;

                    case "-o":
                    case "--outdir":
                        options.OutputPath = index + 1 < args.Length ? args[++index] : string.Empty;
                        break;

                    default:
                        options.InputPaths.Add(arg);
                        break;
                }
            }

            // Validate we have any input paths to work on
            if (options.InputPaths.Count == 0)
            {
                Console.WriteLine("At least one path is required!");
                return null;
            }

            // Validate the output path
            bool validPath = ValidateExtractionPath(options);
            if (!validPath)
                return null;

            return options;
        }

        /// <summary>
        /// Display help text
        /// </summary>
        public static void DisplayHelp()
        {
            Console.WriteLine("Extraction Tool");
            Console.WriteLine();
            Console.WriteLine("ExtractionTool.exe <options> file|directory ...");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("-?, -h, --help           Display this help text and quit");
            Console.WriteLine("-d, --debug              Enable debug mode");
            Console.WriteLine("-o, --outdir [PATH]      Set output path for extraction (required)");
        }

        /// <summary>
        /// Validate the extraction path
        /// </summary>
        private static bool ValidateExtractionPath(Options options)
        {
            // Null or empty output path
            if (string.IsNullOrEmpty(options.OutputPath))
            {
                Console.WriteLine("Output directory required for extraction!");
                Console.WriteLine();
                return false;
            }

            // Malformed output path or invalid location
            try
            {
                options.OutputPath = Path.GetFullPath(options.OutputPath);
                Directory.CreateDirectory(options.OutputPath);
            }
            catch
            {
                Console.WriteLine("Output directory could not be created!");
                Console.WriteLine();
                return false;
            }

            return true;
        }
    }
}