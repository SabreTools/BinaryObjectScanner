using System;
using System.Collections.Concurrent;
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
#if NET48
        public static IEnumerable<IPathCheck> PathCheckClasses
#else
        public static IEnumerable<IPathCheck?> PathCheckClasses
#endif
        {
            get
            {
                if (pathCheckClasses == null)
                    pathCheckClasses = InitCheckClasses<IPathCheck>();

                return pathCheckClasses;
            }
        }

        #endregion

        #region Internal Instances

        /// <summary>
        /// Cache for all IPathCheck types
        /// </summary>
#if NET48
        private static IEnumerable<IPathCheck> pathCheckClasses;
#else
        private static IEnumerable<IPathCheck?>? pathCheckClasses;
#endif

        #endregion

        #region Multiple Implementation Wrappers

        /// <summary>
        /// Handle a single path based on all path check implementations
        /// </summary>
        /// <param name="path">Path of the file or directory to check</param>
        /// <param name="scanner">Scanner object to use for options and scanning</param>
        /// <returns>Set of protections in file, null on error</returns>
#if NET48
        public static ConcurrentDictionary<string, ConcurrentQueue<string>> HandlePathChecks(string path, IEnumerable<string> files)
#else
        public static ConcurrentDictionary<string, ConcurrentQueue<string>> HandlePathChecks(string path, IEnumerable<string>? files)
#endif
        {
            // Create the output dictionary
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

            // Preprocess the list of files
            files = files?.Select(f => f.Replace('\\', '/'))?.ToList();

            // Iterate through all checks
            Parallel.ForEach(PathCheckClasses, checkClass =>
            {
                var subProtections = checkClass?.PerformCheck(path, files);
                if (subProtections != null)
                    AppendToDictionary(protections, path, subProtections);
            });

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
#if NET48
        public static ConcurrentQueue<string> HandleDetectable(IDetectable impl, string fileName, Stream stream, bool includeDebug)
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
#if NET48
        public static ConcurrentDictionary<string, ConcurrentQueue<string>> HandleExtractable(IExtractable impl, string fileName, Stream stream, Scanner scanner)
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
#if NET48
        private static ConcurrentQueue<string> PerformCheck(this IPathCheck impl, string path, IEnumerable<string> files)
#else
        private static ConcurrentQueue<string>? PerformCheck(this IPathCheck impl, string? path, IEnumerable<string>? files)
#endif
        {
            // If we have an invalid path
            if (string.IsNullOrWhiteSpace(path))
                return null;

            // Setup the output dictionary
            var protections = new ConcurrentQueue<string>();

            // If we have a file path
            if (File.Exists(path))
            {
                var protection = impl.CheckFilePath(path);
                var subProtections = ProcessProtectionString(protection);
                if (subProtections != null)
                    protections.AddRange(subProtections);
            }

            // If we have a directory path
            if (Directory.Exists(path) && files?.Any() == true)
            {
                var subProtections = impl.CheckDirectoryPath(path, files);
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
#if NET48
        private static IEnumerable<T> InitCheckClasses<T>()
#else
        private static IEnumerable<T?> InitCheckClasses<T>()
#endif
        {
            return InitCheckClasses<T>(typeof(GameEngine._DUMMY).Assembly)
                .Concat(InitCheckClasses<T>(typeof(Packer._DUMMY).Assembly))
                .Concat(InitCheckClasses<T>(typeof(Protection._DUMMY).Assembly));
        }

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
#if NET48
        private static IEnumerable<T> InitCheckClasses<T>(Assembly assembly)
#else
        private static IEnumerable<T?> InitCheckClasses<T>(Assembly assembly)
#endif
        {
            return assembly.GetTypes()?
                .Where(t => t.IsClass && t.GetInterface(typeof(T).Name) != null)?
#if NET48
                .Select(t => (T)Activator.CreateInstance(t)) ?? Array.Empty<T>();
#else
                .Select(t => (T?)Activator.CreateInstance(t)) ?? Array.Empty<T>();
#endif
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Process a protection string if it includes multiple protections
        /// </summary>
        /// <param name="protection">Protection string to process</param>
        /// <returns>Set of protections parsed, null on error</returns>
#if NET48
        private static ConcurrentQueue<string> ProcessProtectionString(string protection)
#else
        private static ConcurrentQueue<string>? ProcessProtectionString(string? protection)
#endif
        {
            // If we have an invalid protection string
            if (string.IsNullOrWhiteSpace(protection))
                return null;

            // Setup the output queue
            var protections = new ConcurrentQueue<string>();

            // If we have an indicator of multiple protections
            if (protection.Contains(";"))
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
