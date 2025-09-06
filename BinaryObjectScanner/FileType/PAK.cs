using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Package File
    /// </summary>
    public class PAK : ExtractableBase<SabreTools.Serialization.Wrappers.PAK>
    {
        /// <inheritdoc/>
        public PAK(SabreTools.Serialization.Wrappers.PAK? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
