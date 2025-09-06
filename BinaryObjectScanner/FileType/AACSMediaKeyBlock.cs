using System;
using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// AACS media key block
    /// </summary>
    public class AACSMediaKeyBlock : DetectableBase<SabreTools.Serialization.Wrappers.AACSMediaKeyBlock>
    {
        /// <inheritdoc/>
        public AACSMediaKeyBlock(SabreTools.Serialization.Wrappers.AACSMediaKeyBlock? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
        {
            // Derive the version, if possible
            var typeAndVersion = Array.Find(_wrapper.Records ?? [], r => r?.RecordType == SabreTools.Models.AACS.RecordType.TypeAndVersion);
            if (typeAndVersion == null)
                return "AACS (Unknown Version)";
            else
                return $"AACS {(typeAndVersion as SabreTools.Models.AACS.TypeAndVersionRecord)?.VersionNumber}";
        }
    }
}
