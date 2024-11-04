using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;
#endif

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// bzip2 archive
    /// </summary>
    public class BZip2 : IExtractable
    {
        /// <inheritdoc/>
        public string? Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, includeDebug);
        }

        /// <inheritdoc/>
        public string? Extract(Stream? stream, string file, bool includeDebug)
        {
            if (stream == null)
                return null;

#if NET462_OR_GREATER || NETCOREAPP
            try
            {
                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (var bz2File = new BZip2Stream(stream, CompressionMode.Decompress, true))
                {
                    string tempFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
                    using (FileStream fs = File.OpenWrite(tempFile))
                    {
                        bz2File.CopyTo(fs);
                    }
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
#else
            return null;
#endif
        }
    }
}
