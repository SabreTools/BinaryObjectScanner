namespace BurnOutSharp.Models.AACS
{
    /// <summary>
    /// This record gives the associated encrypted media key data for the
    /// subset-differences identified in the Explicit Subset-Difference Record.
    /// </summary>
    /// <see href="http://web.archive.org/web/20180718234519/https://aacsla.com/jp/marketplace/evaluating/aacs_technical_overview_040721.pdf"/>
    public sealed class MediaKeyDataRecord : Record
    {
        /// <summary>
        /// Each subset difference has its associated 16 bytes in this
        /// record, in the same order it is encountered in the subset-difference
        /// record. This 16 bytes is the ciphertext value C in the media
        /// key calculation.
        /// </summary>
        public byte[][] MediaKeyData;
    }
}