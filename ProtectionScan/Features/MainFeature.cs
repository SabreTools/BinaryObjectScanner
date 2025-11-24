using System;
using System.Collections.Generic;
using System.IO;
#if NETCOREAPP
using System.Text.Json;
#endif
using BinaryObjectScanner;
using SabreTools.CommandLine;
using SabreTools.CommandLine.Inputs;

namespace ProtectionScan.Features
{
    internal sealed class MainFeature : Feature
    {
        #region Feature Definition

        public const string DisplayName = "main";

        /// <remarks>Flags are unused</remarks>
        private static readonly string[] _flags = [];

        /// <remarks>Description is unused</remarks>
        private const string _description = "";

        #endregion

        #region Inputs

        private const string _debugName = "debug";
        internal readonly FlagInput DebugInput = new(_debugName, ["-d", "--debug"], "Enable debug mode");

        private const string _fileOnlyName = "file-only";
        internal readonly FlagInput FileOnlyInput = new(_fileOnlyName, ["-f", "--file"], "Print to file only");

#if NETCOREAPP
        private const string _jsonName = "json";
        internal readonly FlagInput JsonInput = new(_jsonName, ["-j", "--json"], "Output to json file");

        private const string _nestedName = "nested";
        internal readonly FlagInput NestedInput = new(_nestedName, ["-n", "--nested"], "Output to nested json file");
#endif

        private const string _noArchivesName = "no-archives";
        internal readonly FlagInput NoArchivesInput = new(_noArchivesName, ["-na", "--no-archives"], "Disable scanning archives");

        private const string _noContentsName = "no-contents";
        internal readonly FlagInput NoContentsInput = new(_noContentsName, ["-nc", "--no-contents"], "Disable scanning for content checks");

        private const string _noPathsName = "no-paths";
        internal readonly FlagInput NoPathsInput = new(_noPathsName, ["-np", "--no-paths"], "Disable scanning for path checks");

        private const string _noSubdirsName = "no-subdirs";
        internal readonly FlagInput NoSubdirsInput = new(_noSubdirsName, ["-ns", "--no-subdirs"], "Disable scanning subdirectories");

        #endregion

        /// <summary>
        /// Enable debug output for relevant operations
        /// </summary>
        public bool Debug { get; private set; }

        /// <summary>
        /// Output information to file only, skip printing to console
        /// </summary>
        public bool FileOnly { get; private set; }

#if NETCOREAPP
        /// <summary>
        /// Enable JSON output
        /// </summary>
        public bool Json { get; private set; }

        /// <summary>
        /// Enable nested JSON output
        /// </summary>
        public bool Nested { get; private set; }
#endif

        public MainFeature()
            : base(DisplayName, _flags, _description)
        {
            RequiresInputs = true;

            Add(DebugInput);
            Add(FileOnlyInput);
#if NETCOREAPP
            JsonInput.Add(NestedInput);
            Add(JsonInput);
#endif
            Add(NoContentsInput);
            Add(NoArchivesInput);
            Add(NoPathsInput);
            Add(NoSubdirsInput);
        }

