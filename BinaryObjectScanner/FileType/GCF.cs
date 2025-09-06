using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Game Cache File
    /// </summary>
    public class GCF : ExtractableBase<SabreTools.Serialization.Wrappers.GCF>
    {
        /// <inheritdoc/>
        public GCF(SabreTools.Serialization.Wrappers.GCF? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
