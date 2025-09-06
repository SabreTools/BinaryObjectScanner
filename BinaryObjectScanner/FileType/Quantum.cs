using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Quantum Archive
    /// </summary>
    public class Quantum : ExtractableBase<SabreTools.Serialization.Wrappers.Quantum>
    {
        /// <inheritdoc/>
        public Quantum(SabreTools.Serialization.Wrappers.Quantum? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
