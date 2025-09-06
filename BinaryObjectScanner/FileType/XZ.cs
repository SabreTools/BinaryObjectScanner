using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// xz archive
    /// </summary>
    public class XZ : ExtractableBase<SabreTools.Serialization.Wrappers.XZ>
    {
        /// <inheritdoc/>
        public XZ(SabreTools.Serialization.Wrappers.XZ? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Handle invalid inputs
            if (stream == null || stream.Length == 0)
                return false;

            // Create the wrapper
            var xz = SabreTools.Serialization.Wrappers.XZ.Create(stream);
            if (xz == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            xz.Extract(outDir, includeDebug);

            return true;
        }
    }
}
