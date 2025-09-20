using System;
using System.IO;
using SabreTools.Models.AACS;

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
        {
            var record = Array.Find(_wrapper.Records, r => r.RecordType == RecordType.TypeAndVersion);
            if (record is TypeAndVersionRecord tavr)
                return $"AACS {tavr.VersionNumber}";

            return "AACS (Unknown Version)";
        }
    }
}
