using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Valve Package File
    /// </summary>
    public class VPK : ExtractableBase<SabreTools.Serialization.Wrappers.VPK>
    {
        /// <inheritdoc/>
        public VPK(SabreTools.Serialization.Wrappers.VPK? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var vpk = SabreTools.Serialization.Wrappers.VPK.Create(stream);
            if (vpk == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            vpk.Extract(outDir, includeDebug);

            return true;
        }
    }
}
