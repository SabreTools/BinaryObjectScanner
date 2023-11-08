using System;
using System.Collections.Generic;
using System.IO;

namespace Test
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
        public List<string> InputPaths { get; private set; } = new List<string>();

        #region Extraction

        /// <summary>
        /// Perform archive extraction
        /// </summary>
        public bool EnableExtraction { get; private set; } = false;

        /// <summary>
        /// Output path for archive extraction
        /// </summary>
        public string OutputPath { get; private set; } = string.Empty;

        #endregion

        #region Information

        /// <summary>
        /// Perform information printing
        /// </summary>
        public bool EnableInformation { get; private set; } = false;

#if NET6_0_OR_GREATER
        /// <summary>
        /// Enable JSON output
        /// </summary>
        public bool Json { get; private set; } = false;
#endif

        #endregion

        #region Scanning

        /// <summary>
        /// Perform protection scanning
        /// </summary>
        public bool EnableScanning { get; private set; } = false;

        /// <summary>
        /// Scan archives during protection scanning
        /// </summary>
        public bool ScanArchives { get; private set; } = true;

        /// <summary>
        /// Scan file contents during protection scanning
        /// </summary>
        public bool ScanContents { get; private set; } = true;

        /// <summary>
        /// Scan game engines during protection scanning
        /// </summary>
        public bool ScanGameEngines { get; private set; } = true;

        /// <summary>
        /// Scan packers during protection scanning
        /// </summary>
        public bool ScanPackers { get; private set; } = true;

        /// <summary>
        /// Scan file paths during protection scanning
        /// </summary>
        public bool ScanPaths { get; private set; } = true;

        #endregion

        #endregion

        /// <summary>
        /// Parse commandline arguments into an Options object
        /// </summary>
        public static Options ParseOptions(string[] args)
        {
            // If we have invalid arguments
            if (args == null || args.Length == 0)
                return null;

            // Create an Options object
            var options = new Options();

            // Parse the features
            int index = 0;
            for (; index < args.Length; index++)
            {
                string arg = args[index];
                bool featureFound = false;
                switch (arg)
                {
                    case "-?":
                    case "-h":
                    case "--help":
                        return null;

                    case "-x":
                    case "--extract":
                        options.EnableExtraction = true;
                        featureFound = true;
                        break;

                    case "-i":
                    case "--info":
                        options.EnableInformation = true;
                        featureFound = true;
                        break;

                    case "-s":
                    case "--scan":
                        options.EnableScanning = true;
                        featureFound = true;
                        break;

                    default:
                        break;
                }

                // If the flag wasn't a feature
                if (!featureFound)
                    break;
            }

            // Parse the options and paths
            for (; index < args.Length; index++)
            {
                string arg = args[index];
                switch (arg)
                {
                    case "-d":
                    case "--debug":
                        options.Debug = true;
                        break;

                    #region Extraction

                    case "-o":
                    case "--outdir":
                        options.OutputPath = index + 1 < args.Length ? args[++index] : null;
                        break;

                    #endregion

                    #region Information

                    case "-j":
                    case "--json":
#if NET6_0_OR_GREATER
                        options.Json = true;
#else
                        Console.WriteLine("JSON output not available in .NET Framework 4.8");
#endif
                        break;


                    #endregion

                    #region Scanning

                    case "-na":
                    case "--no-archives":
                        options.ScanArchives = false;
                        break;

                    case "-nc":
                    case "--no-contents":
                        options.ScanContents = false;
                        break;

                    case "-ng":
                    case "--no-game-engines":
                        options.ScanGameEngines = false;
                        break;

                    case "-np":
                    case "--no-packers":
                        options.ScanPackers = false;
                        break;

                    case "-ns":
                    case "--no-paths":
                        options.ScanPaths = false;
                        break;

                    #endregion

                    default:
                        options.InputPaths.Add(arg);
                        break;
                }
            }

            // If we have no features set, enable protection scanning
            if (!options.EnableExtraction && !options.EnableInformation && !options.EnableScanning)
                options.EnableScanning = true;

            // Validate we have any input paths to work on
            if (options.InputPaths.Count == 0)
            {
                Console.WriteLine("At least one path is required!");
                return null;
            }

            // If we have extraction enabled, validate the path
            if (options.EnableExtraction)
            {
                bool validPath = ValidateExtractionPath(options);
                if (!validPath)
                    return null;
            }

            return options;
        }

        /// <summary>
        /// Display help text
        /// </summary>
        public static void DisplayHelp()
        {
            Console.WriteLine("BinaryObjectScanner Test Program");
            Console.WriteLine();
            Console.WriteLine("test.exe <features> <options> file|directory ...");
            Console.WriteLine();
            Console.WriteLine("Features:");
            Console.WriteLine("-x, --extract            Extract archive formats");
            Console.WriteLine("-i, --info               Print executable info");
            Console.WriteLine("-s, --scan               Enable protection scanning (default if none)");
            Console.WriteLine();
            Console.WriteLine("Common options:");
            Console.WriteLine("-?, -h, --help           Display this help text and quit");
            Console.WriteLine("-d, --debug              Enable debug mode");
            Console.WriteLine();
            Console.WriteLine("Extraction options:");
            Console.WriteLine("-o, --outdir [PATH]      Set output path for extraction (required)");
#if NET6_0_OR_GREATER
            Console.WriteLine();
            Console.WriteLine("Information options:");
            Console.WriteLine("-j, --json               Print executable info as JSON");
#endif
            Console.WriteLine();
            Console.WriteLine("Scanning options:");
            Console.WriteLine("-nc, --no-contents       Disable scanning for content checks");
            Console.WriteLine("-na, --no-archives       Disable scanning archives");
            Console.WriteLine("-ng, --no-game-engines   Disable scanning for game engines");
            Console.WriteLine("-np, --no-packers        Disable scanning for packers");
            Console.WriteLine("-ns, --no-paths          Disable scanning for path checks");
        }

        /// <summary>
        /// Validate the extraction path
        /// </summary>
        private static bool ValidateExtractionPath(Options options)
        {
            // Null or empty output path
            if (string.IsNullOrWhiteSpace(options.OutputPath))
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