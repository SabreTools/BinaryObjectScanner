using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// RAR archive
    /// </summary>
    public class RAR : ExtractableBase<SabreTools.Serialization.Wrappers.RAR>
    {
        /// <inheritdoc/>
        public RAR(SabreTools.Serialization.Wrappers.RAR? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}