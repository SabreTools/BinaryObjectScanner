using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// gzip archive
    /// </summary>
    public class GZip : ExtractableBase<SabreTools.Serialization.Wrappers.GZip>
    {
        /// <inheritdoc/>
        public GZip(SabreTools.Serialization.Wrappers.GZip? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
