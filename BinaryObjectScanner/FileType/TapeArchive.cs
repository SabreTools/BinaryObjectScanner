using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Tape archive
    /// </summary>
    public class TapeArchive : ExtractableBase<SabreTools.Serialization.Wrappers.TapeArchive>
    {
        /// <inheritdoc/>
        public TapeArchive(SabreTools.Serialization.Wrappers.TapeArchive? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var tar = SabreTools.Serialization.Wrappers.TapeArchive.Create(stream);
            if (tar == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            tar.Extract(outDir, includeDebug);

            return true;
        }
    }
}
