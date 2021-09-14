using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Tools;

namespace BurnOutSharp.FileType
{
    public class Executable : IScannable
    {
        /// <summary>
        /// Cache for all IContentCheck types
        /// </summary>
        private static readonly IEnumerable<IContentCheck> contentCheckClasses = InitContentCheckClasses();

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

            // Load the current file content
            byte[] fileContent = null;
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

            // TODO: Start moving toward reading from the stream directly. In theory,
            // deserialization can be done at this point, and if all of the sections are populated
            // properly, nearly all of the content checks can be dealt with without having
            // to take up as much memory as it does right now reading into the fileContent
            // byte array

            // Create PortableExecutable and NewExecutable objects for use in the checks
            stream.Seek(0, SeekOrigin.Begin);
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            NewExecutable nex = NewExecutable.Deserialize(fileContent, 0);

            // Create PortableExecutable and NewExecutable objects for use in the checks
            // PortableExecutable pex = PortableExecutable.Deserialize(stream);
            // stream.Seek(0, SeekOrigin.Begin);
            // NewExecutable nex = NewExecutable.Deserialize(stream);
            // stream.Seek(0, SeekOrigin.Begin);

            // Iterate through all content checks
            Parallel.ForEach(contentCheckClasses, contentCheckClass =>
            {
                // Track if any protection is found
                bool foundProtection = false;

                // Check using custom content checks first
                string protection = contentCheckClass.CheckContents(file, fileContent, scanner.IncludeDebug, pex, nex);
                foundProtection |= !string.IsNullOrWhiteSpace(protection);
                if (ShouldAddProtection(contentCheckClass, scanner, protection))
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

            return protections;
        }

        /// <summary>
        /// Initialize all IContentCheck implementations
        /// </summary>
        private static IEnumerable<IContentCheck> InitContentCheckClasses()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && t.GetInterface(nameof(IContentCheck)) != null)
                .Select(t => Activator.CreateInstance(t) as IContentCheck);
        }
    
        /// <summary>
        /// Check to see if a protection should be added or not
        /// </summary>
        /// <param name="contentCheckClass">Class that was last used to check</param>
        /// <param name="scanner">Scanner object for state tracking</param>
        /// <param name="protection">The protection result to be checked</param>
        private bool ShouldAddProtection(IContentCheck contentCheckClass, Scanner scanner, string protection)
        {
            // If we have a valid content check based on settings
            if (!contentCheckClass.GetType().Namespace.ToLowerInvariant().Contains("packertype") || scanner.ScanPackers)
            {
                if (!string.IsNullOrWhiteSpace(protection))
                    return true;
            }

            return false;
        }
    }
}
