using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BurnOutSharp.FileType
{
    internal class Textfile : IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic)
        {
            return ShouldScan(magic, null);
        }

        /// <summary>
        /// Determine if a file signature or extension matches one of the expected values
        /// </summary>
        /// <param name="magic">Byte array representing the file header</param>
        /// <param name="extension">Extension for the file being checked</param>
        /// <returns>True if the signature is valid, false otherwise</returns>
        public bool ShouldScan(byte[] magic, string extension)
        {
            // Rich Text File
            if (magic.StartsWith(new byte?[] { 0x7b, 0x5c, 0x72, 0x74, 0x66, 0x31 }))
                return true;

            // HTML
            if (magic.StartsWith(new byte?[] { 0x3c, 0x68, 0x74, 0x6d, 0x6c }))
                return true;

            // HTML and XML
            if (magic.StartsWith(new byte?[] { 0x3c, 0x21, 0x44, 0x4f, 0x43, 0x54, 0x59, 0x50, 0x45 }))
                return true;

            // Microsoft Office File (old)
            if (magic.StartsWith(new byte?[] { 0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1 }))
                return true;

            // Generic textfile (no header)
            if (string.Equals(extension?.TrimStart('.'), "txt", StringComparison.OrdinalIgnoreCase))
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

            try
            {
                // Load the current file content
                string fileContent = null;
                using (var sr = new StreamReader(stream, Encoding.Default, true, 1024 * 1024, true))
                {
                    fileContent = sr.ReadToEnd();
                }

                // CD-Key
                if (fileContent.Contains("a valid serial number is required"))
                    Utilities.AppendToDictionary(protections, file, "CD-Key / Serial");
                else if (fileContent.Contains("serial number is located"))
                    Utilities.AppendToDictionary(protections, file, "CD-Key / Serial");

                // MediaMax
                if (fileContent.Contains("MediaMax technology"))
                    Utilities.AppendToDictionary(protections, file, "MediaMax CD-3");

                // XCP
                if (fileContent.Contains("http://cp.sonybmg.com/xcp/"))
                    Utilities.AppendToDictionary(protections, file, "XCP");
            }
            catch
            {
                // We don't care what the error was
            }

            return protections;
        }
    }
}
