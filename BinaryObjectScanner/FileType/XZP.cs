using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// XBox Package File
    /// </summary>
    public class XZP : IExtractable<SabreTools.Serialization.Wrappers.XZP>
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
            var xzp = SabreTools.Serialization.Wrappers.XZP.Create(stream);
            if (xzp == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            xzp.Extract(outDir, includeDebug);

            return true;
        }
    }
}
