using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// PKWARE ZIP archive and derivatives
    /// </summary>
    public class PKZIP : ExtractableBase<SabreTools.Serialization.Wrappers.PKZIP>
    {
        /// <inheritdoc/>
        public PKZIP(SabreTools.Serialization.Wrappers.PKZIP? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var pkzip = SabreTools.Serialization.Wrappers.PKZIP.Create(stream);
            if (pkzip == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            pkzip.Extract(outDir, includeDebug);

            return true;
        }
    }
}
