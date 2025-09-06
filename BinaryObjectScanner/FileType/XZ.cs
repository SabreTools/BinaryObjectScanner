using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// xz archive
    /// </summary>
    public class XZ : ExtractableBase<SabreTools.Serialization.Wrappers.XZ>
    {
        /// <inheritdoc/>
        public XZ(SabreTools.Serialization.Wrappers.XZ? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
