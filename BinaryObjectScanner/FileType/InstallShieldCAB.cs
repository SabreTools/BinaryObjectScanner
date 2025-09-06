using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// InstallShield cabinet file
    /// </summary>
    public class InstallShieldCAB : ExtractableBase<SabreTools.Serialization.Wrappers.InstallShieldCabinet>
    {
        /// <inheritdoc/>
        public InstallShieldCAB(SabreTools.Serialization.Wrappers.InstallShieldCabinet? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            // Create the wrapper
            var iscab = SabreTools.Serialization.Wrappers.InstallShieldCabinet.Create(stream);
            if (iscab == null)
                return false;

            // Loop through and extract all files
            Directory.CreateDirectory(outDir);
            iscab.Extract(outDir, includeDebug);

            return true;
        }
    }
}
