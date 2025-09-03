using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// LZ-compressed file, QBasic variant
    /// </summary>
    public class LZQBasic : IExtractable
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
            // Create the wrapper
            var qbasic = SabreTools.Serialization.Wrappers.LZQBasic.Create(stream);
            if (qbasic == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            qbasic.Extract(outDir, includeDebug);

            return true;
        }
    }
}
