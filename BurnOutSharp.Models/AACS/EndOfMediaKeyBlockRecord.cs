namespace BurnOutSharp.Models.AACS
{
    /// <summary>
    /// A properly formatted MKB shall contain an End of Media Key Block Record.
    /// When a device encounters this Record it stops processing the MKB, using
    /// whatever Km value it has calculated up to that point as the final Km for
    /// that MKB (pending possible checks for correctness of the key, as
    /// described previously).
    /// </summary>
    /// <see href="http://web.archive.org/web/20180718234519/https://aacsla.com/jp/marketplace/evaluating/aacs_technical_overview_040721.pdf"/>
    public sealed class EndOfMediaKeyBlockRecord : Record
    {
        /// <summary>
        /// AACS LA’s signature on the data in the Media Key Block up to,
        /// but not including, this record. Devices depending on the Version
        /// Number in the Type and Version Record must verify the signature.
        /// Other devices may ignore the signature data. If any device
        /// determines that the signature does not verify or is omitted, it
        /// must refuse to use the Media Key.
        /// </summary>
        public byte[] SignatureData;
    }
}