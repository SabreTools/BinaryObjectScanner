namespace BurnOutSharp.Models.N3DS
{
    /// <summary>
    /// Certificates contain cryptography information for verifying Signatures.
    /// These certificates are also signed. The parent/child relationship between
    /// certificates, makes all the certificates effectively signed by 'Root',
    /// the public key for which is stored in NATIVE_FIRM.
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/Certificates"/>
    public sealed class Certificate
    {
        /// <summary>
        /// Signature Type
        /// </summary>
        public SignatureType SignatureType;

        /// <summary>
        /// Signature size
        /// </summary>
        public ushort SignatureSize;

        /// <summary>
        /// Padding size
        /// </summary>
        public byte PaddingSize;

        /// <summary>
        /// Signature
        /// </summary>
        public byte[] Signature;

        /// <summary>
        /// Issuer
        /// </summary>
        public string Issuer;

        /// <summary>
        /// Key Type
        /// </summary>
        public PublicKeyType KeyType;

        /// <summary>
        /// Name
        /// </summary>
        public string Name;

        /// <summary>
        /// Expiration time as UNIX Timestamp, used at least for CTCert
        /// </summary>
        public uint ExpirationTime;

        // This contains the Public Key(i.e. Modulus & Public Exponent)
        #region RSA

        /// <summary>
        /// Modulus
        /// </summary>
        public byte[] Modulus;

        /// <summary>
        /// Public Exponent
        /// </summary>
        public uint PublicExponent;

        #endregion

        // This contains the ECC public key, and is as follows:
        #region ECC

        /// <summary>
        /// Public Key
        /// </summary>
        public byte[] PublicKey;

        #endregion
    }
}
