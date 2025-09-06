using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// PKWARE ZIP archive and derivatives
    /// </summary>
    public class PKZIP : ExtractableBase<SabreTools.Serialization.Wrappers.PKZIP>
    {
        /// <inheritdoc/>
        public PKZIP(SabreTools.Serialization.Wrappers.PKZIP? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
