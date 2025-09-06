using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// LZ-compressed file, QBasic variant
    /// </summary>
    public class LZQBasic : ExtractableBase<SabreTools.Serialization.Wrappers.LZQBasic>
    {
        /// <inheritdoc/>
        public LZQBasic(SabreTools.Serialization.Wrappers.LZQBasic? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var qbasic = SabreTools.Serialization.Wrappers.LZQBasic.Create(stream);
            if (qbasic == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            qbasic.Extract(outDir, includeDebug);

            return true;
        }
    }
}
