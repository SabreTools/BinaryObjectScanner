using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// AACS media key block
    /// </summary>
    public class AACSMediaKeyBlock : DetectableBase<SabreTools.Serialization.Wrappers.AACSMediaKeyBlock>
    {
        /// <inheritdoc/>
        public AACSMediaKeyBlock(SabreTools.Serialization.Wrappers.AACSMediaKeyBlock wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
            => $"AACS {_wrapper.Version ?? "(Unknown Version)"}";
    }
}
