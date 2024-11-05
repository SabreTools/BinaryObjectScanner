﻿using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
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
        {
            if (!File.Exists(file))
                return false;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, outDir, includeDebug);
        }

        /// <inheritdoc/>
        public bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            if (stream == null || !stream.CanRead)
                return false;

#if NET462_OR_GREATER || NETCOREAPP
            try
            {
                using var sevenZip = SevenZipArchive.Open(stream);
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
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
#else
            return false;
#endif
        }
    }
}
