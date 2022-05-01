using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BurnOutSharp.ExecutableType.Microsoft.NE;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Tools;

namespace BurnOutSharp.FileType
{
    public class Executable : IScannable
    {
        #region Checking Class Instances

        /// <summary>
        /// Cache for all IContentCheck types
        /// </summary>
        private static readonly IEnumerable<IContentCheck> contentCheckClasses = Initializer.InitContentCheckClasses();

        /// <summary>
        /// Cache for all INEContentCheck types
        /// </summary>
        private static readonly IEnumerable<INEContentCheck> neContentCheckClasses = Initializer.InitNEContentCheckClasses();

        /// <summary>
        /// Cache for all IPEContentCheck types
        /// </summary>
        private static readonly IEnumerable<IPEContentCheck> peContentCheckClasses = Initializer.InitPEContentCheckClasses();

        #endregion

        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic)
        {
            // DOS MZ executable file format (and descendants)
            if (magic.StartsWith(new byte?[] { 0x4d, 0x5a }))
                return true;

            // Executable and Linkable Format
            if (magic.StartsWith(new byte?[] { 0x7f, 0x45, 0x4c, 0x46 }))
                return true;

            // Mach-O binary (32-bit)
            if (magic.StartsWith(new byte?[] { 0xfe, 0xed, 0xfa, 0xce }))
                return true;

            // Mach-O binary (32-bit, reverse byte ordering scheme)
            if (magic.StartsWith(new byte?[] { 0xce, 0xfa, 0xed, 0xfe }))
                return true;

            // Mach-O binary (64-bit)
            if (magic.StartsWith(new byte?[] { 0xfe, 0xed, 0xfa, 0xcf }))
                return true;

            // Mach-O binary (64-bit, reverse byte ordering scheme)
            if (magic.StartsWith(new byte?[] { 0xcf, 0xfa, 0xed, 0xfe }))
                return true;

            // Prefrred Executable File Format
            if (magic.StartsWith(new byte?[] { 0x4a, 0x6f, 0x79, 0x21, 0x70, 0x65, 0x66, 0x66 }))
                return true;

            return false;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
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
                catch
                {
                    Utilities.AppendToDictionary(protections, file, "[Out of memory attempting to open]");
                    return protections;
                }
            }

            // Create PortableExecutable and NewExecutable objects for use in the checks
            stream.Seek(0, SeekOrigin.Begin);
            PortableExecutable pex = new PortableExecutable(stream);
            stream.Seek(0, SeekOrigin.Begin);
            NewExecutable nex = new NewExecutable(stream);
            stream.Seek(0, SeekOrigin.Begin);

            // Iterate through all generic content checks
            if (fileContent != null)
            {
                Parallel.ForEach(contentCheckClasses, contentCheckClass =>
                {
                    string protection = contentCheckClass.CheckContents(file, fileContent, scanner.IncludeDebug, pex, nex);
                    if (ShouldAddProtection(contentCheckClass, scanner.ScanPackers, protection))
                        Utilities.AppendToDictionary(protections, file, protection);

                    // If we have an IScannable implementation
                    if (contentCheckClass is IScannable scannable)
                    {
                        if (file != null && !string.IsNullOrEmpty(protection))
                        {
                            var subProtections = scannable.Scan(scanner, null, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }
                    }
                });
            }

            // If we have a NE executable, iterate through all NE content checks
            if (nex?.Initialized == true)
            {
                Parallel.ForEach(neContentCheckClasses, contentCheckClass =>
                {
                    // Check using custom content checks first
                    string protection = contentCheckClass.CheckNEContents(file, nex, scanner.IncludeDebug);
                    if (ShouldAddProtection(contentCheckClass, scanner.ScanPackers, protection))
                        Utilities.AppendToDictionary(protections, file, protection);

                    // If we have an IScannable implementation
                    if (contentCheckClass is IScannable scannable)
                    {
                        if (file != null && !string.IsNullOrEmpty(protection))
                        {
                            var subProtections = scannable.Scan(scanner, null, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }
                    }
                });
            }

            // If we have a PE executable, iterate through all PE content checks
            if (pex?.Initialized == true)
            {
                Parallel.ForEach(peContentCheckClasses, contentCheckClass =>
                {
                    // Check using custom content checks first
                    string protection = contentCheckClass.CheckPEContents(file, pex, scanner.IncludeDebug);
                    if (ShouldAddProtection(contentCheckClass, scanner.ScanPackers, protection))
                        Utilities.AppendToDictionary(protections, file, protection);

                    // If we have an IScannable implementation
                    if (contentCheckClass is IScannable scannable)
                    {
                        if (file != null && !string.IsNullOrEmpty(protection))
                        {
                            var subProtections = scannable.Scan(scanner, null, file);
                            Utilities.PrependToKeys(subProtections, file);
                            Utilities.AppendToDictionary(protections, subProtections);
                        }
                    }
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
            // If we have a valid content check based on settings
            if (scanPackers || !checkClass.GetType().Namespace.ToLowerInvariant().Contains("packertype"))
            {
                if (!string.IsNullOrWhiteSpace(protection))
                    return true;
            }

            return false;
        }

        #endregion
    }
}
