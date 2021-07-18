using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;

namespace BurnOutSharp.FileType
{
    internal class BZip2 : IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte?[] { 0x42, 0x52, 0x68 }))
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
            // If the BZip2 file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (BZip2Stream bz2File = new BZip2Stream(stream, CompressionMode.Decompress, true))
                {
                    // If an individual entry fails
                    try
                    {
                        string tempFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
                        using (FileStream fs = File.OpenWrite(tempFile))
                        {
                            bz2File.CopyTo(fs);
                        }
                    }
                    catch { }
                }

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch { }

                // Remove temporary path references
                Utilities.StripFromKeys(protections, tempPath);

                return protections;
            }
            catch { }

            return null;
        }
    }
}
