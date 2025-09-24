using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// StarForce Filesystem file
    /// </summary>
    /// <see href="https://forum.xentax.com/viewtopic.php?f=21&t=2084"/>
    /// TODO: Implement extraction
    public class SFFS : DetectableBase<SabreTools.Serialization.Wrappers.SFFS>
    {
        /// <inheritdoc/>
        public SFFS(SabreTools.Serialization.Wrappers.SFFS wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
            => "StarForce Filesystem Container";
    }
}
