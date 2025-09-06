using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// LZ-compressed file, SZDD variant
    /// </summary>
    public class LZSZDD : ExtractableBase<SabreTools.Serialization.Wrappers.LZSZDD>
    {
        /// <inheritdoc/>
        public LZSZDD(SabreTools.Serialization.Wrappers.LZSZDD? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var szdd = SabreTools.Serialization.Wrappers.LZSZDD.Create(stream);
            if (szdd == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            szdd.Extract(outDir, includeDebug);

            return true;
        }
    }
}
