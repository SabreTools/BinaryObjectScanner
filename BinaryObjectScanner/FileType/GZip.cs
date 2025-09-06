using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// gzip archive
    /// </summary>
    public class GZip : ExtractableBase<SabreTools.Serialization.Wrappers.GZip>
    {
        /// <inheritdoc/>
        public GZip(SabreTools.Serialization.Wrappers.GZip? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var gcf = SabreTools.Serialization.Wrappers.GZip.Create(stream);
            if (gcf == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            gcf.Extract(outDir, includeDebug);

            return true;
        }
    }
}
