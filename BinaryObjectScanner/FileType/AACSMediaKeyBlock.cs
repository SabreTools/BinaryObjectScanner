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
        public override string? Detect(Stream stream, string file, bool includeDebug)
        {
            // Create the wrapper
            var mkb = SabreTools.Serialization.Wrappers.AACSMediaKeyBlock.Create(stream);
            if (mkb == null)
                return null;

            // Derive the version, if possible
            var typeAndVersion = Array.Find(mkb.Records ?? [], r => r?.RecordType == SabreTools.Models.AACS.RecordType.TypeAndVersion);
            if (typeAndVersion == null)
                return "AACS (Unknown Version)";
            else
                return $"AACS {(typeAndVersion as SabreTools.Models.AACS.TypeAndVersionRecord)?.VersionNumber}";
        }
    }
}
