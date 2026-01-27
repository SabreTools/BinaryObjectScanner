using System.IO;

#pragma warning disable IDE0290 // Use primary constructor
namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// RealArcade Mezzanine files, which contain metadata. Known to use the ".mez" file extension.
    ///
    /// TODO: Add further parsing, game ID should be possible to parse.
    /// </summary>
    public class RealArcadeMezzanine : DetectableBase<SabreTools.Serialization.Wrappers.RealArcadeMezzanine>
    {
        /// <inheritdoc/>
        public RealArcadeMezzanine(SabreTools.Serialization.Wrappers.RealArcadeMezzanine wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
            => "RealArcade Mezzanine";
    }
}
