using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// LZ-compressed file, SZDD variant
    /// </summary>
    public class LZSZDD : ExtractableBase<SabreTools.Serialization.Wrappers.LZSZDD>
    {
        /// <inheritdoc/>
        public LZSZDD(SabreTools.Serialization.Wrappers.LZSZDD? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
