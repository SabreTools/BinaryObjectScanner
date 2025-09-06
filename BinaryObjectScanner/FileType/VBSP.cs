using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life 2 Level
    /// </summary>
    public class VBSP : ExtractableBase<SabreTools.Serialization.Wrappers.VBSP>
    {
        /// <inheritdoc/>
        public VBSP(SabreTools.Serialization.Wrappers.VBSP? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var vbsp = SabreTools.Serialization.Wrappers.VBSP.Create(stream);
            if (vbsp == null)
                return false;

            // TODO: Introduce helper methods for all specialty lump types

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            vbsp.Extract(outDir, includeDebug);

            return true;
        }
    }
}
