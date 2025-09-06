using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life 2 Level
    /// </summary>
    public class VBSP : ExtractableBase<SabreTools.Serialization.Wrappers.VBSP>
    {
        /// <inheritdoc/>
        public VBSP(SabreTools.Serialization.Wrappers.VBSP? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
