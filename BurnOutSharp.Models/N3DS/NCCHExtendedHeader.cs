namespace BurnOutSharp.Models.N3DS
{
    /// <summary>
    /// The exheader has two sections:
    /// - The actual exheader data, containing System Control Info (SCI) and Access Control Info (ACI);
    /// - A signed copy of NCCH HDR public key, and exheader ACI. This version of the ACI is used as limitation to the actual ACI.
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header"/>
    public sealed class NCCHExtendedHeader
    {
        /// <summary>
        /// SCI
        /// </summary>
        public SystemControlInfo SCI;

        /// <summary>
        /// ACI
        /// </summary>
        public AccessControlInfo ACI;

        /// <summary>
        /// AccessDesc signature (RSA-2048-SHA256)
        /// </summary>
        public byte[] AccessDescSignature;

        /// <summary>
        /// NCCH HDR RSA-2048 public key
        /// </summary>
        public byte[] NCCHHDRPublicKey;

        /// <summary>
        /// ACI (for limitation of first ACI)
        /// </summary>
        public AccessControlInfo ACIForLimitations;
    }
}
