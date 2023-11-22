using System;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Utilities;
using static BinaryObjectScanner.Utilities.Dictionary;

namespace BinaryObjectScanner
{
    internal static class Handler
    {
        #region Public Collections

        /// <summary>
        /// Cache for all IPathCheck types
        /// </summary>
        public static IEnumerable<IPathCheck?> PathCheckClasses
        {
            get
            {
                pathCheckClasses ??= InitCheckClasses<IPathCheck>();
                return pathCheckClasses;
            }
        }

        #endregion

        #region Internal Instances

        /// <summary>
        /// Cache for all IPathCheck types
        /// </summary>
        private static IEnumerable<IPathCheck?>? pathCheckClasses;

        #endregion

        #region Multiple Implementation Wrappers

        /// <summary>
        /// Handle a single path based on all path check implementations
        /// </summary>
        /// <param name="path">Path of the file or directory to check</param>
        /// <param name="scanner">Scanner object to use for options and scanning</param>
        /// <returns>Set of protections in file, null on error</returns>
#if NET20 || NET35
        public static Dictionary<string, Queue<string>> HandlePathChecks(string path, IEnumerable<string>? files)
#else
        public static ConcurrentDictionary<string, ConcurrentQueue<string>> HandlePathChecks(string path, IEnumerable<string>? files)
#endif
        {
            // Create the output dictionary
#if NET20 || NET35
            var protections = new Dictionary<string, Queue<string>>();
#else
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
#endif

            // Preprocess the list of files
            files = files?.Select(f => f.Replace('\\', '/'))?.ToList();

            // Iterate through all checks
#if NET20 || NET35
            foreach (var checkClass in PathCheckClasses)
#else
            Parallel.ForEach(PathCheckClasses, checkClass =>
#endif
            {
                var subProtections = checkClass?.PerformCheck(path, files);
                if (subProtections != null)
                    AppendToDictionary(protections, path, subProtections);
#if NET20 || NET35
            }
#else
            });
#endif

            return protections;
        }

        #endregion

        #region Single Implementation Handlers

        /// <summary>
        /// Handle files based on an IDetectable implementation
        /// </summary>
        /// <param name="impl">IDetectable class representing the file type</param>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
#if NET20 || NET35
        public static Queue<string>? HandleDetectable(IDetectable impl, string fileName, Stream stream, bool includeDebug)
#else
        public static ConcurrentQueue<string>? HandleDetectable(IDetectable impl, string fileName, Stream stream, bool includeDebug)
#endif
        {
            var protection = impl.Detect(stream, fileName, includeDebug);
            return ProcessProtectionString(protection);
        }

        /// <summary>
        /// Handle files based on an IExtractable implementation
        /// </summary>
        /// <param name="impl">IDetectable class representing the file type</param>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <param name="scanner">Scanner object to use on extractable contents</param>
        /// <returns>Set of protections in file, null on error</returns>
#if NET20 || NET35
        public static Dictionary<string, Queue<string>>? HandleExtractable(IExtractable impl, string fileName, Stream? stream, Scanner scanner)
#else
        public static ConcurrentDictionary<string, ConcurrentQueue<string>>? HandleExtractable(IExtractable impl, string fileName, Stream? stream, Scanner scanner)
#endif
        {
            // If the extractable file itself fails
            try
            {
                // Extract and get the output path
                var tempPath = impl.Extract(stream, fileName, scanner.IncludeDebug);
                if (tempPath == null)
                    return null;

                // Collect and format all found protections
                var subProtections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch (Exception ex)
                {
                    if (scanner.IncludeDebug) Console.WriteLine(ex);
                }

                // Prepare the returned protections
                StripFromKeys(subProtections, tempPath);
                PrependToKeys(subProtections, fileName);
                return subProtections;
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }

        /// <summary>
        /// Handle files based on an IPathCheck implementation
        /// </summary>
        /// <param name="impl">IPathCheck class representing the file type</param>
        /// <param name="path">Path of the file or directory to check</param>
        /// <returns>Set of protections in path, null on error</returns>
#if NET20 || NET35
        private static Queue<string>? PerformCheck(this IPathCheck impl, string? path, IEnumerable<string>? files)
#else
        private static ConcurrentQueue<string>? PerformCheck(this IPathCheck impl, string? path, IEnumerable<string>? files)
#endif
        {
            // If we have an invalid path
            if (string.IsNullOrEmpty(path))
                return null;

            // Setup the output dictionary
#if NET20 || NET35
            var protections = new Queue<string>();
#else
            var protections = new ConcurrentQueue<string>();
#endif

            // If we have a file path
            if (File.Exists(path))
            {
                var protection = impl.CheckFilePath(path!);
                var subProtections = ProcessProtectionString(protection);
                if (subProtections != null)
                    protections.AddRange(subProtections);
            }

            // If we have a directory path
            if (Directory.Exists(path) && files?.Any() == true)
            {
                var subProtections = impl.CheckDirectoryPath(path!, files);
                if (subProtections != null)
                    protections.AddRange(subProtections);
            }

            return protections;
        }

        #endregion

        #region Initializers

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
        private static IEnumerable<T?> InitCheckClasses<T>() =>
            InitCheckClasses<T>(typeof(Handler).Assembly);

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
        private static IEnumerable<T?> InitCheckClasses<T>(Assembly assembly)
        {
            return assembly.GetTypes()?
                .Where(t => t.IsClass && t.GetInterface(typeof(T).Name) != null)?
                .Select(t => (T?)Activator.CreateInstance(t)) ?? [];
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Process a protection string if it includes multiple protections
        /// </summary>
        /// <param name="protection">Protection string to process</param>
        /// <returns>Set of protections parsed, null on error</returns>
#if NET20 || NET35
        private static Queue<string>? ProcessProtectionString(string? protection)
#else
        private static ConcurrentQueue<string>? ProcessProtectionString(string? protection)
#endif
        {
            // If we have an invalid protection string
            if (string.IsNullOrEmpty(protection))
                return null;

            // Setup the output queue
#if NET20 || NET35
            var protections = new Queue<string>();
#else
            var protections = new ConcurrentQueue<string>();
#endif

            // If we have an indicator of multiple protections
            if (protection!.Contains(";"))
            {
                var splitProtections = protection.Split(';');
                protections.AddRange(splitProtections);
            }
            else
            {
                protections.Enqueue(protection);
            }

            return protections;
        }

        #endregion
    }
}
