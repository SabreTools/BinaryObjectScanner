using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public static IEnumerable<IContentCheck> ContentCheckClasses
        {
            get
            {
                contentCheckClasses ??= InitCheckClasses<IContentCheck>();
                return contentCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all ILinearExecutableCheck types
        /// </summary>
        public static IEnumerable<ILinearExecutableCheck> LinearExecutableCheckClasses
        {
            get
            {
                linearExecutableCheckClasses ??= InitCheckClasses<ILinearExecutableCheck>();
                return linearExecutableCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all IMSDOSExecutableCheck types
        /// </summary>
        public static IEnumerable<IMSDOSExecutableCheck> MSDOSExecutableCheckClasses
        {
            get
            {
                msdosExecutableCheckClasses ??= InitCheckClasses<IMSDOSExecutableCheck>();
                return msdosExecutableCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all INewExecutableCheck types
        /// </summary>
        public static IEnumerable<INewExecutableCheck> NewExecutableCheckClasses
        {
            get
            {
                newExecutableCheckClasses ??= InitCheckClasses<INewExecutableCheck>();
                return newExecutableCheckClasses ?? [];
            }
        }

        /// <summary>
        /// Cache for all IPortableExecutableCheck types
        /// </summary>
        public static IEnumerable<IPortableExecutableCheck> PortableExecutableCheckClasses
        {
            get
            {
                portableExecutableCheckClasses ??= InitCheckClasses<IPortableExecutableCheck>();
                return portableExecutableCheckClasses ?? [];
            }
        }

        #endregion

        #region Internal Instances

        /// <summary>
        /// Cache for all IContentCheck types
        /// </summary>
        private static IEnumerable<IContentCheck>? contentCheckClasses;

        /// <summary>
        /// Cache for all ILinearExecutableCheck types
        /// </summary>
        private static IEnumerable<ILinearExecutableCheck>? linearExecutableCheckClasses;

        /// <summary>
        /// Cache for all IMSDOSExecutableCheck types
        /// </summary>
        private static IEnumerable<IMSDOSExecutableCheck>? msdosExecutableCheckClasses;

        /// <summary>
        /// Cache for all INewExecutableCheck types
        /// </summary>
        private static IEnumerable<INewExecutableCheck>? newExecutableCheckClasses;

        /// <summary>
        /// Cache for all IPortableExecutableCheck types
        /// </summary>
        private static IEnumerable<IPortableExecutableCheck>? portableExecutableCheckClasses;

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
        public IDictionary<ILinearExecutableCheck, string> RunLinearExecutableChecks(string file, Stream stream, LinearExecutable lex, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new CheckDictionary<ILinearExecutableCheck>();

            // Iterate through all checks
            LinearExecutableCheckClasses.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckLinearExecutable(file, lex, includeDebug);
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
        public IDictionary<IMSDOSExecutableCheck, string> RunMSDOSExecutableChecks(string file, Stream stream, MSDOS mz, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new CheckDictionary<IMSDOSExecutableCheck>();

            // Iterate through all checks
            MSDOSExecutableCheckClasses.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckMSDOSExecutable(file, mz, includeDebug);
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
        public IDictionary<INewExecutableCheck, string> RunNewExecutableChecks(string file, Stream stream, NewExecutable nex, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new CheckDictionary<INewExecutableCheck>();

            // Iterate through all checks
            NewExecutableCheckClasses.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckNewExecutable(file, nex, includeDebug);
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
        public IDictionary<IPortableExecutableCheck, string> RunPortableExecutableChecks(string file, Stream stream, PortableExecutable pex, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new CheckDictionary<IPortableExecutableCheck>();

            // Iterate through all checks
            PortableExecutableCheckClasses.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckPortableExecutable(file, pex, includeDebug);
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

        #region Initializers

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
        private static IEnumerable<T>? InitCheckClasses<T>() =>
            InitCheckClasses<T>(Assembly.GetExecutingAssembly()) ?? [];

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
        private static IEnumerable<T>? InitCheckClasses<T>(Assembly assembly)
        {
            List<T> classTypes = [];

            // If not all types can be loaded, use the ones that could be
            List<Type> assemblyTypes = [];
            try
            {
                assemblyTypes = assembly.GetTypes().ToList<Type>();
            }
            catch (ReflectionTypeLoadException rtle)
            {
                assemblyTypes = rtle.Types.Where(t => t != null)!.ToList<Type>();
            }

            // Loop through all types 
            foreach (Type type in assemblyTypes)
            {
                // If the type isn't a class or doesn't implement the interface
                if (!type.IsClass || type.GetInterface(typeof(T).Name) == null)
                    continue;

                // Try to create a concrete instance of the type
                var instance = (T?)Activator.CreateInstance(type);
                if (instance != null)
                    classTypes.Add(instance);
            }

            return classTypes;
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
