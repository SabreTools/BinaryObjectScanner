namespace BurnOutSharp.Models.AACS
{
    /// <summary>
    /// A properly formatted MKB shall have exactly one Verify Media Key Record
    /// as its first record. The presence of the Verify Media Key Record in an MKB
    /// is mandatory, but the use of the Record by a device is not mandatory. The
    /// device may use the Verify Media Key Record to verify the correctness of a
    /// given MKB, or of its processing of it. If everything is correct, the device
    /// should observe the condition:
    ///     [AES_128D(vKm, C]msb_64 == 0x0123456789ABCDEF)]
    /// where Km is the Media Key value.
    /// </summary>
    /// <see href="http://web.archive.org/web/20180718234519/https://aacsla.com/jp/marketplace/evaluating/aacs_technical_overview_040721.pdf"/>
    public sealed class VerifyMediaKeyRecord : Record
    {
        /// <summary>
        /// Bytes 4 through 19 of the Record contain the ciphertext value
        ///     Cv = AES-128E (Km, 0x0123456789ABCDEF || 0xXXXXXXXXXXXXXXXX)
        /// where 0xXXXXXXXXXXXXXXXX is an arbitrary 8-byte value, and Km is
        /// the correct final Media Key value.
        /// </summary>
        public byte[] CiphertextValue;
    }
}