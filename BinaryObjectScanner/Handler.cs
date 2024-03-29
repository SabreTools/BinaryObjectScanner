﻿using System;
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
using SabreTools.Serialization.Wrappers;
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
            files = files?.Select(f => f.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar))?.ToList();

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
        /// <param name="impl">IExtractable class representing the file type</param>
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
        /// Handle files based on an IExtractableMSDOSExecutable implementation
        /// </summary>
        /// <param name="impl">IExtractableMSDOSExecutable class representing the file type</param>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="mz">MSDOS to scan the contents of</param>
        /// <param name="scanner">Scanner object to use on extractable contents</param>
        /// <returns>Set of protections in file, null on error</returns>
#if NET20 || NET35
        public static Dictionary<string, Queue<string>>? HandleExtractable(IExtractableMSDOSExecutable impl, string fileName, MSDOS mz, Scanner scanner)
#else
        public static ConcurrentDictionary<string, ConcurrentQueue<string>>? HandleExtractable(IExtractableMSDOSExecutable impl, string fileName, MSDOS mz, Scanner scanner)
#endif
        {
            // If the extractable file itself fails
            try
            {
                // Extract and get the output path
                var tempPath = impl.Extract(fileName, mz, scanner.IncludeDebug);
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
        /// Handle files based on an IExtractableLinearExecutable implementation
        /// </summary>
        /// <param name="impl">IExtractableLinearExecutable class representing the file type</param>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="lex">LinearExecutable to scan the contents of</param>
        /// <param name="scanner">Scanner object to use on extractable contents</param>
        /// <returns>Set of protections in file, null on error</returns>
#if NET20 || NET35
        public static Dictionary<string, Queue<string>>? HandleExtractable(IExtractableLinearExecutable impl, string fileName, LinearExecutable lex, Scanner scanner)
#else
        public static ConcurrentDictionary<string, ConcurrentQueue<string>>? HandleExtractable(IExtractableLinearExecutable impl, string fileName, LinearExecutable lex, Scanner scanner)
#endif
        {
            // If the extractable file itself fails
            try
            {
                // Extract and get the output path
                var tempPath = impl.Extract(fileName, lex, scanner.IncludeDebug);
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
        /// Handle files based on an IExtractableNewExecutable implementation
        /// </summary>
        /// <param name="impl">IExtractableNewExecutable class representing the file type</param>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="nex">NewExecutable to scan the contents of</param>
        /// <param name="scanner">Scanner object to use on extractable contents</param>
        /// <returns>Set of protections in file, null on error</returns>
#if NET20 || NET35
        public static Dictionary<string, Queue<string>>? HandleExtractable(IExtractableNewExecutable impl, string fileName, NewExecutable nex, Scanner scanner)
#else
        public static ConcurrentDictionary<string, ConcurrentQueue<string>>? HandleExtractable(IExtractableNewExecutable impl, string fileName, NewExecutable nex, Scanner scanner)
#endif
        {
            // If the extractable file itself fails
            try
            {
                // Extract and get the output path
                var tempPath = impl.Extract(fileName, nex, scanner.IncludeDebug);
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
        /// Handle files based on an IExtractablePortableExecutable implementation
        /// </summary>
        /// <param name="impl">IExtractablePortableExecutable class representing the file type</param>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="pex">PortableExecutable to scan the contents of</param>
        /// <param name="scanner">Scanner object to use on extractable contents</param>
        /// <returns>Set of protections in file, null on error</returns>
#if NET20 || NET35
        public static Dictionary<string, Queue<string>>? HandleExtractable(IExtractablePortableExecutable impl, string fileName, PortableExecutable pex, Scanner scanner)
#else
        public static ConcurrentDictionary<string, ConcurrentQueue<string>>? HandleExtractable(IExtractablePortableExecutable impl, string fileName, PortableExecutable pex, Scanner scanner)
#endif
        {
            // If the extractable file itself fails
            try
            {
                // Extract and get the output path
                var tempPath = impl.Extract(fileName, pex, scanner.IncludeDebug);
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
            InitCheckClasses<T>(Assembly.GetExecutingAssembly());

        /// <summary>
        /// Initialize all implementations of a type
        /// </summary>
        private static IEnumerable<T?> InitCheckClasses<T>(Assembly assembly)
        {
            List<T?> classTypes = [];

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
