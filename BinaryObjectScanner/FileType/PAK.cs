using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Package File
    /// </summary>
    public class PAK : ExtractableBase<SabreTools.Serialization.Wrappers.PAK>
    {
        /// <inheritdoc/>
        public PAK(SabreTools.Serialization.Wrappers.PAK? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var pak = SabreTools.Serialization.Wrappers.PAK.Create(stream);
            if (pak == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            pak.Extract(outDir, includeDebug);

            return true;
        }
    }
}
