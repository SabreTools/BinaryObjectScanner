using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// NovaLogic Game Archive Format
    /// </summary>
    public class PFF : IExtractable
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
            var pff = SabreTools.Serialization.Wrappers.PFF.Create(stream);
            if (pff == null)
                return false;

            // Extract all files
            Directory.CreateDirectory(outDir);
            pff.ExtractAll(outDir);

            return true;
        }
    }
}
