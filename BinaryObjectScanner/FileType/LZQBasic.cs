using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// LZ-compressed file, QBasic variant
    /// </summary>
    public class LZQBasic : ExtractableBase<SabreTools.Serialization.Wrappers.LZQBasic>
    {
        /// <inheritdoc/>
        public LZQBasic(SabreTools.Serialization.Wrappers.LZQBasic? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
