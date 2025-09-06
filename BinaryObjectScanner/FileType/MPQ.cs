using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// MoPaQ game data archive
    /// </summary>
    public class MPQ : ExtractableBase<SabreTools.Serialization.Wrappers.MoPaQ>
    {
        /// <inheritdoc/>
        public MPQ(SabreTools.Serialization.Wrappers.MoPaQ? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var mpq = SabreTools.Serialization.Wrappers.MoPaQ.Create(stream);
            if (mpq == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            mpq.Extract(outDir, includeDebug);

            return true;
        }
    }
}
