namespace BinaryObjectScanner.Models.AACS
{
    /// <summary>
    /// This record gives the associated encrypted media key data for the
    /// subset-differences identified in the Explicit Subset-Difference Record.
    /// </summary>
    /// <see href="https://aacsla.com/wp-content/uploads/2019/02/AACS_Spec_Common_Final_0953.pdf"/>
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