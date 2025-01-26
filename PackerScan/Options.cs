using System;
using System.Collections.Generic;

namespace PackerScan
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
        /// Input path to use for detection
        /// </summary>
        public string? InputPath { get; private set; } = null;

        /// <summary>
        /// Scan for self-extracting archives too
        /// </summary>
        public bool ScanArchives { get; private set; } = true;

        /// <summary>
        /// Scan for installers too
        /// </summary>
        public bool ScanInstallers { get; private set; } = true;

        /// <summary>
        /// Scan for other things like embedded archives or executables too
        /// </summary>
        public bool ScanOthers { get; private set; } = true;

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

                    case "-ni":
                    case "--no-installers":
                        options.ScanInstallers = false;
                        break;

                    case "-no":
                    case "--no-others":
                        options.ScanOthers = false;
                        break;

                    default:
                        options.InputPath = arg;
                        break;
                }
            }
            
            // Validate we have any input paths to work on
            if (options.InputPath == null)
            {
                Console.WriteLine("Please provide an input Portable Executable");
                return null;
            }

            return options;
        }

        /// <summary>
        /// Display help text
        /// </summary>
        public static void DisplayHelp()
        {
            Console.WriteLine("Packer Scanner");
            Console.WriteLine();
            Console.WriteLine("PackerScan.exe <options> file|directory ...");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("-?, -h, --help           Display this help text and quit");
            Console.WriteLine("-d, --debug              Enable debug mode");
            Console.WriteLine("-na, --no-archives       Disable scanning for self-extracting archives");
            Console.WriteLine("-ni, --no-installers     Disable scanning for installers");
            Console.WriteLine("-no, --no-others         Disable scanning for other artifacts like embedded archives or executables");
        }
    }
}
