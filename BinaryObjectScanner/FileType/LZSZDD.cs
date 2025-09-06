using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// LZ-compressed file, SZDD variant
    /// </summary>
    public class LZSZDD : IExtractable<SabreTools.Serialization.Wrappers.LZSZDD>
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
            var szdd = SabreTools.Serialization.Wrappers.LZSZDD.Create(stream);
            if (szdd == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            szdd.Extract(outDir, includeDebug);

            return true;
        }
    }
}
