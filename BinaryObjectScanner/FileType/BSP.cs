using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Level
    /// </summary>
    public class BSP : ExtractableBase<SabreTools.Serialization.Wrappers.BSP>
    {
        /// <inheritdoc/>
        public BSP(SabreTools.Serialization.Wrappers.BSP? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
