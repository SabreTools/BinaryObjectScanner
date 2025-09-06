using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// XBox Package File
    /// </summary>
    public class XZP : ExtractableBase<SabreTools.Serialization.Wrappers.XZP>
    {
        /// <inheritdoc/>
        public XZP(SabreTools.Serialization.Wrappers.XZP? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var xzp = SabreTools.Serialization.Wrappers.XZP.Create(stream);
            if (xzp == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            xzp.Extract(outDir, includeDebug);

            return true;
        }
    }
}
