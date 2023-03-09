using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SharpCompress.Compressors.Xz;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// xz archive
    /// </summary>
    public class XZ : IExtractable
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

            using (XZStream xzFile = new XZStream(stream))
            {
                string tempFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
                using (FileStream fs = File.OpenWrite(tempFile))
                {
                    xzFile.CopyTo(fs);
                }
            }

            return tempPath;
        }
    }
}
