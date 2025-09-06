using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Tape archive
    /// </summary>
    public class TapeArchive : IExtractable<SabreTools.Serialization.Wrappers.TapeArchive>
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
            var tar = SabreTools.Serialization.Wrappers.TapeArchive.Create(stream);
            if (tar == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            tar.Extract(outDir, includeDebug);

            return true;
        }
    }
}
