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
            if (stream == null || !stream.CanRead)
                return false;

            try
            {
                // Try opening the stream
                using var xzFile = new XZStream(stream);

                // Ensure directory separators are consistent
                string filename = Guid.NewGuid().ToString();
                if (Path.DirectorySeparatorChar == '\\')
                    filename = filename.Replace('/', '\\');
                else if (Path.DirectorySeparatorChar == '/')
                    filename = filename.Replace('\\', '/');

                // Ensure the full output directory exists
                filename = Path.Combine(outDir, filename);
                var directoryName = Path.GetDirectoryName(filename);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                // Extract the file
                using FileStream fs = File.OpenWrite(filename);
                xzFile.CopyTo(fs);
                fs.Flush();

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
