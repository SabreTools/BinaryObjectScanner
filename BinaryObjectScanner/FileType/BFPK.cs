using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// BFPK custom archive format
    /// </summary>
    public class BFPK : ExtractableBase<SabreTools.Serialization.Wrappers.BFPK>
    {
        /// <inheritdoc/>
        public BFPK(SabreTools.Serialization.Wrappers.BFPK? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
