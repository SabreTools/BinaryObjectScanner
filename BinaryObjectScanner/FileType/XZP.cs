using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// XBox Package File
    /// </summary>
    public class XZP : ExtractableBase<SabreTools.Serialization.Wrappers.XZP>
    {
        /// <inheritdoc/>
        public XZP(SabreTools.Serialization.Wrappers.XZP? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
