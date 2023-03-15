using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Utilities;
using BinaryObjectScanner.Wrappers;
using static BinaryObjectScanner.Utilities.Dictionary;

namespace BurnOutSharp
{
    internal static class Handler
    {
        #region Multiple Implementation Wrappers

        /// <summary>
        /// Handle a single file based on all content check implementations
        /// </summary>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <param name="scanner">Scanner object to use for options and scanning</param>
        /// <returns>Set of protections in file, null on error</returns>
        public static ConcurrentQueue<string> HandleContentChecks(string fileName, Stream stream, Scanner scanner)
        {
            // If we have an invalid file
            if (string.IsNullOrWhiteSpace(fileName))
                return null;
            else if (!File.Exists(fileName))
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
                if (scanner.IncludeDebug) Console.WriteLine(ex);
                return null;
            }

            // Create the output queue
            var protections = new ConcurrentQueue<string>();

            // Iterate through all checks
            Parallel.ForEach(ScanningClasses.ContentCheckClasses, checkClass =>
            {
                // Get the protection for the class, if possible
                var subProtections = checkClass.PerformCheck(fileName, fileContent, scanner.IncludeDebug);
                if (subProtections != null)
                {
                    // If we are filtering the output of the check
                    if (!CheckIfPacker(checkClass) || !scanner.ScanPackers)
                        return;

                    protections.AddRange(subProtections);
                }
            });

            return protections;
        }

        /// <summary>
        /// Handle a single file based on all linear executable check implementations
        /// </summary>
        /// <param name="fileName">Name of the source file of the executable, for tracking</param>
        /// <param name="lex">Executable to scan</param>
        /// <param name="scanner">Scanner object to use for options and scanning</param>
        /// <returns>Set of protections in file, null on error</returns>
        public static ConcurrentDictionary<string, ConcurrentQueue<string>> HandleLinearExecutableChecks(string fileName, Stream stream, LinearExecutable lex, Scanner scanner)
        {
            // Create the output dictionary
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

            // Iterate through all checks
            Parallel.ForEach(ScanningClasses.LinearExecutableCheckClasses, checkClass =>
            {
                // Get the protection for the class, if possible
                var subProtections = checkClass.PerformCheck(fileName, lex, scanner.IncludeDebug);
                if (subProtections == null)
                    return;

                // If we are filtering the output of the check
                if (!CheckIfPacker(checkClass) || !scanner.ScanPackers)
                    return;

                // Add all found protections to the output
                AppendToDictionary(protections, fileName, subProtections);

                // If we have an extractable implementation
                if (checkClass is IExtractable extractable)
                {
                    var extractedProtections = HandleExtractable(extractable, fileName, stream, scanner);
                    if (extractedProtections != null)
                        AppendToDictionary(protections, extractedProtections);
                }
            });

            return protections;
        }

        /// <summary>
        /// Handle a single file based on all new executable check implementations
        /// </summary>
        /// <param name="fileName">Name of the source file of the executable, for tracking</param>
        /// <param name="nex">Executable to scan</param>
        /// <param name="scanner">Scanner object to use for options and scanning</param>
        /// <returns>Set of protections in file, null on error</returns>
        public static ConcurrentDictionary<string, ConcurrentQueue<string>> HandleNewExecutableChecks(string fileName, Stream stream, NewExecutable nex, Scanner scanner)
        {
            // Create the output dictionary
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

            // Iterate through all checks
            Parallel.ForEach(ScanningClasses.NewExecutableCheckClasses, checkClass =>
            {
                // Get the protection for the class, if possible
                var subProtections = checkClass.PerformCheck(fileName, nex, scanner.IncludeDebug);
                if (subProtections == null)
                    return;

                // If we are filtering the output of the check
                if (!CheckIfPacker(checkClass) || !scanner.ScanPackers)
                    return;

                // Add all found protections to the output
                AppendToDictionary(protections, fileName, subProtections);

                // If we have an extractable implementation
                if (checkClass is IExtractable extractable)
                {
                    var extractedProtections = HandleExtractable(extractable, fileName, stream, scanner);
                    if (extractedProtections != null)
                        AppendToDictionary(protections, extractedProtections);
                }
            });

            return protections;
        }

        /// <summary>
        /// Handle a single path based on all path check implementations
        /// </summary>
        /// <param name="path">Path of the file or directory to check</param>
        /// <param name="scanner">Scanner object to use for options and scanning</param>
        /// <returns>Set of protections in file, null on error</returns>
        public static ConcurrentDictionary<string, ConcurrentQueue<string>> HandlePathChecks(string path, IEnumerable<string> files)
        {
            // Create the output dictionary
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

            // Preprocess the list of files
            files = files?.Select(f => f.Replace('\\', '/'))?.ToList();

            // Iterate through all checks
            Parallel.ForEach(ScanningClasses.PathCheckClasses, checkClass =>
            {
                var subProtections = checkClass.PerformCheck(path, files);
                if (subProtections != null)
                    AppendToDictionary(protections, subProtections);
            });

            return protections;
        }

