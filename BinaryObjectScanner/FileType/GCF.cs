using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Game Cache File
    /// </summary>
    public class GCF : IExtractable<SabreTools.Serialization.Wrappers.GCF>
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
            var gcf = SabreTools.Serialization.Wrappers.GCF.Create(stream);
            if (gcf == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            gcf.Extract(outDir, includeDebug);

            return true;
        }
    }
}
