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
            // Handle invalid inputs
            if (stream == null || stream.Length == 0)
                return false;

            // Create the wrapper
            var bzip = SabreTools.Serialization.Wrappers.BZip2.Create(stream);
            if (bzip == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            bzip.Extract(outDir, includeDebug);

            return true;
        }
    }
}
