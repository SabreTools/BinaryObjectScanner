using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            var wrapper = WrapperFactory.CreateExecutableWrapper(stream);
            if (wrapper == null)
                return false;

            // Extract all files
            bool extractAny = false;
            Directory.CreateDirectory(outDir);
            if (wrapper is PortableExecutable pex)
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
            else if (wrapper is NewExecutable nex)
            {
                if (new Packer.EmbeddedFile().CheckExecutable(file, nex, includeDebug) != null)
                    extractAny |= nex.ExtractFromOverlay(outDir, includeDebug);

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

        #region Helpers -- Remove when dependent libraries updated

        /// <summary>
        /// Map to contain overlay strings to avoid rereading
        /// </summary>
        private static readonly Dictionary<WrapperBase, List<string>> _overlayStrings = [];

        /// <summary>
        /// Lock object for <see cref="_overlayStrings"/> 
        /// </summary>
        private static readonly object _overlayStringsLock = new();

        /// <summary>
        /// Overlay strings, if they exist
        /// </summary>
        public static List<string>? GetOverlayStrings(NewExecutable nex)
        {
            lock (_overlayStringsLock)
            {
                // Use the cached data if possible
                if (_overlayStrings.TryGetValue(nex, out var strings))
                    return strings;

                // Get the available source length, if possible
                long dataLength = nex.Length;
                if (dataLength == -1)
                    return null;

                // If a required property is missing
                if (nex.Header == null || nex.SegmentTable == null || nex.ResourceTable?.ResourceTypes == null)
                    return null;

                // Get the overlay data, if possible
                byte[]? overlayData = nex.OverlayData;
                if (overlayData == null || overlayData.Length == 0)
                {
                    _overlayStrings[nex] = [];
                    return [];
                }

                // Otherwise, cache and return the strings
                _overlayStrings[nex] = ReadStringsFrom(overlayData, charLimit: 3) ?? [];
                return _overlayStrings[nex];
            }
        }

        /// <summary>
        /// Overlay strings, if they exist
        /// </summary>
        public static List<string>? GetOverlayStrings(PortableExecutable pex)
        {
            lock (_overlayStringsLock)
            {
                // Use the cached data if possible
                if (_overlayStrings.TryGetValue(pex, out var strings))
                    return strings;

                // Get the available source length, if possible
                long dataLength = pex.Length;
                if (dataLength == -1)
                    return null;

                // If the section table is missing
                if (pex.SectionTable == null)
                    return null;

                // Get the overlay data, if possible
                byte[]? overlayData = pex.OverlayData;
                if (overlayData == null || overlayData.Length == 0)
                {
                    _overlayStrings[pex] = [];
                    return [];
                }

                // Otherwise, cache and return the strings
                _overlayStrings[pex] = ReadStringsFrom(overlayData, charLimit: 4) ?? [];
                return _overlayStrings[pex];
            }
        }

        /// <summary>
        /// Read string data from the source
        /// </summary>
        /// <param name="charLimit">Number of characters needed to be a valid string, default 5</param>
        /// <returns>String list containing the requested data, null on error</returns>
        private static List<string>? ReadStringsFrom(byte[]? input, int charLimit = 5)
        {
            // Validate the data
            if (input == null || input.Length == 0)
                return null;

            // Limit to 16KiB of data
            if (input.Length > 16384)
            {
                int offset = 0;
                input = input.ReadBytes(ref offset, 16384);
            }

            // Check for ASCII strings
                var asciiStrings = ReadStringsWithEncoding(input, charLimit, Encoding.ASCII);

            // Check for UTF-8 strings
            // We are limiting the check for Unicode characters with a second byte of 0x00 for now
            var utf8Strings = ReadStringsWithEncoding(input, charLimit, Encoding.UTF8);

            // Ignore duplicate strings across encodings
            List<string> sourceStrings = [.. asciiStrings, .. utf8Strings];

            // Sort the strings and return
            sourceStrings.Sort();
            return sourceStrings;
        }

        /// <summary>
        /// Read string data from the source with an encoding
        /// </summary>
        /// <param name="bytes">Byte array representing the source data</param>
        /// <param name="charLimit">Number of characters needed to be a valid string</param>
        /// <param name="encoding">Character encoding to use for checking</param>
        /// <returns>String list containing the requested data, empty on error</returns>
        /// <remarks>
        /// This method has a couple of notable implementation details:
        /// - Strings can only have a maximum of 64 characters
        /// - Characters that fall outside of the extended ASCII set will be unused
        /// </remarks>
#if NET20
        private static List<string> ReadStringsWithEncoding(byte[]? bytes, int charLimit, Encoding encoding)
#else
        private static HashSet<string> ReadStringsWithEncoding(byte[]? bytes, int charLimit, Encoding encoding)
#endif
        {
            if (bytes == null || bytes.Length == 0)
                return [];
            if (charLimit <= 0 || charLimit > bytes.Length)
                return [];

            // Create the string set to return
#if NET20
            var strings = new List<string>();
#else
            var strings = new HashSet<string>();
#endif

            // Open the text reader with the correct encoding
            using var ms = new MemoryStream(bytes);
            using var reader = new StreamReader(ms, encoding);

            // Create a string builder for the loop
            var sb = new StringBuilder();

            // Check for strings
            long lastOffset = 0;
            while (!reader.EndOfStream)
            {
                // Read the next character from the stream
                char c = (char)reader.Read();

                // If the character is invalid
                if (char.IsControl(c) || (c & 0xFF00) != 0)
                {
                    // Seek to the end of the last found string
                    string str = sb.ToString();
                    lastOffset += encoding.GetByteCount(str) + 1;
                    ms.Seek(lastOffset, SeekOrigin.Begin);
                    reader.DiscardBufferedData();

                    // Add the string if long enough
                    if (str.Length >= charLimit)
                        strings.Add(str);

                    // Clear the builder and continue
#if NET20 || NET35
                    sb = new();
#else
                    sb.Clear();
#endif
                    continue;
                }

                // Otherwise, add the character to the builder and continue
                sb.Append(c);
            }

            // Handle any remaining data
            if (sb.Length >= charLimit)
                strings.Add(sb.ToString());

            return strings;
        }
    

        #endregion
    }
}
