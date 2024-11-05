using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Data;
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
                var contentProtections = RunContentChecks(file, stream, includeDebug);
                protections.AddRange(contentProtections.Values);
            }

            if (wrapper is MSDOS mz)
            {
                var subProtections = RunExecutableChecks(file, mz, StaticChecks.MSDOSExecutableCheckClasses, includeDebug);
                protections.AddRange(subProtections.Values);
            }
            else if (wrapper is LinearExecutable lex)
            {
                var subProtections = RunExecutableChecks(file, lex, StaticChecks.LinearExecutableCheckClasses, includeDebug);
                protections.AddRange(subProtections.Values);
            }
            else if (wrapper is NewExecutable nex)
            {
                var subProtections = RunExecutableChecks(file, nex, StaticChecks.NewExecutableCheckClasses, includeDebug);
                protections.AddRange(subProtections.Values);
            }
            else if (wrapper is PortableExecutable pex)
            {
                var subProtections = RunExecutableChecks(file, pex, StaticChecks.PortableExecutableCheckClasses, includeDebug);
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

            // Read the file contents
            byte[] fileContent = [];
            try
            {
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
