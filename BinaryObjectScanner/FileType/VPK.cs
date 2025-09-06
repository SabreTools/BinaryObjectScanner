using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Valve Package File
    /// </summary>
    public class VPK : ExtractableBase<SabreTools.Serialization.Wrappers.VPK>
    {
        /// <inheritdoc/>
        public VPK(SabreTools.Serialization.Wrappers.VPK? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
