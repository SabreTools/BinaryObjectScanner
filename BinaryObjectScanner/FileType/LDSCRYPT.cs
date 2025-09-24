using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Link Data Security encrypted file
    /// </summary>
    public class LDSCRYPT : DetectableBase<SabreTools.Serialization.Wrappers.LDSCRYPT>
    {
        /// <inheritdoc/>
        public LDSCRYPT(SabreTools.Serialization.Wrappers.LDSCRYPT wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
            => "LDSCRYPT";
    }
}
