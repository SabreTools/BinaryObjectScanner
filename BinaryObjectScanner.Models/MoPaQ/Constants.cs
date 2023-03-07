namespace BinaryObjectScanner.Models.MoPaQ
{
    public static class Constants
    {
        #region Archive Header

        public static readonly byte[] ArchiveHeaderSignatureBytes = new byte[] { 0x4d, 0x50, 0x51, 0x1a };

        public static readonly string ArchiveHeaderSignatureString = System.Text.Encoding.ASCII.GetString(ArchiveHeaderSignatureBytes);

        public const uint ArchiveHeaderSignatureUInt32 = 0x1a51504d;

        #endregion

        #region User Data

        public static readonly byte[] UserDataSignatureBytes = new byte[] { 0x4d, 0x50, 0x51, 0x1b };

        public static readonly string UserDataSignatureString = System.Text.Encoding.ASCII.GetString(UserDataSignatureBytes);

        public const uint UserDataSignatureUInt32 = 0x1b51504d;

        #endregion

        #region HET Table

        public static readonly byte[] HetTableSignatureBytes = new byte[] { 0x48, 0x45, 0x54, 0x1a };

        public static readonly string HetTableSignatureString = System.Text.Encoding.ASCII.GetString(HetTableSignatureBytes);

        public const uint HetTableSignatureUInt32 = 0x1a544548;

        #endregion

        #region BET Table

        public static readonly byte[] BetTableSignatureBytes = new byte[] { 0x42, 0x45, 0x54, 0x1a };

        public static readonly string BetTableSignatureString = System.Text.Encoding.ASCII.GetString(BetTableSignatureBytes);

        public const uint BetTableSignatureUInt32 = 0x1a544542;

        #endregion

        #region Patch Header

        /// <summary>
        /// Signature as an unsigned Int32 value
        /// </summary>
        public const uint PatchSignatureValue = 0x48435450;

        /// <summary>
        /// Signature as an unsigned Int32 value
        /// </summary>
        public const uint Md5SignatureValue = 0x5F35444D;

        /// <summary>
        /// Signature as an unsigned Int32 value
        /// </summary>
        public const uint XFRMSignatureValue = 0x4D524658;

        /// <summary>
        /// Signature as an unsigned Int64 value
        /// </summary>
        public const ulong BSDIFF40SignatureValue = 0x3034464649445342;

        #endregion
        
        #region Encryption

        public const uint MPQ_HASH_KEY2_MIX = 0x400;

        /// <summary>
        /// Obtained by HashString("(block table)", MPQ_HASH_FILE_KEY)
        /// </summary>
        public const uint MPQ_KEY_BLOCK_TABLE = 0xEC83B3A3;

        /// <summary>
        /// Obtained by HashString("(hash table)", MPQ_HASH_FILE_KEY)
        /// </summary>
        public const uint MPQ_KEY_HASH_TABLE = 0xC3AF3770;

        public const uint STORM_BUFFER_SIZE = 0x500;

        #endregion
    }
}