        /// <summary>
        /// Handle a single file based on all portable executable check implementations
        /// </summary>
        /// <param name="fileName">Name of the source file of the executable, for tracking</param>
        /// <param name="pex">Executable to scan</param>
        /// <param name="scanner">Scanner object to use for options and scanning</param>
        /// <returns>Set of protections in file, null on error</returns>
        public static ConcurrentDictionary<string, ConcurrentQueue<string>> HandlePortableExecutableChecks(string fileName, Stream stream, PortableExecutable pex, Scanner scanner)
        {
            // Create the output dictionary
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

            // Iterate through all checks
            Parallel.ForEach(ScanningClasses.PortableExecutableCheckClasses, checkClass =>
            {
                // Get the protection for the class, if possible
                var subProtections = checkClass.PerformCheck(fileName, pex, scanner.IncludeDebug);
                if (subProtections == null)
                    return;

                // If we are filtering the output of the check
                if (!CheckIfPacker(checkClass) || !scanner.ScanPackers)
                    return;

                // Add all found protections to the output
                AppendToDictionary(protections, fileName, subProtections);

                // If we have an extractable implementation
                if (checkClass is IExtractable extractable)
                {
                    var extractedProtections = HandleExtractable(extractable, fileName, stream, scanner);
                    if (extractedProtections != null)
                        AppendToDictionary(protections, extractedProtections);
                }
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
        public static ConcurrentQueue<string> HandleDetectable(IDetectable impl, string fileName, Stream stream, bool includeDebug)
        {
            string protection = impl.Detect(stream, fileName, includeDebug);
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
        public static ConcurrentDictionary<string, ConcurrentQueue<string>> HandleExtractable(IExtractable impl, string fileName, Stream stream, Scanner scanner)
        {
            // If the extractable file itself fails
            try
            {
                // Extract and get the output path
                string tempPath = impl.Extract(stream, fileName, scanner.IncludeDebug);
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
        /// Handle files based on an IContentCheck implementation
        /// </summary>
        /// <param name="impl">IDetectable class representing the check</param>
        /// <param name="fileName">Name of the source file of the byte array, for tracking</param>
        /// <param name="fileContent">Contents of the source file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
        private static ConcurrentQueue<string> PerformCheck(this IContentCheck impl, string fileName, byte[] fileContent, bool includeDebug)
        {
            string protection = impl.CheckContents(fileName, fileContent, includeDebug);
            return ProcessProtectionString(protection);
        }

        /// <summary>
        /// Handle files based on an ILinearExecutableCheck implementation
        /// </summary>
        /// <param name="impl">ILinearExecutableCheck class representing the check</param>
        /// <param name="fileName">Name of the source file of the executable, for tracking</param>
        /// <param name="lex">LinearExecutable to check</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
        private static ConcurrentQueue<string> PerformCheck(this ILinearExecutableCheck impl, string fileName, LinearExecutable lex, bool includeDebug)
        {
            string protection = impl.CheckLinearExecutable(fileName, lex, includeDebug);
            return ProcessProtectionString(protection);
        }

        /// <summary>
        /// Handle files based on an INewExecutableCheck implementation
        /// </summary>
        /// <param name="impl">INewExecutableCheck class representing the check</param>
        /// <param name="fileName">Name of the source file of the executable, for tracking</param>
        /// <param name="nex">NewExecutable to check</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
        private static ConcurrentQueue<string> PerformCheck(this INewExecutableCheck impl, string fileName, NewExecutable nex, bool includeDebug)
        {
            string protection = impl.CheckNewExecutable(fileName, nex, includeDebug);
            return ProcessProtectionString(protection);
        }

        /// <summary>
        /// Handle files based on an IPathCheck implementation
        /// </summary>
        /// <param name="impl">IPathCheck class representing the file type</param>
        /// <param name="path">Path of the file or directory to check</param>
        /// <returns>Set of protections in path, null on error</returns>
        private static ConcurrentDictionary<string, ConcurrentQueue<string>> PerformCheck(this IPathCheck impl, string path, IEnumerable<string> files)
        {
            // If we have an invalid path
            if (string.IsNullOrWhiteSpace(path))
                return null;

            // Setup the output dictionary
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

            // If we have a file path
            if (File.Exists(path))
            {
                string protection = impl.CheckFilePath(path);
                var subProtections = ProcessProtectionString(protection);
                if (subProtections != null)
                    AppendToDictionary(protections, path, subProtections);
            }

            // If we have a directory path
            if (Directory.Exists(path) && files?.Any() == true)
            {
                var subProtections = impl.CheckDirectoryPath(path, files);
                if (subProtections != null)
                    AppendToDictionary(protections, path, subProtections);
            }

            return protections;
        }

        /// <summary>
        /// Handle files based on an IPortableExecutableCheck implementation
        /// </summary>
        /// <param name="impl">IPortableExecutableCheck class representing the check</param>
        /// <param name="fileName">Name of the source file of the executable, for tracking</param>
        /// <param name="pex">NewExecutable to check</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, null on error</returns>
        private static ConcurrentQueue<string> PerformCheck(this IPortableExecutableCheck impl, string fileName, PortableExecutable pex, bool includeDebug)
        {
            string protection = impl.CheckPortableExecutable(fileName, pex, includeDebug);
            return ProcessProtectionString(protection);
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

        /// <summary>
        /// Process a protection string if it includes multiple protections
        /// </summary>
        /// <param name="protection">Protection string to process</param>
        /// <returns>Set of protections parsed, null on error</returns>
        private static ConcurrentQueue<string> ProcessProtectionString(string protection)
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
