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
            // Create the wrapper
            var bfpk = SabreTools.Serialization.Wrappers.BFPK.Create(stream);
            if (bfpk == null)
                return false;

            // Extract all files
            Directory.CreateDirectory(outDir);
            bfpk.Extract(outDir, includeDebug);

            return true;
        }
    }
}
