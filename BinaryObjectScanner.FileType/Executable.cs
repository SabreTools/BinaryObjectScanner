using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Utilities;
using BinaryObjectScanner.Wrappers;

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
                if (contentCheckClasses == null)
                    contentCheckClasses = InitCheckClasses<IContentCheck>();

                return contentCheckClasses;
            }
        }

        /// <summary>
        /// Cache for all ILinearExecutableCheck types
        /// </summary>
        public static IEnumerable<ILinearExecutableCheck> LinearExecutableCheckClasses
        {
            get
            {
                if (linearExecutableCheckClasses == null)
                    linearExecutableCheckClasses = InitCheckClasses<ILinearExecutableCheck>();

                return linearExecutableCheckClasses;
            }
        }

        /// <summary>
        /// Cache for all IMSDOSExecutableCheck types
        /// </summary>
        public static IEnumerable<IMSDOSExecutableCheck> MSDOSExecutableCheckClasses
        {
            get
            {
                if (msdosExecutableCheckClasses == null)
                    msdosExecutableCheckClasses = InitCheckClasses<IMSDOSExecutableCheck>();

                return msdosExecutableCheckClasses;
            }
        }

        /// <summary>
        /// Cache for all INewExecutableCheck types
        /// </summary>
        public static IEnumerable<INewExecutableCheck> NewExecutableCheckClasses
        {
            get
            {
                if (newExecutableCheckClasses == null)
                    newExecutableCheckClasses = InitCheckClasses<INewExecutableCheck>();

                return newExecutableCheckClasses;
            }
        }

        /// <summary>
        /// Cache for all IPortableExecutableCheck types
        /// </summary>
        public static IEnumerable<IPortableExecutableCheck> PortableExecutableCheckClasses
        {
            get
            {
                if (portableExecutableCheckClasses == null)
                    portableExecutableCheckClasses = InitCheckClasses<IPortableExecutableCheck>();

                return portableExecutableCheckClasses;
            }
        }

        #endregion

        #region Internal Instances

        /// <summary>
        /// Cache for all IContentCheck types
        /// </summary>
        private static IEnumerable<IContentCheck> contentCheckClasses;

        /// <summary>
        /// Cache for all ILinearExecutableCheck types
        /// </summary>
        private static IEnumerable<ILinearExecutableCheck> linearExecutableCheckClasses;

        /// <summary>
        /// Cache for all IMSDOSExecutableCheck types
        /// </summary>
        private static IEnumerable<IMSDOSExecutableCheck> msdosExecutableCheckClasses;

        /// <summary>
        /// Cache for all INewExecutableCheck types
        /// </summary>
        private static IEnumerable<INewExecutableCheck> newExecutableCheckClasses;

        /// <summary>
        /// Cache for all IPortableExecutableCheck types
        /// </summary>
        private static IEnumerable<IPortableExecutableCheck> portableExecutableCheckClasses;

        #endregion

        /// <inheritdoc/>
        public string Detect(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Detect(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string Detect(Stream stream, string file, bool includeDebug)
        {
            // Try to create a wrapper for the proper executable type
            var wrapper = WrapperFactory.CreateExecutableWrapper(stream);
            if (wrapper == null)
                return null;

            // Create the internal queue
            var protections = new ConcurrentQueue<string>();

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

            return string.Join(";", protections);
        }

        #region Check Runners

        /// <summary>
        /// Handle a single file based on all content check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
        public ConcurrentDictionary<IContentCheck, string> RunContentChecks(string file, Stream stream, bool includeDebug)
        {
            // If we have an invalid file
            if (string.IsNullOrWhiteSpace(file))
                return null;
            else if (!File.Exists(file))
                return null;

            // Read the file contents
            byte[] fileContent = null;
            try
            {
                using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
                {
                    fileContent = br.ReadBytes((int)stream.Length);
                    if (fileContent == null)
                        return null;
                }
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }

            // Create the output dictionary
            var protections = new ConcurrentDictionary<IContentCheck, string>();

            // Iterate through all checks
            Parallel.ForEach(ContentCheckClasses, checkClass =>
            {
                // Get the protection for the class, if possible
                string protection = checkClass.CheckContents(file, fileContent, includeDebug);
                if (string.IsNullOrWhiteSpace(protection))
                    return;

                // If we are filtering the output of the check
                if (!CheckIfPacker(checkClass) || !IncludePackers)
                    return;

                protections.TryAdd(checkClass, protection);
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
        public ConcurrentDictionary<ILinearExecutableCheck, string> RunLinearExecutableChecks(string file, Stream stream, LinearExecutable lex, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new ConcurrentDictionary<ILinearExecutableCheck, string>();

            // Iterate through all checks
            Parallel.ForEach(LinearExecutableCheckClasses, checkClass =>
            {
                // Get the protection for the class, if possible
                string protection = checkClass.CheckLinearExecutable(file, lex, includeDebug);
                if (string.IsNullOrWhiteSpace(protection))
                    return;

                // If we are filtering the output of the check
                if (!CheckIfPacker(checkClass) || !IncludePackers)
                    return;

                protections.TryAdd(checkClass, protection);
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
        public ConcurrentDictionary<IMSDOSExecutableCheck, string> RunMSDOSExecutableChecks(string file, Stream stream, MSDOS mz, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new ConcurrentDictionary<IMSDOSExecutableCheck, string>();

            // Iterate through all checks
            Parallel.ForEach(MSDOSExecutableCheckClasses, checkClass =>
            {
                // Get the protection for the class, if possible
                string protection = checkClass.CheckMSDOSExecutable(file, mz, includeDebug);
                if (string.IsNullOrWhiteSpace(protection))
                    return;

                // If we are filtering the output of the check
                if (!CheckIfPacker(checkClass) || !IncludePackers)
                    return;

                protections.TryAdd(checkClass, protection);
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
        public ConcurrentDictionary<INewExecutableCheck, string> RunNewExecutableChecks(string file, Stream stream, NewExecutable nex, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new ConcurrentDictionary<INewExecutableCheck, string>();

            // Iterate through all checks
            Parallel.ForEach(NewExecutableCheckClasses, checkClass =>
            {
                // Get the protection for the class, if possible
                string protection = checkClass.CheckNewExecutable(file, nex, includeDebug);
                if (string.IsNullOrWhiteSpace(protection))
                    return;

                // If we are filtering the output of the check
                if (!CheckIfPacker(checkClass) || !IncludePackers)
                    return;

                protections.TryAdd(checkClass, protection);
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
        public ConcurrentDictionary<IPortableExecutableCheck, string> RunPortableExecutableChecks(string file, Stream stream, PortableExecutable pex, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new ConcurrentDictionary<IPortableExecutableCheck, string>();

            // Iterate through all checks
            Parallel.ForEach(PortableExecutableCheckClasses, checkClass =>
            {
                // Get the protection for the class, if possible
                string protection = checkClass.CheckPortableExecutable(file, pex, includeDebug);
                if (string.IsNullOrWhiteSpace(protection))
                    return;

                // If we are filtering the output of the check
                if (!CheckIfPacker(checkClass) || !IncludePackers)
                    return;

                protections.TryAdd(checkClass, protection);
            });

            return protections;
        }

        #endregion

        #region Initializers

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
        private static IEnumerable<T> InitCheckClasses<T>()
            => InitCheckClasses<T>(typeof(BinaryObjectScanner.Packer._DUMMY).Assembly)
                .Concat(InitCheckClasses<T>(typeof(BinaryObjectScanner.Protection._DUMMY).Assembly));

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
        private static IEnumerable<T> InitCheckClasses<T>(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => t.IsClass && t.GetInterface(typeof(T).Name) != null)
                .Select(t => (T)Activator.CreateInstance(t));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check to see if an implementation is a packer using reflection
        /// </summary>
        /// <param name="impl">Implementation that was last used to check</param>
        private static bool CheckIfPacker(object impl)
        {
            return impl.GetType().Namespace.ToLowerInvariant().Contains("packer");
        }

        #endregion
    }
}
