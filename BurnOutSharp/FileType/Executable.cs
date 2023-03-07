using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BurnOutSharp.Interfaces;
using BinaryObjectScanner.Wrappers;
using static BinaryObjectScanner.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Executable or library
    /// </summary>
    public class Executable : IScannable
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // Files can be protected in multiple ways
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

            // Load the current file content for debug only
            byte[] fileContent = null;
            if (scanner.IncludeDebug)
            {
                try
                {
                    using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
                    {
                        fileContent = br.ReadBytes((int)stream.Length);
                    }
                }
                catch (Exception ex)
                {
                    if (scanner.IncludeDebug) Console.WriteLine(ex);

                    // Enable for odd files, keep disabled otherwise
                    // AppendToDictionary(protections, file, "[Out of memory attempting to open]");
                    // return protections;
                }
            }

            // Get the wrapper for the appropriate executable type
            WrapperBase wrapper = Tools.Utilities.DetermineExecutableType(stream);
            if (wrapper == null)
                return protections;

            // Iterate through all generic content checks
            if (fileContent != null)
            {
                Parallel.ForEach(ScanningClasses.ContentCheckClasses, contentCheckClass =>
                {
                    string protection = contentCheckClass.CheckContents(file, fileContent, scanner.IncludeDebug);
                    if (ShouldAddProtection(contentCheckClass, scanner.ScanPackers, protection))
                        AppendToDictionary(protections, file, protection);

                    // If we have an IScannable implementation
                    if (contentCheckClass is IScannable scannable)
                    {
                        if (file != null && !string.IsNullOrEmpty(protection))
                        {
                            var subProtections = scannable.Scan(scanner, file);
                            PrependToKeys(subProtections, file);
                            AppendToDictionary(protections, subProtections);
                        }
                    }
                });
            }

            // If we have an MS-DOS executable
            if (wrapper is MSDOS mz)
            {
                // No-op
            }

            // If we have a New Executable
            else if (wrapper is NewExecutable nex)
            {
                Parallel.ForEach(ScanningClasses.NewExecutableCheckClasses, contentCheckClass =>
                {
                    // Check using custom content checks first
                    string protection = contentCheckClass.CheckNewExecutable(file, nex, scanner.IncludeDebug);
                    if (ShouldAddProtection(contentCheckClass, scanner.ScanPackers, protection))
                        AppendToDictionary(protections, file, protection);

                    // If we have an IScannable implementation
                    if (contentCheckClass is IScannable scannable)
                    {
                        if (file != null && !string.IsNullOrEmpty(protection))
                        {
                            var subProtections = scannable.Scan(scanner, file);
                            PrependToKeys(subProtections, file);
                            AppendToDictionary(protections, subProtections);
                        }
                    }
                });
            }

            // If we have a Linear Executable
            else if (wrapper is LinearExecutable lex)
            {
                // No-op
            }

            // If we have a Portable Executable
            else if (wrapper is PortableExecutable pex)
            {
                Parallel.ForEach(ScanningClasses.PortableExecutableCheckClasses, contentCheckClass =>
                {
                    // Check using custom content checks first
                    string protection = contentCheckClass.CheckPortableExecutable(file, pex, scanner.IncludeDebug);
                    if (ShouldAddProtection(contentCheckClass, scanner.ScanPackers, protection))
                        AppendToDictionary(protections, file, protection);

                    // If we have an IScannable implementation
                    if (contentCheckClass is IScannable scannable)
                    {
                        if (file != null && !string.IsNullOrEmpty(protection))
                        {
                            var subProtections = scannable.Scan(scanner, file);
                            PrependToKeys(subProtections, file);
                            AppendToDictionary(protections, subProtections);
                        }
                    }
                });
            }

            // No other executable formats currently identified or supported

            return protections;
        }

        #region Helpers

        /// <summary>
        /// Check to see if a protection should be added or not
        /// </summary>
        /// <param name="checkClass">Class that was last used to check</param>
        /// <param name="scanPackers">Determines if packers should be included in the output</param>
        /// <param name="protection">The protection result to be checked</param>
        private bool ShouldAddProtection(object checkClass, bool scanPackers, string protection)
        {
            // If we have an invalid protection
            if (string.IsNullOrWhiteSpace(protection))
                return false;

            // If we have a valid content check based on settings
            if (scanPackers || !checkClass.GetType().Namespace.ToLowerInvariant().Contains("packertype"))
                return true;

            // Everything else fails
            return false;
        }

        #endregion
    }
}
