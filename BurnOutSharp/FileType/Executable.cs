using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BurnOutSharp.FileType
{
    internal class Executable : IScannable
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
        public ConcurrentDictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // Files can be protected in multiple ways
            var protections = new ConcurrentDictionary<string, List<string>>();

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
                Utilities.AppendToDictionary(protections, file, "[File too large to be scanned]");
                return protections;
            }

            // If we can, seek to the beginning of the stream
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            // Iterate through all content checks
            Parallel.ForEach(contentCheckClasses, contentCheckClass =>
            {
                string protection = contentCheckClass.CheckContents(file, fileContent, scanner.IncludePosition);

                // If we have a valid content check based on settings
                if (!contentCheckClass.GetType().Namespace.ToLowerInvariant().Contains("packertype") || scanner.ScanPackers)
                {
                    if (!string.IsNullOrWhiteSpace(protection))
                        Utilities.AppendToDictionary(protections, file, protection);
                }

                // If we have an IScannable implementation
                if (contentCheckClass is IScannable)
                {
                    IScannable scannable = contentCheckClass as IScannable;
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
    }
}
