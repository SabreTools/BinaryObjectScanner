using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Executable or library
    /// </summary>
    /// <remarks>
    /// Due to the complexity of executables, all extraction handling
    /// another class that is used by the scanner
    /// </remarks>
    public class Executable : IDetectable
    {
        #region Properties

        /// <summary>
        /// Determines if game engines are counted as detected protections or not
        /// </summary>
        public bool IncludeGameEngines { get; set; }

        /// <summary>
        /// Determines if packers are counted as detected protections or not
        /// </summary>
        public bool IncludePackers { get; set; }

        /// <summary>
        /// Cache for all IContentCheck types
        /// </summary>
        public static List<IContentCheck> ContentCheckClasses
        {
            get
            {
                contentCheckClasses ??= Factory.InitCheckClasses<IContentCheck>();
                return contentCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all IExecutableCheck<LinearExecutable> types
        /// </summary>
        public static List<IExecutableCheck<LinearExecutable>> LinearExecutableCheckClasses
        {
            get
            {
                linearExecutableCheckClasses ??= Factory.InitCheckClasses<IExecutableCheck<LinearExecutable>>();
                return linearExecutableCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all IExecutableCheck<MSDOS> types
        /// </summary>
        public static List<IExecutableCheck<MSDOS>> MSDOSExecutableCheckClasses
        {
            get
            {
                msdosExecutableCheckClasses ??= Factory.InitCheckClasses<IExecutableCheck<MSDOS>>();
                return msdosExecutableCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all IExecutableCheck<NewExecutable> types
        /// </summary>
        public static List<IExecutableCheck<NewExecutable>> NewExecutableCheckClasses
        {
            get
            {
                newExecutableCheckClasses ??= Factory.InitCheckClasses<IExecutableCheck<NewExecutable>>();
                return newExecutableCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all IExecutableCheck<PortableExecutable> types
        /// </summary>
        public static List<IExecutableCheck<PortableExecutable>> PortableExecutableCheckClasses
        {
            get
            {
                portableExecutableCheckClasses ??= Factory.InitCheckClasses<IExecutableCheck<PortableExecutable>>();
                return portableExecutableCheckClasses ?? [];
            }
        }

        #endregion

        #region Internal Instances

        /// <summary>
        /// Cache for all IContentCheck types
        /// </summary>
        private static List<IContentCheck>? contentCheckClasses;

        /// <summary>
        /// Cache for all IExecutableCheck<LinearExecutable> types
        /// </summary>
        private static List<IExecutableCheck<LinearExecutable>>? linearExecutableCheckClasses;

        /// <summary>
        /// Cache for all IExecutableCheck<MSDOS> types
        /// </summary>
        private static List<IExecutableCheck<MSDOS>>? msdosExecutableCheckClasses;

        /// <summary>
        /// Cache for all IExecutableCheck<NewExecutable> types
        /// </summary>
        private static List<IExecutableCheck<NewExecutable>>? newExecutableCheckClasses;

        /// <summary>
        /// Cache for all IExecutableCheck<PortableExecutable> types
        /// </summary>
        private static List<IExecutableCheck<PortableExecutable>>? portableExecutableCheckClasses;

        #endregion

        /// <inheritdoc/>
        public string? Detect(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Detect(fs, file, includeDebug);
        }

        /// <inheritdoc/>
        public string? Detect(Stream stream, string file, bool includeDebug)
        {
            // Try to create a wrapper for the proper executable type
            var wrapper = WrapperFactory.CreateExecutableWrapper(stream);
            if (wrapper == null)
                return null;

            // Create the internal list
            var protections = new List<string>();

            // Only use generic content checks if we're in debug mode
            if (includeDebug)
            {
                var subProtections = RunContentChecks(file, stream, includeDebug);
                if (subProtections != null)
                    protections.AddRange(subProtections.Values);
            }

            if (wrapper is MSDOS mz)
            {
                var subProtections = RunMSDOSExecutableChecks(file, stream, mz, includeDebug);
                if (subProtections != null)
                    protections.AddRange(subProtections.Values);
            }
            else if (wrapper is LinearExecutable lex)
            {
                var subProtections = RunLinearExecutableChecks(file, stream, lex, includeDebug);
                if (subProtections != null)
                    protections.AddRange(subProtections.Values);
            }
            else if (wrapper is NewExecutable nex)
            {
                var subProtections = RunNewExecutableChecks(file, stream, nex, includeDebug);
                if (subProtections != null)
                    protections.AddRange(subProtections.Values);
            }
            else if (wrapper is PortableExecutable pex)
            {
                var subProtections = RunPortableExecutableChecks(file, stream, pex, includeDebug);
                if (subProtections != null)
                    protections.AddRange(subProtections.Values);
            }

            return string.Join(";", [.. protections]);
        }

        #region Check Runners

        /// <summary>
        /// Handle a single file based on all content check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
        public IDictionary<IContentCheck, string>? RunContentChecks(string? file, Stream stream, bool includeDebug)
        {
            // If we have an invalid file
            if (string.IsNullOrEmpty(file))
                return null;
            else if (!File.Exists(file))
                return null;

            // Read the file contents
            byte[] fileContent = [];
            try
            {
                fileContent = stream.ReadBytes((int)stream.Length);
                if (fileContent == null)
                    return null;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }

            // Create the output dictionary
            var protections = new CheckDictionary<IContentCheck>();

            // Iterate through all checks
            ContentCheckClasses.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckContents(file!, fileContent, includeDebug);
                if (string.IsNullOrEmpty(protection))
                    return;

                // If we are filtering on game engines
                if (CheckIfGameEngine(checkClass) && !IncludeGameEngines)
                    return;

                // If we are filtering on packers
                if (CheckIfPacker(checkClass) && !IncludePackers)
                    return;

                protections.Append(checkClass, protection);
            });

            return protections;
        }

        /// <summary>
        /// Handle a single file based on all linear executable check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the executable, for tracking</param>
        /// <param name="lex">Executable to scan</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
        public IDictionary<IExecutableCheck<LinearExecutable>, string> RunLinearExecutableChecks(string file, Stream stream, LinearExecutable lex, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new CheckDictionary<IExecutableCheck<LinearExecutable>>();

            // Iterate through all checks
            LinearExecutableCheckClasses.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckExecutable(file, lex, includeDebug);
                if (string.IsNullOrEmpty(protection))
                    return;

                // If we are filtering on game engines
                if (CheckIfGameEngine(checkClass) && !IncludeGameEngines)
                    return;

                // If we are filtering on packers
                if (CheckIfPacker(checkClass) && !IncludePackers)
                    return;

                protections.Append(checkClass, protection);
            });

            return protections;
        }

        /// <summary>
        /// Handle a single file based on all MS-DOS executable check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the executable, for tracking</param>
        /// <param name="mz">Executable to scan</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
        public IDictionary<IExecutableCheck<MSDOS>, string> RunMSDOSExecutableChecks(string file, Stream stream, MSDOS mz, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new CheckDictionary<IExecutableCheck<MSDOS>>();

            // Iterate through all checks
            MSDOSExecutableCheckClasses.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckExecutable(file, mz, includeDebug);
                if (string.IsNullOrEmpty(protection))
                    return;

                // If we are filtering on game engines
                if (CheckIfGameEngine(checkClass) && !IncludeGameEngines)
                    return;

                // If we are filtering on packers
                if (CheckIfPacker(checkClass) && !IncludePackers)
                    return;

                protections.Append(checkClass, protection);
            });

            return protections;
        }

        /// <summary>
        /// Handle a single file based on all new executable check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the executable, for tracking</param>
        /// <param name="nex">Executable to scan</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
        public IDictionary<IExecutableCheck<NewExecutable>, string> RunNewExecutableChecks(string file, Stream stream, NewExecutable nex, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new CheckDictionary<IExecutableCheck<NewExecutable>>();

            // Iterate through all checks
            NewExecutableCheckClasses.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckExecutable(file, nex, includeDebug);
                if (string.IsNullOrEmpty(protection))
                    return;

                // If we are filtering on game engines
                if (CheckIfGameEngine(checkClass) && !IncludeGameEngines)
                    return;

                // If we are filtering on packers
                if (CheckIfPacker(checkClass) && !IncludePackers)
                    return;

                protections.Append(checkClass, protection);
            });

            return protections;
        }

        /// <summary>
        /// Handle a single file based on all portable executable check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the executable, for tracking</param>
        /// <param name="pex">Executable to scan</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
        public IDictionary<IExecutableCheck<PortableExecutable>, string> RunPortableExecutableChecks(string file, Stream stream, PortableExecutable pex, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new CheckDictionary<IExecutableCheck<PortableExecutable>>();

            // Iterate through all checks
            PortableExecutableCheckClasses.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckExecutable(file, pex, includeDebug);
                if (string.IsNullOrEmpty(protection))
                    return;

                // If we are filtering on game engines
                if (CheckIfGameEngine(checkClass) && !IncludeGameEngines)
                    return;

                // If we are filtering on packers
                if (CheckIfPacker(checkClass) && !IncludePackers)
                    return;

                protections.Append(checkClass, protection);
            });

            return protections;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check to see if an implementation is a game engine using reflection
        /// </summary>
        /// <param name="impl">Implementation that was last used to check</param>
        private static bool CheckIfGameEngine(object impl)
        {
            return impl?.GetType()?.Namespace?.ToLowerInvariant()?.Contains("gameengine") ?? false;
        }

        /// <summary>
        /// Check to see if an implementation is a packer using reflection
        /// </summary>
        /// <param name="impl">Implementation that was last used to check</param>
        private static bool CheckIfPacker(object impl)
        {
            return impl.GetType()?.Namespace?.ToLowerInvariant()?.Contains("packer") ?? false;
        }

        #endregion
    }
}
