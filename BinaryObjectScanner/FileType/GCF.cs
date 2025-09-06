using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Game Cache File
    /// </summary>
    public class GCF : ExtractableBase<SabreTools.Serialization.Wrappers.GCF>
    {
        /// <inheritdoc/>
        public GCF(SabreTools.Serialization.Wrappers.GCF? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var gcf = SabreTools.Serialization.Wrappers.GCF.Create(stream);
            if (gcf == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            gcf.Extract(outDir, includeDebug);

            return true;
        }
    }
}
