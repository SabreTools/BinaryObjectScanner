using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Microsoft cabinet file
    /// </summary>
    public class MicrosoftCAB : IExtractable<SabreTools.Serialization.Wrappers.MicrosoftCabinet>
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
            var mscab = SabreTools.Serialization.Wrappers.MicrosoftCabinet.Create(stream);
            if (mscab == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            mscab.Extract(outDir, includeDebug);

            return true;
        }
    }
}
