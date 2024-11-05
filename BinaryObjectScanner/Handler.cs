using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Data;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner
{
    internal static class Handler
    {
        #region Multiple Implementation Wrappers

        /// <summary>
        /// Handle a single path based on all path check implementations
        /// </summary>
        /// <param name="path">Path of the file or directory to check</param>
        /// <param name="scanner">Scanner object to use for options and scanning</param>
        /// <returns>Set of protections in file, null on error</returns>
        public static ProtectionDictionary HandlePathChecks(string path, IEnumerable<string>? files)
        {
            // Create the output dictionary
            var protections = new ProtectionDictionary();

            // Preprocess the list of files
            files = files?
                .Select(f => f.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar))?
                .ToList();

            // Iterate through all checks
            StaticChecks.PathCheckClasses.IterateWithAction(checkClass =>
            {
                var subProtections = checkClass.PerformCheck(path, files);
                protections.Append(path, subProtections);
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
        public static List<string>? HandleDetectable(IDetectable impl, string fileName, Stream stream, bool includeDebug)
        {
            var protection = impl.Detect(stream, fileName, includeDebug);
            return ProcessProtectionString(protection);
        }

        /// <summary>
        /// Handle files based on an IPathCheck implementation
        /// </summary>
        /// <param name="impl">IPathCheck class representing the file type</param>
        /// <param name="path">Path of the file or directory to check</param>
        /// <returns>Set of protections in path, empty on error</returns>
        private static List<string> PerformCheck(this IPathCheck impl, string? path, IEnumerable<string>? files)
        {
            // If we have an invalid path
            if (string.IsNullOrEmpty(path))
                return [];

            // Setup the list
            var protections = new List<string>();

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

        #region Helpers

        /// <summary>
        /// Process a protection string if it includes multiple protections
        /// </summary>
        /// <param name="protection">Protection string to process</param>
        /// <returns>Set of protections parsed, null on error</returns>
        private static List<string>? ProcessProtectionString(string? protection)
        {
            // If we have an invalid protection string
            if (string.IsNullOrEmpty(protection))
                return null;

            // Setup the output queue
            var protections = new List<string>();

            // If we have an indicator of multiple protections
            if (protection!.Contains(";"))
            {
                var splitProtections = protection.Split(';');
                protections.AddRange(splitProtections);
            }
            else
            {
                protections.Add(protection);
            }

            return protections;
        }

        #endregion
    }
}
