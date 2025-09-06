using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Package File
    /// </summary>
    public class PAK : IExtractable<SabreTools.Serialization.Wrappers.PAK>
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
            var pak = SabreTools.Serialization.Wrappers.PAK.Create(stream);
            if (pak == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            pak.Extract(outDir, includeDebug);

            return true;
        }
    }
}
