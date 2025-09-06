using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// SGA game archive
    /// </summary>
    public class SGA : ExtractableBase<SabreTools.Serialization.Wrappers.SGA>
    {
        /// <inheritdoc/>
        public SGA(SabreTools.Serialization.Wrappers.SGA? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var sga = SabreTools.Serialization.Wrappers.SGA.Create(stream);
            if (sga == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            sga.Extract(outDir, includeDebug);

            return true;
        }
    }
}
