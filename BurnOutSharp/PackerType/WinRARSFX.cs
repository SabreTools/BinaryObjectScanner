using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;

namespace BurnOutSharp.PackerType
{
    public class WinRARSFX : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                // Software\WinRAR SFX
                new ContentMatchSet(new byte?[]
                {
                    0x53, 0x6F, 0x66, 0x74, 0x77, 0x61, 0x72, 0x65,
                    0x5C, 0x57, 0x69, 0x6E, 0x52, 0x41, 0x52, 0x20,
                    0x53, 0x46, 0x58
                }, "WinRAR SFX"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false) => null;

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
            // If the rar file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Should be using stream instead of file, but stream fails to extract anything. My guess is that the executable portion of the archive is causing stream to fail, but not file.
                using (RarArchive zipFile = RarArchive.Open(file, new SharpCompress.Readers.ReaderOptions() {LookForHeader = true}))
                {
                    foreach (var entry in zipFile.Entries)
                    {
                        // If an individual entry fails
                        try
                        {
                            // If we have a directory, skip it
                            if (entry.IsDirectory)
                                continue;

                            string tempFile = Path.Combine(tempPath, entry.Key);
                            entry.WriteToFile(tempFile);
                        }
                        catch { }
                    }
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
