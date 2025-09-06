using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Level
    /// </summary>
    public class BSP : ExtractableBase<SabreTools.Serialization.Wrappers.BSP>
    {
        /// <inheritdoc/>
        public BSP(SabreTools.Serialization.Wrappers.BSP? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var bsp = SabreTools.Serialization.Wrappers.BSP.Create(stream);
            if (bsp == null)
                return false;

            // TODO: Introduce helper methods for all specialty lump types

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            bsp.Extract(outDir, includeDebug);

            return true;
        }
    }
}
