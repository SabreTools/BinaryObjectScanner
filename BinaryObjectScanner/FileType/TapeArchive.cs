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
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
