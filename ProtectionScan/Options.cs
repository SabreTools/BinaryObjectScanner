using System;
using System.Collections.Generic;

namespace ProtectionScan
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

            return options;
        }

        /// <summary>
        /// Display help text
        /// </summary>
        public static void DisplayHelp()
        {
            Console.WriteLine("Protection Scanner");
            Console.WriteLine();
            Console.WriteLine("ProtectionScan.exe <options> file|directory ...");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("-?, -h, --help           Display this help text and quit");
            Console.WriteLine("-d, --debug              Enable debug mode");
            Console.WriteLine("-nc, --no-contents       Disable scanning for content checks");
            Console.WriteLine("-na, --no-archives       Disable scanning archives");
            Console.WriteLine("-ng, --no-game-engines   Disable scanning for game engines");
            Console.WriteLine("-np, --no-packers        Disable scanning for packers");
            Console.WriteLine("-ns, --no-paths          Disable scanning for path checks");
        }
    }
}