        /// <inheritdoc/>
        public override bool Execute()
        {
            // Create progress indicator
            var fileProgress = new Progress<ProtectionProgress>();
            fileProgress.ProgressChanged += Changed;

            // Get the options from the arguments
            Debug = GetBoolean(_debugName);
            FileOnly = GetBoolean(_fileOnlyName);
#if NETCOREAPP
            Json = GetBoolean(_jsonName);
            Nested = GetBoolean(_nestedName);
#endif

            // Create scanner for all paths
            var scanner = new Scanner(
                !GetBoolean(_noArchivesName),
                !GetBoolean(_noContentsName),
                !GetBoolean(_noPathsName),
                !GetBoolean(_noSubdirsName),
                GetBoolean(_debugName),
                fileProgress);

            // Loop through the input paths
            for (int i = 0; i < Inputs.Count; i++)
            {
                string arg = Inputs[i];
                GetAndWriteProtections(scanner, arg);
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool VerifyInputs() => Inputs.Count > 0;

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

        /// <summary>
        /// Wrapper to get and log protections for a single path
        /// </summary>
        /// <param name="scanner">Scanner object to use</param>
        /// <param name="path">File or directory path</param>
        private void GetAndWriteProtections(Scanner scanner, string path)
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

                WriteProtectionResults(path, protections);
#if NETCOREAPP
                if (Json)
                    WriteProtectionResultJson(path, protections);
#endif
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
        private void WriteProtectionResults(string path, Dictionary<string, List<string>> protections)
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
                FileOnly = false;
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

                // Only print to console if enabled
                if (!FileOnly)
                    Console.WriteLine(line);

                sw?.WriteLine(line);
                sw?.Flush();
            }

            // Dispose of the writer
            sw?.Dispose();
        }

#if NETCOREAPP
        /// <summary>
        /// Write the protection results from a single path to a json file, if possible
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="protections">Dictionary of protections found, if any</param>
        private void WriteProtectionResultJson(string path, Dictionary<string, List<string>> protections)
        {
            if (protections == null)
            {
                Console.WriteLine($"No protections found for {path}");
                return;
            }

            try
            {
                var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
                string serializedData;

                if (Nested)
                {
                    // A nested dictionary is used to achieve proper serialization.
                    Dictionary<string, object> nestedDictionary = [];
                    path = path.TrimEnd(['\\', '/']);

                    // Sort the keys for consistent output
                    string[] keys = [.. protections.Keys];
                    Array.Sort(keys);

                    var modifyNodeList = new List<(Dictionary<string, object>, string, string[])>();

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

                        // Inserts key and protections into nested dictionary, with the key trimmed of the base path.
                        InsertNode(nestedDictionary, key, path, fileProtections, modifyNodeList);
                    }

                    // Adds the non-leaf-node protections back in
                    for (int i = 0; i < modifyNodeList.Count; i++)
                    {
                        List<object> modifyNode =
                        [
                            modifyNodeList[i].Item3,
                            modifyNodeList[i].Item1[modifyNodeList[i].Item2],
                        ];

                        modifyNodeList[i].Item1[modifyNodeList[i].Item2] = modifyNode;
                    }

                    // Create the output data
                    serializedData = JsonSerializer.Serialize(nestedDictionary, jsonSerializerOptions);
                }
                else
                {
                    // Create the output data
                    serializedData = JsonSerializer.Serialize(protections, jsonSerializerOptions);
                }

                // Write the output data
                // TODO: This prints plus symbols wrong, probably some other things too
                using var jsw = new StreamWriter(File.OpenWrite($"protection-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}.json"));
                jsw.WriteLine(serializedData);
                jsw.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(Debug ? ex : "[Exception opening file, please try again]");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Inserts file protection dictionary entries into a nested dictionary based on path
        /// </summary>
        /// <param name="dict">Existing output dictionary</param>
        /// <param name="key">The key for the given protection entry</param>
        /// <param name="path">Original base path used for scanning</param>
        /// <param name="protections">Set of protections found, if any</param>
        /// <param name="modifyNodeList">Set representing overlapping nodes to be processed after</param>
        private static void InsertNode(Dictionary<string, object> dict,
            string key,
            string path,
            string[] protections,
            List<(Dictionary<string, object>, string, string[])> modifyNodeList)
        {
            // Remove the base path from the key for processing
            key = key[path.Length..];

            // Split the input path, if possible
            var pathParts = key.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            if (pathParts.Length == 0)
            {
                modifyNodeList.Add((dict, path, protections));
                return;
            }

            // Create the node if it doesn't already exist
            if (!dict.ContainsKey(path))
                dict[path] = new Dictionary<string, object>();

            var current = (Dictionary<string, object>)dict[path];

            // Traverses the nested dictionary until the "leaf" dictionary is reached.
            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                var part = pathParts[i];

                // Inserts new subdictionaries if one doesn't already exist
                if (!current.TryGetValue(part, out object? innerObject))
                {
                    var innerDictionary = new Dictionary<string, object>();
                    innerObject = innerDictionary;
                    current[part] = innerObject;
                    current = innerDictionary;
                    continue;
                }

                if (innerObject is string[] existingProtections)
                {
                    modifyNodeList.Add((current, part, existingProtections));
                    innerObject = new Dictionary<string, object>();
                }

                current[part] = innerObject;
                current = (Dictionary<string, object>)current[part];
            }

            // If the "leaf" dictionary has been reached, add the file and its protections.
            if (current.ContainsKey(pathParts[^1]) && current[pathParts[^1]] is string[] existing)
            {
                string[] combined = [.. existing, .. protections];
                current[pathParts[^1]] = combined;
            }
            else
            {
                current.Add(pathParts[^1], protections);
            }
        }
#endif
    }
}
