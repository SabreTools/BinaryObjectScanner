using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// MoPaQ game data archive
    /// </summary>
    public class MPQ : ExtractableBase<SabreTools.Serialization.Wrappers.MoPaQ>
    {
        /// <inheritdoc/>
        public MPQ(SabreTools.Serialization.Wrappers.MoPaQ? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
