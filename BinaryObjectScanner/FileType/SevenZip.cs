using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// 7-zip archive
    /// </summary>
    public class SevenZip : IExtractable
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
            var sevenZip = SabreTools.Serialization.Wrappers.SevenZip.Create(stream);
            if (sevenZip == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            sevenZip.Extract(outDir, includeDebug);

            return true;
        }
    }
}