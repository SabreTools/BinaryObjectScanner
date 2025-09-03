using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// LZ-compressed file, KWAJ variant
    /// </summary>
    public class LZKWAJ : IExtractable
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
            var kwaj = SabreTools.Serialization.Wrappers.LZKWAJ.Create(stream);
            if (kwaj == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            kwaj.Extract(outDir, includeDebug);

            return true;
        }
    }
}
