using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Game Cache File (GCF)
    /// </summary>
    public class GCF : DetectableBase<SabreTools.Serialization.Wrappers.GCF>
    {
        /// <inheritdoc/>
        public GCF(SabreTools.Serialization.Wrappers.GCF wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
        {
            // Filename is worth reporting, since it's descriptive and has to match the Steam2 CDN in order to work.
            string fileName = Path.GetFileName(file);
            uint depotId = _wrapper.Model.Header.CacheID;
            uint manifestVersion = _wrapper.Model.Header.LastVersionPlayed;

            // At the moment, all samples of GCF files on redump are unencrypted. Combined with being uncertain about
            // whether this is the best way to check whether the GCF is encrypted, this block will be left commented
            // out until further research is done.
            /*bool encrypted = false;
            if (_wrapper.Files != null && _wrapper.Files.Length > 0)
                encrypted = _wrapper.Files[0].Encrypted;

            string encryptedString = encrypted ? "encrypted" : "unencrypted";
            string returnString = $"{fileName} - {depotId} (v{manifestVersion}, {encryptedString})";*/
            string returnString = $"{fileName} - {depotId} (v{manifestVersion})";
            return returnString;
        }
    }
}
