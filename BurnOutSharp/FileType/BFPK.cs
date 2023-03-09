using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using BinaryObjectScanner.Interfaces;
using static BinaryObjectScanner.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// BFPK custom archive format
    /// </summary>
    public class BFPK : IExtractable, IScannable
    {
        /// <inheritdoc/>
        public string Extract(string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file);
            }
        }

        /// <inheritdoc/>
        public string Extract(Stream stream, string file)
        {
            // Create the wrapper
            BinaryObjectScanner.Wrappers.BFPK bfpk = BinaryObjectScanner.Wrappers.BFPK.Create(stream);
            if (bfpk == null)
                return null;

            // Create a temp output directory
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            // Extract all files
            bfpk.ExtractAll(tempPath);

            return tempPath;
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
            // If the BFPK file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Create the wrapper
                BinaryObjectScanner.Wrappers.BFPK bfpk = BinaryObjectScanner.Wrappers.BFPK.Create(stream);
                if (bfpk == null)
                    return null;

                // Extract all files
                bfpk.ExtractAll(tempPath);

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch (Exception ex)
                {
                    if (scanner.IncludeDebug) Console.WriteLine(ex);
                }

                // Remove temporary path references
                StripFromKeys(protections, tempPath);

                return protections;
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
