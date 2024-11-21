using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Data;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Executable or library
    /// </summary>
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
            // Get all non-nested protections
            var protections = DetectDict(stream, file, getProtections: null, includeDebug);
            if (protections.Count == 0)
                return null;

            // Create the internal list
            var protectionList = new List<string>();
            foreach (string key in protections.Keys)
            {
                protectionList.AddRange(protections[key]);
            }

            return string.Join(";", [.. protectionList]);
        }

        /// <inheritdoc cref="IDetectable.Detect(Stream, string, bool)"/>
        /// <remarks>
        /// Ideally, we wouldn't need to circumvent the proper handling of file types just for Executable,
        /// but due to the complexity of scanning, this is not currently possible.
        /// </remarks>
        public ProtectionDictionary DetectDict(Stream stream,
            string file,
            Func<string, ProtectionDictionary>? getProtections,
            bool includeDebug)
        {
            // Create the output dictionary
            var protections = new ProtectionDictionary();

            // Try to create a wrapper for the proper executable type
            IWrapper? wrapper;
            try
            {
                wrapper = WrapperFactory.CreateExecutableWrapper(stream);
                if (wrapper == null)
                    return protections;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return protections;
            }

            // Only use generic content checks if we're in debug mode
            if (includeDebug)
            {
                var subProtections = RunContentChecks(file, stream, includeDebug);
                protections.Append(file, subProtections.Values);
            }

            if (wrapper is MSDOS mz)
            {
                // Standard checks
                var subProtections
                    = RunExecutableChecks(file, mz, StaticChecks.MSDOSExecutableCheckClasses, includeDebug);
                protections.Append(file, subProtections.Values);

                // Extractable checks
                var extractedProtections
                    = HandleExtractableProtections(file, mz, subProtections.Keys, getProtections, includeDebug);
                protections.Append(extractedProtections);
            }
            else if (wrapper is LinearExecutable lex)
            {
                // Standard checks
                var subProtections
                    = RunExecutableChecks(file, lex, StaticChecks.LinearExecutableCheckClasses, includeDebug);
                protections.Append(file, subProtections.Values);

                // Extractable checks
                var extractedProtections
                    = HandleExtractableProtections(file, lex, subProtections.Keys, getProtections, includeDebug);
                protections.Append(extractedProtections);
            }
            else if (wrapper is NewExecutable nex)
            {
                // Standard checks
                var subProtections
                    = RunExecutableChecks(file, nex, StaticChecks.NewExecutableCheckClasses, includeDebug);
                protections.Append(file, subProtections.Values);

                // Extractable checks
                var extractedProtections
                    = HandleExtractableProtections(file, nex, subProtections.Keys, getProtections, includeDebug);
                protections.Append(extractedProtections);
            }
            else if (wrapper is PortableExecutable pex)
            {
                // Standard checks
                var subProtections
                    = RunExecutableChecks(file, pex, StaticChecks.PortableExecutableCheckClasses, includeDebug);
                protections.Append(file, subProtections.Values);

                // Extractable checks
                var extractedProtections
                    = HandleExtractableProtections(file, pex, subProtections.Keys, getProtections, includeDebug);
                protections.Append(extractedProtections);
            }

            return protections;
        }

        #region Check Runners

        /// <summary>
        /// Handle a single file based on all content check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, empty on error</returns>
        public IDictionary<IContentCheck, string> RunContentChecks(string? file, Stream stream, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new CheckDictionary<IContentCheck>();

            // If we have an invalid file
            if (string.IsNullOrEmpty(file))
                return protections;
            else if (!File.Exists(file))
                return protections;

            // If the stream isn't seekable
            if (!stream.CanSeek)
                return protections;

            // Read the file contents
            byte[] fileContent = [];
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                fileContent = stream.ReadBytes((int)stream.Length);
                if (fileContent == null)
                    return protections;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return protections;
            }

            // Iterate through all checks
            StaticChecks.ContentCheckClasses.IterateWithAction(checkClass =>
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
        /// Handle a single file based on all executable check implementations
        /// </summary>
        /// <param name="file">Name of the source file of the executable, for tracking</param>
        /// <param name="exe">Executable to scan</param>
        /// <param name="checks">Set of checks to use</param>
        /// <param name="scanner">Scanner for handling recursive protections</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in file, empty on error</returns>
        public IDictionary<U, string> RunExecutableChecks<T, U>(string file, T exe, List<U> checks, bool includeDebug)
            where T : WrapperBase
            where U : IExecutableCheck<T>
        {
            // Create the output dictionary
            var protections = new CheckDictionary<U>();

            // Iterate through all checks
            checks.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckExecutable(file, exe, includeDebug);
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
        /// Handle extractable protections, such as executable packers
        /// </summary>
        /// <param name="file">Name of the source file of the stream, for tracking</param>
        /// <param name="exe">Executable to scan the contents of</param>
        /// <param name="checks">Set of classes returned from Exectuable scans</param>
        /// <param name="getProtections">Optional function for handling recursive protections</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections found from extraction, empty on error</returns>
        private static ProtectionDictionary HandleExtractableProtections<T, U>(string file,
            T exe,
            ICollection<U> checks,
            Func<string, ProtectionDictionary>? getProtections,
            bool includeDebug)
                where T : WrapperBase
                where U : IExecutableCheck<T>
        {
            // Create the output dictionary
            var protections = new ProtectionDictionary();

            // If we have an invalid set of classes
            if (checks == null)
                return protections;

            // If we have any extractable packers
            var extractables = checks
                .Where(c => c is IExtractableExecutable<T>)
                .Select(c => c as IExtractableExecutable<T>);
            extractables.IterateWithAction(extractable =>
            {
                var subProtections = PerformExtractableCheck(extractable!, file, exe, getProtections, includeDebug);
                protections.Append(subProtections);
            });

            return protections;
        }

        /// <summary>
        /// Handle files based on an IExtractableExecutable implementation
        /// </summary>
        /// <param name="file">Name of the source file of the stream, for tracking</param>
        /// <param name="exe">Executable to scan the contents of</param>
        /// <param name="impl">IExtractableExecutable class representing the file type</param>
        /// <param name="getProtections">Optional function for handling recursive protections</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Set of protections in path, empty on error</returns>
        private static ProtectionDictionary PerformExtractableCheck<T>(IExtractableExecutable<T> impl,
            string file,
            T exe,
            Func<string, ProtectionDictionary>? getProtections,
            bool includeDebug)
                where T : WrapperBase
        {
            // If we have an invalid extractable somehow
            if (impl == null)
                return [];

            // If the extractable file itself fails
            try
            {
                // Extract and get the output path
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                bool extracted = impl.Extract(file, exe, tempPath, includeDebug);

                // Collect and format all found protections
                ProtectionDictionary? subProtections = null;
                if (extracted && getProtections != null)
                    subProtections = getProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    if (Directory.Exists(tempPath))
                        Directory.Delete(tempPath, true);
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.WriteLine(ex);
                }

                // Prepare the returned protections
                subProtections?.StripFromKeys(tempPath);
                subProtections?.PrependToKeys(file);
                return subProtections ?? [];
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return [];
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check to see if an implementation is a game engine using reflection
        /// </summary>
        /// <param name="impl">Implementation that was last used to check</param>
        private static bool CheckIfGameEngine(object impl)
        {
            return impl.GetType().Namespace?.ToLowerInvariant()?.Contains("gameengine") ?? false;
        }

        /// <summary>
        /// Check to see if an implementation is a packer using reflection
        /// </summary>
        /// <param name="impl">Implementation that was last used to check</param>
        private static bool CheckIfPacker(object impl)
        {
            return impl.GetType().Namespace?.ToLowerInvariant()?.Contains("packer") ?? false;
        }

        #endregion
    }
}
