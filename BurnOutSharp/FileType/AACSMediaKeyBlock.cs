using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using BurnOutSharp.Interfaces;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// AACS media key block
    /// </summary>
    public class AACSMediaKeyBlock : IScannable
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
            // If the MKB file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Create the wrapper
                Wrappers.AACSMediaKeyBlock mkb = Wrappers.AACSMediaKeyBlock.Create(stream);
                if (mkb == null)
                    return null;

                // Setup the output
                var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
                protections[file] = new ConcurrentQueue<string>();

                var typeAndVersion = mkb.Records.FirstOrDefault(r => r.RecordType == Models.AACS.RecordType.TypeAndVersion);
                if (typeAndVersion == null)
                    protections[file].Enqueue("AACS (Unknown Version)");
                else
                    protections[file].Enqueue($"AACS {(typeAndVersion as Models.AACS.TypeAndVersionRecord).VersionNumber}");

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
