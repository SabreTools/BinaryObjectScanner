using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Utilities;
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

            // Create the internal queue
#if NET20 || NET35
            var protections = new Queue<string>();
#else
            var protections = new ConcurrentQueue<string>();
#endif

            // Only use generic content checks if we're in debug mode
            if (includeDebug)
            {
                var subProtections = RunContentChecks(file, stream, includeDebug);
                if (subProtections != null)
                    protections.AddRange(subProtections.Values.ToArray());
            }

            if (wrapper is MSDOS mz)
            {
                var subProtections = RunMSDOSExecutableChecks(file, stream, mz, includeDebug);
                if (subProtections != null)
                    protections.AddRange(subProtections.Values.ToArray());
            }
            else if (wrapper is LinearExecutable lex)
            {
                var subProtections = RunLinearExecutableChecks(file, stream, lex, includeDebug);
                if (subProtections != null)
                    protections.AddRange(subProtections.Values.ToArray());
            }
            else if (wrapper is NewExecutable nex)
            {
                var subProtections = RunNewExecutableChecks(file, stream, nex, includeDebug);
                if (subProtections != null)
                    protections.AddRange(subProtections.Values.ToArray());
            }
            else if (wrapper is PortableExecutable pex)
            {
                var subProtections = RunPortableExecutableChecks(file, stream, pex, includeDebug);
                if (subProtections != null)
                    protections.AddRange(subProtections.Values.ToArray());
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
#if NET20 || NET35
        public Dictionary<IContentCheck, string>? RunContentChecks(string? file, Stream stream, bool includeDebug)
#else
        public ConcurrentDictionary<IContentCheck, string>? RunContentChecks(string? file, Stream stream, bool includeDebug)
#endif
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
#if NET20 || NET35 || NET40
                using var br = new BinaryReader(stream, Encoding.Default);
#else
                using var br = new BinaryReader(stream, Encoding.Default, true);
#endif
                fileContent = br.ReadBytes((int)stream.Length);
                if (fileContent == null)
                    return null;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }

            // Create the output dictionary
#if NET20 || NET35
            var protections = new Dictionary<IContentCheck, string>();
#else
            var protections = new ConcurrentDictionary<IContentCheck, string>();
#endif

            // Iterate through all checks
#if NET20 || NET35
            foreach (var checkClass in ContentCheckClasses)
#else
            Parallel.ForEach(ContentCheckClasses, checkClass =>
#endif
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckContents(file!, fileContent, includeDebug);
                if (string.IsNullOrEmpty(protection))
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // If we are filtering on game engines
                if (CheckIfGameEngine(checkClass) && !IncludeGameEngines)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // If we are filtering on packers
                if (CheckIfPacker(checkClass) && !IncludePackers)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

#if NET20 || NET35
                protections[checkClass] = protection!;
            }
#else
                protections.TryAdd(checkClass, protection!);
            });
#endif

            return protections;
        }

        /// <summary>
        /// Handle a single file based on all linear executable check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the executable, for tracking</param>
        /// <param name="lex">Executable to scan</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
#if NET20 || NET35
        public Dictionary<ILinearExecutableCheck, string> RunLinearExecutableChecks(string file, Stream stream, LinearExecutable lex, bool includeDebug)
#else
        public ConcurrentDictionary<ILinearExecutableCheck, string> RunLinearExecutableChecks(string file, Stream stream, LinearExecutable lex, bool includeDebug)
#endif
        {
            // Create the output dictionary
#if NET20 || NET35
            var protections = new Dictionary<ILinearExecutableCheck, string>();
#else
            var protections = new ConcurrentDictionary<ILinearExecutableCheck, string>();
#endif

            // Iterate through all checks
#if NET20 || NET35
            foreach (var checkClass in LinearExecutableCheckClasses)
#else
            Parallel.ForEach(LinearExecutableCheckClasses, checkClass =>
#endif
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckLinearExecutable(file, lex, includeDebug);
                if (string.IsNullOrEmpty(protection))
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // If we are filtering on game engines
                if (CheckIfGameEngine(checkClass) && !IncludeGameEngines)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // If we are filtering on packers
                if (CheckIfPacker(checkClass) && !IncludePackers)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

#if NET20 || NET35
                protections[checkClass] = protection!;
            }
#else
                protections.TryAdd(checkClass, protection!);
            });
#endif

            return protections;
        }

        /// <summary>
        /// Handle a single file based on all MS-DOS executable check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the executable, for tracking</param>
        /// <param name="mz">Executable to scan</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
#if NET20 || NET35
        public Dictionary<IMSDOSExecutableCheck, string> RunMSDOSExecutableChecks(string file, Stream stream, MSDOS mz, bool includeDebug)
