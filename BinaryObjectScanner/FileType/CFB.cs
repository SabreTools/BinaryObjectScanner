using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Compound File Binary
    /// </summary>
    public class CFB : ExtractableBase<SabreTools.Serialization.Wrappers.CFB>
    {
        /// <inheritdoc/>
        public CFB(SabreTools.Serialization.Wrappers.CFB? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
