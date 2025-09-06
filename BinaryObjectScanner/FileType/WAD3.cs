using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Texture Package File
    /// </summary>
    public class WAD3 : ExtractableBase<SabreTools.Serialization.Wrappers.WAD3>
    {
        /// <inheritdoc/>
        public WAD3(SabreTools.Serialization.Wrappers.WAD3? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var wad = SabreTools.Serialization.Wrappers.WAD3.Create(stream);
            if (wad == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            wad.Extract(outDir, includeDebug);

            return true;
        }
    }
}
