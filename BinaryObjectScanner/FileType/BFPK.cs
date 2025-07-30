using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// BFPK custom archive format
    /// </summary>
    public class BFPK : IExtractable
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
            var bfpk = SabreTools.Serialization.Wrappers.BFPK.Create(stream);
            if (bfpk == null)
                return false;

            // Extract all files
            Directory.CreateDirectory(outDir);
            bfpk.ExtractAll(outDir);

            return true;
        }
    }
}
