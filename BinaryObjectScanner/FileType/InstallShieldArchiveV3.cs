using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// InstallShield archive v3
    /// </summary>
    public class InstallShieldArchiveV3 : ExtractableBase<SabreTools.Serialization.Wrappers.InstallShieldArchiveV3>
    {
        /// <inheritdoc/>
        public InstallShieldArchiveV3(SabreTools.Serialization.Wrappers.InstallShieldArchiveV3? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var isv3 = SabreTools.Serialization.Wrappers.InstallShieldArchiveV3.Create(stream);
            if (isv3 == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            isv3.Extract(outDir, includeDebug);

            return true;
        }
    }
}
