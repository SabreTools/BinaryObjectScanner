using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// bzip2 archive
    /// </summary>
    public class BZip2 : ExtractableBase<SabreTools.Serialization.Wrappers.BZip2>
    {
        /// <inheritdoc/>
        public BZip2(SabreTools.Serialization.Wrappers.BZip2? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
