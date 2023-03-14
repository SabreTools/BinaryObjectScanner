using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BurnOutSharp;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Wrappers;
using static BinaryObjectScanner.Utilities.Dictionary;
using System.Linq;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Executable or library
    /// </summary>
    public class Executable : IDetectable, IExtractable, IScannable
    {
        /// <inheritdoc/>
        public string Detect(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Detect(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string Detect(Stream stream, string file, bool includeDebug)
        {
            // Implementation notes:
            // - Executables may house more than one sort of packer/protection (see Textfile for details)
            // - The output of Detect directly influences which types of packers should be attempted for extraction
            // - There are no other file types that require this input, so is it worth changing the interface?
            // - Can this somehow delegate to the proper extractable type?

            return null;

            // The below code is a copy of what is currently in Scan, but without any of the
            // extraction code or packer filtering code. It is not currently enabled since it
            // is not as complete as the IScannable implementation and therefore cannot be reasonably
            // used as a replacement yet.

            // Files can be protected in multiple ways
            var protections = new ConcurrentQueue<string>();

            // Load the current file content for debug only
            byte[] fileContent = null;
            if (includeDebug)
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
                    if (includeDebug) Console.WriteLine(ex);
                }
            }

            // Get the wrapper for the appropriate executable type
            WrapperBase wrapper = WrapperFactory.CreateExecutableWrapper(stream);
            if (wrapper == null)
                return null;

            // Iterate through all generic content checks
            if (fileContent != null)
            {
                Parallel.ForEach(ScanningClasses.ContentCheckClasses, checkClass =>
                {
                    string protection = checkClass.CheckContents(file, fileContent, includeDebug);
                    protections.Enqueue(protection);
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
                Parallel.ForEach(ScanningClasses.NewExecutableCheckClasses, checkClass =>
                {
                    string protection = checkClass.CheckNewExecutable(file, nex, includeDebug);
                    protections.Enqueue(protection);
                });
            }

            // If we have a Linear Executable
            else if (wrapper is LinearExecutable lex)
            {
                Parallel.ForEach(ScanningClasses.LinearExecutableCheckClasses, checkClass =>
                {
                    string protection = checkClass.CheckLinearExecutable(file, lex, includeDebug);
                    protections.Enqueue(protection);
                });
            }

            // If we have a Portable Executable
            else if (wrapper is PortableExecutable pex)
            {
                Parallel.ForEach(ScanningClasses.PortableExecutableCheckClasses, checkClass =>
                {
                    string protection = checkClass.CheckPortableExecutable(file, pex, includeDebug);
                    protections.Enqueue(protection);
                });
            }

            return string.Join(";", protections.ToArray());
        }

        /// <inheritdoc/>
        public string Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string Extract(Stream stream, string file, bool includeDebug)
        {
            // Implementation notes:
            // - Executables may house more than one sort of extractable packer/protection
            // - We currently have no way of defining what folder is output for a given extractable
            // - Everything else should be basically the same for other extractable types
            // - Which extractions to run is directly influenced by the detected protections
            // - Can this somehow delegate to the proper extractable type?
            // - Can we have a check in Scanner that then runs all extractable implementations if the class is Executable?

            // The following packers fully implement IExtractable (extract and doesn't return null)
            // - <see cref="BinaryObjectScanner.Packer.CExe" />
            // - <see cref="BinaryObjectScanner.Packer.EmbeddedExecutable" />
            // - <see cref="BinaryObjectScanner.Packer.WinRARSFX" />
            // - <see cref="BinaryObjectScanner.Packer.WinZipSFX" />
            // - <see cref="BinaryObjectScanner.Packer.WiseInstaller" />

            return null;
        }

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
            WrapperBase wrapper = WrapperFactory.CreateExecutableWrapper(stream);
            if (wrapper == null)
                return protections;

            // Iterate through all generic content checks
            if (fileContent != null)
            {
                Parallel.ForEach(ScanningClasses.ContentCheckClasses, checkClass =>
                {
                    // Get the protection for the class, if possible
                    string protection = checkClass.CheckContents(file, fileContent, scanner.IncludeDebug);
                    if (ShouldAddProtection(checkClass, scanner.ScanPackers, protection))
                        AppendToDictionary(protections, file, protection);

                    // If we had a protection, check if it is extractable
                    if (!string.IsNullOrWhiteSpace(protection))
                        HandleExtractable(scanner, stream, file, checkClass, protections);
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
                Parallel.ForEach(ScanningClasses.NewExecutableCheckClasses, checkClass =>
                {
                    // Get the protection for the class, if possible
                    string protection = checkClass.CheckNewExecutable(file, nex, scanner.IncludeDebug);
                    if (ShouldAddProtection(checkClass, scanner.ScanPackers, protection))
                        AppendToDictionary(protections, file, protection);

                    // If we had a protection, check if it is extractable
                    if (!string.IsNullOrWhiteSpace(protection))
                        HandleExtractable(scanner, stream, file, checkClass, protections);
                });
            }

            // If we have a Linear Executable
            else if (wrapper is LinearExecutable lex)
            {
                Parallel.ForEach(ScanningClasses.LinearExecutableCheckClasses, checkClass =>
                {
                    // Get the protection for the class, if possible
                    string protection = checkClass.CheckLinearExecutable(file, lex, scanner.IncludeDebug);
                    if (ShouldAddProtection(checkClass, scanner.ScanPackers, protection))
                        AppendToDictionary(protections, file, protection);

                    // If we had a protection, check if it is extractable
                    if (!string.IsNullOrWhiteSpace(protection))
                        HandleExtractable(scanner, stream, file, checkClass, protections);
                });
            }

            // If we have a Portable Executable
            else if (wrapper is PortableExecutable pex)
            {
                Parallel.ForEach(ScanningClasses.PortableExecutableCheckClasses, checkClass =>
                {
                    // Get the protection for the class, if possible
                    string protection = checkClass.CheckPortableExecutable(file, pex, scanner.IncludeDebug);
                    if (ShouldAddProtection(checkClass, scanner.ScanPackers, protection))
                        AppendToDictionary(protections, file, protection);

                    // If we had a protection, check if it is extractable
                    if (!string.IsNullOrWhiteSpace(protection))
                        HandleExtractable(scanner, stream, file, checkClass, protections);
                });
            }

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
            if (scanPackers || !checkClass.GetType().Namespace.ToLowerInvariant().Contains("packer"))
                return true;

            // Everything else fails
            return false;
        }

        /// <summary>
        /// Handle extractable protections and packers
        /// </summary>
        /// <param name="scanner">Scanner object for state tracking</param>
        /// <param name="stream">Stream representing the input file</param>
        /// <param name="file">Path to the input file</param>
        /// <param name="checkingClass">Class representing the current packer or protection</param>
        /// <param name="protections">Set of existing protections to append to</param>
        private static void HandleExtractable(Scanner scanner, Stream stream, string file, object checkingClass, ConcurrentDictionary<string, ConcurrentQueue<string>> protections)
        {
            // If we don't have an IExtractable implementation
            if (!(checkingClass is IExtractable extractable))
                return;

            // If we have an invalid file
            if (file == null)
                return;

            // If the extractable file itself fails
            try
            {
                // Extract and get the output path
                string tempPath = extractable.Extract(stream, file, scanner.IncludeDebug);
                if (tempPath != null)
                    return;

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
                StripFromKeys(protections, tempPath);
                PrependToKeys(subProtections, file);
                AppendToDictionary(protections, subProtections);
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }
        }

        #endregion
    }
}
