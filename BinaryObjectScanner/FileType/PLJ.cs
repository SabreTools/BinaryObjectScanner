using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// PlayJ audio file
    /// </summary>
    public class PLJ : DetectableBase<SabreTools.Serialization.Wrappers.PlayJAudioFile>
    {
        /// <inheritdoc/>
        public PLJ(SabreTools.Serialization.Wrappers.PlayJAudioFile? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
            => "PlayJ Audio File";
    }
}
