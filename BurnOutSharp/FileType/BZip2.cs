using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SharpCompress.Compressors;
using SharpCompress.Compressors.BZip2;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// bzip2 archive
    /// </summary>
    public class BZip2 : IExtractable
    {
        /// <inheritdoc/>
        public string Extract(string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file);
            }
        }

        /// <inheritdoc/>
        public string Extract(Stream stream, string file)
        {
            // Create a temp output directory
            string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            using (BZip2Stream bz2File = new BZip2Stream(stream, CompressionMode.Decompress, true))
            {
                string tempFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
                using (FileStream fs = File.OpenWrite(tempFile))
                {
                    bz2File.CopyTo(fs);
                }
            }

            return tempPath;
        }
    }
}
