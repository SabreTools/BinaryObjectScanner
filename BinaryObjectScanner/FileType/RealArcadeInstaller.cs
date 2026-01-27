using System.IO;

#pragma warning disable IDE0290 // Use primary constructor
namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// RealArcade Installer. Known to use the ".rgs" file extension.
    ///
    /// TODO: Add further parsing, game ID and name should be possible to parse.
    /// </summary>
    public class RealArcadeInstaller : DetectableBase<SabreTools.Serialization.Wrappers.RealArcadeInstaller>
    {
        /// <inheritdoc/>
        public RealArcadeInstaller(SabreTools.Serialization.Wrappers.RealArcadeInstaller wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
            => "RealArcade Installer";
    }
}
