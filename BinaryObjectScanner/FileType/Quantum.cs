using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Quantum Archive
    /// </summary>
    public class Quantum : ExtractableBase<SabreTools.Serialization.Wrappers.Quantum>
    {
        /// <inheritdoc/>
        public Quantum(SabreTools.Serialization.Wrappers.Quantum? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var qtm = SabreTools.Serialization.Wrappers.Quantum.Create(stream);
            if (qtm == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            qtm.Extract(outDir, includeDebug);

            return true;
        }
    }
}
