using System.IO;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Game Cache File (GCF)
    /// </summary>
    public class GCF : DetectableBase<SabreTools.Wrappers.GCF>
    {
        /// <inheritdoc/>
        public GCF(SabreTools.Wrappers.GCF wrapper) : base(wrapper) { }

        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
        {
            // Filename is worth reporting, since it's descriptive and has to match the Steam2 CDN in order to work.
            string fileName = Path.GetFileName(file);
            uint depotId = _wrapper.Model.Header.CacheID;
            uint depotVersion = _wrapper.Model.Header.LastVersionPlayed;

            // While there is a depot-level encrypted flag on the GCF, it's unfortunately completely unreliable.
            // Technically speaking, there are some known GCFs that only have some files encrypted. HL2 discs from
            // 2004 have a base_source_engine.gcf (depot 200) like this. However, currently unsure of the performance
            // impacts of looping the entirety of _wrapper.Files[] just to cover a fairly niche edge case
            bool encrypted = false;
            if (_wrapper.Files != null && _wrapper.Files.Length > 0)
                encrypted = _wrapper.Files[0].Encrypted;

            // Only note encryption status if the GCF is encrypted
            string encryptedString = encrypted ? ", encrypted" : "";
            string returnString = $"{fileName} - {depotId} (v{depotVersion}{encryptedString})";
            return returnString;
        }
    }
}
