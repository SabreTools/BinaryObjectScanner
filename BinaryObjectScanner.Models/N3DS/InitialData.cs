namespace BinaryObjectScanner.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCSD#InitialData"/>
    public sealed class InitialData
    {
        /// <summary>
        /// Card seed keyY (first u64 is Media ID (same as first NCCH partitionId))
        /// </summary>
        public byte[] CardSeedKeyY;

        /// <summary>
        /// Encrypted card seed (AES-CCM, keyslot 0x3B for retail cards, see CTRCARD_SECSEED)
        /// </summary>
        public byte[] EncryptedCardSeed;

        /// <summary>
        /// Card seed AES-MAC
        /// </summary>
        public byte[] CardSeedAESMAC;

        /// <summary>
        /// Card seed nonce
        /// </summary>
        public byte[] CardSeedNonce;

        /// <summary>
        /// Reserved3
        /// </summary>
        public byte[] Reserved;

        /// <summary>
        /// Copy of first NCCH header (excluding RSA signature)
        /// </summary>
        public NCCHHeader BackupHeader;
    }
}
