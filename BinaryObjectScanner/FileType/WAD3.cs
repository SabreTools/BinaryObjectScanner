using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Texture Package File
    /// </summary>
    public class WAD3 : ExtractableBase<SabreTools.Serialization.Wrappers.WAD3>
    {
        /// <inheritdoc/>
        public WAD3(SabreTools.Serialization.Wrappers.WAD3? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
