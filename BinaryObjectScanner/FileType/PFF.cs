using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// NovaLogic Game Archive Format
    /// </summary>
    public class PFF : ExtractableBase<SabreTools.Serialization.Wrappers.PFF>
    {
        /// <inheritdoc/>
        public PFF(SabreTools.Serialization.Wrappers.PFF? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
