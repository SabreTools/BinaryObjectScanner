using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// RAR archive
    /// </summary>
    public class RAR : ExtractableBase<SabreTools.Serialization.Wrappers.RAR>
    {
        /// <inheritdoc/>
        public RAR(SabreTools.Serialization.Wrappers.RAR? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Handle invalid inputs
            if (stream == null || stream.Length == 0)
                return false;

            // Create the wrapper
            var rar = SabreTools.Serialization.Wrappers.RAR.Create(stream);
            if (rar == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            rar.Extract(outDir, includeDebug);

            return true;
        }
    }
}