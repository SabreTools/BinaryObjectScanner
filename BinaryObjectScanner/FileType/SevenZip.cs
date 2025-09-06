using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// 7-zip archive
    /// </summary>
    public class SevenZip : ExtractableBase<SabreTools.Serialization.Wrappers.SevenZip>
    {
        /// <inheritdoc/>
        public SevenZip(SabreTools.Serialization.Wrappers.SevenZip? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}