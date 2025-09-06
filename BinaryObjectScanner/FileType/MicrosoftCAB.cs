using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Microsoft cabinet file
    /// </summary>
    public class MicrosoftCAB : ExtractableBase<SabreTools.Serialization.Wrappers.MicrosoftCabinet>
    {
        /// <inheritdoc/>
        public MicrosoftCAB(SabreTools.Serialization.Wrappers.MicrosoftCabinet? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
