using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// LZ-compressed file, KWAJ variant
    /// </summary>
    public class LZKWAJ : ExtractableBase<SabreTools.Serialization.Wrappers.LZKWAJ>
    {
        /// <inheritdoc/>
        public LZKWAJ(SabreTools.Serialization.Wrappers.LZKWAJ? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
