using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Compressors.Xz;
#endif

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// xz archive
    /// </summary>
    public class XZ : IExtractable
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
#if NET462_OR_GREATER || NETCOREAPP
            try
            {
                // Try opening the stream
                using var xzFile = new XZStream(stream);

                // Create the output file path
                Directory.CreateDirectory(outDir);
                string tempFile = Path.Combine(outDir, Guid.NewGuid().ToString());

                // Extract the file
                using FileStream fs = File.OpenWrite(tempFile);
                xzFile.CopyTo(fs);

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
