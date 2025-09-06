using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// SGA game archive
    /// </summary>
    public class SGA : ExtractableBase<SabreTools.Serialization.Wrappers.SGA>
    {
        /// <inheritdoc/>
        public SGA(SabreTools.Serialization.Wrappers.SGA? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