#else
        public ConcurrentDictionary<IMSDOSExecutableCheck, string> RunMSDOSExecutableChecks(string file, Stream stream, MSDOS mz, bool includeDebug)
#endif
        {
            // Create the output dictionary
#if NET20 || NET35
            var protections = new Dictionary<IMSDOSExecutableCheck, string>();
#else
            var protections = new ConcurrentDictionary<IMSDOSExecutableCheck, string>();
#endif

            // Iterate through all checks
#if NET20 || NET35
            foreach (var checkClass in MSDOSExecutableCheckClasses)
#else
            Parallel.ForEach(MSDOSExecutableCheckClasses, checkClass =>
#endif
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckMSDOSExecutable(file, mz, includeDebug);
                if (string.IsNullOrEmpty(protection))
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // If we are filtering on game engines
                if (CheckIfGameEngine(checkClass) && !IncludeGameEngines)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // If we are filtering on packers
                if (CheckIfPacker(checkClass) && !IncludePackers)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

#if NET20 || NET35
                protections[checkClass] = protection!;
            }
#else
                protections.TryAdd(checkClass, protection!);
            });
#endif

            return protections;
        }

        /// <summary>
        /// Handle a single file based on all new executable check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the executable, for tracking</param>
        /// <param name="nex">Executable to scan</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
#if NET20 || NET35
        public Dictionary<INewExecutableCheck, string> RunNewExecutableChecks(string file, Stream stream, NewExecutable nex, bool includeDebug)
#else
        public ConcurrentDictionary<INewExecutableCheck, string> RunNewExecutableChecks(string file, Stream stream, NewExecutable nex, bool includeDebug)
#endif
        {
            // Create the output dictionary
#if NET20 || NET35
            var protections = new Dictionary<INewExecutableCheck, string>();
#else
            var protections = new ConcurrentDictionary<INewExecutableCheck, string>();
#endif

            // Iterate through all checks
#if NET20 || NET35
            foreach (var checkClass in NewExecutableCheckClasses)
#else
            Parallel.ForEach(NewExecutableCheckClasses, checkClass =>
#endif
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckNewExecutable(file, nex, includeDebug);
                if (string.IsNullOrEmpty(protection))
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // If we are filtering on game engines
                if (CheckIfGameEngine(checkClass) && !IncludeGameEngines)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // If we are filtering on packers
                if (CheckIfPacker(checkClass) && !IncludePackers)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

#if NET20 || NET35
                protections[checkClass] = protection!;
            }
#else
                protections.TryAdd(checkClass, protection!);
            });
#endif

            return protections;
        }

        /// <summary>
        /// Handle a single file based on all portable executable check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the executable, for tracking</param>
        /// <param name="pex">Executable to scan</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
#if NET20 || NET35
        public Dictionary<IPortableExecutableCheck, string> RunPortableExecutableChecks(string file, Stream stream, PortableExecutable pex, bool includeDebug)
#else
        public ConcurrentDictionary<IPortableExecutableCheck, string> RunPortableExecutableChecks(string file, Stream stream, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Create the output dictionary
#if NET20 || NET35
            var protections = new Dictionary<IPortableExecutableCheck, string>();
#else
            var protections = new ConcurrentDictionary<IPortableExecutableCheck, string>();
#endif

            // Iterate through all checks
#if NET20 || NET35
            foreach (var checkClass in PortableExecutableCheckClasses)
#else
            Parallel.ForEach(PortableExecutableCheckClasses, checkClass =>
#endif
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckPortableExecutable(file, pex, includeDebug);
                if (string.IsNullOrEmpty(protection))
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // If we are filtering on game engines
                if (CheckIfGameEngine(checkClass) && !IncludeGameEngines)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

                // If we are filtering on packers
                if (CheckIfPacker(checkClass) && !IncludePackers)
#if NET20 || NET35
                    continue;
#else
                    return;
#endif

#if NET20 || NET35
                protections[checkClass] = protection!;
            }
#else
                protections.TryAdd(checkClass, protection!);
            });
#endif

            return protections;
        }

#endregion

        #region Initializers

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
        private static IEnumerable<T>? InitCheckClasses<T>() =>
            InitCheckClasses<T>(typeof(Handler).Assembly) ?? [];

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
        private static IEnumerable<T>? InitCheckClasses<T>(Assembly assembly)
        {
            List<T?> types = [];
            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass && type.GetInterface(typeof(T).Name) != null)
                    {
                        var instance = (T?)Activator.CreateInstance(type);
                        if (instance != null)
                            types.Add(instance);
                    }
                }
            }
            catch { }

            return types;
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
