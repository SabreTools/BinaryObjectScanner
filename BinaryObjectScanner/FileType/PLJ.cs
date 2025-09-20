using System.IO;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// PlayJ audio file
    /// </summary>
    public class PLJ : DetectableBase<PlayJAudioFile>
    {
        /// <inheritdoc/>
        public PLJ(PlayJAudioFile? wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
            => "PlayJ Audio File";
    }
}
