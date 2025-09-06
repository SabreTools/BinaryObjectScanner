using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// LZ-compressed file, KWAJ variant
    /// </summary>
    public class LZKWAJ : ExtractableBase<SabreTools.Serialization.Wrappers.LZKWAJ>
    {
        /// <inheritdoc/>
        public LZKWAJ(SabreTools.Serialization.Wrappers.LZKWAJ? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var kwaj = SabreTools.Serialization.Wrappers.LZKWAJ.Create(stream);
            if (kwaj == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            kwaj.Extract(outDir, includeDebug);

            return true;
        }
    }
}
