using System;
using System.Collections.Generic;
using System.IO;
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
            Add(JsonInput);
            Add(NestedInput);
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
                if (Nested)
                    WriteProtectionResultNestedJson(path, protections);
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
                // Attempt to open a protection file for writing
                using var jsw = new StreamWriter(File.OpenWrite($"protection-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}.json"));

                // Create the output data
                var jsonSerializerOptions = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                string serializedData = System.Text.Json.JsonSerializer.Serialize(protections, jsonSerializerOptions);

                // Write the output data
                // TODO: this prints plus symbols wrong, probably some other things too
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
        /// Write the protection results from a single path to a nested json file, if possible
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <param name="protections">Dictionary of protections found, if any</param>
        private void WriteProtectionResultNestedJson(string path, Dictionary<string, List<string>> protections)
        {
            if (protections == null)
            {
                Console.WriteLine($"No protections found for {path}");
                return;
            }

            try
            {
                // Attempt to open a protection file for writing
                using var jsw = new StreamWriter(File.OpenWrite($"protection-{DateTime.Now:yyyy-MM-dd_HHmmss.ffff}.json"));

                // A nested dictionary is used in order to avoid complex and unnecessary custom serialization.
                // A dictionary with an object value is used so that it's not necessary to first parse entries into a 
                // traditional node system and then bubble up the entire chain creating non-object dictionaries.
                var nestedDictionary = new Dictionary<string, object>();
                var trimmedPath = path.TrimEnd(['\\', '/']); 
                
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
                    //foreach (var fileProtection in fileProtections)

                    // Inserts key and protections into nested dictionary, with the key trimmed of the base path.
                    DeepInsert(ref nestedDictionary, key.Substring(trimmedPath.Length), fileProtections);
                }

                // While it's possible to hardcode the root dictionary key to be changed to the base path beforehand, 
                // it's cleaner to avoid trying to circumvent the path splitting logic, and just move the root 
                // dictionary value into an entry with the base path as the key.
                // There is no input as far as has been tested that can result in there not being a root dictionary key
                // of an empty string, so this is safe.
                // The only exception is if absolutely no protections were returned whatsoever, which is why there's a
                // safeguard here at all
                
                var finalDictionary = new Dictionary<string, Dictionary<string, object>>()
                {
                    {trimmedPath, nestedDictionary}
                };
                
                // Create the output data
                var jsonSerializerOptions = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                string serializedData = System.Text.Json.JsonSerializer.Serialize(finalDictionary, jsonSerializerOptions);

                // Write the output data
                // TODO: this prints plus symbols wrong, probably some other things too
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
        /// <param name="nestedDictionary">File or directory path</param>
        /// <param name="path">The "key" for the given protection entry, already trimmed of its base path</param>
        /// <param name="protections">The scanned protection(s) for a given file</param>
        public static void DeepInsert(ref Dictionary<string, object> nestedDictionary, string path, string[] protections)
        {
            var current = nestedDictionary; 
            path = path.TrimStart(Path.DirectorySeparatorChar);
            var pathParts = path.Split(Path.DirectorySeparatorChar); 

            // Traverses the nested dictionary until the "leaf" dictionary is reached.
            for (int i = 0; i < pathParts.Length; i++)
            {
                var part = pathParts[i];
                if (i != (pathParts.Length - 1))
                {
                    if (!current.ContainsKey(part)) // Inserts new subdictionaries if one doesn't already exist
                    {
                        var innerObject = new Dictionary<string, object>();
                        current[part] = innerObject;
                        current =  innerObject;
                    }
                    else // Traverses already existing subdictionaries
                    {
                        var innerObject = current[part];
                        
                        // If i.e. a packer has protections detected on it, and then files within it also have 
                        // detections of their own, the later traversal of the files within it will fail, as
                        // the subdictionary for that packer has already been set to <string, string>. Since it's
                        // no longer object after being assigned once, the existing value must be pulled, then the
                        // new subdictionary can be added, and then the existing value can be re-added within the
                        // packer with a key of an empty string, in order to indicate it's for the packer itself, and
                        // to avoid potential future collisions.
                        if (innerObject.GetType() != typeof(Dictionary<string, object>))
                        {
                            current[part] = new Dictionary<string, object>();
                            current = (Dictionary<string, object>)current[part];
                            current.Add("", innerObject);
                        }
                        else
                        {
                            current[part] = innerObject;
                            current =  (Dictionary<string, object>)innerObject;       
                        }
                    }
                }
                else // If the "leaf" dictionary has been reached, add the file and its protections.
                {
                    current.Add(part, protections);
                }
            }
        }
#endif
    }
}
