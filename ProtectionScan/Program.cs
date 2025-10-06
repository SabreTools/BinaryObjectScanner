using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner;
using SabreTools.CommandLine;
using SabreTools.CommandLine.Inputs;

namespace ProtectionScan
{
    class Program
    {
        #region Constants

        private const string _debugName = "debug";
        private const string _helpName = "help";
        private const string _noArchivesName = "no-archives";
        private const string _noContentsName = "no-contents";
        private const string _noPathsName = "no-paths";
        private const string _noSubdirsName = "no-subdirs";

        #endregion

        static void Main(string[] args)
        {
#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            // Register the codepages
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif

            // Create progress indicator
            var fileProgress = new Progress<ProtectionProgress>();
            fileProgress.ProgressChanged += Changed;

            // Create the command set
            var commandSet = CreateCommands();

            // If we have no args, show the help and quit
            if (args == null || args.Length == 0)
            {
                commandSet.OutputAllHelp();
                return;
            }

            // Loop through and process the options
            int firstFileIndex = 0;
            for (; firstFileIndex < args.Length; firstFileIndex++)
            {
                string arg = args[firstFileIndex];

                var input = commandSet.GetTopLevel(arg);
                if (input == null)
                    break;

                input.ProcessInput(args, ref firstFileIndex);
            }

            // If help was specified
            if (commandSet.GetBoolean(_helpName))
            {
                commandSet.OutputAllHelp();
                return;
            }

            // Create scanner for all paths
            var scanner = new Scanner(
                !commandSet.GetBoolean(_noArchivesName),
                !commandSet.GetBoolean(_noContentsName),
                !commandSet.GetBoolean(_noPathsName),
                !commandSet.GetBoolean(_noSubdirsName),
                !commandSet.GetBoolean(_debugName),
                fileProgress);

            // Loop through the input paths
            for (int i = firstFileIndex; i < args.Length; i++)
            {
                string arg = args[i];
                GetAndWriteProtections(scanner, arg);
            }
        }

        /// <summary>
        /// Create the command set for the program
        /// </summary>
        private static CommandSet CreateCommands()
        {
            List<string> header = [
                "Protection Scanner",
                string.Empty,
                "ProtectionScan <options> file|directory ...",
                string.Empty,
            ];

            var commandSet = new CommandSet(header);

            commandSet.Add(new FlagInput(_helpName, ["-?", "-h", "--help"], "Display this help text"));
            commandSet.Add(new FlagInput(_debugName, ["-d", "--debug"], "Enable debug mode"));
            commandSet.Add(new FlagInput(_noContentsName, ["-nc", "--no-contents"], "Disable scanning for content checks"));
            commandSet.Add(new FlagInput(_noArchivesName, ["-na", "--no-archives"], "Disable scanning archives"));
            commandSet.Add(new FlagInput(_noPathsName, ["-np", "--no-paths"], "Disable scanning for path checks"));
            commandSet.Add(new FlagInput(_noSubdirsName, ["-ns", "--no-subdirs"], "Disable scanning subdirectories"));

            return commandSet;
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
        private static void WriteProtectionResultFile(string path, Dictionary<string, List<string>> protections)
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
                var value = protections[key];
                if (value.Count == 0)
                    continue;

                // Sort the detected protections for consistent output
                string[] fileProtections = [.. value];
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
