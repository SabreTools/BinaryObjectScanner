using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// bzip2 archive
    /// </summary>
    public class BZip2 : IExtractable
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
            // Handle invalid inputs
            if (stream == null || stream.Length == 0)
                return false;

            // Create the wrapper
            var bzip = SabreTools.Serialization.Wrappers.BZip2.Create(stream);
            if (bzip == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            bzip.Extract(outDir, includeDebug);

            return true;
        }
    }
}
