using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using SharpCompress.Compressors.Xz;
using static BinaryObjectScanner.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// xz archive
    /// </summary>
    public class XZ : IScannable
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
            // If the xz file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (XZStream xzFile = new XZStream(stream))
                {
                    // If an individual entry fails
                    try
                    {
                        string tempFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
                        using (FileStream fs = File.OpenWrite(tempFile))
                        {
                            xzFile.CopyTo(fs);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (scanner.IncludeDebug) Console.WriteLine(ex);
                    }
                }

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
