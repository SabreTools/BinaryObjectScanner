using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// BD+ SVM
    /// </summary>
    public class BDPlusSVM : DetectableBase<SabreTools.Serialization.Wrappers.BDPlusSVM>
    {
        /// <inheritdoc/>
        public BDPlusSVM(SabreTools.Serialization.Wrappers.BDPlusSVM wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
            => $"BD+ {_wrapper.Date}";
    }
}
