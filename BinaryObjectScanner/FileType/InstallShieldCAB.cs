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
            Directory.CreateDirectory(outDir);
            return _wrapper.Extract(outDir, includeDebug);
        }
    }
}
