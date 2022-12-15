using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using StormLibSharp;
using static BurnOutSharp.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// MPQ game data archive
    /// </summary>
    public class MPQ : IScannable
    {
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

        // TODO: Add stream opening support
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the mpq file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (MpqArchive mpqArchive = new MpqArchive(file, FileAccess.Read))
                {
                    // Try to open the listfile
                    string listfile = null;
                    MpqFileStream listStream = mpqArchive.OpenFile("(listfile)");
                   
                    // If we can't read the listfile, we just return
                    if (!listStream.CanRead)
                        return null;

                    // Read the listfile in for processing
                    using (StreamReader sr = new StreamReader(listStream))
                    {
                        listfile = sr.ReadToEnd();
                    }

                    // Split the listfile by newlines
                    string[] listfileLines = listfile.Replace("\r\n", "\n").Split('\n');

                    // Loop over each entry
                    foreach (string sub in listfileLines)
                    {
                        // If an individual entry fails
                        try
                        {
                            string tempFile = Path.Combine(tempPath, sub);
                            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));
                            mpqArchive.ExtractFile(sub, tempFile);
                        }
                        catch (Exception ex)
                        {
                            if (scanner.IncludeDebug) Console.WriteLine(ex);
                        }
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
