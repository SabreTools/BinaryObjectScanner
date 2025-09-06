using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// PKWARE ZIP archive and derivatives
    /// </summary>
    public class PKZIP : IExtractable<SabreTools.Serialization.Wrappers.PKZIP>
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
            var pkzip = SabreTools.Serialization.Wrappers.PKZIP.Create(stream);
            if (pkzip == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            pkzip.Extract(outDir, includeDebug);

            return true;
        }
    }
}
