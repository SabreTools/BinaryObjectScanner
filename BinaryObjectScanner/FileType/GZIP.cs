﻿using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Compression.Deflate;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// gzip archive
    /// </summary>
    public class GZIP : IExtractable
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

            try
            {
                // Try opening the stream
                using var gzipFile = new GZipStream(stream, CompressionMode.Decompress, true);

                // Create the output file path
                Directory.CreateDirectory(outDir);
                string tempFile = Path.Combine(outDir, Guid.NewGuid().ToString());

                // Extract the file
                using FileStream fs = File.OpenWrite(tempFile);
                gzipFile.CopyTo(fs);

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
        }
    }
}
