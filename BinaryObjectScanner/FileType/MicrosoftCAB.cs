using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Microsoft cabinet file
    /// </summary>
    public class MicrosoftCAB : ExtractableBase<SabreTools.Serialization.Wrappers.MicrosoftCabinet>
    {
        /// <inheritdoc/>
        public MicrosoftCAB(SabreTools.Serialization.Wrappers.MicrosoftCabinet? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var mscab = SabreTools.Serialization.Wrappers.MicrosoftCabinet.Create(stream);
            if (mscab == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            mscab.Extract(outDir, includeDebug);

            return true;
        }
    }
}
