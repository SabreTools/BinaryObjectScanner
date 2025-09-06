using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// 7-zip archive
    /// </summary>
    public class SevenZip : ExtractableBase<SabreTools.Serialization.Wrappers.SevenZip>
    {
        /// <inheritdoc/>
        public SevenZip(SabreTools.Serialization.Wrappers.SevenZip? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Handle invalid inputs
            if (stream == null || stream.Length == 0)
                return false;

            // Create the wrapper
            var sevenZip = SabreTools.Serialization.Wrappers.SevenZip.Create(stream);
            if (sevenZip == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            sevenZip.Extract(outDir, includeDebug);

            return true;
        }
    }
}