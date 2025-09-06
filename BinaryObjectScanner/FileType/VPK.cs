using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Valve Package File
    /// </summary>
    public class VPK : IExtractable<SabreTools.Serialization.Wrappers.VPK>
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
            var vpk = SabreTools.Serialization.Wrappers.VPK.Create(stream);
            if (vpk == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            vpk.Extract(outDir, includeDebug);

            return true;
        }
    }
}
