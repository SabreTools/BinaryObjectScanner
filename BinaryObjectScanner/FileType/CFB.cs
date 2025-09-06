using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Compound File Binary
    /// </summary>
    public class CFB : ExtractableBase<SabreTools.Serialization.Wrappers.CFB>
    {
        /// <inheritdoc/>
        public CFB(SabreTools.Serialization.Wrappers.CFB? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var cfb = SabreTools.Serialization.Wrappers.CFB.Create(stream);
            if (cfb == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            cfb.Extract(outDir, includeDebug);

            return true;
        }
    }
}
