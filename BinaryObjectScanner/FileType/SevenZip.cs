using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Readers;
#endif

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// 7-zip archive
    /// </summary>
    public class SevenZip : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string file, string outDir, bool includeDebug)
            => Extract(file, outDir, lookForHeader: false, includeDebug);

        /// <inheritdoc cref="IExtractable.Extract(string, string, bool)"/>
        public bool Extract(string file, string outDir, bool lookForHeader, bool includeDebug)
        {
            if (!File.Exists(file))
                return false;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, outDir, lookForHeader, includeDebug);
        }

        /// <inheritdoc/>
        public bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
            => Extract(stream, file, outDir, lookForHeader: false, includeDebug);

        /// <inheritdoc cref="IExtractable.Extract(Stream?, string, string, bool)"/>
        public bool Extract(Stream? stream, string file, string outDir, bool lookForHeader, bool includeDebug)
        {
            if (stream == null || !stream.CanRead)
                return false;

#if NET462_OR_GREATER || NETCOREAPP
            try
            {
                var readerOptions = new ReaderOptions() { LookForHeader = lookForHeader };
                var sevenZip = SevenZipArchive.Open(stream, readerOptions);
                
                // Try to read the file path if no entries are found
                if (sevenZip.Entries.Count == 0 && !string.IsNullOrEmpty(file) && File.Exists(file))
                    sevenZip = SevenZipArchive.Open(file, readerOptions);

                // Currently doesn't flag solid 7z archives with only 1 solid block as solid, but practically speaking
                // this is not much of a concern.
                if (sevenZip.IsSolid)
                    return Extract_Solid(sevenZip, outDir, includeDebug);
                else
                    return Extract_Non_Solid(sevenZip, outDir, includeDebug);

            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
#else
            return false;
#endif
        }
#if NET462_OR_GREATER || NETCOREAPP
        /// <inheritdoc cref="IExtractable.Extract(Stream?, string, string, bool)"/>
        // Extraction method for non-solid archives. This iterates over each entry in the archive to extract every file
        // individually, in order to extract all valid files from the archive.
        public bool Extract_Non_Solid(SevenZipArchive sevenZip, string outDir, bool includeDebug) 
        {

            
            foreach (var entry in sevenZip.Entries)
            {
                try
                {
                    // If the entry is a directory
                    if (entry.IsDirectory)
                        continue;

                    // If the entry has an invalid key
                    if (entry.Key == null)
                        continue;

                    // If we have a partial entry due to an incomplete multi-part archive, skip it
                    if (!entry.IsComplete)
                        continue;

                    string tempFile = Path.Combine(outDir, entry.Key);
                    var directoryName = Path.GetDirectoryName(tempFile);
                    if (directoryName != null && !Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);

                    entry.WriteToFile(tempFile);
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.WriteLine(ex);
                }
            }
            return true;
        }
        
        /// <inheritdoc cref="IExtractable.Extract(Stream?, string, string, bool)"/>
        
        // Extraction method for solid archives. Uses ExtractAllEntries because extraction for solid archives must be
        // done sequentially, and files beyond a corrupted point in a solid archive will be unreadable anyways.
        public bool Extract_Solid(SevenZipArchive sevenZip, string outDir, bool includeDebug)
        {
                try
                {
                    if (!Directory.Exists(outDir))
                        Directory.CreateDirectory(outDir);
                    
                    sevenZip.ExtractAllEntries().WriteAllToDirectory(outDir, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true // Ideally would mark this false, but can't find documentation on how it works
                    });
                    
                }
                catch (Exception ex)
                {
                    if (includeDebug) Console.WriteLine(ex);
                }

                return true;
        }
#endif
    }
}
