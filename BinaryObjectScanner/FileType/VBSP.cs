using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life 2 Level
    /// </summary>
    public class VBSP : IExtractable<SabreTools.Serialization.Wrappers.VBSP>
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
            var vbsp = SabreTools.Serialization.Wrappers.VBSP.Create(stream);
            if (vbsp == null)
                return false;

            // TODO: Introduce helper methods for all specialty lump types

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            vbsp.Extract(outDir, includeDebug);

            return true;
        }
    }
}
