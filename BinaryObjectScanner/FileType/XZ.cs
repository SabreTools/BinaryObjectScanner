using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SharpCompress.Compressors.Xz;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// xz archive
    /// </summary>
    public class XZ : IExtractable
    {
        /// <inheritdoc/>
#if NET48
        public string Extract(string file, bool includeDebug)
#else
        public string? Extract(string file, bool includeDebug)
#endif
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(Stream stream, string file, bool includeDebug)
#else
        public string? Extract(Stream? stream, string file, bool includeDebug)
#endif
        {
            try
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
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }
    }
}
