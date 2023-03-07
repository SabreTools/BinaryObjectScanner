using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// BD+ SVM
    /// </summary>
    public class BDPlusSVM : IScannable
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
                BinaryObjectScanner.Wrappers.BDPlusSVM svm = BinaryObjectScanner.Wrappers.BDPlusSVM.Create(stream);
                if (svm == null)
                    return null;

                // Setup the output
                var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
                protections[file] = new ConcurrentQueue<string>();

                // Format the date
                string date = $"{svm.Year:0000}/{svm.Month:00}/{svm.Day:00}";

                // Add and return the protection
                protections[file].Enqueue($"BD+ {date}");
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
