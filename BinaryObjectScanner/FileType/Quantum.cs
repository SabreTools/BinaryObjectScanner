using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Quantum Archive
    /// </summary>
    public class Quantum : IExtractable
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
            var qtm = SabreTools.Serialization.Wrappers.Quantum.Create(stream);
            if (qtm == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            qtm.Extract(outDir, includeDebug);

            return true;
        }
    }
}
