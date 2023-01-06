namespace BurnOutSharp.Models.N3DS
{
    /// <summary>
    /// A format used to store an encrypted titlekey (using 128-Bit AES-CBC).
    /// With 3DS, the Ticket format was updated (now v1) from Wii/DSi format (v0).
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/Ticket"/>
    public sealed class Ticket
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
        /// ECC PublicKey
        /// </summary>
        public byte[] ECCPublicKey;

        /// <summary>
        /// Version (For 3DS this is always 1)
        /// </summary>
        public byte Version;

        /// <summary>
        /// CaCrlVersion
        /// </summary>
        public byte CaCrlVersion;

        /// <summary>
        /// SignerCrlVersion
        /// </summary>
        public byte SignerCrlVersion;

        /// <summary>
        /// TitleKey (normal-key encrypted using one of the common keyYs; see below)
        /// </summary>
        /// <remarks>
        /// The titlekey is decrypted by using the AES engine with the ticket common-key keyslot.
        /// The keyY is selected through an index (ticket offset 0xB1) into a plaintext array
        /// of 6 keys ("common keyYs") stored in the data section of Process9. AES-CBC mode is used
        /// where the IV is the big-endian titleID. Note that on a retail unit index0 is a retail keyY,
        /// while on a dev-unit index0 is the dev common-key which is a normal-key.
        /// (On retail for these keyYs, the hardware key-scrambler is used)
        /// 
        /// The titlekey is used to decrypt content downloaded from the CDN using 128-bit AES-CBC with
        /// the content index (as big endian u16, padded with trailing zeroes) as the IV.
        /// </remarks>
        public byte[] TitleKey;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved1;

        /// <summary>
        /// TicketID
        /// </summary>
        public ulong TicketID;

        /// <summary>
        /// ConsoleID
        /// </summary>
        public uint ConsoleID;

        /// <summary>
        /// TitleID
        /// </summary>
        public ulong TitleID;

        /// <summary>
        /// Reserved
        /// </summary>
        public ushort Reserved2;

        /// <summary>
        /// Ticket title version
        /// </summary>
        /// <remarks>
        /// The Ticket Title Version is generally the same as the title version stored in the
        /// Title Metadata. Although it doesn't have to match the TMD version to be valid.
        /// </remarks>
        public ushort TicketTitleVersion;

        /// <summary>
        /// Reserved
        /// </summary>
        public ulong Reserved3;

        /// <summary>
        /// License Type
        /// </summary>
        public byte LicenseType;

        /// <summary>
        /// Index to the common keyY used for this ticket, usually 0x1 for retail system titles;
        /// see below.
        /// </summary>
        public byte CommonKeyYIndex;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved4;

        /// <summary>
        /// eShop Account ID?
        /// </summary>
        public uint eShopAccountID;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved5;

        /// <summary>
        /// Audit
        /// </summary>
        public byte Audit;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved6;

        /// <summary>
        /// Limits
        /// </summary>
        /// <remarks>
        /// In demos, the first u32 in the "Limits" section is 0x4, then the second u32 is the max-playcount.
        /// </remarks>
        public int[] Limits;

        /// <summary>
        /// The Content Index of a ticket has its own size defined within itself,
        /// with seemingly a minimal of 20 bytes, the second u32 in big endian defines
        /// the full value of X.
        /// </summary>
        public int ContentIndexSize;

        /// <summary>
        /// Content Index
        /// </summary>
        public byte[] ContentIndex;

        /// <summary>
        /// Certificate chain
        /// </summary>
        /// <remarks>
        /// https://www.3dbrew.org/wiki/Ticket#Certificate_Chain
        /// </remarks>
        public Certificate[] CertificateChain;
    }
}
