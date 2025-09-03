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
    public class Executable : IDetectable, IExtractable
    {
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
            var protections = DetectDict(stream, file, includeDebug);
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
        public ProtectionDictionary DetectDict(Stream stream, string file, bool includeDebug)
        {
            // Create the output dictionary
            var protections = new ProtectionDictionary();

            // Try to create a wrapper for the proper executable type
            SabreTools.Serialization.Interfaces.IWrapper? wrapper;
            try
            {
                wrapper = WrapperFactory.CreateExecutableWrapper(stream);
                if (wrapper == null)
                    return protections;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
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
            }
            else if (wrapper is LinearExecutable lex)
            {
                // Standard checks
                var subProtections
                    = RunExecutableChecks(file, lex, StaticChecks.LinearExecutableCheckClasses, includeDebug);
                protections.Append(file, subProtections.Values);
            }
            else if (wrapper is NewExecutable nex)
            {
                // Standard checks
                var subProtections
                    = RunExecutableChecks(file, nex, StaticChecks.NewExecutableCheckClasses, includeDebug);
                protections.Append(file, subProtections.Values);
            }
            else if (wrapper is PortableExecutable pex)
            {
                // Standard checks
                var subProtections
                    = RunExecutableChecks(file, pex, StaticChecks.PortableExecutableCheckClasses, includeDebug);
                protections.Append(file, subProtections.Values);
            }

            return protections;
        }

        /// <inheritdoc/>
        public bool Extract(string file, string outDir, bool includeDebug)
        {
            if (!File.Exists(file))
                return false;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, outDir, includeDebug);
        }

        /// <inheritdoc/>
        public bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var exe = WrapperFactory.CreateExecutableWrapper(stream);
            if (exe == null)
                return false;

            // Extract all files
            bool extractAny = false;
            Directory.CreateDirectory(outDir);
            if (exe is PortableExecutable pex)
            {
                if (new Packer.CExe().CheckExecutable(file, pex, includeDebug) != null)
                    extractAny |= pex.ExtractCExe(outDir, includeDebug);

                if (new Packer.EmbeddedFile().CheckExecutable(file, pex, includeDebug) != null)
                {
                    extractAny |= pex.ExtractFromOverlay(outDir, includeDebug);
                    extractAny |= pex.ExtractFromResources(outDir, includeDebug);
                }

                if (new Packer.WiseInstaller().CheckExecutable(file, pex, includeDebug) != null)
                    extractAny |= pex.ExtractWise(outDir, includeDebug);
            }
            else if (exe is NewExecutable nex)
            {
                if (new Packer.EmbeddedFile().CheckExecutable(file, nex, includeDebug) != null)
                {
                    extractAny |= nex.ExtractFromOverlay(outDir, includeDebug);
                }

                if (new Packer.WiseInstaller().CheckExecutable(file, nex, includeDebug) != null)
                    extractAny |= nex.ExtractWise(outDir, includeDebug);
            }

            return extractAny;
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
                // If the stream isn't seekable
                if (!stream.CanSeek)
                    return protections;

                stream.Seek(0, SeekOrigin.Begin);
                fileContent = stream.ReadBytes((int)stream.Length);
                if (fileContent == null)
                    return protections;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return protections;
            }

            // Iterate through all checks
            StaticChecks.ContentCheckClasses.IterateWithAction(checkClass =>
            {
                // Get the protection for the class, if possible
                var protection = checkClass.CheckContents(file!, fileContent, includeDebug);
                if (string.IsNullOrEmpty(protection))
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

                protections.Append(checkClass, protection);
            });

            return protections;
        }

        #endregion
    }
}
